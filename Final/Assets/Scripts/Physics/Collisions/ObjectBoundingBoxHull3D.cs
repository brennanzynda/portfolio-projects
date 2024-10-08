using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBoundingBoxHull3D : CollisionHull3D
{
    public ObjectBoundingBoxHull3D() : base(CollisionHullType3D.hull_obb) { }

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
        bool test = other.TestCollisionVsOBB(this, ref c);
        return test;
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoxHull3D other, ref Collision c)
    {
        //see aabb
        bool test = other.TestCollisionVsOBB(this, ref c);
        return test;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c)
    {
        // AABB-OBB part 2 twice
        // 1. Multiply this box by inverse transform of other to move it into other space
        // 2. other box is now treated as it being at origin with no rotation, do aabb with bounds of this
        // 3. Multiply other by inverse transform of this to move into this space
        // 4. this box is treated as it being at origin with no rotation, do aabb with bounds of other

        Vector3 minBox1 = gameObject.GetComponent<Renderer>().bounds.min;
        Vector3 maxBox1 = gameObject.GetComponent<Renderer>().bounds.max;

        Vector3 minBox2 = other.gameObject.GetComponent<Renderer>().bounds.min;
        Vector3 maxBox2 = other.gameObject.GetComponent<Renderer>().bounds.max;

        if ((maxBox1.x >= minBox2.x && maxBox2.x >= minBox1.x))
        {
            if (maxBox1.y >= minBox2.y && maxBox2.y >= minBox1.y)
            {
                if (maxBox1.z >= minBox2.z && maxBox2.z >= minBox1.z)
                {
                    //Debug.Log("bounds collision");
                    //collision with bounds of obbs, now have to do the change of basis
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
                                // Test the opposite aspect
                                Vector3 trans2MinBox1 = new Vector3((0.0f - 0.5f * transform.localScale.x), (0.0f - 0.5f * transform.localScale.y), (0.0f - 0.5f * transform.localScale.z));
                                Vector3 trans2MaxBox1 = new Vector3((0.0f + 0.5f * transform.localScale.x), (0.0f + 0.5f * transform.localScale.y), (0.0f + 0.5f * transform.localScale.z));
                                Vector3 trans2MinBox2 = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(minBox2);
                                Vector3 trans2MaxBox2 = other.GetComponent<Particle3D>().getWorldInverseTransform().MultiplyPoint3x4(maxBox2);
                                if ((trans2MaxBox1.x >= trans2MinBox2.x && trans2MaxBox2.x >= trans2MinBox1.x))
                                {
                                    if (trans2MaxBox1.y >= trans2MinBox2.y && trans2MaxBox2.y >= trans2MinBox1.y)
                                    {
                                        if (trans2MaxBox1.z >= trans2MinBox2.z && trans2MaxBox2.z >= trans2MinBox1.z)
                                        {
                                            return true;
                                        }
                                    }
                                }
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
        return other.TestCollisionVsOBB(this, ref c);
    }
}
