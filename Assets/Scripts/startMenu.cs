using UnityEngine;
using System.Collections;
using UnityEngine.UI;
// going to implement the award mode
public enum GAMETYPE { SINGLE,MULTI,AWARD};
public class startMenu : MonoBehaviour {
    public GameObject spriteA;
    public GameObject spriteB;
    public GameObject idInputText;
    public GameObject playButton;
    public GAMETYPE gameType = GAMETYPE.SINGLE;
    public UIInput input;
    public static bool first_time = true;
	// Use this for initialization
	void Start () {
      if(first_time){
        spriteB.SetActive(false);
        playButton.SetActive(false);
        first_time = false;
      }else{
        spriteA.SetActive(false);
      }
	}

	// Update is called once per frame
	void Update () {

	}

    public void playA()
    {
        spriteA.SetActive(false);
        spriteB.SetActive(true);
        PlayerPrefs.SetString("UID", input.text);
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

    private bool validate_text(string id){
        if(id.Length >= 16 || id.Length <= 0 || id.Contains(" ")){
          return false;
        }

        return true;
    }

    public void checkID(){
        if(validate_text(input.text)){
          playButton.SetActive(true);
        }else{
          playButton.SetActive(false);
        }
    }


    public void Quit()
    { Application.Quit(); }
}
