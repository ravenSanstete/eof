using UnityEngine;
using System.Collections;

public class GameControls : MonoBehaviour
{
    private GameObject car;
    private Driving driving;
    public bool isStart = false;
    public bool isOver = false;
    private UILabel timeLabel, startLabel, bestLabel;
    private float time = 0;
    private bool isGameCompleted = false;
    private GameObject[] CarComputer;
    private string type = GAMETYPE.SINGLE.ToString();
    // Use this for initialization
    void Start()
    {
        type = PlayerPrefs.GetString("GAMETYPE");
        if (type == GAMETYPE.SINGLE.ToString())
        {
            CarComputer = GameObject.FindGameObjectsWithTag("CarComputer");
            foreach (GameObject item in CarComputer)
            {
                item.SetActive(false);
            }
        }
        car = GameObject.FindGameObjectWithTag("Car");
        driving = car.GetComponent<Driving>();
        timeLabel = GameObject.Find("Time").GetComponent<UILabel>();
        startLabel = GameObject.Find("Start").GetComponent<UILabel>();
        bestLabel = GameObject.Find("Best").GetComponent<UILabel>();
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
                    startLabel.text = "";
                    timeLabel.text = (Mathf.RoundToInt(time) - 3).ToString();
                }
                else
                {
                    if (type != GAMETYPE.SINGLE.ToString())
                        return;
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

    private int lap = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider_Bottom")
        {


            driving.startPos = transform.position;
            driving.startQua = transform.rotation;



            if (this.name == "Finish")
            {
              // a much finer detection of success
              if(other.gameObject.transform.parent.GetComponent<Rigidbody>().velocity.x <= 0){
                lap += 1;
              }else{
                lap -= 1;
              }
              if (lap >= 1){
                  isOver = true;

                  if(PlayerPrefs.GetString("GAMETYPE") == GAMETYPE.SINGLE.ToString())
                  { }
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
            }
        }
    }
}
