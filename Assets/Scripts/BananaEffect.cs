using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaEffect : MonoBehaviour {
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


	void OnTriggerEnter(Collider obj){
		if(obj.gameObject.name == "Collider_Bottom"){
			Driving.banana = true;
			this.gameObject.SetActive(false);
		}
		return;
	}
}
