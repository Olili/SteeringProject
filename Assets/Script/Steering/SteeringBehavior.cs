using System;
using System.Collections;
using UnityEngine;


public class SteeringBehavior
{
    public float factor = 1;
    public  Steering steeringComponent;
    public SteeringBehavior(Steering _steeringComponent,float _factor = 1) 
    {
        steeringComponent = _steeringComponent;
        factor = _factor;
    }

    public virtual Vector3 ComputeSteering()
    {
        throw new NotImplementedException();
    }
    public virtual void OnUpdate()
    {

    }
    public virtual void OnDrawGizmo()
    {
    }
}
public class Seek : SteeringBehavior
{
    protected Transform target;
    public Seek(Steering _steeringComponent,Transform _target,float factor =1) : base(_steeringComponent,factor)
    {
        target = _target;
    }
    public override Vector3 ComputeSteering()
    {
        return SeekForce(target.position);
    }
    public Vector3 SeekForce(Vector3 targetPos)
    {
        Vector3 desiredVelocityPlan = steeringComponent.GetDvOnPlan(targetPos);
        Vector3 force = desiredVelocityPlan - steeringComponent.Rb.velocity;

        force = Vector3.ClampMagnitude(force, steeringComponent.maxSpeed);
        return force * factor;
    }
}
public class Flee : SteeringBehavior
{
    protected Transform target;
    public Flee(Steering _steeringComponent, Transform _target,float factor = 1) : base(_steeringComponent,factor)
    {
        target = _target;
    }
    public override  Vector3 ComputeSteering()
    {
        return FleeForce(target.position);
    }
    public Vector3 FleeForce(Vector3 targetPos)
    {
        Vector3 desiredVelocityPlan = -steeringComponent.GetDvOnPlan(targetPos);
        Vector3 force = desiredVelocityPlan - steeringComponent.Rb.velocity;

        force = Vector3.ClampMagnitude(force, steeringComponent.maxSpeed);
        return force * factor;
    }
}
public class Arrival : SteeringBehavior
{
    Transform target;
    float slowingRadius;
    public Arrival(Steering _steeringComponent, Transform _target,float _slowingRadius = 5) : base(_steeringComponent)
    {
        target = _target;
        slowingRadius = _slowingRadius;
    }
    public override Vector3 ComputeSteering()
    {
        return ComputeArrivalForce(target.position);
    }
    public Vector3 ComputeArrivalForce(Vector3 position)
    {
        Vector3 desiredVelocityPlan = steeringComponent.GetDvOnPlan(position);
        float distance = Vector3.Distance(steeringComponent.CenterDown, position);

        if (distance < slowingRadius)
        {
            desiredVelocityPlan = desiredVelocityPlan * (distance / slowingRadius);
        }
        Vector3 force = desiredVelocityPlan - steeringComponent.Rb.velocity;
        force = Vector3.ClampMagnitude(force, steeringComponent.maxSpeed);
        return force * factor;
    }
}
public class Pursuit : Seek
{
    Rigidbody targetRb;
    public Pursuit(Steering _steeringComponent, Transform _target, Rigidbody _targetRb) : base(_steeringComponent,_target)
    {
        targetRb = _targetRb;
    }
    public override Vector3 ComputeSteering()
    {
        float T = 0.5f;
        Vector3 futurePosition = target.position + targetRb.velocity * T;
        return SeekForce(futurePosition);
    }
}
public class Evade : Flee
{
    Rigidbody targetRb;
    public Evade(Steering _steeringComponent, Transform _target, Rigidbody _targetRb) : base(_steeringComponent, _target)
    {
        targetRb = _targetRb;
    }
    public override Vector3 ComputeSteering()
    {
        float T = 0.5f;
        Vector3 futurePosition = target.position + targetRb.velocity * T;
        return FleeForce(futurePosition);
    }
}
public class Wander : SteeringBehavior
{
    float wanderAngle = 0;
    float wanderDelta = 30;
    public Wander(Steering _steeringComponent,float _wanderDelta = 30) : base(_steeringComponent)
    {
        wanderDelta = _wanderDelta;
    }
    /*
//         Je choisis une direction random. 
//         Je cree un cercle ayant pour centre la direction actuelle.
//         et un rayon arbitraire. 
//         Je choisis un point aléatoire dans ce cercle. 
//         Je décide de seek vers cette direction. 
//         Ou
//         Je choisis une direction random. 
//         Je cree un cercle ayant pour centre la direction actuelle.
//         et un rayon arbitraire. 
//         Je choisis un point aléatoire dans ce cercle. 
//         Ma direction actuelle devient cette direction. 
//     */
    public override Vector3 ComputeSteering()
    {
        float circleRadius = 1;
        float circleDistance = 2;
        Vector3 circleCenter = steeringComponent.CenterDown + steeringComponent.Rb.velocity.normalized * circleDistance;

        wanderAngle += UnityEngine.Random.Range(-wanderDelta, wanderDelta);
        Vector3 circleOffset = new Vector3(Mathf.Cos(wanderAngle * Mathf.Deg2Rad), 0, Mathf.Sin(wanderAngle * Mathf.Deg2Rad)) * circleRadius;
        Vector3 target = circleCenter + circleOffset;
        Vector3 desiredVelocityPlan = steeringComponent.GetDvOnPlan(target);
        Vector3 force = desiredVelocityPlan - steeringComponent.Rb.velocity;
        return  Vector3.ClampMagnitude(force, steeringComponent.maxSpeed) * factor;
    }
}
#region Boids Steering Movement 



public class BoidBehavior : SteeringBehavior
{
    float timer = 0;
    static float updateRate = 0.1f;
    protected Collider[] closeNeighbours;
    protected float separationRayFactor =1f ; // TODO: Refacto : Maybe different for each boids behavior;
    public BoidBehavior(Steering _steeringComponent) : base(_steeringComponent)
    {
    }
    public void GetNeighbouringAgents()
    {
        int separationMask = LayerMask.GetMask(new string[] { "Agent" });
        Vector3 separationForce = Vector3.zero;
        float sphereCheckRadius =steeringComponent.AgentCollExtent.magnitude * separationRayFactor;
        closeNeighbours = Physics.OverlapSphere(steeringComponent.transform.position, sphereCheckRadius, separationMask);
    }
    // TODO: USE COROUTINE / INVOQUE REPEATING
    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > updateRate)
        {
            timer = 0;
            GetNeighbouringAgents();
        }
    }
}
class Separation : BoidBehavior
{
    public Separation(Steering _steeringComponent) : base(_steeringComponent)
    {
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 separationForce = Vector3.zero;
        float sphereCheckRadius =steeringComponent.AgentCollExtent.magnitude * separationRayFactor;
        if (closeNeighbours != null)
        {
            int i;
            for (i = 0; i < closeNeighbours.Length; i++)
            {
                if (closeNeighbours[i] != null && closeNeighbours[i].transform != steeringComponent.transform)
                {
                    Vector3 vecFromOther = steeringComponent.CenterDown - closeNeighbours[i].GetComponent<Steering>().CenterDown;
                    float distance = vecFromOther.magnitude;
                    vecFromOther.Normalize();
                    if (distance != 0)
                    {
                        vecFromOther = vecFromOther * sphereCheckRadius / distance * steeringComponent.maxSpeed;
                        separationForce += vecFromOther;
                    }
                }
            }
            separationForce = Vector3.ClampMagnitude(separationForce, steeringComponent.maxSpeed);
#if UNITY_EDITOR
            steeringComponent.giz.separateForce = separationForce;
#endif
            return separationForce * factor;
        }
        else
            return Vector3.zero;
    }
}
#endregion
class Alignement : BoidBehavior
{
    Rigidbody leaderRb;
    public Alignement(Steering _steeringComponent,Rigidbody _leaderRb = null) : base(_steeringComponent)
    {
        leaderRb = _leaderRb;
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 averageDirection = Vector3.zero;
        if (leaderRb == null)
            for (int i = 0; i < closeNeighbours.Length; i++)
            {
                if (closeNeighbours[i].transform !=steeringComponent.transform)
                    averageDirection += closeNeighbours[i].attachedRigidbody.velocity;
            }
        else
        {
            averageDirection = leaderRb.velocity;
        }
        return (averageDirection.normalized) * steeringComponent.maxSpeed * factor;
    }
}
class Cohesion : BoidBehavior
{
    Seek seekSteer;
    public Cohesion(Steering _steeringComponent) : base(_steeringComponent)
    {
        seekSteer = new Seek(_steeringComponent,null);
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 averagePosition = Vector3.zero;
        for (int i = 0; i < closeNeighbours.Length; i++)
        {
            averagePosition += closeNeighbours[i].transform.position;
        }

        return seekSteer.SeekForce(averagePosition / closeNeighbours.Length);
    }
}
class ObstacleAvoidance : SteeringBehavior
{
    Collider[] obstacles;
    float collisionAvoidanceRay = 5; // TODO:  Must Be Tweeakable
    float timer;
    static float updateRate = 0.1f;
    public ObstacleAvoidance(Steering _steeringComponent,float factor = 1) : base(_steeringComponent,factor)
    {
    }
    public Collider[] UpdateObstacles()
    {
        int separationMask = LayerMask.GetMask(new string[] { "Obstacle" });
        obstacles = Physics.OverlapSphere(steeringComponent.transform.position, collisionAvoidanceRay, separationMask);
        return obstacles;
    }
    public override void OnUpdate()
    {
        timer += Time.deltaTime;
        if (timer > updateRate)
        {
            UpdateObstacles();
            timer = 0;
        }
    }
    public override Vector3 ComputeSteering()
    {
        Collider closestObstacle = null;
        float closestDistance = 1000;
        Vector3 closestPoint;
        // si pas d'obstacle rien a faire
        if (obstacles == null)
            return Vector3.zero;
        // On cherche l'obstacle le plus proche du puppet
        // On vérifie aussi qu'on va vers lui
        for (int i = 0; i < obstacles.Length; i++)
        {
            closestPoint = obstacles[i].ClosestPointOnBounds(steeringComponent.transform.position);
            Vector3 puppetToObstacle = closestPoint - steeringComponent.transform.position;
            float angle = Vector3.Angle(steeringComponent.transform.forward, puppetToObstacle);
            if ((angle <= 90 && puppetToObstacle.magnitude < closestDistance) ||
                puppetToObstacle == Vector3.zero)
            {
                closestDistance = puppetToObstacle.magnitude;
                closestObstacle = obstacles[i];
            }
        }
        Vector3 avoidanceForce = Vector3.zero;
        if (closestObstacle != null && closestDistance < collisionAvoidanceRay)
        {
            Vector3 puppetToObstacle = closestObstacle.transform.position - steeringComponent.transform.position;
            // Soit il faut tourner à gauche/ sens AntiHoraire :
            if (Vector3.Dot(steeringComponent.transform.right, puppetToObstacle) > 0)
            {
                avoidanceForce.x = -puppetToObstacle.z;
                avoidanceForce.z = puppetToObstacle.x;
            }
            else // Soit il faut tourner à droite /sens Horaire: 
            {
                avoidanceForce.x = puppetToObstacle.z;
                avoidanceForce.z = -puppetToObstacle.x;
            }

            float power = ((collisionAvoidanceRay - closestDistance) / collisionAvoidanceRay * factor);
            avoidanceForce = avoidanceForce.normalized * steeringComponent.maxSpeed * power;
#if UNITY_EDITOR
            steeringComponent.giz.avoidance = avoidanceForce;
#endif
            return avoidanceForce;
        }
        else
            return Vector3.zero;
    }
}
public class FlowFollowing : SteeringBehavior
{
    FlowField flowField;
    float anticipation;
    bool normalized;
    public FlowFollowing(Steering _steeringComponent, FlowField _flowfield,float factor = 1, bool _normalized = true, float _anticipation = 0.5f) 
        : base(_steeringComponent,factor)
    {
        anticipation = _anticipation;
        normalized = _normalized;
        flowField = _flowfield;
        flowField.GetValue(Vector3.zero);
    }

    public override Vector3 ComputeSteering()
    {
        Vector3 nextPoint = steeringComponent.transform.position + steeringComponent.Rb.velocity * anticipation;
        Vector3 fieldDirection = flowField.GetValue(nextPoint);
        if (normalized)
        {
            fieldDirection = fieldDirection.normalized * steeringComponent.maxSpeed;
        }
        Vector3 force = fieldDirection - steeringComponent.Rb.velocity;
        force = Vector3.ClampMagnitude(force, steeringComponent.maxSpeed);
        return force * factor;
    }
}

public class FormationFolllowing : SteeringBehavior
{
    Formation formation;
    Formation.Slot slot;
    Arrival arrival;
    public FormationFolllowing(Steering _steeringComponent,Formation _formation) : base(_steeringComponent)
    {
        formation = _formation;
        slot = formation.GetSlot(steeringComponent.transform.position);
        arrival = new Arrival(steeringComponent, null);
    }
    public override Vector3 ComputeSteering()
    {
        if (slot != null)
            return arrival.ComputeArrivalForce(slot.position);
        else
        {
            slot = formation.GetSlot(steeringComponent.transform.position);
            return Vector3.zero;
        }
    }

}