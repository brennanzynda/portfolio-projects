using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour {

    public List<CollisionHull2D> particles;
	// Use this for initialization
	void Start () {
        
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
                CollisionHull2D.Collision collision = new CollisionHull2D.Collision();
                if (CollisionHull2D.TestCollision(particles[i], particles[j], ref collision))
                {
                    if (collision.status)
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
                        //Vec2 impulsePerIMass = collision.contactNormal * impulse
                        float totalInvMass = particles[i].GetComponent<Particle2D>().GetInverseMass() + particles[j].GetComponent<Particle2D>().GetInverseMass();
                        float impulse = deltaVel / totalInvMass;
                        Vector2 impulsePerIMass = collision.contact[collision.contactCount].normal * impulse;

                        //i: velocity += impulsePerIMass * inverseMass
                        particles[i].GetComponent<Particle2D>().velocity += impulsePerIMass * particles[i].GetComponent<Particle2D>().GetInverseMass();

                        //j: velocity += impulsePerImass * -inverseMass
                        particles[j].GetComponent<Particle2D>().velocity += impulsePerIMass * -particles[j].GetComponent<Particle2D>().GetInverseMass();

                        //for interpenetration,  movePerIMass = contactNormal * (penetration / totalInverseMass)
                        if ((collision.contact[collision.contactCount].interpenetrationDepth > 0))
                        {
                            Vector2 movePerIMass = collision.contact[collision.contactCount].normal * (collision.contact[collision.contactCount].interpenetrationDepth / totalInvMass);
  
                            //i: particleMovement = movePerIMass* i.inverseMass
                            //i.position+=particleMovement
                            Vector2 particleMovement = movePerIMass * particles[i].GetComponent<Particle2D>().GetInverseMass();
                            particles[i].GetComponent<Particle2D>().position += particleMovement;

                            ////j: particleMovement = movePerIMass* -j.inverseMass
                            //j.position+=particleMovement
                            particleMovement = movePerIMass * -particles[j].GetComponent<Particle2D>().GetInverseMass();
                            particles[j].GetComponent<Particle2D>().position += particleMovement;
                        }
                    }

                }
            }
        }
    }
}
