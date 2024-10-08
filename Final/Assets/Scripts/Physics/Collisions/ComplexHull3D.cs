using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexHull3D : CollisionHull3D
{
    public ComplexHull3D() : base(CollisionHullType3D.hull_complex) { }

    public List<CollisionHull3D> members;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool TestCollisionVsCircle(SphereHull3D other, ref Collision c)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].TestCollisionVsCircle(other, ref c))
            {
                return true;
            }
        }
        return false;
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoxHull3D other, ref Collision c)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].TestCollisionVsAABB(other, ref c))
            {
                return true;
            }
        }
        return false;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].TestCollisionVsOBB(other, ref c))
            {
                return true;
            }
        }
        return false;
    }

    public override bool TestCollisionVsComplex(ComplexHull3D other, ref Collision c)
    {
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i].TestCollisionVsComplex(other, ref c))
            {
                return true;
            }
        }
        return false;
    }
}
