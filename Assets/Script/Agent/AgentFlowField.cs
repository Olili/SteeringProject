using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentFlowField : Agent {

    [SerializeField] FlowField flowField;
    [SerializeField] Transform target;


    public void Start()
    {
        steering.AddBehavior(new Seek(steering,target));
    }
    void FixedUpdate () {

        //steering.FlowFollowing(flowField);
        ////steering.Seek(transform.position + transform.forward);
        //steering.Move();
        //if (rb.velocity != Vector3.zero)
        //    transform.forward = rb.velocity;
    }
}
