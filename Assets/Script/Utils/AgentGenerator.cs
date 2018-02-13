using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentGenerator : MonoBehaviour {

    List<Formation> formations;
    int unitCounter;
    [SerializeField] GameObject agentModel;
    List<Steering> steeringUnits;
    int curFormation;
	// Use this for initialization
	void Start ()
    {
        formations = new List<Formation>
        {
            new GameObject("SpiralFormation").AddComponent<SpiralFormation>(),
            new GameObject("CircleFormation").AddComponent<CircleFormation>(),
            new GameObject("LineFormation").AddComponent<LineFormation>(),
            new GameObject("RectFormation").AddComponent<RectFormation>(),
            new GameObject("VFormation").AddComponent<VFormation>(),
        };
        steeringUnits = new List<Steering>();
    }
    public void SpawnFormationAgent()
    {
        GameObject agent = Instantiate(agentModel, Vector3.zero, Quaternion.identity, null);
        Steering steering = agent.GetComponent<Steering>();
        steering.AddBehavior(new FormationFolllowing(steering));
        steering.AddBehavior(new Separation(steering));
        steeringUnits.Add(steering);
        unitCounter++;
    }
    public void RemoveAgent()
    {
        if (steeringUnits.Count >0)
        {
            Destroy(steeringUnits[0].gameObject);
            steeringUnits.Remove(steeringUnits[0]);
        }
    }
    int counter = 0;
    // Update is called once per frame
    void Update () {



        if (Input.GetKey(KeyCode.S))
        {
            SpawnFormationAgent();
        }
        if (Input.GetKey(KeyCode.R))
        {
            RemoveAgent();
        }
        formations[curFormation].UpdateFormation(steeringUnits,true);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                formations[curFormation].transform.position = hit.point;
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            curFormation = (++curFormation) % formations.Count;
        }
    }
    public void OnGUI()
    {
        formations[curFormation].GUIShow();
    }
}
