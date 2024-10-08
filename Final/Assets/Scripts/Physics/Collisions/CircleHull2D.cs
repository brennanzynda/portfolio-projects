using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleHull2D : CollisionHull2D
{
    public CircleHull2D() : base(CollisionHullType2D.hull_circle) { }

    [Range(0.0f,100.0f)]
    public float radius;

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
        //collision passes if distance between them <= sum of radii
        //optimzed collision if distance squared <= sum squared
        // 1. get both centers
        // 2. take the difference between centers
        // 3. disntace squared = dot(diff,diff)
        // 4. sum of radii
        // 5. square sum of radii
        // 6. Test: distSq <= sumSq

        Vector2 center = transform.position;
        Vector2 otherCenter = other.transform.position;
        Vector2 diff = center - otherCenter;
        float diffSquared = Vector2.Dot(diff, diff);
        float radiiSum = radius + other.radius;

        bool test = diffSquared <= (radiiSum * radiiSum);
        float depth = (radiiSum * radiiSum) - diffSquared;
        
        c.contact[c.contactCount].interpenetrationDepth = depth;

        //calculate penetration depth and put in the contact
        return test;
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull2D other, ref Collision c)
    {
        //https://yal.cc/rectangle-circle-intersection-test/ used to find equation
        // calculate closest point by clamping center; point vs circle test
        // 1. Find the nearest point on the box to the center of the circle
        // 2. Take the distance between the center of the cricle and the nearest point on the AABB
        // 3. Test: distSq <= radiusSq

        Vector3 nearestPoint = other.GetComponent<Renderer>().bounds.ClosestPoint(transform.position);

        float pointX = transform.position.x - nearestPoint.x;
        float pointY = transform.position.y - nearestPoint.y;

        bool test = (pointX * pointX + pointY * pointY) < (radius * radius);
        float depth = (radius * radius) - (pointX * pointX + pointY * pointY);

        c.contact[c.contactCount].interpenetrationDepth = depth;

        return test;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other, ref Collision c)
    {
        // same as above, but first
        // move circle center into box's space by multiplying by its world transform inverse
        // boxPos.InverseTransformPoint(circlePos)
        // 1. Find circle center
        // 2. transform circle into space of box
        // 3. Do AABB test

        Vector2 circleCenter = transform.position;
        Vector2 transformedCirclePos = other.transform.InverseTransformPoint(circleCenter); //move circle center into box space

        float halfWidth = 0.5f * other.gameObject.transform.localScale.x;
        float halfHeight = 0.5f * other.gameObject.transform.localScale.y;

        //clamp center
        float nearestX = Mathf.Clamp(transformedCirclePos.x, other.gameObject.transform.localPosition.x - halfWidth, other.gameObject.transform.localPosition.x + halfWidth);
        float nearestY = Mathf.Clamp(transformedCirclePos.y, other.gameObject.transform.localPosition.y - halfHeight, other.gameObject.transform.localPosition.y + halfHeight);

        float pointX = transformedCirclePos.x - nearestX;
        float pointY = transformedCirclePos.y - nearestY;

        bool test = (pointX * pointX + pointY * pointY) < (radius * radius);
        return test;
    }
}

