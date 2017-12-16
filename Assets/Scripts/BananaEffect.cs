using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BananaEffect : MonoBehaviour {
	private int item_type_count = 5;
	public static bool item_enabled = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	void switch_on_item(){
		UnityEngine.Random.seed = (int)DateTime.Now.Ticks;
		int rd = (int)Mathf.Floor(UnityEngine.Random.Range(0, 100)) % 5;


		switch (rd) {
				case 0: Driving.banana = true;
								break;
				case 1: Driving.shrink = true;
								break;
				case 2: Driving.mirror = true;
								break;
				case 3: Driving.mirror = true;
								break;
				case 4: Driving.freeze = true;
								break;
				default: break;
			}
	}

	void OnTriggerEnter(Collider obj){
		if(obj.gameObject.name == "Collider_Bottom"){

			if(BananaEffect.item_enabled){
				switch_on_item();
			}
			//otherwise, only record the data
			this.gameObject.SetActive(false);
		}
		return;
	}
}
