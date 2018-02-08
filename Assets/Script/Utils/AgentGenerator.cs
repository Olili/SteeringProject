using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGenerator : MonoBehaviour {

    List<Formation> formations;
    int unitCounter;
    [SerializeField] GameObject agentModel;
	// Use this for initialization
	void Start () {
        formations = new List<Formation>
        {
            new GameObject("formation").AddComponent<CircleFormation>()
        };
    }
    public void SpawnFormationAgent()
    {
        GameObject agent = Instantiate(agentModel, Vector3.zero, Quaternion.identity, null);
        Steering steering = agent.GetComponent<Steering>();
        steering.AddBehavior(new FormationFolllowing(steering, formations[0]));
        unitCounter++;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnFormationAgent();
        }
        formations[0].UpdateFormation(Vector3.zero, Quaternion.identity, unitCounter);
    }
}
