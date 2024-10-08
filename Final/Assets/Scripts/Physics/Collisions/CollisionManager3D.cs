using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*TO DO:
    4 force generators being used at any point
    collision detection for sphere,aabb,obb,and complex hull(multiple in one, cylinder, rays)
    collision response(contact and normals, physical response between spheres, one trigger response)
*/
public class CollisionManager3D : MonoBehaviour {

    public List<CollisionHull3D> particles;
    public Text tmp;
    ScoreHandler scoreHandler;
	// Use this for initialization
	void Start ()
    {
        scoreHandler = tmp.GetComponent<ScoreHandler>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        CheckForCollisions();
	}

    private void CheckForCollisions()
    {
        //loop through list checking collisions with each. Not sure if this is efficient or not
        for(int i = 0; i< particles.Count; i++)
        {
            particles[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
        }
        for (int i = 0; i < particles.Count - 1; i++)
        {
            for (int j = i + 1; j < particles.Count; j++)
            {
                CollisionHull3D.Collision collision = new CollisionHull3D.Collision();
                if (CollisionHull3D.TestCollision(particles[i], particles[j],ref collision))
                {
                    if (collision.status)
                    {
                        if (!collision.isTrigger)//colliding and not a trigger
                        {
                            particles[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                            particles[j].gameObject.GetComponent<MeshRenderer>().material.color = Color.green;
                            //do collision response because collision passed

                            //newSepVelocity = -separating * restitution
                            //deltaVel = newSep - separating
                            float newSepVelocity = -collision.closingVelocity * collision.contact[collision.contactCount].restitution;
                            float deltaVel = newSepVelocity - collision.closingVelocity;

                            //total invMass = i.inverseMass + j.inverseMass
                            //impulse = deltaVel/totalInverse
                            //Vec3 impulsePerIMass = collision.contactNormal * impulse
                            float totalInvMass = particles[i].GetComponent<Particle3D>().GetInverseMass() + particles[j].GetComponent<Particle3D>().GetInverseMass();
                            float impulse = deltaVel / totalInvMass;
                            Vector3 impulsePerIMass = collision.contact[collision.contactCount].normal * impulse;

                            //i: velocity += impulsePerIMass * inverseMass
                            particles[i].GetComponent<Particle3D>().velocity += impulsePerIMass * particles[i].GetComponent<Particle3D>().GetInverseMass();

                            //j: velocity += impulsePerImass * -inverseMass
                            particles[j].GetComponent<Particle3D>().velocity += impulsePerIMass * -particles[j].GetComponent<Particle3D>().GetInverseMass();

                            //for interpenetration,  movePerIMass = contactNormal * (penetration / totalInverseMass)
                            if ((collision.contact[collision.contactCount].interpenetrationDepth > 0))
                            {
                                Vector3 movePerIMass = collision.contact[collision.contactCount].normal * (collision.contact[collision.contactCount].interpenetrationDepth / totalInvMass);

                                //i: particleMovement = movePerIMass* i.inverseMass
                                //i.position+=particleMovement
                                Vector3 particleMovement = movePerIMass * particles[i].GetComponent<Particle3D>().GetInverseMass();
                                particles[i].GetComponent<Particle3D>().position += particleMovement;

                                ////j: particleMovement = movePerIMass* -j.inverseMass
                                //j.position+=particleMovement
                                particleMovement = movePerIMass * -particles[j].GetComponent<Particle3D>().GetInverseMass();
                                particles[j].GetComponent<Particle3D>().position += particleMovement;
                            }
                        }
                        else//trigger response
                        {
                            //Debug.Log("trigger");
                            if (particles[i].gameObject.tag == "Capsule" || particles[j].gameObject.tag == "Capsule")
                            {
                                scoreHandler.AddScore();
                            }
                            particles[i].gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                            particles[j].gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                    }
                }
            }
        }
    }
}
