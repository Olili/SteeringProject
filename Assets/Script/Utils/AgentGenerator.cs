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
            new GameObject("formation").AddComponent<VFormation>()
        };
    }
    public void SpawnFormationAgent()
    {
        GameObject agent = Instantiate(agentModel, Vector3.zero, Quaternion.identity, null);
        Steering steering = agent.GetComponent<Steering>();
        steering.AddBehavior(new FormationFolllowing(steering, formations[0]));
        steering.AddBehavior(new Separation(steering));
        unitCounter++;
    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown(KeyCode.S))
        {
            SpawnFormationAgent();
        }
        formations[0].UpdateFormation(unitCounter);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                formations[0].transform.position = hit.point;
            }
        }
    }
}
