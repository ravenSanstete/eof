using UnityEngine;
using System.Collections;

public class smoothFlow : MonoBehaviour
{
    public AudioClip soundBG;
    private Transform target;//跟随目标：车
    private float height = 3;
    private float distance = -7;
    private float smoothSpeed = 1;
	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag("Car").transform;

        AudioSource.PlayClipAtPoint(soundBG, new Vector3(0, 0, 0));
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 currentForward = transform.forward;
        currentForward.y = 0;
       Vector3 forward= Vector3.Lerp(currentForward.normalized, targetForward.normalized, smoothSpeed * Time.deltaTime);
        this.transform.position = target.position + Vector3.up * height + forward * distance;
        transform.LookAt(target);
	}
    public void BackTOMenu()
    {
        Application.LoadLevel(0);
    }
}
