using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseToolAction : BaseAction
{
    private bool isUsed = false;
    public float maxDistance = 5f;
    public bool useMaxDistance = false;

    public UseToolAction()
    {
        AddPrecondition("hasTool", true);
        AddPrecondition("hasResource", false);
        AddEffect("hasResource", true);
        AddEffect("hasTool", false);
        
    }
    public override void reset()
    {
        isUsed = false;
        return;
    }

    public override bool IsDone()
    {
        if (isUsed)
        {
            AI.GetComponent<Particle3D>().target = target;
        }
        return isUsed;
    }

    public override bool CanRun(GameObject obj)
    {
        GameObject resTarget = GameObject.FindGameObjectWithTag("Resource");
        float distance = (target.transform.position - resTarget.transform.position).magnitude;
        if (useMaxDistance)
        {
            if (distance <= maxDistance)
            {
                return true;
            }
            return false;
        }
        return true;
    }

    public override bool Perform(GameObject obj)
    {
        return (isUsed = true);
    }

    public override bool NeedToBeInRange()
    {
        return false;
    }
}
