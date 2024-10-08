using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceGenerator
{
    public static Vector2 GenerateForceInAxisDirection(float forceAmount, Vector2 direction)
    {
        Vector2 f_direction = direction * forceAmount;
        return f_direction;
    }
    public static Vector2 GenerateForce_Gravity(float particleMass, float gravitationConstant, Vector2 worldUp)
    {
        //f_gravity: f = mg
        Vector2 f_gravity = particleMass * gravitationConstant * worldUp;
        return f_gravity;
    }

    public static Vector2 GenerateForce_normal(Vector2 f_gravity, Vector2 surfaceNormal_unit)
    {
        // f_normal = proj(f_gravity, surfaceNormal_unit)
        Vector2 f_normal = -Vector3.Project(f_gravity, surfaceNormal_unit);
        return f_normal;
    }

    public static Vector2 GenerateForce_sliding(Vector2 f_gravity, Vector2 f_normal)
    {
        // f_sliding = f_gravity + f_normal
        Vector2 f_sliding = f_gravity + f_normal;
        return f_sliding;
    }

    public static Vector2 GenerateForce_friction_static(Vector2 f_normal, Vector2 f_opposing, float frictionCoefficient_static)
    {
        // f_friction_s = -f_opposing if less than max, else -coeff*f_normal (max amount is coeff*|f_normal|)
        Vector2 f_friction_s;
        float max = frictionCoefficient_static * f_normal.magnitude;
        //check if force is less than max needed to move
        if (f_opposing.magnitude < max)
        {
            f_friction_s = -f_opposing;
        }
        else //if enough to move
        {
            f_friction_s = -frictionCoefficient_static * max * -f_opposing.normalized;
        }
        return f_friction_s;
    }

    public static Vector2 GenerateForce_friction_kinetic(Vector2 f_normal, Vector2 particleVelocity, float frictionCoefficient_kinetic)
    {
        // f_friction_k = -coeff*|f_normal| * unit(vel)
        Vector2 f_friction_k = -frictionCoefficient_kinetic * f_normal.magnitude * particleVelocity.normalized;
        return f_friction_k;
    }

    public static Vector2 GenerateForce_drag(Vector2 particleVelocity, float fluidDensity, float objectArea_crossSection, float objectDragCoefficient)
    {
        // f_drag = (p * u^2 * area * coeff)/2
        float dragMag = 0.5f * (fluidDensity * particleVelocity.magnitude * particleVelocity.magnitude * objectArea_crossSection * objectDragCoefficient);
        Vector2 f_drag = dragMag * -particleVelocity.normalized;

        return f_drag;
    }

    public static Vector2 GenerateForce_spring(Vector2 particlePosition, Vector2 anchorPosition, Vector2 springRestingLength, float springStiffnessCoefficient)
    {
        // f_spring = -coeff*(spring length - spring resting length)
        Vector2 f_spring;
        Vector2 springLength = particlePosition - anchorPosition; //length from anchor
        if (springLength.magnitude - springRestingLength.magnitude > 0)
        {
            f_spring = -springStiffnessCoefficient * (springLength - springRestingLength);
        }
        else //no force if particle would be behind the spring
        {
            f_spring = new Vector2(0f, 0f);
        }

        return f_spring;
    }

    public static float GenerateTorque(Vector2 forcePoint, Vector2 force, Vector2 centerOfMass)
    {
        float torque;
        Vector2 momentArm = forcePoint - centerOfMass;

        torque = momentArm.x * force.y - momentArm.y * force.x;
        return torque;
    }
}
