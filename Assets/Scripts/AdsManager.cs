using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour {

    private string unityAdsAndroidGameId = "1144965";



    void Start () {

        Advertisement.Initialize(unityAdsAndroidGameId, true);
	}
	

	void Update () {
	
	}


    public void ShowAd()
    {
        if(Advertisement.IsReady("video"))
        {
            Advertisement.Show("video");
            GetComponent<MainManager>().playAdCounter = 0;
        }
    }

    public void ShowRewardedAd()
    {   
        if (Advertisement.IsReady("rewardedVideo"))
        {
            ShowOptions options = new ShowOptions();
            options.resultCallback = AdCallbackHandler;
            Advertisement.Show("rewardedVideo", options);
        }
    }

    void AdCallbackHandler(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                GetComponent<MainManager>().rewardedAdReward();       
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                break;
        }
    }

}
