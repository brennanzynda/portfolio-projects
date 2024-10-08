using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetToolAction : BaseAction
{
    private bool pickedUp = false;

    public GetToolAction()
    {
        AddPrecondition("hasTool", false);
        AddEffect("hasTool", true);
    }
    public override void reset()
    {
        pickedUp = false;
    }

    public override bool IsDone()
    {
        if (pickedUp)
        {
            AI.GetComponent<AI>().hasTool = true;
            Debug.Log("ToolGot");
            AI.GetComponent<Particle3D>().target = target;
        }
        return pickedUp;
    }

    public override bool CanRun(GameObject obj)
    {
        float dist = (target.transform.position - obj.transform.position).magnitude;
        if (dist < 0.2f)
        {
            return true;
        }
        return false;
    }

    public override bool Perform(GameObject obj)
    {
        return (pickedUp = true);
    }

    public override bool NeedToBeInRange()
    {
        return false;
    }
}
