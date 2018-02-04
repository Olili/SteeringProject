using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Steering : MonoBehaviour {

    static readonly float slowingRadius = 2;
            // component
    protected Rigidbody rb;
    Vector3 onPlanNormal;
    bool isOnGround;

        // data
    protected Vector3 steering;
    Vector3 computedVelocity;
    float separationRayFactor = 1f; // 

        // updateFrequencie
    bool updateNeighbours = true;
    Collider[] closeNeighbours;
    Collider[] farawayNeighbours;
    Collider[] obstacles;
    public float collisionAvoidanceRay;
    float timer = 0;
    float delay = 0.1f;

    // Tweek : 
    [SerializeField] float maxSpeed;
    [SerializeField] float maxAcceleration;
    [SerializeField] Vector3 agentCollExtent;
    [Range(0,90)][SerializeField] float maxSlope;


    // info : 
    public bool isSliding;

    #region getterSetters
    public bool IsOnGround
    {
        get
        {
            return isOnGround;
        }
        set
        {
            if (value == true)
                rb.useGravity = false;
            else
                rb.useGravity = true;
            isOnGround = value;
        }
    }
    public Vector3 GetSteering
    {
        get
        {
            return steering;
        }
    }
    #endregion
    public void Awake()
    {
        steering = Vector3.zero;
        rb = GetComponent<Rigidbody>();
    }
    public void Start()
    {

    }
    /// <summary>
    /// Renvoi la velocité finale calculée par le steering 
    /// /!\ Ne dois pas être appellé + d'une fois par fixedUpdate
    /// /!\ Renvois 0 si aucune force de steering n'a été appliquée cette frame
    /// /!\ Renvois la velocité inchangée
    /// </summary>
    public Vector3 ComputedVelocity
    {
        get
        {
            if (!CheckGround())
            {
                EndFrameReset();
                return rb.velocity;
            }
            else if(steering == Vector3.zero)
            {
                EndFrameReset();
                return Vector3.zero;
            }

            steering = Vector3.ClampMagnitude(steering, maxSpeed);
            Vector3 velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
            velocity += (steering * 50 * Time.fixedDeltaTime * maxAcceleration);
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
            computedVelocity = new Vector3(velocity.x, velocity.y, velocity.z);

            EndFrameReset();
           

            return computedVelocity;
        }
    }

    public bool CheckGround()
    {
        int mask = LayerMask.GetMask(new string[] { "Ground" });
        RaycastHit hit;
        Vector3 center = transform.position;
        if (Physics.Raycast(center, -onPlanNormal, out hit, (agentCollExtent.y * 0.5f + 0.1f),mask,QueryTriggerInteraction.Ignore))
        {
                // On est dans une pente il faut tomber
            if (Vector3.Angle(Vector3.up, hit.normal) > maxSlope)
            {
                IsOnGround = false;
                onPlanNormal = Vector3.up;
            }
                // On est proche du sol mais pas assez
            else if (Vector3.Distance(hit.point, transform.position) > 0.5f)
            {
                rb.AddForce(Physics.gravity * 10, ForceMode.Acceleration);
                IsOnGround = true;
                onPlanNormal = hit.normal;
            }
            else
            {
                IsOnGround = true;
                onPlanNormal = hit.normal;
            }
            
        }
        else // Raycast failure : on est loin du sol
        {
            IsOnGround = false;
            onPlanNormal = Vector3.up;
        }
        return IsOnGround;
    }
    public void Move()
    {
        Vector3 velocity = ComputedVelocity;
      
        rb.velocity = velocity;
    }

    void EndFrameReset()
    {
#if UNITY_EDITOR
        giz.steeringForce = steering;
#endif
        steering = Vector3.zero;
        timer += Time.fixedDeltaTime;
        if (timer > delay)
        {
            updateNeighbours = true;
            timer = 0;
                // un peu crade mais pour tester vite
            UpdateObstacles();
        }
    }


    #region Basic steering Movement
    public void Seek(Vector3 target, float factor = 1)
    {
        Vector3 desiredVelocityPlan = GetDvOnPlan(target);
        Vector3 force = desiredVelocityPlan - rb.velocity;

        force = Vector3.ClampMagnitude(force, maxSpeed);
        steering += force * factor;
    }
    public void Flee(Vector3 target, float factor = 1)
    {
        Vector3 desiredVelocityPlan = -GetDvOnPlan(target);
        Vector3 force = desiredVelocityPlan - rb.velocity;

        force = Vector3.ClampMagnitude(force, maxSpeed);
        steering += force * factor;

    }
    public void Arrival(Vector3 target,float factor = 1)
    {
        Vector3 desiredVelocityPlan = GetDvOnPlan(target);
        float distance = desiredVelocityPlan.magnitude;

        if (desiredVelocityPlan.magnitude < slowingRadius)
        {
            desiredVelocityPlan = desiredVelocityPlan * (distance / slowingRadius);
        }
        Vector3 force = desiredVelocityPlan - rb.velocity;
        force = Vector3.ClampMagnitude(force, maxSpeed);
        steering += force * factor;
    }
    public void Pursuit(Vector3 target, Vector3 targetVelocity,float factor = 1)
    {
        float T = 0.5f;
        Vector3 futurePosition = target + targetVelocity * T;
        Seek(futurePosition,factor);
    }

    public void Evade(Vector3 target, Vector3 targetVelocity, float factor = 1)
    {
        float T = 0.5f;
        Vector3 futurePosition = target + targetVelocity * T;
        Flee(futurePosition, factor);
    }
    float wanderAngle = 0;
    /*
         Je choisis une direction random. 
         Je cree un cercle ayant pour centre la direction actuelle.
         et un rayon arbitraire. 
         Je choisis un point aléatoire dans ce cercle. 
         Je décide de seek vers cette direction. 
         Ou
         Je choisis une direction random. 
         Je cree un cercle ayant pour centre la direction actuelle.
         et un rayon arbitraire. 
         Je choisis un point aléatoire dans ce cercle. 
         Ma direction actuelle devient cette direction. 
     */
    public void Wander(float factor = 1)
    {
        float wanderT = 30;
        float circleRadius = 1;
        float circleDistance = 2;
        Vector3 circleCenter = transform.position + rb.velocity.normalized * circleDistance;

        wanderAngle += Random.Range(-wanderT, wanderT);
        Vector3 circleOffset = new Vector3(Mathf.Cos(wanderAngle * Mathf.Deg2Rad), 0, Mathf.Sin(wanderAngle * Mathf.Deg2Rad)) * circleRadius;
        Vector3 target = circleCenter + circleOffset;
        Seek(target, factor);
    }
    #endregion
    #region Advanced Steering Movement 
    // Peut être améliorer : Lorsqu'une unité est un milieu d'un gros groupe elle est coincée et vibre.
    // de plus ici on push dans une sphere autour de nous.  Mais ce qui compte c'est surtout d'éviter de vers quoi on avance.
    public void Separation(float factor = 1)
    {
        Collider[] agentCollided;
        Vector3 separationForce = Vector3.zero;
        float sphereCheckRadius = agentCollExtent.magnitude * separationRayFactor;
        agentCollided = GetNeighbouringAgents();
        if (agentCollided != null)
        {
            int i;
            for (i = 0; i < agentCollided.Length; i++)
            {
                if (agentCollided[i] !=null&& agentCollided[i].transform != this.transform)
                {
                    Vector3 vecFromOther = transform.position - agentCollided[i].transform.position;
                    float distance = vecFromOther.magnitude;
                    vecFromOther.Normalize();
                    if (distance != 0)
                    {
                        vecFromOther = vecFromOther * sphereCheckRadius / distance * maxSpeed;
                        separationForce += vecFromOther;
                    }
                }
            }
            separationForce = Vector3.ClampMagnitude(separationForce, maxSpeed);
#if UNITY_EDITOR
            giz.separateForce = separationForce;
#endif
            steering += separationForce* factor;
        }
    }
    public void Alignement(float factor = 1)
    {
        Vector3 averageDirection = Vector3.zero;
        closeNeighbours = GetNeighbouringAgents();
        for (int i = 0; i < closeNeighbours.Length; i++)
        {
            if (closeNeighbours[i].transform!= transform)
                averageDirection += closeNeighbours[i].attachedRigidbody.velocity;
        }
        steering += (averageDirection.normalized) * maxSpeed * factor;
    }
    public void Alignement(Vector3 leaderVel,float factor = 1)
    {
        Vector3 averageDirection = leaderVel;
        steering += (averageDirection.normalized) * maxSpeed * factor;
    }
    public void Cohesion(float factor = 1)
    {
        Vector3 averagePosition = Vector3.zero;
        closeNeighbours = GetNeighbouringAgents();
        for (int i = 0; i < closeNeighbours.Length; i++)
        {
            averagePosition += closeNeighbours[i].transform.position;
        }
        Seek(averagePosition / closeNeighbours.Length);
    }

    public void ObstaclesAvoidance(float factor = 1)
    {
        Collider closestObstacle = null;
        float closestDistance = 1000;
        Vector3 closestPoint;
        // si pas d'obstacle rien a faire
        if (obstacles == null)
            return;
        // On cherche l'obstacle le plus proche du puppet
        // On vérifie aussi qu'on va vers lui
        for (int i = 0; i < obstacles.Length;i++)
        {
            closestPoint = obstacles[i].ClosestPointOnBounds(transform.position);
            Vector3 puppetToObstacle = closestPoint - transform.position;
            float angle = Vector3.Angle(transform.forward, puppetToObstacle);
            if ((angle <= 90 && puppetToObstacle.magnitude < closestDistance) ||
                puppetToObstacle == Vector3.zero)
            {
                closestDistance = puppetToObstacle.magnitude;
                closestObstacle = obstacles[i];
            }
        }
        Vector3 avoidanceForce = Vector3.zero;
        if (closestObstacle !=null)
        {
            Vector3 puppetToObstacle = closestObstacle.transform.position - transform.position;
            // Soit il faut tourner à gauche/ sens AntiHoraire :
            if (Vector3.Dot(transform.right, puppetToObstacle) > 0)
            {
                avoidanceForce.x = -puppetToObstacle.z;
                avoidanceForce.z = puppetToObstacle.x;
            }
            else // Soit il faut tourner à droite /sens Horaire: 
            {
                avoidanceForce.x = puppetToObstacle.z;
                avoidanceForce.z = -puppetToObstacle.x;
            }

            //if (Vector3.Angle(puppet.Rb.velocity, avoidanceForce)<10)
            //{
            //    avoidanceForce = Vector3.Cross(avoidanceForce, puppet.transform.up);
            //}

            float power = ((collisionAvoidanceRay - closestDistance)/ collisionAvoidanceRay * factor);
            avoidanceForce = avoidanceForce.normalized * maxSpeed * power;
            steering += avoidanceForce;
        }
#if UNITY_EDITOR
        giz.avoidance = avoidanceForce;
#endif
    }

    public void FlowFollowing(FlowField flowField,bool normalized = true,float factor = 1)
    {
        float T = 0.5f;
        Vector3 nextPoint = transform.position + rb.velocity * T;
        Vector3 fieldDirection = flowField.GetValue(nextPoint);
        if (normalized)
        {
            fieldDirection = fieldDirection.normalized * maxSpeed;
        }
        Vector3 force = fieldDirection - rb.velocity;
        force = Vector3.ClampMagnitude(force, maxSpeed);
        steering += force * factor;
    }
    #endregion


    public Collider[] GetNeighbouringAgents()
    {
        if (updateNeighbours)
        {
            int separationMask = LayerMask.GetMask(new string[] { "Agent" });
            Vector3 separationForce = Vector3.zero;
            float sphereCheckRadius = agentCollExtent.magnitude * separationRayFactor;
            closeNeighbours = Physics.OverlapSphere(transform.position, sphereCheckRadius, separationMask);

            updateNeighbours = false;
        }
        return closeNeighbours;
    }
    public Collider[] UpdateObstacles()
    {
        int separationMask = LayerMask.GetMask(new string[] { "Obstacle" });
        obstacles = Physics.OverlapSphere(transform.position, collisionAvoidanceRay, separationMask);
        return obstacles;
    }
    
    public virtual Vector3 GetDvOnPlan(Vector3 target)
    {
        Vector3 dV = (target - transform.position);
        float distance = dV.magnitude;
        dV.Normalize();
        Vector3 right = Vector3.Cross(dV, onPlanNormal);
        Vector3 planDv = Vector3.Cross(onPlanNormal, right);

        //if (distance > puppet.stats.Get(Stats.StatType.move_speed))
            return planDv.normalized * maxSpeed;
        //else
        //    return planDv.normalized * distance;

    }
   

#if UNITY_EDITOR
    [System.Serializable]
    public struct GizmosForSteering 
        {
        public bool Velocity;
        public bool Steering;
        public bool SeparateCheckSphere;
        public bool Separate;
        public bool collisionAvoidance;

        public Vector3 steeringForce;
        public Vector3 separateForce;
        public Vector3 avoidance;
        public float separateSphereLenght;
    }
    public GizmosForSteering giz;
#endif
}
