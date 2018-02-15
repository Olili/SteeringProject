using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Steering : MonoBehaviour
{
        //  Component
    Rigidbody rb;
    new Collider collider;
        // Data
    [SerializeField]protected List<SteeringBehavior> steeringBehaviorStack;
    Vector3 steering;
    bool isOnGround;
    Vector3 onPlanNormal;
    
    // Agent specific : 
    [SerializeField]
    protected Vector3 agentCollExtent;
    protected Vector3 center;
    // Tweekables : 
    [Range(0, 90)] [SerializeField]
    public  float maxSlope;

    public float maxSpeed;
    public float maxAcceleration;

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
    public Vector3 CenterDown
    {
        get
        {
            return collider.bounds.center - (Vector3.up * collider.bounds.extents.y);
        }
    }

    public Rigidbody Rb { get { return rb; } }

    public Vector3 OnPlanNormal
    {
        get
        {
            return onPlanNormal;
        }

        protected set
        {
            onPlanNormal = value;
        }
    }

    public Vector3 AgentCollExtent
    {
        get
        {
            return agentCollExtent;
        }
    }
    #endregion

    // TODO : REFACTO WITH SURFACE STEEERING / FLYING STEERING
    public bool CheckGround()
    {
        int mask = LayerMask.GetMask(new string[] { "Ground" });
        RaycastHit hit;
        Vector3 center = transform.position+ onPlanNormal * 0.1f;
        if (Physics.Raycast(center, -onPlanNormal, out hit, (0.2f), mask, QueryTriggerInteraction.Ignore))
        {
            // On est dans une pente il faut tomber
            if (Vector3.Angle(Vector3.up, hit.normal) > maxSlope)
            {
                IsOnGround = false;
                onPlanNormal = Vector3.up;
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
    public Vector3 GetPlanDirection(Vector3 dir,Vector3 planUp)
    {
        Vector3 right = Vector3.Cross(dir, planUp);
        Vector3 planDirection = Vector3.Cross(planUp, right);
        return planDirection;
    }
    // Return desiredVelocity
    public virtual Vector3 GetDvOnPlan(Vector3 target)
    {
        Vector3 dV = (target - CenterDown);
        float distance = dV.magnitude;
        dV.Normalize();
        Vector3 right = Vector3.Cross(dV, onPlanNormal);
        Vector3 planDv = Vector3.Cross(onPlanNormal, right);
        return planDv.normalized * maxSpeed;
    }
    public void AddBehavior(SteeringBehavior steeringBehavior)
    {
        if (steeringBehaviorStack == null)
            steeringBehaviorStack = new List<SteeringBehavior>();
        steeringBehavior.steeringComponent = this;
        steeringBehaviorStack.Add(steeringBehavior);

#if UNITY_EDITOR
        gizBehavior.Add(new GizmosForSteeringBehavior(steeringBehavior.ToString()));
#endif

    }
    public void RemoveBehavior(SteeringBehavior steeringBehavior,int i =0)
    {
        steeringBehaviorStack.Remove(steeringBehavior);
#if UNITY_EDITOR
        gizBehavior.RemoveAt(i);
#endif
    }
    public void RemoveBehavior<T>() where T : SteeringBehavior
    {
        for (int i =0;i < steeringBehaviorStack.Count;i++)
        {
            if (steeringBehaviorStack[i] is T)
            {
                RemoveBehavior(steeringBehaviorStack[i],i);
                return;
            }
        }
    }
    public T FindBehavior<T>() where T: SteeringBehavior
    {
        for (int i = 0; i < steeringBehaviorStack.Count; i++)
            if (steeringBehaviorStack[i] is T)
                return steeringBehaviorStack[i] as T;
        return null;
    }
    public virtual void Awake()
    {
        steering = Vector3.zero;
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        agentCollExtent = collider.bounds.extents;
        steeringBehaviorStack = new List<SteeringBehavior>();
#if UNITY_EDITOR
        giz.steeringBehaviorStack = steeringBehaviorStack;
#endif

    }
    
    public virtual void Update()
    {
        center = transform.position + Vector3.up * agentCollExtent.y;
        CheckGround();
        for (int i = 0; i < steeringBehaviorStack.Count; i++)
            steeringBehaviorStack[i].OnUpdate();
    }
    public virtual void FixedUpdate()
    {
        if (!isOnGround)
            return;

        steering = Vector3.zero;
            // compute all steering forces.
        for (int i = 0; i < steeringBehaviorStack.Count;i++)
        {
            steering += steeringBehaviorStack[i].ComputeSteering();
        }
            
        steering = Vector3.ClampMagnitude(steering, maxSpeed);
        Vector3 velocity = rb.velocity;
        velocity += (steering * 50 * Time.fixedDeltaTime * maxAcceleration);
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity.magnitude > 0.1f)
            rb.velocity = velocity;
        else
            rb.velocity = Vector3.zero;
    
#if UNITY_EDITOR
    giz.steeringForce = steering;
#endif
        steering = Vector3.zero;
    }
    // EDITOR GIZMO : 
#if UNITY_EDITOR
    [System.Serializable]
    public struct GizmosForSteering
    {
        public bool Velocity;
        public bool Steering;
        public Vector3 steeringForce;
        public List<SteeringBehavior> steeringBehaviorStack;
    }
    public GizmosForSteering giz;
    [System.Serializable]
    public struct GizmosForSteeringBehavior
    {
        public string name;
        public bool showForce;
        public Vector3 steerForce;
        public bool showRadius;
        public float radius;
        public GizmosForSteeringBehavior(string _name)
        {
            name = _name;
            showForce = false;
            steerForce = Vector3.zero;
            showRadius = false;
            radius = 0;
        }
    }
    public List<GizmosForSteeringBehavior> gizBehavior;
    public void OnDrawGizmosSelected()
    {
        if (steeringBehaviorStack!=null)
            for (int i = 0; i < steeringBehaviorStack.Count; i++)
            {
                steeringBehaviorStack[i].OnDrawGizmo();
            }
    }
#endif

}
