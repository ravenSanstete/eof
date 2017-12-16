using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Extra : MonoBehaviour {
	    // some check points
	    private GameObject[] poss = new GameObject[48];
	    private GameObject item_ghost;
			private GameObject[] CarComputer;
	    private bool has_created = false;
			private string type = GAMETYPE.SINGLE.ToString(); // which controls the default behavior of the gametype
			private int AWARD_COUNT = 4;
	    private int GHOST_COUNT = 2;
	    private int TOTAL_COUNT = 8;
			private string [] log_uuids;
			private string LOG_FORMAT = "/Users/morino/Desktop/bp/{0}.csv";

	    private void GetPoss()
	    {
	        int a = 101;
	        while (a<149)
	        {
	            GameObject g=GameObject.Find("pos"+a);
	            if(g!=null)
	            {
	                poss[a - 101] = g;
	                a++;
	            }
	            else
	            {
	                break;
	            }
	        }
	    }

			//a trunk, later the network module will let it ask the server for log data
			private void fetch_logs(){

				//these are just fake codes, which will be implemented after morino has some knowledge
				// about how were the network module implemented
				int i = 0;
				while(i < log_uuids.Length){
					log_uuids[i] = "64284839-a483-4081-b3d1-f74919459283";
					i++;
				}
			}


	// Use this for initialization
	void Start (){
				log_uuids = new string[TOTAL_COUNT];
				fetch_logs();

				GetPoss();

				type = PlayerPrefs.GetString("GAMETYPE");
				CarComputer = GameObject.FindGameObjectsWithTag("CarComputer");

        //indicates the single driver mode
        if (type == GAMETYPE.SINGLE.ToString())
        {
            foreach (GameObject item in CarComputer)
            {
                item.SetActive(false);
            }
        }else{
          // maybe the ghost or the reward mode
          // control the count of computercar over the lane
          int total_deact = TOTAL_COUNT - (type == GAMETYPE.MULTI.ToString()? GHOST_COUNT: AWARD_COUNT);
          int current_count = 0;
          //print(total_deact);
          // only deactivate the specified number of cars on the scene
          // travel over the list
          int j = 0;
          while(current_count < total_deact && j < CarComputer.Length){
            CarComputer[j].SetActive(false);
            current_count ++;
            j++;
          }
        }



				//setup the datapath for each active carcomputer
				for(int j = 0; j < CarComputer.Length; j++){
					if(CarComputer[j].activeSelf){
						//then set the log path
						CarComputer[j].GetComponent<Ghost>().log_path = String.Format(LOG_FORMAT, log_uuids[j]);
					}
				}



		    //set up the items
		    GameObject banana_item = GameObject.Find("BananaItem");

		  	for(int i = 0; i < poss.Length; i++){
		        Instantiate(banana_item, poss[i].transform.position, Quaternion.identity);
		    }

		   	banana_item.SetActive(false);


				// morino should set all the data path for each car in this script




	}

	// Update is called once per frame
	void Update () {

	}

}
