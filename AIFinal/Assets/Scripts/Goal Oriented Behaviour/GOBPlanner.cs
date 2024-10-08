using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOBPlanner
{
    public Queue<BaseAction> Plan(GameObject target, HashSet<BaseAction> actions, HashSet<KeyValuePair<string, object>> state, HashSet<KeyValuePair<string, object>> goal)
    {
        foreach (BaseAction a in actions)
        {
            a.reset();
        }

        HashSet<BaseAction> doableActions = new HashSet<BaseAction>();
        foreach (BaseAction a in actions)
        {
            if (a.CanRun(target))
            {
                doableActions.Add(a);
            }
        }

        List<Node> leaves = new List<Node>();

        Node start = new Node(null, 0, state, null);
        bool success = buildGraph(start, leaves, doableActions, goal);
        if (!success)
        {
            return null;
        }

        Node cheapest = null;
        foreach (Node leaf in leaves)
        {
            if (cheapest == null)
            {
                cheapest = leaf;
            }
            else
            {
                if (leaf.mCost < cheapest.mCost)
                {
                    cheapest = leaf;
                }
            }
        }

        List<BaseAction> actionList = new List<BaseAction>();
        Node n = cheapest;
        while (n != null)
        {
            if (n.mAction != null)
            {
                actionList.Insert(0, n.mAction);
            }
            n = n.mParent;
        }

        Queue<BaseAction> queue = new Queue<BaseAction>();
        foreach (BaseAction a in actionList)
        {
            queue.Enqueue(a);
        }

        return queue;
    }

    private bool buildGraph(Node parent, List<Node> leaves, HashSet<BaseAction> actions, HashSet<KeyValuePair<string, object>> goal)
    {
        bool found = false;

        foreach (BaseAction a in actions)
        {
            if (inState(a.Preconds, parent.mState))
            {
                HashSet<KeyValuePair<string, object>> current = makeState (parent.mState, a.Effects);

                Node node = new Node(parent, parent.mCost + a.cost, current, a);

                if (inState(goal, current))
                {
                    leaves.Add(node);
                    found = true;
                }
                else
                {
                    HashSet<BaseAction> sub = ActionSubset(actions, a);
                    found = buildGraph(node, leaves, sub, goal);
                }
            }
        }
        return found;
    }

    private HashSet<BaseAction> ActionSubset(HashSet<BaseAction> actions, BaseAction rm)
    {
        HashSet<BaseAction> sub = new HashSet<BaseAction>();
        foreach (BaseAction a in actions)
        {
            if (!a.Equals(rm))
            {
                sub.Add(a);
            }
        }
        return sub;
    }

    private HashSet<KeyValuePair<string, object>> makeState(HashSet<KeyValuePair<string, object>> current, HashSet<KeyValuePair<string, object>> change)
    {
        HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

        foreach (KeyValuePair<string, object> s in current)
        {
            state.Add(new KeyValuePair<string, object>(s.Key, s.Value));
        }

        foreach (KeyValuePair<string, object> c in change)
        {
            bool exists = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(c))
                {
                    exists = true;
                    break;
                }
            }
            if (exists)
            {
                state.RemoveWhere((KeyValuePair<string, object> keyValuePair) => { return keyValuePair.Key.Equals(c.Key); });
                KeyValuePair<string, object> updated = new KeyValuePair<string, object>(c.Key, c.Value);
                state.Add(updated);
            }
            else
            {
                state.Add(new KeyValuePair<string, object>(c.Key, c.Value));
            }
        }
        return state;
    }

    private bool inState(HashSet<KeyValuePair<string, object>> test, HashSet<KeyValuePair<string, object>> state)
    {
        bool all = true;
        foreach (KeyValuePair<string, object> t in test)
        {
            bool match = false;
            foreach (KeyValuePair<string, object> s in state)
            {
                if (s.Equals(t))
                {
                    match = true;
                    break;
                }
            }
            if (!match)
            {
                all = false;
            }
        }
        return all;
    }
    private class Node
    {
        public Node mParent;
        public float mCost;
        public HashSet<KeyValuePair<string, object>> mState;
        public BaseAction mAction;

        public Node(Node parent, float runCost, HashSet<KeyValuePair<string, object>> state, BaseAction action)
        {
            mParent = parent;
            mCost = runCost;
            mState = state;
            mAction = action;
        }
    }
}
