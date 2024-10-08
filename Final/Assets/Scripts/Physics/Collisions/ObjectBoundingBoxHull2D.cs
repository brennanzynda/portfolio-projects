using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundingBoxHull2D : CollisionHull2D
{
    public ObjectBoundingBoxHull2D() : base(CollisionHullType2D.hull_obb) { }


    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<Particle2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool TestCollisionVsCircle(CircleHull2D other, ref Collision c)
    {
        //see circle
        bool test = other.TestCollisionVsOBB(this, ref c);
        return test;
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull2D other, ref Collision c)
    {
        //see aabb
        bool test = other.TestCollisionVsOBB(this, ref c);
        return test;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other, ref Collision c)
    {
        // AABB-OBB part 2 twice
        // 1. Calculate 2 normal unit vectors for each box (pi/4 rotation difference)
        // 2. Project 4 points from one box onto one normal line of the other
        // 3. Store minimum/maximum values (points on line)
        // 4. Project vertices from original box onto line
        // 5. Do AABB test on new points
        // 6. Repeat for the other normal line, then all steps for the other box
        // 7. Test fails if any of these don't pass
        float box1Cos = Mathf.Cos(gameObject.transform.rotation.eulerAngles.magnitude);
        float box1Sin = Mathf.Sin(gameObject.transform.rotation.eulerAngles.magnitude);
        float box2Cos = Mathf.Cos(other.gameObject.transform.rotation.eulerAngles.magnitude);
        float box2Sin = Mathf.Sin(other.gameObject.transform.rotation.eulerAngles.magnitude);

        Vector2 box1normal1 = new Vector2(-box1Cos, box1Sin);
        Vector2 box1normal2 = new Vector2(box1Cos, box1Sin);
        Vector2 box2normal1 = new Vector2(-box2Cos, box2Sin);
        Vector2 box2normal2 = new Vector2(box2Cos, box2Sin);

        Vector2 box1Min = GetComponent<Renderer>().bounds.min;
        Vector2 box2Min = other.gameObject.GetComponent<Renderer>().bounds.min;
        Vector2 box1Max = GetComponent<Renderer>().bounds.max;
        Vector2 box2Max = other.gameObject.GetComponent<Renderer>().bounds.max;

        other.transform.InverseTransformPoint(box1Min);
        other.transform.InverseTransformPoint(box1Max);
        other.transform.InverseTransformPoint(box2Min);
        other.transform.InverseTransformPoint(box2Max);

        // Projecting all the points onto all the projections lines for each box onto box 2 normals

        Vector2 p1b2n1 = Vector3.Project(box1Min, box2normal1);
        Vector2 p2b2n1 = Vector3.Project(box1Max, box2normal1);
        Vector2 p3b2n1 = Vector3.Project(box2Min, box2normal1);
        Vector2 p4b2n1 = Vector3.Project(box2Max, box2normal1);

        Vector2 p1b2n2 = Vector3.Project(box1Min, box2normal2);
        Vector2 p2b2n2 = Vector3.Project(box1Max, box2normal2);
        Vector2 p3b2n2 = Vector3.Project(box2Min, box2normal2);
        Vector2 p4b2n2 = Vector3.Project(box2Max, box2normal2);

        // Box 2 normal line 1
        if (!(Vector2.Dot(p4b2n1, p4b2n1) >= Vector2.Dot(p1b2n1, p1b2n1) && Vector2.Dot(p3b2n1, p3b2n1) >= Vector2.Dot(p2b2n1, p2b2n1)))
        {
            // Box 2 normal line 2
            if (!(Vector2.Dot(p4b2n2, p4b2n2) >= Vector2.Dot(p1b2n2, p1b2n2) && Vector2.Dot(p3b2n2, p3b2n2) >= Vector2.Dot(p2b2n2, p2b2n2)))
            {
                other.transform.TransformPoint(box1Min);
                other.transform.TransformPoint(box1Max);
                other.transform.TransformPoint(box2Min);
                other.transform.TransformPoint(box2Max);
                gameObject.transform.InverseTransformPoint(box1Min);
                gameObject.transform.InverseTransformPoint(box1Max);
                gameObject.transform.InverseTransformPoint(box2Min);
                gameObject.transform.InverseTransformPoint(box2Max);

                // Projecting all the points onto all the projections lines for each box onto box 1 normals
                Vector2 p1b1n1 = Vector3.Project(box1Min, box1normal1);
                Vector2 p2b1n1 = Vector3.Project(box1Max, box1normal1);
                Vector2 p3b1n1 = Vector3.Project(box2Min, box1normal1);
                Vector2 p4b1n1 = Vector3.Project(box2Max, box1normal1);

                Vector2 p1b1n2 = Vector3.Project(box1Min, box1normal2);
                Vector2 p2b1n2 = Vector3.Project(box1Max, box1normal2);
                Vector2 p3b1n2 = Vector3.Project(box2Min, box1normal2);
                Vector2 p4b1n2 = Vector3.Project(box2Max, box1normal2);

                // Box 1 normal 1
                if (!(Vector2.Dot(p4b1n1, p4b1n1) >= Vector2.Dot(p1b1n1, p1b1n1) && Vector2.Dot(p3b1n1, p3b1n1) >= Vector2.Dot(p2b1n1, p2b1n1)))
                {
                    // Box 1 normal 2
                    if (!(Vector2.Dot(p4b1n2, p4b1n2) >= Vector2.Dot(p1b1n2, p1b1n2) && Vector2.Dot(p3b1n2, p3b1n2) >= Vector2.Dot(p2b1n2, p2b1n2)))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
