using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator3D
{
    public static Vector3 GenerateForce_Gravity(float particleMass, float gravitationConstant, Vector3 worldUp)
    {
        //f_gravity: f = mg
        Vector3 f_gravity = particleMass * gravitationConstant * worldUp;
        return f_gravity;
    }

    public static Vector3 GenerateForceInAxisDirection(float forceAmount, Vector3 direction)
    {
        Vector3 f_direction = direction * forceAmount;
        return f_direction;
    }

    public static Vector3 GenerateForce_normal(Vector3 f_gravity, Vector3 surfaceNormal_unit)
    {
        // f_normal = proj(f_gravity, surfaceNormal_unit)
        Vector2 f_normal =  -Vector3.Project(f_gravity,surfaceNormal_unit);
        return f_normal;
    }

    public static Vector3 GenerateForce_sliding(Vector3 f_gravity, Vector3 f_normal)
    {
        // f_sliding = f_gravity + f_normal
        Vector3 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }

    public static Vector3 GenerateForce_friction_static(Vector3 f_normal, Vector3 f_opposing, float frictionCoefficient_static)
    {
        // f_friction_s = -f_opposing if less than max, else -coeff*f_normal (max amount is coeff*|f_normal|)
        Vector3 f_friction_s;
        float max = frictionCoefficient_static * f_normal.magnitude;
        //check if force is less than max needed to move
        if(f_opposing.magnitude < max)
        {
            f_friction_s = -f_opposing;
        }
        else //if enough to move
        {
            f_friction_s = -frictionCoefficient_static * max * -f_opposing.normalized;
        }
        return f_friction_s;
    }

    public static Vector3 GenerateForce_friction_kinetic(Vector3 f_normal, Vector3 particleVelocity, float frictionCoefficient_kinetic)
    {
        // f_friction_k = -coeff*|f_normal| * unit(vel)
        Vector3 f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity.normalized;
        return f_friction_k;
    }

    public static Vector3 GenerateForce_drag(Vector3 particleVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        // f_drag = (p * u^2 * area * coeff)/2
        float dragMag = 0.5f * (fluidDensity * particleVelocity.magnitude * particleVelocity.magnitude * objectArea_crossSection * objectDragCoefficient);
        Vector3 f_drag = dragMag * -particleVelocity.normalized;

        return f_drag;
    }

    public static Vector3 GenerateForce_spring(Vector3 particlePosition, Vector3 anchorPosition, Vector3 springRestingLength, float springStiffnessCoefficient)
    {
        // f_spring = -coeff*(spring length - spring resting length)
        Vector3 f_spring;
        Vector3 springLength = particlePosition - anchorPosition; //length from anchor
        if(springLength.magnitude-springRestingLength.magnitude > 0)
        {
           f_spring = -springStiffnessCoefficient * (springLength - springRestingLength);
        }
        else //no force if particle would be behind the spring
        {
            f_spring = new Vector3(0.0f, 0.0f, 0.0f);
        }
        
        return f_spring;
    }

    public static Vector3 GenerateTorque(Vector3 forcePoint, Vector3 force, Vector3 centerOfMass)
    {
        //momentArm = forcePoint - worldCOM
        //torque = momentArm X Force
        Vector3 torque;
        Vector3 momentArm = forcePoint - centerOfMass;
        torque = Vector3.Cross(momentArm, force);
        return torque;
    }
}
