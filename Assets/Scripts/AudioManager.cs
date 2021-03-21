using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

    public AudioClip[] audioClips;
    public AudioSource[] audioSource;

    void Start()
    {
        audioSource = new AudioSource[5];
        audioSource[0] = gameObject.GetComponent<AudioSource>();

        for (int i = 0; i < 4; i++)
        {
            audioSource[i + 1] = gameObject.AddComponent<AudioSource>();
            audioSource[i + 1].clip = audioClips[i];
            audioSource[i + 1].playOnAwake = false;
            audioSource[i + 1].volume = 0.7f;
            if (i == 0) { audioSource[i + 1].loop = true; audioSource[i + 1].volume = 0.2f; } //Thrust
        }
    }

    //-------------------Playing sounds----------------------
    public void Music()
    {
        audioSource[0].Play();
    }
    public void Thrust(bool t)
    {
        if (t) { audioSource[1].Play(); } else { audioSource[1].Stop(); }       
    }
    public void Click()
    {
        audioSource[2].Play();        
    } 
    public void Explode()
    {
        audioSource[3].Play();
    }
    public void CoinCollect()
    {
        audioSource[4].Play();
    }
 
   
}