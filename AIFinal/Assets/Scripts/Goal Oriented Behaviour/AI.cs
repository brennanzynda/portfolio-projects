using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour, Interface
{

    public float moveSpeed = 1f;
    [HideInInspector]
    public bool hasTool = false;

    [HideInInspector]
    public bool hasResource = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HashSet<KeyValuePair<string, object>> getWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        worldData.Add(new KeyValuePair<string, object>("hasTool", hasTool));
        worldData.Add(new KeyValuePair<string, object>("hasResource", hasResource));

        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> createGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>();

        goal.Add(new KeyValuePair<string, object>("getResources", true));
        return goal;
    }

    public void planFailed(HashSet<KeyValuePair<string, object>> failed)
    {
    }

    public void planFound(HashSet<KeyValuePair<string, object>> goal, Queue<BaseAction> actions)
    {
    }

    public void actionsFinished()
    {
    }

    public void planAborted(BaseAction aborter)
    {
    }

    public bool move(BaseAction nextAction)
    {
        /* // move towards the NextAction's target
         float step = moveSpeed * Time.deltaTime;
         gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, nextAction.target.transform.position, step);

         if (gameObject.transform.position.Equals(nextAction.target.transform.position))
         {
             // we are at the target location, we are done
             nextAction.SetInRange(true);
             return true;
         }
         else
         {
             return false;
         }*/

        return false;
    }
}
