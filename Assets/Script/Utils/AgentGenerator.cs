using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGenerator : MonoBehaviour {

    Formation[] formations;
	// Use this for initialization
	void Start () {
        formations[0] = new CircleFormation();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
