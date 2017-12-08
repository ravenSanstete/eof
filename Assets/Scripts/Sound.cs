using UnityEngine;
using System.Collections;
using System;

public  class Sound : MonoBehaviour {
    public  AudioClip[] sounds;
	// Use this for initialization
    void Awake()
    {
        object a = Resources.Load(@"Sound", typeof(GameObject));
    }
	
    //// Update is called once per frame
   //public void play(string name)
   // {
   //     if (sounds == null)
   //         return;
   //     foreach (AudioClip item in sounds)
   //     {
   //         AudioSource.PlayClipAtPoint(item, new Vector3(0, 0, 0));
   //         break;
   //     }       
   // }
}
