using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle3D : MonoBehaviour
{
    public float rotationForce = 10.0f;
    public float movementForce = 2.0f;
    public Transform rightThruster, leftThruster, topThruster, bottomThruster;
    public enum ParticleShape
    {
        S_SPHERE,
        H_SPHERE,
        S_BOX,
        H_BOX,
        S_CUBE,
        H_CUBE,
        S_CYLINDER,
        S_CONE
    };

    //step 1
    public Vector3 position, velocity, acceleration;
    public Quaternion rotation;
    public Vector3 angularVelocity, angularAcceleration, torque;
    public ParticleShape shape;

    Matrix4x4 worldTransformMatrix, worldTransformMatrixInv;

    public Vector3 localCenterOfMass, worldCenterOfMass;

    public float startingMass = 1.0f;
    float mass, massInv;

    public bool freezeXRot, freezeYRot, freezeZRot;
    //3D matrix local and world inertia tensors, change world in update with change of basis
    Matrix4x4 localInertiaTensor, worldInertiaTensor, inverseLocalIT, inverseWorldIT;

    public float density = 1.2f;//air density
    public float dragCoeff; //depends on shape so not set
    float crossSectionArea = 0.0f; //also depends on shape
    float kineticFrictionCoeff = 1.0f;//copper vs.copper https://www.engineersedge.com/coeffients_of_friction.htm

    public void SetMass(float newMass)
    {
        mass = newMass > 0.0f ? newMass : 0.0f;
        //mass = Mathf.Max(0.0f, newMass);
        massInv = mass > 0.0f ? 1.0f / mass : 0.0f; //prevent infinity
        //calculate MOI

        Matrix4x4 inertiaTensor = Matrix4x4.identity;
        //inertia tensors for shapes gotten from slides, fractions converted to decimals to avoid un-needed division
        switch (shape)
        {
            case ParticleShape.S_SPHERE:
                float sRadius = .5f * transform.localScale.x;

                inertiaTensor.m00 = .4f * mass * sRadius * sRadius;
                inertiaTensor.m11 = .4f * mass * sRadius * sRadius;
                inertiaTensor.m22 = .4f * mass * sRadius * sRadius;
                dragCoeff = .47f;
                crossSectionArea = Mathf.PI * sRadius * sRadius;
                break;
            case ParticleShape.H_SPHERE:
                float hRadius = .5f * transform.localScale.x;

                inertiaTensor.m00 = .6667f * mass * hRadius * hRadius;
                inertiaTensor.m11 = .6667f * mass * hRadius * hRadius;
                inertiaTensor.m22 = .6667f * mass * hRadius * hRadius;
                break;
            case ParticleShape.S_BOX:
                float sWidth = transform.localScale.x;
                float sHeight = transform.localScale.y;
                float sDepth = transform.localScale.z;

                inertiaTensor.m00 = .0833f * mass * (sHeight * sHeight + sDepth * sDepth);
                inertiaTensor.m11 = .0833f * mass * (sDepth * sDepth + sWidth * sWidth);
                inertiaTensor.m22 = .0833f * mass * (sWidth * sWidth + sHeight * sHeight);
                break;
            case ParticleShape.H_BOX:
                float hWidth = transform.localScale.x;
                float hHeight = transform.localScale.y;
                float hDepth = transform.localScale.z;

                inertiaTensor.m00 = 1.6667f * mass * (hHeight * hHeight + hDepth * hDepth);
                inertiaTensor.m11 = 1.6667f * mass * (hDepth * hDepth + hWidth * hWidth);
                inertiaTensor.m22 = 1.6667f * mass * (hWidth * hWidth + hHeight * hHeight);
                break;
            case ParticleShape.S_CUBE:
                float sSideSize = transform.localScale.x;

                //I = 1/6 m* sideSize^2
                inertiaTensor.m00 = .1667f * mass * sSideSize * sSideSize;
                inertiaTensor.m11 = .1667f * mass * sSideSize * sSideSize;
                inertiaTensor.m22 = .1667f * mass * sSideSize * sSideSize;
                break;
            case ParticleShape.H_CUBE:
                float hSideSize = transform.localScale.x;

                //I = 5/3 m* sideSize^2  https://physics.stackexchange.com/questions/105229/tensor-of-inertia-of-a-hollow-cube/105234
                inertiaTensor.m00 = 1.6667f * mass * hSideSize * hSideSize;
                inertiaTensor.m11 = 1.6667f * mass * hSideSize * hSideSize;
                inertiaTensor.m22 = 1.6667f * mass * hSideSize * hSideSize;
                break;
            case ParticleShape.S_CYLINDER:
                float cyRadius = .5f * transform.localScale.x;
                float cyHeight = transform.localScale.y;

                inertiaTensor.m00 = .0833f * mass * (3f * cyRadius * cyRadius + cyHeight * cyHeight);
                inertiaTensor.m11 = .0833f * mass * (3f * cyRadius * cyRadius + cyHeight * cyHeight);
                inertiaTensor.m22 = .5f * mass * cyRadius * cyRadius;
                break;
            case ParticleShape.S_CONE:
                float coRadius = .5f * transform.localScale.x;
                float coHeight = transform.localScale.y;

                inertiaTensor.m00 = .6f * mass * coHeight * coHeight + .15f * mass * coRadius * coRadius;
                inertiaTensor.m11 = .6f * mass * coHeight * coHeight + .15f * mass * coRadius * coRadius;
                inertiaTensor.m22 = .3f * mass * coRadius * coRadius;
                break;
            default:
                break;
        }
        localInertiaTensor = inertiaTensor;
        inverseLocalIT = localInertiaTensor.inverse;
        //inertiaInv = momentOfIntertia > 0.0f ? 1.0f / momentOfIntertia : 0.0f;
    }

    public void updateWorldInertiaTensor()
    {
        //IWorld = R * I^-1 * R^-1
        //uses orientation of transform matrix to calc 
        inverseWorldIT = worldTransformMatrix * inverseLocalIT * worldTransformMatrixInv;
    }

    public float GetMass()
    {
        return mass;
    }
    public float GetInverseMass()
    {
        return massInv;
    }

    public Matrix4x4 getWorldTransform()
    {
        return worldTransformMatrix;
    }

    public Matrix4x4 getWorldInverseTransform()
    {
        return worldTransformMatrixInv;
    }

    //step 2 
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

    Vector3 force;
    public void AddForce(Vector3 newForce)
    {
        //D'Alembert
        force += newForce;
    }

    public void AddTorque(Vector3 newTorque)
    {
       torque += newTorque;
    }

    void UpdateAcceleration()
    {
        //Newton2
        acceleration = force * massInv;
        force.Set(0.0f, 0.0f, 0.0f);//clear forces
    }
    void UpdateAngularAcceleration()
    {
        //World Interia Tensor Inv * world Torque, multiplication uses rotation part of transformation matrix
        angularAcceleration = inverseWorldIT * torque;
        torque.Set(0.0f, 0.0f, 0.0f);//clear torque
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
        Quaternion newRotation = VectorTimesQuaternion(angularVelocity,rotation);
        rotation = quaternionPlusQuaternion(quaternionTimesScalar(newRotation, dt * 0.5f), rotation);
        angularVelocity += angularAcceleration * dt;
    }

    void updateRotationKinematic(float dt)
    {
        //same as position but with rotation variables
        //rotation += (angularVelocity * dt) + (.5f * angularAcceleration * dt * dt);

        angularVelocity += angularAcceleration * dt;
    }

    // Start is called before the first frame update
    void Start()
    {
        //set local variables in start
        localCenterOfMass = new Vector3(0.0f, 0.0f, 0.0f);
        SetMass(startingMass);
        updateWorldTransformMatrix(rotation, position);
        if (tag == "Boi")//initial force to wall to get it to move
        {
            Vector3 boiForce = ForceGenerator3D.GenerateForceInAxisDirection(1000.0f, -transform.forward);
            AddForce(boiForce);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //step 3
        //updating position
        updatePositionKinematic(Time.fixedDeltaTime);
        updateRotationEulerExplicit(Time.fixedDeltaTime);
        UpdateAcceleration();
        UpdateAngularAcceleration();

        transform.position = position;

        //local com transformed to world space, uses position part of transformation matrix
        worldCenterOfMass = worldTransformMatrix.MultiplyPoint3x4(localCenterOfMass);

        rotation.Normalize();
        if(freezeXRot)
        {
            rotation.x = 0.0f;//prevent x axis rotation
        }
        if (freezeYRot)
        {
            rotation.y = 0.0f;//prevent y axis rotation
        }
        if (freezeZRot)
        {
            rotation.z = 0.0f;//prevent z axis rotation
        }
        transform.rotation = rotation;

        updateWorldTransformMatrix(rotation, position);
        updateWorldInertiaTensor();

        if (tag == "Player")
        {
            Vector3 drag = ForceGenerator3D.GenerateForce_drag(velocity, density, crossSectionArea, dragCoeff);
            AddForce(drag);
            //Vector3 babyGrav = ForceGenerator3D.GenerateForce_Gravity(mass, -.1f, Vector3.up);
            //AddForce(babyGrav);
            CheckInput();
        }
        if (tag == "Boi")//used for ground wall that moves
        {
            Vector3 boiGravity = ForceGenerator3D.GenerateForce_Gravity(mass, -9.8f, Vector3.up);
            Vector3 boiNormal = ForceGenerator3D.GenerateForce_normal(boiGravity, 9.8f * Vector3.up);
            Vector3 boiFriction = ForceGenerator3D.GenerateForce_friction_kinetic(boiNormal, velocity, kineticFrictionCoeff);

            AddForce(boiGravity);
            AddForce(boiNormal);
            AddForce(boiFriction);
        }

    }
    Quaternion quaternionTimesScalar(Quaternion q, float s)
    {
        q.x *= s;
        q.y *= s;
        q.z *= s;
        q.w *= s;

        return q;
    }
    Quaternion VectorTimesQuaternion(Vector3 v, Quaternion q)
    {
        Vector3 quaternionValues = new Vector3(q.x, q.y, q.z);
        Vector3 newQuaternionXYZ = q.w * v + Vector3.Cross(v, quaternionValues);
        Quaternion output = new Quaternion(newQuaternionXYZ.x,newQuaternionXYZ.y,newQuaternionXYZ.z,-Vector3.Dot(v,quaternionValues));
        return output;
    }

    Quaternion quaternionPlusQuaternion(Quaternion q1, Quaternion q2)
    {
        Quaternion newQ = new Quaternion(q1.x+q2.x,q1.y+q2.y,q1.z+q2.z,q1.w+q2.w);
        return newQ;
    }

    void updateWorldTransformMatrix(Quaternion rot, Vector3 pos)
    {
        //rotation matrix in top left, position in right column
        //pg.191 in book rotation matrix from quaternion
        Matrix4x4 matrix = Matrix4x4.identity; //for bottom row to be 0 0 0 1
        matrix.m00 = 1 - (2 * rot.y * rot.y + 2 * rot.z * rot.z);
        matrix.m01 = 2 * rot.x * rot.y + 2 * rot.z * rot.w;
        matrix.m02 = 2 * rot.x * rot.z - 2 * rot.y * rot.w;
        matrix.m03 = position.x;
        matrix.m10 = 2 * rot.x * rot.y - 2 * rot.z * rot.w;
        matrix.m11 = 1 - (2 * rot.x * rot.x + 2 * rot.z * rot.z);
        matrix.m12 = 2 * rot.y * rot.z + 2 * rot.x * rot.w;
        matrix.m13 = position.y;
        matrix.m20 = 2 * rot.x * rot.z + 2 * rot.y * rot.w;
        matrix.m21 = 2 * rot.y * rot.z - 2 * rot.x * rot.w;
        matrix.m22 = 1 - (2 * rot.x * rot.x + 2 * rot.y * rot.y);
        matrix.m23 = position.z;

        worldTransformMatrix = matrix;
        worldTransformMatrixInv = inverseWorldTransform(worldTransformMatrix);
    }

    Matrix4x4 inverseWorldTransform(Matrix4x4 worldTransform)
    {
        //deck4 slide 8, inverse of rotation matrix is transpose, inverse of positions will be the negatives
        Matrix4x4 inverse = worldTransform.transpose; //3x3 rotation of inverse will be from this

        //positions from transpose are in bottom row, so get rid of those
        inverse.m30 = 0;
        inverse.m31 = 0;
        inverse.m32 = 0;

        //transform of transform matrix becomes -R^-1 * position, multiply vector uses only rotation part
        Vector3 inverseTransform = inverse.MultiplyVector(position);

        //negative positions of inverse will be in old positions
        inverse.m03 = -inverseTransform.x;
        inverse.m13 = -inverseTransform.y;
        inverse.m23 = -inverseTransform.z;

        return inverse;
    }

    void CheckInput()
    {
        if (Input.GetKey(KeyCode.S))
        {
            //rotate backwards
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(rotationForce, -topThruster.forward);
            Vector3 pointOfForce = topThruster.position;

            torque = ForceGenerator3D.GenerateTorque(pointOfForce, newForce, worldCenterOfMass);
            AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.W))
        {
            //rotate forward
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(rotationForce, topThruster.forward);
            Vector3 pointOfForce = topThruster.position;

            torque = ForceGenerator3D.GenerateTorque(pointOfForce, newForce, worldCenterOfMass);
            AddTorque(torque);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //move ship forward
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(movementForce, topThruster.up);
            AddForce(newForce);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            //move ship back
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(movementForce, -topThruster.up);
            AddForce(newForce);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //move ship right
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(movementForce, leftThruster.right);
            AddForce(newForce);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            //move ship left
            Vector3 newForce = ForceGenerator3D.GenerateForceInAxisDirection(movementForce, -rightThruster.right);
            AddForce(newForce);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            //reset orientation and velocity in case stuff gets too crazy and can't correct it easily
            rotation.x = 0.0f;
            rotation.y = 0.0f;
            rotation.z = 0.0f;
            velocity.x = 0.0f;
            velocity.y = 0.0f;
            velocity.z = 0.0f;
            angularVelocity.x = 0.0f;
            angularVelocity.y = 0.0f;
            angularVelocity.z = 0.0f;
        }
    }
}
