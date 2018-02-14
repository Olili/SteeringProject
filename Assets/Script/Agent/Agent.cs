using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Agent : MonoBehaviour {

    protected Steering steering;
    protected Rigidbody rb;
    protected delegate void State();
    Animator animator;
    [SerializeField]protected State curState;

    protected float detectionRadius = 10;
    protected float targetLostRadius = 13;

    void Awake () {
        steering = GetComponent<Steering>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        if (curState != null)
            curState();
    }
    void FixedUpdate()
    {
        Vector3 velocity = rb.velocity;
        velocity.y = 0;
        if (velocity != Vector3.zero)
            transform.forward = velocity;
        if (animator != null)
            animator.SetFloat("Speed", rb.velocity.magnitude);
    }
    public Transform GetClosestAgent(string tagToSearch = null)
    {
        int mask = LayerMask.GetMask(new string[] { "Agent" });
        Collider[] coll = Physics.OverlapSphere(transform.position, detectionRadius, mask);
        Transform closestSteerEntity = null;
        float closestDistance = detectionRadius;
        for (int i = 0; i < coll.Length; i++)
        {
            Steering agent = coll[i].GetComponent<Steering>();
            if (agent != steering && agent != null )
            {
                if (tagToSearch != null && !agent.CompareTag(tagToSearch))
                    continue;
                float distance = (transform.position - agent.transform.position).magnitude;
                if (closestDistance >= distance)
                {
                    distance = closestDistance;
                    closestSteerEntity = agent.transform;
                }
            }
        }
        return closestSteerEntity;
    }
}
