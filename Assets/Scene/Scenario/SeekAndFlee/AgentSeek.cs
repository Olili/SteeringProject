using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentSeek : Agent {

  
    public virtual void Start()
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
            curState = Seek;
            Steering targetSteering = closestEntity.GetComponent<Steering>();
            targetSteering.AddBehavior(new Flee(targetSteering, transform));
            targetSteering.RemoveBehavior<Wander>();
        }
    }
    public void Seek()
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
