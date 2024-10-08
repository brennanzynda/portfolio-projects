using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisAllignedBoundingBoxHull2D : CollisionHull2D
{
    public AxisAllignedBoundingBoxHull2D() : base(CollisionHullType2D.hull_aabb) { }

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
        return other.TestCollisionVsAABB(this, ref c);
    }

    public override bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull2D other, ref Collision c)
    {
        // on each axis, maximum extent of a greater than minimum extent of b
        // 1. Find min and max points of both boxes
        // 2. Check x-axis if max1.x >= min2.x
        // 3. Check y-axis if max1.y >= min2.y
        // 4. Test if xCheck && yCheck == true

        Vector2 minBox1 = new Vector2((transform.position.x - .5f * transform.localScale.x), (transform.position.y - .5f * transform.localScale.y));
        Vector2 minBox2 = new Vector2((other.transform.position.x - .5f * other.transform.localScale.x), (other.transform.position.y - .5f * other.transform.localScale.x));

        Vector2 maxBox1 = new Vector2((transform.position.x + .5f * transform.localScale.x), (transform.position.y + .5f * transform.localScale.x));
        Vector2 maxBox2 = new Vector2((other.transform.position.x + .5f * other.transform.localScale.x), (other.transform.position.y + .5f * other.transform.localScale.x));
        if (maxBox1.x >= minBox2.x && maxBox2.x >= minBox1.x)
        {
            if (maxBox1.y >= minBox2.y && maxBox2.y >= minBox1.y)
            {
                return true;
            }
        }
        return false;
    }

    public override bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other, ref Collision c)
    {
        // same as above twice:
        // first, find max extents of OBB, perform AABB vs this
        // then transform this into OBB space, find max extents, AABB test again
        // 1. find max extents of OBB
        // 2. Test AABB vs max extents of OBB
        // 3. If AABB test fails, no collision
        // 4. If passes, transform each point into OBB space and AABB test again
        // 5. If passes, collision, else no collision

        Vector2 minBox1 = new Vector2((transform.position.x - .5f * transform.localScale.x), (transform.position.y - .5f * transform.localScale.y));
        Vector2 maxBox1 = new Vector2((transform.position.x + .5f * transform.localScale.x), (transform.position.y + .5f * transform.localScale.y));

        Vector2 minBox2 = other.gameObject.GetComponent<Renderer>().bounds.min;
        Vector2 maxBox2 = other.gameObject.GetComponent<Renderer>().bounds.max;



        if ((maxBox1.x >= minBox2.x && maxBox2.x >= minBox1.x))
        {
            if (maxBox1.y >= minBox2.y && maxBox2.y >= minBox1.y)
            {
                //Debug.Log("WORKS");
                return true;
            }
        }
        return false;
    }
}
