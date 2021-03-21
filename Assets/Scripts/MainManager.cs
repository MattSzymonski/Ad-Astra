using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityStandardAssets.ImageEffects;
using UnityEngine.Analytics;

public class MainManager : MonoBehaviour {

    [Header("Variables")]
    public bool dead;
    public float gameSpeed;

    private int score;
    private int highscore;
    private int currentCoins;
    public int coins;
    float defaultGameSpeed;
    bool inDeadMenu;

    //Escape button
    bool escape;
    int escapeTimer;
    //Share
    bool isProcessing;

    [Header("GUI")]
    public GameObject menuGui;
    public GameObject gameGui;
    public GameObject deadGui;
    public GameObject deadGuiButtons;
    public GameObject blackoutPanel;
    public GameObject splashScreenGo;
    public Button shareButton;
 
    [Header("Other")]
    public GameObject background;
    public GameObject rocket;
    public bool splashScreen;

    [Header("Ads")]
    public bool enableAds;
    public int adFrequency; //Games played with out add, then display (3 def)
    public int playAdCounter;

    [Header("Effects")]
    public float fpsEffectsMin;
    public bool bfx;
    public bool cfx;
    Bloom bloom;
    ColorCorrectionCurves colorCorrection;
    int maxQuality;


    [Header("Tutorial")]
    public int tutorial;
    public GameObject tutorialGUI;


    void Start ()
    {

        bloom = Camera.main.transform.GetComponent<Bloom>();
        colorCorrection = Camera.main.transform.GetComponent<ColorCorrectionCurves>();

        //Graphic options
        if (PlayerPrefs.HasKey("maxQuality")) { maxQuality = PlayerPrefs.GetInt("maxQuality"); } else { PlayerPrefs.SetInt("maxQuality", 0); }
        if (maxQuality == 1) { colorCorrection.enabled = true; cfx = true; bloom.enabled = true; bfx = true; }

        GetComponent<ColorsManager>().ReloadColors();
       
        dead = true;
        defaultGameSpeed = gameSpeed;

        //Load highscore
        if (PlayerPrefs.HasKey("highscore")) { highscore = PlayerPrefs.GetInt("highscore"); } else { PlayerPrefs.SetInt("highscore", 0); }
        menuGui.transform.Find("Highscore").Find("HighscoreText").GetComponent<Text>().text = "" + highscore;

        //Load coins
        if (PlayerPrefs.HasKey("coins")) { coins = PlayerPrefs.GetInt("coins"); } else { PlayerPrefs.SetInt("coins", 0); }
        menuGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins;
        gameGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins;

        //Load tutorial
        if (PlayerPrefs.HasKey("tutorial")) { tutorial = PlayerPrefs.GetInt("tutorial"); } else { PlayerPrefs.SetInt("tutorial", 0); tutorial = 0; }
        if(tutorial == 0) { Tutorial(0); }


        if (splashScreen) //Start splashscreen
        {
            splashScreenGo.SetActive(true);
            if (tutorial != 0) { StartCoroutine(GuiWait("SCToMenu")); }
                
        }
        else //No splashscreen
        {
            menuGui.GetComponent<Animator>().SetBool("Open", true);
        }                   
    }
	
	
	void Update () {
        
        if(!dead)
        {           
            if(rocket.transform.position.y / 2 > score) { score = (int)Mathf.Round(rocket.transform.position.y / 2); }
            gameGui.transform.Find("CurrentScore").GetComponent<Text>().text = "" + score;
            deadGui.transform.Find("CurrentScore").GetComponent<Text>().text = "" + score;           
            if (gameSpeed < 3.85f) { gameSpeed = defaultGameSpeed + (float)score / 80; } else { gameSpeed = 3.85f; }
         //   Move();         
        }
        else if(inDeadMenu)
        {
            deadGui.transform.Find("CurrentScore").GetComponent<Text>().text = "" + score;
      //      Move();
        }
        EscapeButton();

        Achievements();
 
    }

    void Move()
    {



        //Background
       // background.transform.position = new Vector3(0, Camera.main.transform.position.y - 5.45f, 6.7f);
     //   background.transform.GetComponent<MeshRenderer>().material.SetFloat("_TopLine", rocket.transform.position.y + 2.9f);
      //  background.transform.GetComponent<MeshRenderer>().material.SetFloat("_BottomLine", rocket.transform.position.y - 4.5f);
    }

    public void StartGame()
    {
        dead = false;     
        Camera.main.transform.GetComponent<CameraManager>().zoomOut = true; //Start camera zoomOut
        rocket.GetComponent<Rocket>().takeOff = true; //Start rocket take off
        rocket.transform.Find("Smoke").GetComponent<ParticleSystem>().Play();
        GetComponent<WaypointsPlacer>().starting = true; // WaypointsPlacer  
        gameGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins; //Refresh inGame coin counter

        rocket.GetComponent<Rocket>().RocketProperties(GetComponent<ShopManager>().currentRocket);
        GetComponent<WaypointsPlacer>().RocketProperties(GetComponent<ShopManager>().currentRocket);

        //Menu animation 
        if (tutorial == 0) { IEnumerator cor = TutorialWait(1); StartCoroutine(cor); } else { StartCoroutine(GuiWait("MenuToGame")); AdjustEffectsStart(); }
        
        rocket.transform.Find("Models").transform.localPosition = new Vector3((GetComponent<ShopManager>().currentShopPosition - 1) * -3, (GetComponent<ShopManager>().currentShopPosition - 1) * 3, 0);

        GetComponent<AudioManager>().Thrust(true);
        GetComponent<Animator>().SetInteger("State", 1);
        
    }

    public void Die()
    {
        GetComponent<AudioManager>().Thrust(false);
        GetComponent<AudioManager>().Explode();
        GetComponent<Animator>().SetInteger("State", 2);

        dead = true;
        inDeadMenu = true;
        GetComponent<WaypointsPlacer>().waypointMarker.SetActive(false);
        Camera.main.transform.GetComponent<CameraManager>().zoomIn = true; //Start camera zoomIn

        //Saving new highscore and enabling text
        if (score > highscore)
        {
            deadGui.transform.Find("AnnouncementText").gameObject.SetActive(true); //Display announcement text
            background.transform.Find("HighscoreConfetti").gameObject.SetActive(true);

            PlayerPrefs.SetInt("highscore", score);
            highscore = score;
            menuGui.transform.Find("Highscore").Find("HighscoreText").GetComponent<Text>().text = "" + highscore;
            GetComponent<GoogleManager>().LeaderboardUpdate(highscore);
        }

        //Saving coins
        coins = coins + currentCoins;
        PlayerPrefs.SetInt("coins", coins);
        menuGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins;

        deadGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins; //Left Top corner all coins deadMenu
        deadGui.transform.Find("Coins").Find("CurrentCoinsText").GetComponent<Text>().text = "+" + currentCoins; //Left Top corner currently earned coins

        //Incremental Achievement
        GetComponent<GoogleManager>().IncrementalAchievement();

        //InterstitialAd
        if (enableAds) { playAdCounter++; } //add value to counter and load ad when played times in session equals limit

        //Menu animation 
        StartCoroutine(GuiWait("GameToDead"));
    }


    public void ResetGameButton()
    {
        //Menu animation + resetting
        StartCoroutine(GuiWait("DeadToMenu"));
       
    }

    void ResetGame()
    {     
        //Background    
        //background.transform.position = new Vector3(0f, -0.9f, 6.7f);
       // background.transform.GetComponent<MeshRenderer>().material.SetFloat("_TopLine", 2.9f);
       // background.transform.GetComponent<MeshRenderer>().material.SetFloat("_BottomLine", -4.5f);

        //WaypointPlacer
        GetComponent<WaypointsPlacer>().started = false;
        GetComponent<WaypointsPlacer>().startTimer = 0;
        for (int i = 0; i < GetComponent<WaypointsPlacer>().waypointsOnMap.Count; i++) { Destroy(GetComponent<WaypointsPlacer>().waypointsOnMap[i].gameObject); }
        GetComponent<WaypointsPlacer>().waypointsOnMap.Clear();

        //Asteroids
        GetComponent<MapManager>().generationNumber = 0;
        for (int i = 0; i < GetComponent<MapManager>().obstaclesOnMap.Count; i++) { Destroy(GetComponent<MapManager>().obstaclesOnMap[i].gameObject); }
        GetComponent<MapManager>().obstaclesOnMap.Clear();

        //Coins
        for (int i = 0; i < GetComponent<MapManager>().coinsOnMap.Count; i++) { Destroy(GetComponent<MapManager>().coinsOnMap[i].gameObject); }
        GetComponent<MapManager>().coinsOnMap.Clear();

        //Coin particles
        GameObject[] coinParticles = GameObject.FindGameObjectsWithTag("ParticleSystem");
        for (int i = 0; i < coinParticles.Length; i++) { Destroy(coinParticles[i].gameObject); }

        //Rocket
        rocket.transform.position = new Vector3(0, 0, 0);
        rocket.transform.rotation = Quaternion.Euler(-90, 45, 0);
        rocket.GetComponent<Rocket>().movementSpeed = 0;
        rocket.GetComponent<Rocket>().waypoint = null;
        rocket.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rocket.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ParticleSystem.CollisionModule cm = rocket.transform.Find("Smoke").transform.GetComponent<ParticleSystem>().collision;
        cm.enabled = true;
        rocket.transform.Find("Destroyed").GetComponentInChildren<ParticleSystem>().Clear();
        rocket.transform.Find("Destroyed").GetComponentInChildren<ParticleSystem>().Stop();

        //Camera
        Camera.main.transform.position = new Vector3(0f, 6.2f, -5f);
        Camera.main.transform.GetComponent<CameraManager>().zoomIn = false;
        Camera.main.transform.GetComponent<CameraManager>().zoomOut = false;
        Camera.main.orthographicSize = 3.1f;

        //Disable announcement text and confetti
        background.transform.Find("HighscoreConfetti").gameObject.SetActive(false);
        deadGui.transform.Find("AnnouncementText").gameObject.SetActive(false);

        score = 0;
        currentCoins = 0;
        transform.position = new Vector3(0, 5, 0);
        gameSpeed = defaultGameSpeed;
        inDeadMenu = false;

        GetComponent<ColorsManager>().ReloadColors();
    }


    public void AddCoin()
    {
        GetComponent<AudioManager>().CoinCollect();

        currentCoins++;
        int sum = coins + currentCoins;
        gameGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + sum; //Left Top corner all coins gameMenu
    }

    public void rewardedAdReward()
    {
        coins += 30;
        PlayerPrefs.SetInt("coins", coins);
        menuGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins;
        deadGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + coins;

        Analytics.CustomEvent("RewardAdCompleted");
    }

    void Achievements()
    {
        if (score >= 25) { GetComponent<GoogleManager>().scoreAchievements(1); }
        if (score >= 50) { GetComponent<GoogleManager>().scoreAchievements(2); }
        if (score >= 100) { GetComponent<GoogleManager>().scoreAchievements(3); }
        if (score >= 150) { GetComponent<GoogleManager>().scoreAchievements(4); }
        if (score >= 200) { GetComponent<GoogleManager>().scoreAchievements(5); }
        if (score >= 250) { GetComponent<GoogleManager>().scoreAchievements(6); }
    }

    IEnumerator GuiWait(string transition)
    {
        if (transition == "SCToMenu")
        {
            yield return new WaitForSeconds(3f);
            menuGui.GetComponent<Animator>().SetBool("Open", true);
        }
        if (transition == "MenuToGame")
        {
            menuGui.GetComponent<Animator>().SetBool("Open", false);
            yield return new WaitForSeconds(1f);
            gameGui.GetComponent<Animator>().SetBool("Open", true);
        }
        if (transition == "GameToDead")
        {
            yield return new WaitForSeconds(1f);
            gameGui.GetComponent<Animator>().SetBool("Open", false);
            deadGui.GetComponent<Animator>().SetBool("Open", true);                      
        }
        if (transition == "DeadToMenu")
        {

            yield return new WaitForSeconds(0.2f);

            //Open BlackOutPanel
            blackoutPanel.GetComponent<Animator>().SetBool("Open", true);
            yield return new WaitForSeconds(0.4f);  //  if(enableAdNow) { Wait more} 

            deadGui.GetComponent<Animator>().SetBool("Open", false); //Disable deadGui
            ResetGame(); //reseting game when screen is black

            GetComponent<Animator>().SetInteger("State", 3);
            if (enableAds) { if (playAdCounter >= adFrequency) { GetComponent<AdsManager>().ShowAd(); yield return new WaitForSeconds(0.4f); } else { yield return new WaitForSeconds(0.2f); } } else { yield return new WaitForSeconds(0.2f); } //Show ad when played times in session equals limit then reset counter and adjust black screen time
            blackoutPanel.GetComponent<Animator>().SetBool("Open", false);
            yield return new WaitForSeconds(0.3f);

            menuGui.GetComponent<Animator>().SetBool("Open", true);
        }
    }


    public void Tutorial(int card)
    {
        if (card == 0) //Auto start
        {
            tutorialGUI.SetActive(true);
            tutorialGUI.transform.GetChild(0).gameObject.SetActive(true);
        }

        if (card == 1)//Rocket click
        {           
            //wait to start
            tutorialGUI.transform.GetChild(1).gameObject.SetActive(true);
        }

        if (card == 2)
        {
            IEnumerator cor = TutorialWait(2); StartCoroutine(cor);
        }

        if (card == 3)//GUIClick
        {
            tutorialGUI.transform.GetChild(2).gameObject.SetActive(false);
            //wait to show indicator
            tutorialGUI.transform.GetChild(3).gameObject.SetActive(true);
        }
        if (card == 4)//GUIClick
        {
            tutorialGUI.transform.GetChild(3).gameObject.SetActive(false);
            tutorialGUI.SetActive(false);
            Time.timeScale = 1;
            PlayerPrefs.SetInt("tutorial", 1);
            tutorial = 1;
            StartCoroutine(GuiWait("MenuToGame"));
        }
    }

    IEnumerator TutorialWait(int card)
    {
        if (card == 1)
        {
            tutorialGUI.transform.GetChild(0).gameObject.SetActive(false);        
            yield return new WaitForSeconds(1);
            Time.timeScale = 0;
            Tutorial(1);
        }
        if (card == 2)
        {
            tutorialGUI.transform.GetChild(1).gameObject.SetActive(false);
            Time.timeScale = 1;
            yield return new WaitForSeconds(3); // wait to indicator shop
            Time.timeScale = 0;
            tutorialGUI.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    
    public void RateButton()
    {
        Application.OpenURL("http://play.google.com/store/apps/details?id=" + Application.bundleIdentifier);
    }

    void EscapeButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (escape) { Application.Quit(); } else { escape = true; }
        }
        if (escape) { escapeTimer++; }
        if (escapeTimer > 90) { escape = false; escapeTimer = 0; }
    }

    public void ShareButton()
    {
        shareButton.enabled = false;
        if (!isProcessing)
        {
            StartCoroutine(ShareScreenshot());
        }
    }

    public IEnumerator ShareScreenshot()
    {
        isProcessing = true;
        deadGuiButtons.SetActive(false);
        // wait for graphics to render
        yield return new WaitForEndOfFrame();
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
        // create the texture
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        // put buffer into texture
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        // apply
        screenTexture.Apply();
        //----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- PHOTO
        byte[] dataToSave = screenTexture.EncodeToPNG();
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, dataToSave);
        if (!Application.isEditor)
        {
            // block to open the file and share it ------------START
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "Ad Astra");
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"), "SUBJECT");

            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
        }
        isProcessing = false;
        shareButton.enabled = true;
        deadGuiButtons.SetActive(true);
    }

    public void ShowFPSCounter()
    {
        GetComponent<FPSDisplay>().displayFPS = !GetComponent<FPSDisplay>().displayFPS;
    }

    public void AdjustEffectsStart()
    {       
        StartCoroutine("AdjustColorCorrection");
    }

    IEnumerator AdjustColorCorrection()
    {
        float check1, check2, check3;
       
        //ColorCorrection
        GetComponent<FPSDisplay>().checkFps = true;
        yield return new WaitForSeconds(1.5f);
        check1 = GetComponent<FPSDisplay>().fps;
        yield return new WaitForSeconds(0.25f);
        check2 = GetComponent<FPSDisplay>().fps;
        yield return new WaitForSeconds(0.25f);
        check3 = GetComponent<FPSDisplay>().fps;
        GetComponent<FPSDisplay>().checkFps = false;
        if (check1 > fpsEffectsMin && check2 > fpsEffectsMin && check3 > fpsEffectsMin) { colorCorrection.enabled = true; cfx = true; StartCoroutine("AdjustBloom"); } else { colorCorrection.enabled = false; cfx = false; bloom.enabled = false; bfx = false; PlayerPrefs.SetInt("maxQuality", 0); }
    }

    IEnumerator AdjustBloom()
    {
        float check1, check2, check3;

        //Bloom
        GetComponent<FPSDisplay>().checkFps = true;
        yield return new WaitForSeconds(1.5f);
        check1 = GetComponent<FPSDisplay>().fps;
        yield return new WaitForSeconds(0.25f);
        check2 = GetComponent<FPSDisplay>().fps;
        yield return new WaitForSeconds(0.25f);
        check3 = GetComponent<FPSDisplay>().fps;
        GetComponent<FPSDisplay>().checkFps = false;
        if (check1 > fpsEffectsMin - 5 && check2 > fpsEffectsMin - 5 && check3 > fpsEffectsMin - 5) { bloom.enabled = true; bfx = true; PlayerPrefs.SetInt("maxQuality", 1); } else { bloom.enabled = false; bfx = false;  }
    }


    public void FbButton()
    {
       // Application.OpenURL("https://www.facebook.com/QuickBait.AdAstra/");

        
        Application.OpenURL("fb://page/578562098995830");
    }





}
