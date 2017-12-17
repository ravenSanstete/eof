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
			private int AWARD_COUNT = 8;
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
				log_uuids[0] = "1eba5eab-2316-4ba9-8935-4c20acdfb33f";
				log_uuids[1] = "185b724d-1f5c-4484-98ed-6220ca34a743";
				log_uuids[2] = "0b59a675-01f9-4204-8775-0d5c43af808f";
				log_uuids[3] = "efed9bf0-eff6-4c7c-b25e-84bcda19df96";
				log_uuids[4] = "3dac6187-f230-4e14-bf80-aca9af0a9702";
				log_uuids[5] = "437e9cb1-ca30-4a6e-a12d-a29a8ef7532e";
				log_uuids[6] = "cb4fcdba-4585-4d60-8bf3-514d1f407f6e";
				log_uuids[7] = "9465ba19-a3b8-4e7b-8320-64bcf9cbf88a";
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
						print(j);
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
