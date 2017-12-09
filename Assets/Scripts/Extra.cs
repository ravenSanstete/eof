using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Extra : MonoBehaviour {


	    // some check points
	    private GameObject[] poss = new GameObject[48];
	    private GameObject item_ghost;
	    private bool has_created = false;

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
