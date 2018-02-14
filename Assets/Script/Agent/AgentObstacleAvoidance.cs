using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObstacleAvoidance : Agent {

    Transform target;
    public void Start()
    {
      
        target = new GameObject("target").transform;
        target.position = transform.position + Vector3.forward * 2;
        target.parent = transform;

        steering.AddBehavior(new Seek(steering,target));
        steering.AddBehavior(new Separation(steering));
        steering.AddBehavior(new ObstacleAvoidance(steering));
    }
    void FixedUpdate () {
        //steering.Seek(transform.position + transform.forward);
        //steering.ObstaclesAvoidance();
        //steering.Move();
        Vector3 velocity = rb.velocity;
        velocity.y = 0;
        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }
}

    void FixedUpdate () {

        Vector3 velocity = rb.velocity;
        velocity.y = 0;
        if (velocity != Vector3.zero)
            transform.forward = velocity;
    }
   