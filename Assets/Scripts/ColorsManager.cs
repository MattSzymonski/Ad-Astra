using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ColorsManager : MonoBehaviour {

    int currentScore;
    public float changeSpeed;
    bool menuDraw;

    [Header("Active Colors")]
    public Color bg2New; //Targer color for current bg2 color
    public Color bg2Current; //Current state

    public Color bg2Old; //Old state but also current on beginning  /  Targer color for current bg1 color
    public Color bg1;

    public Color asteroids;
    public int[] asteroidValues;
         
    public List<Color> availableColors; //List of colors never used before or used long time ago
    public List<Color> nonAvailableColors; //List of colors used recently

    [Header("Predefined")]
    public Color[] gameColors; //Predefined colors to draw background
    public Color[] asteroidsColors;  //Predefined colors to draw asteroids
    public GameObject bg; //Background object


    //509FDBFF blue
    //9F1818FF red
    //2A2F52FF pomegranate
    //76FFE2FF turcuse
    //A91FFFFF violet
    //06397FFF dark blue
    //FFF9A8FF light yellow
    //A8D9FFFF light blue
    //000000FF black
    //E2E2E2FF grey

    void Update ()
    {

        if (!GetComponent<MainManager>().dead) { ColorChange(); }

        // currentScore = (int)Mathf.Round(GetComponent<MainManager>().rocket.transform.position.y / 2);
        // if (GetComponent<MainManager>().rocket.transform.position.y/2 > currentScore && !GetComponent<MainManager>().dead) { ColorChange(); }

    }

    

    public void ReloadColors()
    {
        menuDraw = true;
        asteroidValues = new int[3];
        availableColors.Clear();
        nonAvailableColors.Clear();
        for (int i = 0; i < gameColors.Length; i++) { availableColors.Add(gameColors[i]); }  //Adding all colors to available colors list
        DrawColorsStart();
    }

    void DrawColorsStart()
    {
        //Background bottom
        int newBg1Color = Random.Range(0, availableColors.Count);
        bg1 = availableColors[newBg1Color];
        nonAvailableColors.Add(availableColors[newBg1Color]);
        availableColors.RemoveAt(newBg1Color);

        //Background top
        int newBg2Color = 0;
        if (menuDraw && newBg1Color < 6) { newBg2Color = Random.Range(0, availableColors.Count); } else { newBg2Color = Random.Range(0, availableColors.Count - 4); }
        
        bg2Current = availableColors[newBg2Color];
        bg2Old = availableColors[newBg2Color];
        nonAvailableColors.Add(availableColors[newBg2Color]);
        availableColors.RemoveAt(newBg2Color);
        menuDraw = false;

        //Background new top
        int newBg2NewColor = Random.Range(0, availableColors.Count);
        bg2New = availableColors[newBg2NewColor];
        nonAvailableColors.Add(availableColors[newBg2NewColor]);
        availableColors.RemoveAt(newBg2NewColor);

        bg.GetComponent<BackgroundGradient>().bottom = bg1;
        bg.GetComponent<BackgroundGradient>().top = bg2Current;


        //Obstacles
        /*
        asteroids = new Color(Random.value, Random.value, Random.value);
        while ((asteroids.r > 0.7f && asteroids.r < 1) && (asteroids.g < 0.5f) && (asteroids.b > 0.6f && asteroids.b < 0.7f))
        {
            asteroids = new Color(Random.value, Random.value, Random.value);
            if (asteroids.r > 0.5f) { asteroidValues[0] = 1; } else { asteroidValues[0] = -1; }
            if (asteroids.g > 0.5f) { asteroidValues[1] = 1; } else { asteroidValues[1] = -1; }
            if (asteroids.b > 0.5f) { asteroidValues[2] = 1; } else { asteroidValues[2] = -1; }
        }    
        */
        asteroids = asteroidsColors[Random.Range(0, asteroidsColors.Length)];
        if (asteroids.r > 0.5f) { asteroidValues[0] = 1; } else { asteroidValues[0] = -1; }
        if (asteroids.g > 0.5f) { asteroidValues[1] = 1; } else { asteroidValues[1] = -1; }
        if (asteroids.b > 0.5f) { asteroidValues[2] = 1; } else { asteroidValues[2] = -1; }
    }

    Color DrawNewColor(Color color)
    {
        int newColor = Random.Range(0, availableColors.Count);
        color = availableColors[newColor];
        nonAvailableColors.Add(availableColors[newColor]);
        availableColors.RemoveAt(newColor);

        return color;
    }

    void ColorChange()
    {
        bg1 = Color.Lerp(bg1, bg2Old, 0.001f * changeSpeed);
        bg2Current = Color.Lerp(bg2Current, bg2New, 0.1f * changeSpeed * Time.deltaTime);

        if(CheckColorSimilarity(bg2Current,bg2New))
        {            
            bg2Old = bg2Current; //Set old bg2 color as current bg2 color
            bg2New = DrawNewColor(bg2New);  //Draw bg2 new

            if (nonAvailableColors.Count > 5)
            {
                availableColors.Add(nonAvailableColors[0]);
                nonAvailableColors.RemoveAt(0);
            }          
        }

        bg1 = new Color(bg1.r, bg1.g, bg1.b, 1);
        bg2Current = new Color(bg2Current.r, bg2Current.g, bg2Current.b, 1);

        bg.GetComponent<BackgroundGradient>().bottom = bg1;
        bg.GetComponent<BackgroundGradient>().top = bg2Current;

    }

    bool CheckColorSimilarity(Color col1, Color col2)
    {
        bool similar = false;

        if(col1.r >= col2.r - 0.15f && col1.r <= col2.r + 0.15f) {
            if (col1.g >= col2.g - 0.15f && col1.g <= col2.g + 0.15f) {
                if (col1.b >= col2.b - 0.15f && col1.b <= col2.b + 0.15f) {
                    similar = true;
                }
            }           
        }
        return similar;
    }


    public Color ColorAsteroids()
    {
        Color color;
        color = asteroids - new Color(0.01f * asteroidValues[0], 0.01f * asteroidValues[1], 0.01f * asteroidValues[2], 0.0f) * GetComponent<MapManager>().generationNumber;
        return color;      
    }
}
 