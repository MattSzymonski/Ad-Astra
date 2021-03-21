using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class ShopManager : MonoBehaviour {

    MainManager mainManager;
    public GameObject ShopGUI;
    public GameObject BuyConfirm;
    public GameObject menuButtons;
    GameObject rocket;
    

    [Header("Variables")]
    public bool shopOpened;
    public int currentShopPosition;
    public int currentRocket;

    [Header("Is Unlocked")]
    int rocket2;
    int rocket3;
    int rocket4;
    int rocket5;

    int clickedPrice;


    void Start () {

        mainManager = GetComponent<MainManager>();
        rocket = mainManager.rocket;

        //Loading from local     
        if (PlayerPrefs.HasKey("currentRocket")) { currentRocket = PlayerPrefs.GetInt("currentRocket"); currentShopPosition = currentRocket; } else { PlayerPrefs.SetInt("currentRocket", 1); currentRocket = 1; currentShopPosition = 1; }

        if (PlayerPrefs.HasKey("rocket2")) { rocket2 = PlayerPrefs.GetInt("rocket2"); } else { PlayerPrefs.SetInt("rocket2", 0); }
        if (PlayerPrefs.HasKey("rocket3")) { rocket3 = PlayerPrefs.GetInt("rocket3"); } else { PlayerPrefs.SetInt("rocket3", 0); }
        if (PlayerPrefs.HasKey("rocket4")) { rocket4 = PlayerPrefs.GetInt("rocket4"); } else { PlayerPrefs.SetInt("rocket4", 0); }
        if (PlayerPrefs.HasKey("rocket5")) { rocket5 = PlayerPrefs.GetInt("rocket5"); } else { PlayerPrefs.SetInt("rocket5", 0); }

        //Setting on start
        ShopGUI.transform.Find("ShopItems").Find(currentShopPosition.ToString()).Find("SetButton").Find("SetConfirm").GetComponent<Image>().enabled = true;
        if (rocket2 == 1)
        {
            ShopGUI.transform.Find("ShopItems").Find("2").Find("BuyButton").gameObject.SetActive(false);
            ShopGUI.transform.Find("ShopItems").Find("2").Find("SetButton").gameObject.SetActive(true);
        }
        if (rocket3 == 1)
        {
            ShopGUI.transform.Find("ShopItems").Find("3").Find("BuyButton").gameObject.SetActive(false);
            ShopGUI.transform.Find("ShopItems").Find("3").Find("SetButton").gameObject.SetActive(true);
        }
        if (rocket4 == 1)
        {
            ShopGUI.transform.Find("ShopItems").Find("4").Find("BuyButton").gameObject.SetActive(false);
            ShopGUI.transform.Find("ShopItems").Find("4").Find("SetButton").gameObject.SetActive(true);
        }
        if (rocket5 == 1)
        {
            ShopGUI.transform.Find("ShopItems").Find("5").Find("BuyButton").gameObject.SetActive(false);
            ShopGUI.transform.Find("ShopItems").Find("5").Find("SetButton").gameObject.SetActive(true);
        }

        rocket.transform.Find("Models").transform.localPosition = new Vector3((currentShopPosition - 1) * -3, (currentShopPosition - 1) * 3, 0);

        for (int i = 0; i < 5; i++)
        {
            if (i != currentShopPosition - 1) { rocket.transform.Find("Models").GetChild(i).gameObject.SetActive(false); }
        }
    }
	
	
	void Update () {

        MenuPosition();

        if(shopOpened) { MoveRocket(); }
        
    }


    //-----------Positioning-----------
    public void RightButton()
    {
        if (currentShopPosition != 1)
        {
            currentShopPosition--;
            BuyConfirm.SetActive(false);
        }           
    }
    public void LeftButton()
    {
        if (currentShopPosition != 5)
        {
            currentShopPosition++;
            BuyConfirm.SetActive(false);
        }
    }

    void MenuPosition()
    {
        ShopGUI.transform.Find("ShopItems").GetComponent<RectTransform>().localPosition = Vector3.Lerp(ShopGUI.transform.Find("ShopItems").GetComponent<RectTransform>().localPosition, new Vector3(currentShopPosition * -1300, 0, 0), 8 * Time.deltaTime);
    }

    public void MoveRocket()
    {
        rocket.transform.Find("Models").transform.localPosition = Vector3.Lerp(rocket.transform.Find("Models").transform.localPosition, new Vector3((currentShopPosition - 1) * -3, (currentShopPosition - 1) * 3, 0), 8 * Time.deltaTime);
        rocket.transform.position = new Vector3(0,0,0);
        rocket.transform.eulerAngles = new Vector3(270, 45, 0);
    }
    //-----------Buying-----------
    public void BuyRocket(int price)
    {
        if (mainManager.coins >= price)
        {
            clickedPrice = price;
            BuyConfirm.SetActive(true);
        }
    }
    public void BuyConfirmYES()
    {
        mainManager.coins -= clickedPrice;
        mainManager.menuGui.transform.Find("Coins").Find("CoinsText").GetComponent<Text>().text = "" + mainManager.coins;
        PlayerPrefs.SetInt("coins", mainManager.coins);
        PlayerPrefs.SetInt("rocket" + currentShopPosition, 1);

        ShopGUI.transform.Find("ShopItems").Find(currentShopPosition.ToString()).Find("BuyButton").gameObject.SetActive(false);
        ShopGUI.transform.Find("ShopItems").Find(currentShopPosition.ToString()).Find("SetButton").gameObject.SetActive(true);

        GetComponent<GoogleManager>().rocketAchievements(currentShopPosition - 1);

        BuyConfirm.SetActive(false);

        Analytics.CustomEvent(currentShopPosition.ToString());
    }
    public void BuyConfirmNO()
    {
        BuyConfirm.SetActive(false);
    }

    //-----------Setting-----------
    public void SetRocket()
    {
        currentRocket = currentShopPosition;
        PlayerPrefs.SetInt("currentRocket", currentShopPosition);
   
        for (int i = 0; i < 5; i++) { ShopGUI.transform.Find("ShopItems").Find((i + 1).ToString()).Find("SetButton").Find("SetConfirm").GetComponent<Image>().enabled = false; } //Disable all set icons
        ShopGUI.transform.Find("ShopItems").Find(currentShopPosition.ToString()).Find("SetButton").Find("SetConfirm").GetComponent<Image>().enabled = true; //Enable set icon     

        CloseShop();
    }

    //-----------Opening-Closing-----------
    public void OpenShop()
    {
        shopOpened = true;
        menuButtons.SetActive(false);
        ShopGUI.GetComponent<Animator>().SetBool("Open", true); //Open ShopGUI
        for (int i = 0; i < 5; i++)
        {
            rocket.transform.Find("Models").GetChild(i).gameObject.SetActive(true); 
        }         
    }

    public void CloseShop()
    {
        menuButtons.SetActive(true);
        ShopGUI.GetComponent<Animator>().SetBool("Open", false); //Close ShopGUI
        shopOpened = false;
        currentShopPosition = currentRocket;
    

        for (int i = 0; i < 5; i++)
        {
            if (i != currentShopPosition - 1) { rocket.transform.Find("Models").GetChild(i).gameObject.SetActive(false); }
        }
    }


}
