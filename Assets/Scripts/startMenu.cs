using UnityEngine;
using System.Collections;
// going to implement the award mode
public enum GAMETYPE { SINGLE,MULTI,AWARD};
public class startMenu : MonoBehaviour {
    public GameObject spriteA;
    public GameObject spriteB;
    public GAMETYPE gameType = GAMETYPE.SINGLE;
	// Use this for initialization
	void Start () {
        spriteB.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

	}

    public void playA()
    {
        spriteA.SetActive(false);
        spriteB.SetActive(true);
    }
    public void playSingle()
    {
        gameType = GAMETYPE.SINGLE;
        PlayerPrefs.SetString("GAMETYPE", "SINGLE");
        Application.LoadLevel(1);
    }
    public void playMulti()
    {
        gameType = GAMETYPE.MULTI;
        PlayerPrefs.SetString("GAMETYPE", "MULTI");
        Application.LoadLevel(1);
    }
    public void playAward()
    {
      gameType = GAMETYPE.AWARD;
      PlayerPrefs.SetString("GAMETYPE", "AWARD");
      Application.LoadLevel(1);

    }
    public void Quit()
    { Application.Quit(); }
}
