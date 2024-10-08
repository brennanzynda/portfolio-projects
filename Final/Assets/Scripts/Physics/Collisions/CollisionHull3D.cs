using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull3D : MonoBehaviour
{
    public class Collision
    {
        //https://en.wikipedia.org/wiki/Coefficient_of_restitution used for coefficient of restitution value (glass(soda-lime)= .69)
        public struct Contact
        {
            public float interpenetrationDepth;
            public Vector3 normal;
            public float restitution;
        }
        public bool isTrigger = false;
        //collision
        public CollisionHull3D a = null, b = null;
        public Contact[] contact = new Contact[4];
        public int contactCount = 0;
        public bool status = false;

        //resolution
        public float closingVelocity;
    }
    public bool isTrigger = false;
    public enum CollisionHullType3D
    {
        hull_sphere,
        hull_aabb,
        hull_obb,
        hull_complex
    }
    private CollisionHullType3D type { get; }

    protected CollisionHull3D(CollisionHullType3D type_set)
    {
        type = type_set;
    }

    protected Particle3D particle;



    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<Particle3D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //public bool TestCollision(CollisionHull3D a, CollisionHull3D b, ref Collision c)
    public static bool TestCollision(CollisionHull3D a, CollisionHull3D b, ref Collision c)
    {
        bool status = false;
        //what a is doesnt need to be known, but b does as it will be the parameter
        //1. Have a test its collision based on what collisiontype b is

        if (b.type == CollisionHullType3D.hull_sphere)
        {
            if (a.TestCollisionVsCircle((SphereHull3D)b, ref c))
            {
                float coeffOfRest = 0.69f;
                c.a = a;
                c.b = b;

                c.status = true;
                status = true;
                if (a.isTrigger || b.isTrigger)
                {
                    c.isTrigger = true;
                }
                else//actual collision so resolution needs to happen
                {
                    //****fill in collision data because it passed****
                    // seperation velocity = diff of velocity * contact normal

                    Vector3 contactNormal = (a.particle.position - b.particle.position).normalized;
                    float closingVelocityMagnitude = Vector3.Dot((a.particle.velocity - b.particle.velocity), contactNormal);

                    c.closingVelocity = closingVelocityMagnitude;

                    c.contact[c.contactCount].normal = contactNormal;
                    c.contact[c.contactCount].restitution = coeffOfRest;
                }
            }

            else
            {
                c.status = false;
                status = false;
            }

        }
        else if (b.type == CollisionHullType3D.hull_aabb)
        {
            if (a.TestCollisionVsAABB((AxisAllignedBoxHull3D)b, ref c))
            {
                float coeffOfRest = 0.69f;
                c.a = a;
                c.b = b;

                c.status = true;
                status = true;
                if (a.isTrigger || b.isTrigger)
                {
                    c.isTrigger = true;
                }
                else//actual collision so resolution needs to happen
                {
                    //****fill in collision data because it passed****
                    // seperation velocity = diff of velocity * contact normal
                    Vector3 closestPoint = b.GetComponent<Renderer>().bounds.ClosestPoint(a.particle.position);

                    Vector3 contactNormal = (a.particle.position - closestPoint).normalized;

                    float closingVelocityMagnitude = Vector3.Dot((a.particle.velocity - b.particle.velocity), contactNormal);

                    c.closingVelocity = closingVelocityMagnitude;

                    c.contact[c.contactCount].normal = contactNormal;
                    c.contact[c.contactCount].restitution = coeffOfRest;

                    //Debug.Log("collision");
                }
            }
            else
            {
                c.status = false;
                status = false;
            }

        }
        else if (b.type == CollisionHullType3D.hull_obb)
        {
            if (a.TestCollisionVsOBB((ObjectBoundingBoxHull3D)b, ref c))
            {
                //Debug.Log("collision");
                c.status = true;
                status = true;
                if (a.isTrigger || b.isTrigger)
                {
                    c.isTrigger = true;
                }
                else//actual collision so resolution needs to happen
                {
                    Debug.Log("resolve");
                }
            }
            else
            {
                status = false;
            }
        }
        else if (b.type == CollisionHullType3D.hull_complex)
        {
            if (a.TestCollisionVsComplex((ComplexHull3D)b, ref c))
            {
                c.status = true;
                status = true;
                if (a.isTrigger || b.isTrigger)
                {
                    c.isTrigger = true;
                }
                else//actual collision so resolution needs to happen
                {
                    Debug.Log("resolve");
                }
            }
            else
            {
                status = false;
            }
        }


        if(status)
         {
            //Debug.Log("collision");
            //a.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            //b.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

         }
         else
         {
             //a.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
             //b.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
         }
        return status;
    }

    /*
    public abstract bool TestCollisionVsCircle(CircleHull3D other, ref Collision c);

    public abstract bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull3D other, ref Collision c);

    public abstract bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c);
    */
    public abstract bool TestCollisionVsCircle(SphereHull3D other, ref Collision c);

    public abstract bool TestCollisionVsAABB(AxisAllignedBoxHull3D other, ref Collision c);

    public abstract bool TestCollisionVsOBB(ObjectBoundingBoxHull3D other, ref Collision c);

    public abstract bool TestCollisionVsComplex(ComplexHull3D other, ref Collision c);
}
