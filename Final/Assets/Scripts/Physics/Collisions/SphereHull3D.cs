using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereHull3D : CollisionHull3D
{
    public SphereHull3D() : base(CollisionHullType3D.hull_sphere) { }

    [Range(0.0f, 100.0f)]
    public float radius;

    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<Particle3D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override bool TestCollisionVsCircle(SphereHull3D other, ref Collision c)
    {
        //collision passes if distance between them <= sum of radii
        //optimzed collision if distance squared <= sum squared
        // 1. get both centers
        // 2. take the difference between centers
        // 3. disntace squared = dot(diff,diff)
        // 4. sum of radii
        // 5. square sum of radii
        // 6. Test: distSq <= sumSq

        Vector3 center = transform.position;
        Vector3 otherCenter = other.transform.position;
        Vector3 diff = center - otherCenter;
        float diffSquared = Vector3.Dot(diff, diff);
        float radiiSum = radius + other.radius;

        bool test = diffSquared <= (radiiSum * radiiSum);
        float depth = (radiiSum * radiiSum) - diffSquared;

        c.contact[c.contactCount].interpenetrationDepth = depth;

        //calculate penetration depth and put in the contact
        return test;
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoxHull3D other, ref Collision c)
    {
        //https://yal.cc/rectangle-circle-intersection-test/ used to find equation
        // calculate closest point by clamping center; point vs circle test
        // 1. Find the nearest point on the box to the center of the circle
        // 2. Take the distance between the center of the cricle and the nearest point on the AABB
        // 3. Test: distSq <= radiusSq

        Vector3 circleCenter = transform.position;
        Vector3 box = other.transform.position;

        float halfWidth = 0.5f * other.gameObject.transform.localScale.x;
        float halfHeight = 0.5f * other.gameObject.transform.localScale.y;
        float halfDepth = 0.5f * other.gameObject.transform.localScale.z;

        //check transformed circle with local box
        float nearestX = Mathf.Max(box.x - halfWidth, Mathf.Min(circleCenter.x, box.x + halfWidth));
        float nearestY = Mathf.Max(box.y - halfHeight, Mathf.Min(circleCenter.y, box.y + halfHeight));
        float nearestZ = Mathf.Max(box.z - halfDepth, Mathf.Min(circleCenter.z, box.z + halfDepth));


        Vector3 nearestPoint = other.GetComponent<Renderer>().bounds.ClosestPoint(transform.position);

        float distance = Vector3.Distance(transform.position, new Vector3(nearestX, nearestY, nearestZ));

        bool test = (distance*distance) < (radius * radius);
        float depth = (radius * radius) - (distance * distance);

        c.contact[c.contactCount].interpenetrationDepth = depth;

        return test;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c)
    {
        // same as above, but first
        // move circle center into box's space by multiplying by its world transform inverse
        // boxPos.InverseTransformPoint(circlePos)
        // 1. Find circle center
        // 2. transform circle into space of box
        // 3. Do AABB test

        //gave me a better understanding on how multiplying my inverse transform makes it relative to the origin
        //https://stackoverflow.com/questions/28487498/how-do-i-calculate-collision-with-rotation-in-3d-space

        Vector3 circleCenter = transform.position;
        //moves circle into box space, do tests as if the box is at the origin
        Vector3 transformedCirclePos = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(circleCenter);

        //should be origin since its using its inverse transform matrix
        Vector3 tranformedBoxCenter = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(other.transform.position);

        //Debug.Log(circleCenter);
        //Debug.Log(transformedCirclePos);

        float halfWidth = 0.5f * other.gameObject.transform.localScale.x;
        float halfHeight = 0.5f * other.gameObject.transform.localScale.y;
        float halfDepth = 0.5f * other.gameObject.transform.localScale.z;

        //check transformed circle position with local box
        float nearestX = Mathf.Max(tranformedBoxCenter.x - halfWidth, Mathf.Min(transformedCirclePos.x, tranformedBoxCenter.x + halfWidth));
        float nearestY = Mathf.Max(tranformedBoxCenter.y - halfHeight, Mathf.Min(transformedCirclePos.y, tranformedBoxCenter.y + halfHeight));
        float nearestZ = Mathf.Max(tranformedBoxCenter.z - halfDepth, Mathf.Min(transformedCirclePos.z, tranformedBoxCenter.z + halfDepth));

        float distance = Vector3.Distance(transformedCirclePos, new Vector3(nearestX,nearestY,nearestZ));

        bool test = (distance*distance) < (radius * radius);
        return test;
    }

    public override bool TestCollisionVsComplex(ComplexHull3D other, ref Collision c)
    {
        return other.TestCollisionVsCircle(this, ref c);
    }
}
