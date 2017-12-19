using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

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


			private string write_track(float[] track_data){
					FileStream fs;
					string guid = Guid.NewGuid().ToString();
					string log_path = String.Format(LOG_FORMAT, guid);
					//check whether such a file exists
					if(!File.Exists(log_path)){
						 fs = File.Create(log_path);
					}else{
						 fs = File.Open(log_path, FileMode.Open, FileAccess.Write, FileShare.None);
					}

					StreamWriter sw = new StreamWriter(fs);
					int i = 0;
					print(track_data.Length);
					while(i < track_data.Length){
						sw.WriteLine(track_data[i] + " " + track_data[i+1]+ " " + track_data[i+2]+" "+track_data[i+3] + " " + track_data[i+4] + " " + track_data[i+5] + " " + track_data[i+6]); // the format has been modified to be 3 (pos) 4 (quat)
						i = i + 7;
					}
					sw.Close();
					return guid;
			 }

			//a trunk, later the network module will let it ask the server for log data
			private void fetch_logs(){

				//these are just fake codes, which will be implemented after morino has some knowledge
				// about how were the network module implemented
				UnityWebRequest www = UnityWebRequest.Get(Driving.server_url);
				www.Send();

				if(www.isNetworkError || www.isHttpError) {
						Debug.Log(www.error);
				}
				else {
					// Show results as text
					//Debug.Log(www.downloadHandler.text);

					// Or retrieve results as binary data
					byte[] results = www.downloadHandler.data;
					// write data locally
					string json_str = Encoding.UTF8.GetString(results);

					print(json_str);

					// then parsing the json string and write them into arbitraty files

				}

				//setting the path of the file
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
        if (type == GAMETYPE.SINGLE.ToString() || type == GAMETYPE.MULTI.ToString())
        {
            foreach (GameObject item in CarComputer)
            {
                item.SetActive(false);
            }

						BananaEffect.item_enabled = (type == GAMETYPE.MULTI.ToString());
        }else{
          // maybe the ghost or the reward mode
          // control the count of computercar over the lane
          int total_deact = TOTAL_COUNT - AWARD_COUNT;
          int current_count = 0;
          //print(total_deact);
          // only deactivate the specified number of cars on the scene
          // travel over the list
          int k = 0;
          while(current_count < total_deact && k < CarComputer.Length){
            CarComputer[k].SetActive(false);
            current_count ++;
            k++;
          }



				//setup the datapath for each active carcomputer
				for(int j = 0; j < CarComputer.Length; j++){
					if(CarComputer[j].activeSelf){
						//then set the log path
						print(j);
						CarComputer[j].GetComponent<Ghost>().log_path = String.Format(LOG_FORMAT, log_uuids[j]);
					}
				}
      }



			GameObject banana_item = GameObject.Find("BananaItem");

				if(BananaEffect.item_enabled){
		    	//set up the items


		  		for(int i = 0; i < poss.Length; i++){
		        Instantiate(banana_item, poss[i].transform.position, Quaternion.identity);
		    	}
				}


				banana_item.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

	}

}
