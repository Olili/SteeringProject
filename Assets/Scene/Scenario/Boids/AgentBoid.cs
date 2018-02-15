using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Boids : 
 * Chaques entité a séparation 
 * 
 */ 

public class AgentBoid : Agent {

	// Use this for initialization
	void Start () {
        steering.AddBehavior(new Separation(steering, 2));
        steering.AddBehavior(new Cohesion(steering, 5));
        steering.AddBehavior(new Alignement(steering, null, 5));
        steering.AddBehavior(new Wander(steering));
    }
	
}
