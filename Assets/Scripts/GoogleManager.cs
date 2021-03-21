using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;


public class GoogleManager : MonoBehaviour
{
    bool loginSuccess;
   
    public string[] scoreAchievementId;
    public string[] rocketAchievementId;
    public string incrementalAchievementId;
    private string scoreLB = "CggIh8-wu3AQAhAL";

    void Awake()
    {    
       PlayGamesPlatform.Activate();
       Social.localUser.Authenticate((bool success) => { if (success) { loginSuccess = true; } else { loginSuccess = false; } }); //Logging to Google Play Games    
    }

    //---------------------Google-Play-Achievements-------------------------
    public void scoreAchievements(int i)
    {
         Social.ReportProgress(scoreAchievementId[i - 1], 100.0f, (bool success) => { });
    }
    public void rocketAchievements(int i)
    {
         Social.ReportProgress(rocketAchievementId[i - 1], 100.0f, (bool success) => { });
    }
    public void IncrementalAchievement()
    {
        PlayGamesPlatform.Instance.IncrementAchievement(incrementalAchievementId, 1, (bool success) => { });
    }



    public void ShowAchievements() //Show Achievements
    {
        if (!loginSuccess) { Social.localUser.Authenticate((bool success) => { if (success) { loginSuccess = true; } else { loginSuccess = false; } return; }); }//Logging to Google Play Games
        Social.ShowAchievementsUI();
    }

    //---------------------Google-Play-Leaderboards-------------------------
    public void LeaderboardUpdate(int score) //Update Leaderboards
    {
        Social.ReportScore(score, scoreLB, (bool success) => { });
    }
    /*
    public void ShowLeaderboards() //Show Leaderboards
    {
        if (!loginSuccess) { Social.localUser.Authenticate((bool success) => { if (success) { loginSuccess = true; } else { loginSuccess = false; } return; }); }//Logging to Google Play Games
        Social.ShowLeaderboardUI();
    }
    */
    public void ShowLeaderboards() //Show Leaderboards
    {
        if (!loginSuccess) { Social.localUser.Authenticate((bool success) => { if (success) { loginSuccess = true; } else { loginSuccess = false; } return; }); }//Logging to Google Play Games
        PlayGamesPlatform.Instance.ShowLeaderboardUI("CggIh8-wu3AQAhAL");
    }
}

