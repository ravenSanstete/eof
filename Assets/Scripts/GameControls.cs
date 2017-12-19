using UnityEngine;
using System.Collections;

public class GameControls : MonoBehaviour
{
    private GameObject car;
    private Driving driving;
    public bool isStart = false;
    public bool isOver = false;
    private GameObject start_sprite;
    private UILabel timeLabel, startLabel, bestLabel;
    private float time = 0;
    private bool isGameCompleted = false;
    private string type = GAMETYPE.SINGLE.ToString(); // which controls the default behavior of the gametype


    // Use this for initialization
    void Start()
    {
        car = GameObject.FindGameObjectWithTag("Car");
        driving = car.GetComponent<Driving>();
        timeLabel = GameObject.Find("Time").GetComponent<UILabel>();
        startLabel = GameObject.Find("Start").GetComponent<UILabel>();
        bestLabel = GameObject.Find("Best").GetComponent<UILabel>();
        start_sprite = GameObject.Find("Start_Sp");
        type = PlayerPrefs.GetString("GAMETYPE");
        //PlayerPrefs.DeleteAll();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameCompleted || this.name != "Finish")
            return;
        time += Time.deltaTime;
        if (time <= 2)
            startLabel.text = "Ready";
        else
            if (time <= 3)
                startLabel.text = "Go";
            else
            {
                isStart = true;
                if (!isOver)
                {
                    start_sprite.SetActive(false);
                    startLabel.text = "";
                    timeLabel.text = (Mathf.RoundToInt(time) - 3).ToString();
                }
                else
                {

                    if (type != GAMETYPE.SINGLE.ToString() && type != GAMETYPE.MULTI.ToString())
                        return;

                    start_sprite.SetActive(true);
                    time = int.Parse(timeLabel.text);
                    startLabel.text = "Your Time: " + timeLabel.text + "″";
                    if (PlayerPrefs.HasKey("bestTime"))
                    {
                        float best = PlayerPrefs.GetFloat("bestTime");
                        if (time < best)
                        {
                            PlayerPrefs.SetFloat("bestTime", Mathf.RoundToInt(time));
                            bestLabel.text = "You're the best！";
                        }
                        else
                        {
                            bestLabel.text = "Best Time: " + best + "″";
                        }
                    }
                    else
                    {
                        bestLabel.text = "You're the best！";
                        PlayerPrefs.SetFloat("bestTime", Mathf.RoundToInt(time));
                    }
                    isGameCompleted = true;
                }
            }
    }
    public int Rank = 1;
    public int fake_Count = 0;

    private int lap = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider_Bottom")
        {


            driving.startPos = transform.position;
            driving.startQua = transform.rotation;



            if (this.name == "Finish")
            {
              print(this.transform.forward);
              print(other.gameObject.transform.parent.GetComponent<Rigidbody>().velocity);
              // a much finer detection of success
              fake_Count += 1;
              if(Vector3.Dot(Vector3.Normalize(other.gameObject.transform.parent.GetComponent<Rigidbody>().velocity), this.transform.forward) > 0.005){
                lap += 1;
              }else{
                lap -= 1;
              }

              // I only set the condition to be true, since I would like to record some data for test
              if (true/*lap > 0 || fake_Count >= 10 ||  int.Parse(timeLabel.text) >= 150*/){
                  isOver = true;

                  start_sprite.SetActive(true);

                  if(PlayerPrefs.GetString("GAMETYPE") == GAMETYPE.SINGLE.ToString() || PlayerPrefs.GetString("GAMETYPE") == GAMETYPE.MULTI.ToString())
                  {

                  }
                  else
                  {
                    startLabel.text = "Your Rank: " + Rank.ToString();
                  }
                }
            }
        }


        if (other.gameObject.name == "Collider_BottomComputer")
        {
            if (this.name == "Finish")
            {
                    Rank++;
                    print(other.gameObject.transform.parent.name);
                    print("Computer finished");
            }
        }
    }




}
