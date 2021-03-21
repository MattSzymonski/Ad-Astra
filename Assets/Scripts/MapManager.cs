using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {

    MainManager mainManager;

    public GameObject[] obstacles;
    public GameObject coinPrefab;
    public GameObject JpCoinPrefab;

    public int generationNumber;

    public List<GameObject> obstaclesOnMap;
    public List<GameObject> coinsOnMap;

    public bool jpMode;

    void Start () {

        mainManager = GetComponent<MainManager>();

	}
	
	void Update () {

        if (!mainManager.dead)
        {
            if (mainManager.rocket.transform.position.y + 8 > transform.position.y)
            {
                transform.position = new Vector3(0, transform.position.y + 5, 0);
                generationNumber++;
                if (generationNumber > 1 && mainManager.tutorial != 0) { GenerateMap(); }
            }

            if (obstaclesOnMap.Count > 15) { Destroy(obstaclesOnMap[0]); obstaclesOnMap.RemoveAt(0); }
            if (coinsOnMap.Count > 10) { Destroy(coinsOnMap[0]); coinsOnMap.RemoveAt(0); }
        }
	}

    void GenerateMap()
    {
        int roids = 0;
        int score = (int)Mathf.Round(mainManager.transform.position.y / 2);

        if (score > 60) { roids = Random.Range(0, 7); } else if (score > 40) { roids = Random.Range(0, 6); } else { roids = Random.Range(0, 5); }


        if (roids == 2) //1 anywhere
        {
            GameObject roid = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(Random.Range(-1.8f, 1.8f),transform.position.y + Random.Range(-1f,1f), 0), Quaternion.identity) as GameObject;
            roid.transform.rotation = Random.rotation;
            roid.transform.localScale = roid.transform.localScale * Random.Range(0.8f, 1.2f);
            obstaclesOnMap.Add(roid);

            roid.GetComponent<MeshRenderer>().material.color = GetComponent<ColorsManager>().ColorAsteroids();
        }
        if (roids == 3) //1 big on side
        {
            float x = 0, y = Random.Range(-0.5f, 0.5f);
            if (Random.value > 0.5f) { x = Random.Range(-2.4f, -1.5f); } else { x = Random.Range(1.5f, 2.4f); }
            GameObject roid = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(x, transform.position.y + y, 0), Quaternion.identity) as GameObject;
            roid.transform.rotation = Random.rotation;
            roid.transform.localScale = roid.transform.localScale * Random.Range(1.3f, 2.5f);
            obstaclesOnMap.Add(roid);

            roid.GetComponent<MeshRenderer>().material.color = GetComponent<ColorsManager>().ColorAsteroids();
        }
        if (roids == 4) // 1 middle
        {
            for (int i = 0; i < 2; i++)
            {
                float x = 0, y = 0;
                if (i == 1) { x = Random.Range(-2f, 0f); y = Random.Range(-1f, 1f); }
                if (i == 2) { x = Random.Range(0f, 2f); y = Random.Range(-1f, 1f); }

                GameObject roid = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(x, transform.position.y + y, 0), Quaternion.identity) as GameObject;
                roid.transform.rotation = Random.rotation;
                roid.transform.localScale = roid.transform.localScale * Random.Range(0.5f, 1.0f);
                obstaclesOnMap.Add(roid);

                roid.GetComponent<MeshRenderer>().material.color = GetComponent<ColorsManager>().ColorAsteroids();
            }
        }
        if (roids == 5) // 2 on sides
        {
            for (int i = 0; i < 2; i++)
            {
                float x = 0, y = 0;
                if (i == 1) { x = Random.Range(-2.2f, -2.1f); }
                if (i == 2) { x = Random.Range(2.1f, 2.2f); }
                y = Random.Range(-1.2f, 1.2f);

                GameObject roid = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(x, transform.position.y + y, 0), Quaternion.identity) as GameObject;
                roid.transform.rotation = Random.rotation;
                roid.transform.localScale = roid.transform.localScale * Random.Range(0.8f, 1.3f);
                obstaclesOnMap.Add(roid);

                roid.GetComponent<MeshRenderer>().material.color = GetComponent<ColorsManager>().ColorAsteroids();
            }
        }
        if (roids == 6) //4 small
        {
            for (int i = 0; i < 4; i++)
            {
                float x = 0, y = 0;
                if (i == 1) { x = Random.Range(-2.5f, -1.5f); y = Random.Range(-2f, 2f); }
                if (i == 2) { x = Random.Range(-1.5f, -0.5f); y = Random.Range(-2f, 2f); }
                if (i == 3) { x = Random.Range(0.5f, 1.5f); y = Random.Range(-2f, 2f); }
                if (i == 4) { x = Random.Range(1.5f, 2.5f); y = Random.Range(-2f, 2f); }

                GameObject roid = Instantiate(obstacles[Random.Range(0, obstacles.Length)], new Vector3(x, transform.position.y + y, 0), Quaternion.identity) as GameObject;
                roid.transform.rotation = Random.rotation;
                roid.transform.localScale = roid.transform.localScale * Random.Range(0.2f, 0.8f);
                obstaclesOnMap.Add(roid);

                roid.GetComponent<MeshRenderer>().material.color = GetComponent<ColorsManager>().ColorAsteroids();
            }
        }
        GenerateCoin();
    }


    void GenerateCoin()
    {
        if(Random.value > 0.5f)
        {
            float x = Random.Range(-1.9f, 1.9f), y = Random.Range(-0.5f, 0.5f);
            Collider[] hitObstacles = Physics.OverlapSphere(new Vector3(x, transform.position.y + y, 0), 0.3f);
            if (hitObstacles.Length == 0)
            {
                GameObject coinObj;
                if (jpMode) { coinObj = JpCoinPrefab; } else { coinObj = coinPrefab; }

                GameObject newCoin = Instantiate(coinObj, new Vector3(x, transform.position.y + y, 0), Quaternion.identity) as GameObject;
                coinsOnMap.Add(newCoin);              
            }   
            else
            {
                
            }      
        }
    }

    public void JPButton()
    {
        jpMode = !jpMode;
    }

}
