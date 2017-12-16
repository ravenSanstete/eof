using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Ghost : MonoBehaviour {
	public string log_path = "";


	private List<float> data;
	private GameControls gc;
	private int count = 0;
	public Transform flWheelTrans;
	public Transform frWheelTrans;
	public Transform[] wheelTrans;
	private float RPM = 1000.0f;
	private float wheelAngle = 0.0f;
	// Use this for initialization
	void Start () {
			read_data();

	}
	float normalize(float angle){
		return (angle >= 180)? angle-360:angle;
	}
	// Update is called once per frame
	void Update () {
		gc = GameObject.Find("Finish").GetComponent<GameControls>();
		Vector3 p_1 =  new Vector3(data[6*count], data[6*count + 1], data[6*count + 2]);
		Vector3 r_1 =  new Vector3(data[6*count+3], data[6*count + 4], data[6*count + 5]);

		Vector3 p_2 = p_1;
		Vector3 r_2 = r_1;



		if(gc.isStart && 6*count < data.Count-6){
			// which means there is one over me
			if(6*count > 0){
				Vector3 p_0 = new Vector3(data[6*count - 6], data[6*count - 5], data[6*count - 4]);
				p_2 = new Vector3(data[6*count + 6], data[6*count + 7], data[6*count + 8]);
				r_2 = new Vector3(data[6*count + 9], data[6*count + 10], data[6*count + 11]);
				float acceleration = ((p_2 - p_1).magnitude - (p_1 - p_0).magnitude);
				//print(acceleration);
				wheelAngle = Mathf.Min(Vector3.Angle(p_2 - p_1, p_1 - p_0), 5.0f);
				WheelRotated(acceleration, wheelAngle) ;
			}
			count ++;
			//make the ghost much more realistic, add some rotation to the wheel
		}

		this.gameObject.transform.position = p_1;
		this.gameObject.transform.rotation = Quaternion.Euler(r_1);
	}

	private void WheelRotated(float axisV, float wheelAngle)
	{
			foreach (Transform wheel in wheelTrans)
			{
					wheel.Rotate(RPM* Time.deltaTime * 6, 0, 0);
			}
			flWheelTrans.localEulerAngles = new Vector3(RPM* Time.deltaTime * 6, wheelAngle * 3, 0);
			frWheelTrans.localEulerAngles = new Vector3(RPM* Time.deltaTime * 6, wheelAngle * 3, 0);
	}






	void read_data(){
			data = new List<float>();
			if(!File.Exists(log_path)){
				//fill in the static data and return
				data.Add(this.gameObject.transform.position.x);
						data.Add(this.gameObject.transform.position.y);
								data.Add(this.gameObject.transform.position.z);
									data.Add(this.gameObject.transform.eulerAngles.x);
										data.Add(this.gameObject.transform.eulerAngles.y);
											data.Add(this.gameObject.transform.eulerAngles.z);
								return ;
			}


			//otherwise, read it
			FileStream fs = new FileStream(log_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);;
			StreamReader sr = new StreamReader(fs);
			string line;
			// Read and display lines from the file until the end of
			// the file is reached.
			while ((line = sr.ReadLine()) != null){
				string [] pos = line.Split(new Char[] { ' ' });
				if(pos.Length == 6){
				data.Add((float)Convert.ToSingle(pos[0]));
				data.Add((float)Convert.ToSingle(pos[1]));
				data.Add((float)Convert.ToSingle(pos[2]));
				data.Add((float)Convert.ToSingle(pos[3]));
				data.Add((float)Convert.ToSingle(pos[4]));
				data.Add((float)Convert.ToSingle(pos[5]));
			}
			}

			sr.Close();
			print("IN:" + data.Count);

	}
}
