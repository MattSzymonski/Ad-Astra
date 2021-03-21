using UnityEngine;
using System.Collections;


public class Rocket : MonoBehaviour {

    GameObject mainManagerGO;
    MainManager mainManager;
    ParticleSystem smokeParticles;
    ParticleSystem destroyParticles;

    public float movementSpeed;
    public float rotationSpeed;
    public bool takeOff; 
    public GameObject waypoint;
    public GameObject coinParticlePrefab;
    public GameObject jpCoinParticlePrefab;
    public Material mat;

	void Start () {

        mainManagerGO = GameObject.Find("MainManager");
        mainManager = mainManagerGO.GetComponent<MainManager>();
        smokeParticles = transform.Find("Smoke").GetComponent<ParticleSystem>();
        destroyParticles = transform.Find("Destroyed").GetComponentInChildren<ParticleSystem>();
    }
		
	void FixedUpdate () {
	
        if(!mainManager.dead)
        {
            Move();
            TakeOff();        
        }
        ColorChange();
	}

    void Move()
    {
        if (waypoint != null) //Looking at waypoint
        {
            var rotation = Quaternion.LookRotation(waypoint.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        transform.Translate(Vector3.forward * mainManager.gameSpeed * movementSpeed * Time.deltaTime); //Flying forward
        //transform.localPosition = new Vector3(transform.position.x, transform.position.y, 0);
    }

    void TakeOff()
    {
        if (takeOff)
        {
            movementSpeed += 0.3f * Time.deltaTime;
            if (movementSpeed > 1f)
            {
                movementSpeed = 1f;
                takeOff = false;
                ParticleSystem.CollisionModule cm = smokeParticles.collision;
                cm.enabled = false;
            }
        }
    }

    public void RocketProperties(int rocketType)
    {
        switch (rocketType)
        {
            case 1:
                rotationSpeed = 2;
                break;
            case 2:
                rotationSpeed = 2.25f;
                break;
            case 3:
                rotationSpeed = 2.5f;
                break;
            case 4:
                rotationSpeed = 2.75f;
                break;
            case 5:
                rotationSpeed = 3;
                break;
        }
    }

    void ColorChange()
    {
        mat.color = new Color(0.95f, 0.95f, 0.95f) - new Color(1,1,1) * transform.position.z / 3;
        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, 1);
    }

    void OnCollisionEnter(Collision coll)
    {
        if (!mainManager.dead)
        {
            if (coll.gameObject.tag == "Obstacle")
            {
                mainManager.Die();
                smokeParticles.Stop();
                destroyParticles.Play();
                //Debug.Log("BOOM");
            }

            if (coll.gameObject.tag == "Coin")
            {
                mainManager.AddCoin();
                Destroy(coll.gameObject);

                GameObject coinParticle;
                if (mainManager.GetComponent<MapManager>().jpMode) { coinParticle = jpCoinParticlePrefab; } else { coinParticle = coinParticlePrefab; }
                Instantiate(coinParticle, coll.transform.position, Quaternion.identity);
                //Debug.Log("Coin");
            }
        }
    }



}

