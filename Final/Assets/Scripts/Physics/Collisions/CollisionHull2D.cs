using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionHull2D : MonoBehaviour
{
    public class Collision
    {
        //https://en.wikipedia.org/wiki/Coefficient_of_restitution used for coefficient of restitution value (glass(soda-lime)= .69)
        public struct Contact
        {
            public float interpenetrationDepth;
            public Vector2 normal;
            public float restitution;
        }

        //collision
        public CollisionHull2D a = null, b = null;
        public Contact[] contact = new Contact[4];
        public int contactCount = 0;
        public bool status = false;

        //resolution
        public float closingVelocity;
    }

    public enum CollisionHullType2D
    {
        hull_circle,
        hull_aabb,
        hull_obb,
    }
    private CollisionHullType2D type { get; }

    protected CollisionHull2D(CollisionHullType2D type_set)
    {
        type = type_set;
    }

    protected Particle2D particle;



    // Start is called before the first frame update
    void Start()
    {
        particle = GetComponent<Particle2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public bool TestCollision(CollisionHull2D a, CollisionHull2D b, ref Collision c)
    public static bool TestCollision(CollisionHull2D a, CollisionHull2D b, ref Collision c)
    {
        bool status = false;
        //what a is doesnt need to be known, but b does as it will be the parameter
        //1. Have a test its collision based on what collisiontype b is

        if (b.type == CollisionHullType2D.hull_circle)
        {
            if(a.TestCollisionVsCircle((CircleHull2D)b, ref c))
            {
                float coeffOfRest = 0.69f;
                c.a = a;
                c.b = b;

                c.status = true;
                //****fill in collision data because it passed****
                // seperation velocity = diff of velocity * contact normal

                Vector2 contactNormal = (a.particle.position - b.particle.position).normalized;
                float closingVelocityMagnitude = Vector2.Dot((a.particle.velocity - b.particle.velocity), contactNormal);

                c.closingVelocity =  closingVelocityMagnitude;
                
                c.contact[c.contactCount].normal = contactNormal;
                c.contact[c.contactCount].restitution = coeffOfRest;

                //Debug.Log("collision");
                status = true;
            }

            else
            {
                c.status = false;
                status = false;
            }

        }
        else if (b.type == CollisionHullType2D.hull_aabb)
        {
            if(a.TestCollisionVsAABB((AxisAllignedBoundingBoxHull2D)b, ref c))
            {
                if(a.tag == "Player")
                {
                    if (b.tag == "Wall") //if player hit wall
                    {
                        GameManager.instance.EndGame();
                    }
                    else if(b.tag == "End") //if player hits end
                    {
                        GameManager.instance.WinGame();
                    }
                }
                float coeffOfRest = 0.69f;
                c.a = a;
                c.b = b;

                c.status = true;
                //****fill in collision data because it passed****
                // seperation velocity = diff of velocity * contact normal
                Vector2 closestPoint = b.GetComponent<Renderer>().bounds.ClosestPoint(a.particle.position);

                Vector2 contactNormal = (a.particle.position - closestPoint).normalized;
                
                float closingVelocityMagnitude = Vector2.Dot((a.particle.velocity - b.particle.velocity), contactNormal);

                c.closingVelocity = closingVelocityMagnitude;

                c.contact[c.contactCount].normal = contactNormal;
                c.contact[c.contactCount].restitution = coeffOfRest;

                //Debug.Log("collision");
                status = true;
            }
            else
            {
                status = false;
            }

        }
        else if (b.type == CollisionHullType2D.hull_obb)
        {
            if(a.TestCollisionVsOBB((ObjectBoundingBoxHull2D)b, ref c))
            {
                //Debug.Log("collision");
                status = true;
            }
            else
            {
                status = false;
            }
        }


       /* if(status)
        {
            //Debug.Log("collision");
            //a.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
            //b.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
        }
        else
        {
            //a.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
            //b.gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }*/
        return status;
    }

    /*
    public abstract bool TestCollisionVsCircle(CircleHull2D other, ref Collision c);

    public abstract bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull2D other, ref Collision c);

    public abstract bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other, ref Collision c);
    */
    public abstract bool TestCollisionVsCircle(CircleHull2D other, ref Collision c);

    public abstract bool TestCollisionVsAABB(AxisAllignedBoundingBoxHull2D other, ref Collision c);

    public abstract bool TestCollisionVsOBB(ObjectBoundingBoxHull2D other, ref Collision c);
}
