using System;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;
[Serializable]
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
    public virtual Transform GetTarget()
    {
        return null;
    }

#if UNITY_EDITOR
    public Vector3 gizSteeringForce;
#endif
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
    public override Transform GetTarget()
    {
        return target;
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
    public override Transform GetTarget()
    {
        return target;
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
    public override Transform GetTarget()
    {
        return target;
    }
}
public class Pursuit : Seek
{
    Rigidbody targetRb;
    static float Tmax = 5; 
    public Pursuit(Steering _steeringComponent, Transform _target, Rigidbody _targetRb) : base(_steeringComponent,_target)
    {
        targetRb = _targetRb;
    }
    public override Vector3 ComputeSteering()
    {
        float dist = Vector3.Distance(steeringComponent.transform.position, target.position);
        float T = Math.Min(Tmax, dist);
        Vector3 futurePosition = target.position + targetRb.velocity.normalized * T;
        return SeekForce(futurePosition);
    }
}
public class Evade : Flee
{
    Rigidbody targetRb;
    static float Tmax = 5;
    public Evade(Steering _steeringComponent, Transform _target, Rigidbody _targetRb) : base(_steeringComponent, _target)
    {
        targetRb = _targetRb;
    }
    public override Vector3 ComputeSteering()
    {
        float dist = Vector3.Distance(steeringComponent.transform.position, target.position);
        float T = Math.Min(Tmax, dist);
        Vector3 futurePosition = target.position + targetRb.velocity.normalized * T;
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
// TODO : change to work even if unaligned.
class StayWithinBounds : SteeringBehavior
{
    Bounds bounds;
    float distFromWalls;
    public StayWithinBounds(Steering _steeringComponent,Bounds _bounds,float _distFromWalls = 2, float _factor = 1) : base(_steeringComponent, _factor)
    {
        bounds = _bounds;
        distFromWalls = _distFromWalls;
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 roomExtents = (bounds.extents - Vector3.one * distFromWalls) - bounds.center;
        Vector3 center = bounds.center;
        Vector3 avoidWallForce = Vector3.zero;

        if (steeringComponent.transform.position.x < center.x - roomExtents.x)
            avoidWallForce = new Vector3(steeringComponent.maxSpeed, 0, 0);
        else if (steeringComponent.transform.position.x > center.x + roomExtents.x)
            avoidWallForce = new Vector3(-steeringComponent.maxSpeed, 0, 0);
        else if (steeringComponent.transform.position.z < center.z - roomExtents.z)
            avoidWallForce = new Vector3(0, 0, steeringComponent.maxSpeed);
        else if (steeringComponent.transform.position.z > center.z + roomExtents.z)
            avoidWallForce = new Vector3(0, 0, -steeringComponent.maxSpeed);

        return avoidWallForce;
    }
}

#region Boids Steering Movement 



public class BoidBehavior : SteeringBehavior
{
    float timer = 0;
    static float updateRate = 0.1f;
    protected Collider[] closeNeighbours;
    protected float separationRayFactor =1f ; // TODO: Refacto : Maybe different for each boids behavior;
    public BoidBehavior(Steering _steeringComponent,float radius = 1) : base(_steeringComponent)
    {
        separationRayFactor = radius;
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
    public Separation(Steering _steeringComponent, float radius = 1, float factor = 1) : base(_steeringComponent, radius)
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
            //steeringComponent.giz.separateForce = separationForce;
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
    public Alignement(Steering _steeringComponent,Rigidbody _leaderRb = null,float radius = 1,float factor = 1) 
        : base(_steeringComponent,radius)
    {
        leaderRb = _leaderRb;
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 averageDirection = Vector3.zero;
        if (leaderRb == null && closeNeighbours != null && closeNeighbours.Length > 1)
            for (int i = 0; i < closeNeighbours.Length; i++)
            {
                if (closeNeighbours[i].transform !=steeringComponent.transform)
                    averageDirection += closeNeighbours[i].attachedRigidbody.velocity;
            }
        else if (leaderRb != null)
        {
            averageDirection = leaderRb.velocity;
        }
        return (averageDirection.normalized) * steeringComponent.maxSpeed * factor;
    }
}
class Cohesion : BoidBehavior
{
    Seek seekSteer;
    public Cohesion(Steering _steeringComponent, float radius = 1, float factor = 1) : base(_steeringComponent, radius)
    {
        seekSteer = new Seek(_steeringComponent,null);
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 averagePosition = Vector3.zero;
        if (closeNeighbours != null && closeNeighbours.Length > 1)
        {
            for (int i = 0; i < closeNeighbours.Length; i++)
            {
                averagePosition += closeNeighbours[i].transform.position;
                averagePosition.y = 0;
            }
            return seekSteer.SeekForce(averagePosition / closeNeighbours.Length);
        }
        return Vector3.zero;
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
            //steeringComponent.giz.avoidance = avoidanceForce;
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
public class Swarming : SteeringBehavior
{
    public Transform target;
    public float swarmRadius = 8;
    public int index;
    public static int counter = 0;
    public Swarming(Steering _steeringComponent,Transform _target, float _factor = 1) : base(_steeringComponent, _factor)
    {
        target = _target;
        index = counter++;
        
    }
    public void SetRandVelocity()
    {
        Vector3 vel = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;
        steeringComponent.Rb.velocity = vel;
    }
    public override Vector3 ComputeSteering()
    {
        Vector3 velocity = steeringComponent.Rb.velocity;
        Vector3 velocityTangent = new Vector3(-velocity.z,0, velocity.x).normalized;
        Vector3 vEntityToTarget =  target.position- steeringComponent.transform.position;
        Vector3 force;

        if (steeringComponent.Rb.velocity == Vector3.zero)
            SetRandVelocity();

        // choose randomSpeed factor.
        float speedFactor =  Mathf.Lerp(steeringComponent.maxSpeed * 0.15f, steeringComponent.maxSpeed*0.75f, ((float)(index* index)) / (counter*counter));

        //force = velocity * steeringComponent.maxSpeed;
        // Outer zone
        if (vEntityToTarget.magnitude > swarmRadius)       {
            // Increase speed to maximum
            
            float angle = Vector3.Angle(vEntityToTarget, velocity);

            if (angle < 10f)
            {
                // Vary the steering as a function of the index of the entity
                force = velocityTangent * speedFactor;
            }
            else
            {
                if (Vector3.Dot(vEntityToTarget, velocityTangent) < 0)
                    force = velocityTangent * -speedFactor;
                else
                    force = velocityTangent * speedFactor;
            }
        }
        else // inner Zone
        {
            if (Vector3.Dot(vEntityToTarget, velocityTangent) < 0)
                force = velocityTangent * -steeringComponent.maxSpeed * 0.35f;
            else
                force = velocityTangent * steeringComponent.maxSpeed * 0.35f;
        }
        force = Vector3.ClampMagnitude(force, steeringComponent.maxSpeed);
        return force;
    }
    ~Swarming()
    {
        counter--;
    }
       
}

public class FormationFolllowing : SteeringBehavior
{
    Formation.Slot slot;
    Arrival arrival;
    public FormationFolllowing(Steering _steeringComponent) : base(_steeringComponent)
    {
        arrival = new Arrival(steeringComponent, null);
    }
    public void UpdateSlot(Formation.Slot _slot)
    {
        slot = _slot;
    }
   
    public override Vector3 ComputeSteering()
    {
        if (slot != null)
            return arrival.ComputeArrivalForce(slot.position);
        else
            return Vector3.zero;
    }

}