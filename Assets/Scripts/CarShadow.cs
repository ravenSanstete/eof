using UnityEngine;
using System.Collections;

public class CarShadow : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = this.transform.parent.transform.position + Vector3.up * 5;
        this.transform.localEulerAngles = new Vector3(90,0, 0);
	}
}
