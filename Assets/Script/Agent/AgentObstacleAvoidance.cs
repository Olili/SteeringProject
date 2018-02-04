using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentObstacleAvoidance : Agent {

	void FixedUpdate () {
        steering.Seek(transform.position + transform.forward);
        steering.ObstaclesAvoidance();
        steering.Move();
        if (rb.velocity!=Vector3.zero)
            transform.forward = rb.velocity;
    }
}
