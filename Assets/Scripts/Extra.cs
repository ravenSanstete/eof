using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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

	// Use this for initialization
	void Start () {

				GetPoss();

				type = PlayerPrefs.GetString("GAMETYPE");

        //indicates the single driver mode
        if (type == GAMETYPE.SINGLE.ToString())
        {
            CarComputer = GameObject.FindGameObjectsWithTag("CarComputer");
            foreach (GameObject item in CarComputer)
            {
                item.SetActive(false);
            }
        }else{
          // maybe the ghost or the reward mode
          // control the count of computercar over the lane
          int total_deact = TOTAL_COUNT - (type == GAMETYPE.MULTI.ToString()? GHOST_COUNT: AWARD_COUNT);
          int current_count = 0;
          print(total_deact);
          CarComputer = GameObject.FindGameObjectsWithTag("CarComputer");
          // only deactivate the specified number of cars on the scene
          // travel over the list
          int j = 0;
          while(current_count < total_deact && j < CarComputer.Length){
            CarComputer[j].SetActive(false);
            current_count ++;
            j++;
          }
        }


		        //set up the items
		        int i = 0;
		        GameObject banana_item = GameObject.Find("BananaItem");

		        while(i < poss.Length){
		          print(poss[i].transform.position);
		          Instantiate(banana_item, poss[i].transform.position, Quaternion.identity);
		          i++;
		        }

		        banana_item.SetActive(false);


	}

	// Update is called once per frame
	void Update () {

	}

}
