using UnityEngine;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    public bool displayFPS;
    public float fps;
    public bool checkFps;
    MainManager mm;

    void Start()
    {
        mm = GetComponent<MainManager>();
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        if (displayFPS || checkFps)
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            fps = 1.0f / deltaTime;
        }
    }

    void OnGUI()
    {
        if (displayFPS)
        {
            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 100;
            style.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
            float msec = deltaTime * 1000.0f;        
             
            string text = string.Format("{0:0.0} ms ({1:0.} fps) (CCC: {2:0} B: {3:0})", msec, fps, mm.cfx.ToString(), mm.bfx.ToString());
            GUI.Label(rect, text, style);
        }
    }
   
}


