using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentChase : Agent {

  
    public void Start()
    {
        steering.AddBehavior(new Wander(steering));
        steering.RemoveBehavior(new Separation(steering));
        curState = Wander;
    }
    
	public void Wander()
    {
        Transform closestEntity = GetClosestAgent("human");
        if (closestEntity!=null)
        {
            steering.RemoveBehavior<Wander>();
            steering.AddBehavior(new Seek(steering, closestEntity));
            curState = Chase;
            Steering targetSteering = closestEntity.GetComponent<Steering>();
            targetSteering.AddBehavior(new Flee(targetSteering, transform));
            targetSteering.RemoveBehavior<Wander>();
        }
    }
    public void Chase()
    {
        Transform target = steering.FindBehavior<Seek>().GetTarget();

        Debug.DrawLine(transform.position, target.position, Color.red);
        if ((target.position - transform.position).magnitude > targetLostRadius)
        {
            steering.RemoveBehavior<Seek>();
            steering.AddBehavior(new Wander(steering));

            curState = Wander; Steering targetSteering = target.GetComponent<Steering>();
            targetSteering.AddBehavior(new Wander(targetSteering));
            targetSteering.RemoveBehavior<Flee>();
        }
    }
}
