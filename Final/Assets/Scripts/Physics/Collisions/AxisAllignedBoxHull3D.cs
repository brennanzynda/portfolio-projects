using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAllignedBoxHull3D : CollisionHull3D
{
    public AxisAllignedBoxHull3D() : base(CollisionHullType3D.hull_aabb) { }
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
        //see circle
        return other.TestCollisionVsAABB(this, ref c);
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoxHull3D other, ref Collision c)
    {
        // on each axis, maximum extent of a greater than minimum extent of b
        // 1. Find min and max points of both boxes
        // 2. Check x-axis if max1.x >= min2.x
        // 3. Check y-axis if max1.y >= min2.y
        // 4. Test if xCheck && yCheck == true

        Vector3 minBox1 = new Vector3((transform.position.x - .5f * transform.localScale.x), (transform.position.y - .5f * transform.localScale.y), 
            (transform.position.z - .5f * transform.localScale.z));
        Vector3 minBox2 = new Vector3((other.transform.position.x - .5f * other.transform.localScale.x), (other.transform.position.y - .5f * other.transform.localScale.y),
            (other.transform.position.z - .5f * other.transform.localScale.z));

        Vector3 maxBox1 = new Vector3((transform.position.x + .5f * transform.localScale.x), (transform.position.y + .5f * transform.localScale.y),
            (transform.position.z + .5f * transform.localScale.z));
        Vector3 maxBox2 = new Vector3((other.transform.position.x + .5f * other.transform.localScale.x), (other.transform.position.y + .5f * other.transform.localScale.y),
            (other.transform.position.z + .5f * other.transform.localScale.z));
        if (maxBox1.x >= minBox2.x && maxBox2.x >= minBox1.x)
        {
            if (maxBox1.y >= minBox2.y && maxBox2.y >= minBox1.y)
            {
                if (maxBox1.z >= minBox2.z && maxBox2.z >= minBox1.z)
                    return true;
            }
        }
        return false;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c)
    {
        // same as above twice:
        // first, find max extents of OBB, perform AABB vs this
        // then transform this into OBB space, find max extents, AABB test again
        // 1. find max extents of OBB
        // 2. Test AABB vs max extents of OBB
        // 3. If AABB test fails, no collision
        // 4. If passes, transform each point into OBB space and AABB test again
        // 5. If passes, collision, else no collision

        Vector3 minBox1 = new Vector3((transform.position.x - .5f * transform.localScale.x), (transform.position.y - .5f * transform.localScale.y), (transform.position.z - .5f * transform.localScale.z));
        Vector3 maxBox1 = new Vector3((transform.position.x + .5f * transform.localScale.x), (transform.position.y + .5f * transform.localScale.y), (transform.position.z + .5f * transform.localScale.z));

        Vector3 minBox2 = other.gameObject.GetComponent<Renderer>().bounds.min;
        Vector3 maxBox2 = other.gameObject.GetComponent<Renderer>().bounds.max;

        if ((maxBox1.x >= minBox2.x && maxBox2.x >= minBox1.x))
        {
            if (maxBox1.y >= minBox2.y && maxBox2.y >= minBox1.y)
            {
                if (maxBox1.z >= minBox2.z && maxBox2.z >= minBox1.z)
                {
                    //Debug.Log("bounds collision");
                    //collision with bounds of obb, now have to do the change of basis
                    Vector3 transMinBox1 = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(minBox1);
                    Vector3 transMaxBox1 = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(maxBox1);

                    //obb in local space will be the same box dimensions but at the origin with no rotation
                    Vector3 transMinBox2 = new Vector3((0.0f - .5f * transform.localScale.x), (0.0f - .5f * transform.localScale.y), (0.0f - .5f * transform.localScale.z));
                    Vector3 transMaxBox2 = new Vector3((0.0f + .5f * transform.localScale.x), (0.0f + .5f * transform.localScale.y), (0.0f + .5f * transform.localScale.z));
                    //aabb test vs the local obb
                    if ((transMaxBox1.x >= transMinBox2.x && transMaxBox2.x >= transMinBox1.x))
                    {
                        if (transMaxBox1.y >= transMinBox2.y && transMaxBox2.y >= transMinBox1.y)
                        {
                            if (transMaxBox1.z >= transMinBox2.z && transMaxBox2.z >= transMinBox1.z)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public override bool TestCollisionVsComplex(ComplexHull3D other, ref Collision c)
    {
        return other.TestCollisionVsAABB(this, ref c);
    }
}
