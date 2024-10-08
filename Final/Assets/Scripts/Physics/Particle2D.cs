using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle2D : MonoBehaviour
{
    public float rotationForce = 10.0f;
    public float movementForce = 2.0f;
    public Transform rightThruster, leftThruster, topThruster, bottomThruster;
    public enum ParticleShape
    {
        SQUARE,
        BOX,
        CIRCLE
    };

    public ParticleShape shape;
    //spin around z float math
    // lab1 step 1
    public Vector2 position, velocity, acceleration;
    public float angularAcceleration, angularVelocity, rotation;


    //lab2 step 1
    public float startingMass = 1.0f;
    float mass, massInv;

    float momentOfIntertia, inertiaInv;

    public Vector2 localCenterOfMass;
    Vector2 worldCenterOfMass;

    public void SetMass(float newMass)
    {
        mass = newMass > 0.0f ? newMass : 0.0f;
        //mass = Mathf.Max(0.0f, newMass);
        massInv = mass > 0.0f ? 1.0f / mass : 0.0f; //prevent infinity

        //calculate MOI

        // box/square = 1/12 * m(Dx^2 + Dy^2)
        // circle = .5 * m * r^2

        switch (shape)
        {
            case ParticleShape.BOX:
                float xDistance = transform.localScale.x;
                float yDistance = transform.localScale.y;
                float bInertia = (1.0f / 12.0f) * mass * ((xDistance * xDistance) + (yDistance * yDistance));
                momentOfIntertia = bInertia;
                break;
            case ParticleShape.SQUARE:
                float sideLength = transform.localScale.x;
                float sInertia = (1.0f / 12.0f) * mass * (2.0f * (sideLength * sideLength));
                momentOfIntertia = sInertia;
                break;
            case ParticleShape.CIRCLE:
                float radius = .5f * transform.localScale.x;
                float cInertia = .5f * mass * radius * radius;
                momentOfIntertia = cInertia;
                break;
            default:
                break;
        }
        inertiaInv = momentOfIntertia > 0.0f ? 1.0f / momentOfIntertia : 0.0f;
    }

    public float GetMass()
    {
        return mass;
    }

    public float GetInverseMass()
    {
        return massInv;
    }

    //lab2 step 2
    Vector2 force;
    public void AddForce(Vector2 newForce)
    {
        //D'Alembert
        force += newForce;
    }

    float torque;
    public void AddTorque(float newTorque)
    {
        torque += newTorque;
    }

    void UpdateAcceleration()
    {
        //Newton2
        acceleration = force * massInv;

        force.Set(0.0f, 0.0f);//clear forces
    }

    void UpdateAngularAcceleration()
    {
        //Debug.Log(shape + " torque:" + torque);
        angularAcceleration = torque * inertiaInv;
        torque = 0.0f;
    }

    //lab1 step 2 
    void updatePositionEulerExplicit(float dt)
    {
        // x(t+dt) = x(t) + v(t)dt
        // Euler's method:
        // F(t+dt) = F(t) + f(t)dt
        //                + (dF/dt)dt
        position += velocity * dt;

        // v(t+dt) = v(t) + a(t)dt
        velocity += acceleration * dt;

    }

    void updatePositionKinematic(float dt)
    {
        //x(t+dt) = x(t) + v(t)dt + .5*a(t)(dt*dt)
        position += (velocity * dt) + (.5f * acceleration * dt * dt);

        //v(t+dt) = v(t) + a(t)dt
        velocity += acceleration * dt;
    }

    void updateRotationEulerExplicit(float dt)
    {
        //same as position but with rotation variables
        rotation += angularVelocity * dt;
        //keep rotation between -pi and pi
        rotation = Mathf.Clamp(rotation, 0.5f * -Mathf.PI * Mathf.Rad2Deg, 0.5f * Mathf.PI * Mathf.Rad2Deg);

        angularVelocity += angularAcceleration * dt;
    }

    void updateRotationKinematic(float dt)
    {
        //same as position but with rotation variables
        rotation += (angularVelocity * dt) + (.5f * angularAcceleration * dt * dt);

        float min = -Mathf.PI * Mathf.Rad2Deg;
        float max = Mathf.PI * Mathf.Rad2Deg;
        rotation = Mathf.Clamp(rotation, min, max);
        if (rotation <= min || rotation >= max)
        {
            angularAcceleration = 0f;
            angularVelocity = 0;
        }

        angularVelocity += angularAcceleration * dt;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMass(startingMass);
        worldCenterOfMass = position + localCenterOfMass;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //lab1 step 3
        updatePositionKinematic(Time.fixedDeltaTime);
        updateRotationKinematic(Time.fixedDeltaTime);
        UpdateAcceleration();
        UpdateAngularAcceleration();

        transform.position = position;
        worldCenterOfMass = position + localCenterOfMass;

        transform.eulerAngles = new Vector3(0.0f, 0.0f, rotation);

        if (tag == "Player")
        {
            CheckInput();
        }
        //lab1 step 4
        //acceleration.x = -Mathf.Sin(Time.fixedTime);

        //lab2 step 4
        //f_gravity: f = mg
        //Vector2 f_gravity = mass * new Vector2(0.0f, -9.8f);
        //AddForce(f_gravity);

    }

    void CheckInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //rotate CW by generating torque on right thruster location
            Vector2 newForce = ForceGenerator.GenerateForceInAxisDirection(rotationForce, -rightThruster.up);
            Vector2 pointOfForce = rightThruster.position;

            torque = ForceGenerator.GenerateTorque(pointOfForce, newForce, worldCenterOfMass);
            AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //rotate CCW by generating torque on left thruster location
            Vector2 newForce = ForceGenerator.GenerateForceInAxisDirection(rotationForce, -leftThruster.up);
            Vector2 pointOfForce = leftThruster.position;

            torque = ForceGenerator.GenerateTorque(pointOfForce, newForce, worldCenterOfMass);
            AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //move ship in relative "down" direction
            Vector2 newForce = ForceGenerator.GenerateForceInAxisDirection(movementForce, -topThruster.up);
            AddForce(newForce);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //move ship in relative "up" direction
            Vector2 newForce = ForceGenerator.GenerateForceInAxisDirection(movementForce, bottomThruster.up);
            AddForce(newForce);
        }
    }
}
