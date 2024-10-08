using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Base Class Based Heavily Upon https://gamedevelopment.tutsplus.com/tutorials/goal-oriented-action-planning-for-a-smarter-ai--cms-20793 (mostly the KeyValuePair functionality/iterations)
public abstract class BaseAction : MonoBehaviour
{
    private bool inRange = false;

    public float cost = 1f;

    public GameObject target;
    public GameObject AI;

    public void Start()
    {
        AI = GameObject.Find("AIBoi");
    }

    public BaseAction()
    {
        Preconds = new HashSet<KeyValuePair<string, object>>();
        Effects = new HashSet<KeyValuePair<string, object>>();
    }

    public void ResetAction()
    {
        inRange = false;
        target = null;
        reset();
    }

    public abstract void reset();

    public abstract bool IsDone();

    public abstract bool CanRun(GameObject obj);

    public abstract bool Perform(GameObject obj);

    public abstract bool NeedToBeInRange();

    public bool IsInRange()
    {
        return inRange;
    }

    public void SetInRange(bool isInRange)
    {
        inRange = isInRange;
    }

    public void AddPrecondition(string key, object value)
    {
        Preconds.Add(new KeyValuePair<string, object>(key, value));
    }

    public void RemovePrecondition(string key)
    {
        KeyValuePair<string, object> rm = default;
        foreach (KeyValuePair<string, object> keyValuePair in Preconds)
        {
            if (keyValuePair.Key.Equals(key))
            {
                rm = keyValuePair;
            }
        }
        if (!default(KeyValuePair<string,object>).Equals(rm))
        {
            Preconds.Remove(rm);
        }
    }

    public void AddEffect(string key, object value)
    {
        Effects.Add(new KeyValuePair<string, object>(key, value));
    }

    public void RemoveEffect(string key)
    {
        KeyValuePair<string, object> rm = default;
        foreach (KeyValuePair<string, object> keyValuePair in Preconds)
        {
            if (keyValuePair.Key.Equals(key))
            {
                rm = keyValuePair;
            }
        }
        if (!default(KeyValuePair<string, object>).Equals(rm))
        {
            Effects.Remove(rm);
        }
    }

    public HashSet<KeyValuePair<string, object>> Preconds { get; }

    public HashSet<KeyValuePair<string, object>> Effects { get; }
}
