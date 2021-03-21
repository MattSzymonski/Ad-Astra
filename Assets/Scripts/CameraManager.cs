using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    GameObject mainManagerGO;
    MainManager mainManager;

    public bool zoomOut;
    public bool zoomIn;


    void Start () {

        mainManagerGO = GameObject.Find("MainManager");
        mainManager = mainManagerGO.GetComponent<MainManager>();
    }
	

	void FixedUpdate () {

        if(!mainManager.dead)
        {
            transform.position = new Vector3(0, mainManager.rocket.transform.position.y + 4.55f, -5f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(0, mainManager.rocket.transform.position.y + 4.55f, -5f), 2 * Time.deltaTime);
        }
      
        Zoom();
        
    }


    void Zoom()
    {
        if (zoomOut)
        {
            Camera.main.orthographicSize += 0.4f * Time.deltaTime;
            if(Camera.main.orthographicSize > 3.8f) { Camera.main.orthographicSize = 3.8f; zoomOut = false; }
        }
        if (zoomIn)
        {
            Camera.main.orthographicSize -= 0.4f * Time.deltaTime;
            if (Camera.main.orthographicSize < 3.1f) { Camera.main.orthographicSize = 3.1f; zoomIn = false; }
        }
    }


}
