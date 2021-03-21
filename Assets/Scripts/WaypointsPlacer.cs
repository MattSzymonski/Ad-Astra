using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointsPlacer : MonoBehaviour {  

    public GameObject waypointMarker;
    public GameObject waypointPrefab;

    public float markerSpeed;
    public float min, max;
    public bool goRight;

    public bool starting;
    public bool started;
    public float startTimer;

    public List<GameObject> waypointsOnMap;



    void DrawStartDirection() {

        if (Random.value > 0.5f) { goRight = true; } else { goRight = false; }

    }

	
	void Update () {

        if (!GetComponent<MainManager>().dead)
        {
            Enable();
            if (started)
            {
                Touch();
                MoveMarker();
            }
        }
  //      if (waypointsOnMap.Count == 2) { FadeOut(); }
        if (waypointsOnMap.Count > 1) { Destroy(waypointsOnMap[0]); waypointsOnMap.RemoveAt(0);  }

    }

    
    void FadeOut()
    {        
       // waypointsOnMap[0].GetComponent<Animator>().SetBool("FadeOut", true); 
        waypointsOnMap[0].GetComponentInChildren<ParticleSystem>().Stop();
        waypointsOnMap[0].GetComponentInChildren<ParticleSystem>().Clear();
    }
    

    void Enable()
    {
        if (starting)
        {
            startTimer += 0.3f * Time.deltaTime;
            
            if (startTimer > 0.75f)
            {
                startTimer = 1f;
                starting = false;
                started = true;
                DrawStartDirection();
                waypointMarker.SetActive(true);
            }
        }
    }



    void MoveMarker()
    {
        //Moving up
        waypointMarker.transform.position = new Vector3(waypointMarker.transform.position.x, Camera.main.transform.position.y + 0.5f, waypointMarker.transform.position.z);

        //Moving sides
        if (waypointMarker.transform.localPosition.x >= max && goRight) { goRight = false; }
        if (waypointMarker.transform.localPosition.x <= min && !goRight) { goRight = true; }
         
        if(goRight) { waypointMarker.transform.Translate(markerSpeed * GetComponent<MainManager>().gameSpeed * Time.deltaTime, 0, 0, Space.Self); } else { waypointMarker.transform.Translate(-markerSpeed * GetComponent<MainManager>().gameSpeed * Time.deltaTime, 0, 0, Space.Self); }
    }

    void SpawnWaypoint()
    {
        GameObject newWaypoint = Instantiate(waypointPrefab, waypointMarker.transform.position, Quaternion.Euler(0,45,0)) as GameObject;
        GetComponent<MainManager>().rocket.GetComponent<Rocket>().waypoint = newWaypoint;
        
        waypointsOnMap.Add(newWaypoint);
    }

    public void RocketProperties(int rocketType)
    {
        switch (rocketType)
        {
            case 1:
                markerSpeed = 1.33f;
                break;
            case 2:
                markerSpeed = 1.36f;
                break;
            case 3:
                markerSpeed = 1.39f;
                break;
            case 4:
                markerSpeed = 1.42f;
                break;
            case 5:
                markerSpeed = 1.45f;
                break;
        }
    }

    void Touch()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                SpawnWaypoint();
            }
        }
        if(Input.GetMouseButtonDown(0)) { SpawnWaypoint(); }
    }

}
