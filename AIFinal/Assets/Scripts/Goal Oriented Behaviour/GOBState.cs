// Taken from Github Project From http://github.com/sploreg/goap

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public sealed class GOBState : MonoBehaviour
{

    private FSM stateMachine;

    private FSM.FSMState idle;
    private FSM.FSMState move;
    private FSM.FSMState performAction;

    private HashSet<BaseAction> available;
    private Queue<BaseAction> current;

    private Interface data;

    private GOBPlanner planner;
    // Start is called before the first frame update
    void Start()
    {
        stateMachine = new FSM();
        available = new HashSet<BaseAction>();
        current = new Queue<BaseAction>();
        planner = new GOBPlanner();
        findData();
        createIdleState();
        createMoveState();
        createPerformState();
        stateMachine.pushState(idle);
        loadActions();
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update(gameObject);
    }

    public void addAction(BaseAction a)
    {
        available.Add(a);
    }

    public BaseAction getAction(Type action)
    {
        foreach (BaseAction a in available)
        {
            if (a.GetType().Equals(action))
            {
                return a;
            }
        }
        return null;
    }

    public void removeAction(BaseAction action)
    {
        available.Remove(action);
    }

    private bool hasActionPlan()
    {
        return current.Count > 0;
    }

    private void createIdleState()
    {
        idle = (fsm, gameObj) =>
        {
            HashSet<KeyValuePair<string, object>> worldState = data.getWorldState();
            HashSet<KeyValuePair<string, object>> goal = data.createGoalState();

            Queue<BaseAction> plan = planner.Plan(gameObject, available, worldState, goal);
            if (plan != null)
            {
                current = plan;
                data.planFound(goal, plan);

                fsm.popState();
                fsm.pushState(performAction);
            }
            else
            {
                data.planFailed(goal);
                fsm.popState();
                fsm.pushState(idle);
            }
        };
    }

    private void createMoveState()
    {
        move = (fsm, gameObj) =>
        {
            BaseAction action = current.Peek();
            if (action.NeedToBeInRange() && action.target == null)
            {
                fsm.popState();
                fsm.popState();
                fsm.pushState(idle);
                return;
            }

            if (data.move(action))
            {
                fsm.popState();
            }
        };
    }

    private void createPerformState()
    {
        performAction = (fsm, gameObj) =>
        {
            if (!hasActionPlan())
            {
                fsm.popState();
                fsm.pushState(idle);
                data.actionsFinished();
                return;
            }

            BaseAction action = current.Peek();
            if (action.IsDone())
            {
                current.Dequeue();
            }

            if (hasActionPlan())
            {
                action = current.Peek();
                bool inRange = action.NeedToBeInRange() ? action.IsInRange() : true;

                if (inRange)
                {
                    bool success = action.Perform(gameObj);

                    if (!success)
                    {
                        fsm.popState();
                        fsm.pushState(idle);
                        data.planAborted(action);
                    }
                }
                else
                {
                    fsm.pushState(move);
                }
            }
            else
            {
                fsm.popState();
                fsm.pushState(idle);
                data.actionsFinished();
            }
        };
    }

    private void findData()
    {
        foreach (Component comp in gameObject.GetComponents(typeof(Component)))
        {
            if (typeof(Interface).IsAssignableFrom(comp.GetType()))
            {
                data = (Interface)comp;
                return;
            }
        }
    }

    private void loadActions()
    {
        BaseAction[] actions = gameObject.GetComponents<BaseAction>();
        foreach (BaseAction a in actions)
        {
            available.Add(a);
        }
    }
}
