using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFlowField : Agent {

    [SerializeField] FlowField flowField;
    [SerializeField] Transform target;


    public void Start()
    {
        steering.AddBehavior(new FlowFollowing(steering,flowField,0.6f));
        steering.AddBehavior(new ObstacleAvoidance(steering));
    }
    void FixedUpdate () {

        //steering.FlowFollowing(flowField);
        ////steering.Seek(transform.position + transform.forward);
        //steering.Move();
        //if (rb.velocity != Vector3.zero)
        //    transform.forward = rb.velocity;
    }
}
