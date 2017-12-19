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
	private float RPM = 100.0f;
	private float wheelAngle = 0.0f;

	private float current_tp = 0.0f;
	private int STRIDE = 7;



	// Use this for initialization
	void Start () {
			read_data();
			//Driving.upload_track("helo", 150, data);

	}
	float normalize(float angle){
		return (angle >= 180)? angle-360:angle;
	}

	private Vector3 velocity = Vector3.zero;
	// Update is called once per frame
	void FixedUpdate () {
		gc = GameObject.Find("Finish").GetComponent<GameControls>();

		if(gc.isStart){
			//use the current tp to compute the index of the current frame
			int i = (int)Mathf.Floor(current_tp / Driving.fps);
			int j = (i >= data.Count/STRIDE)? i : i +1;

			float split = (i == j)? 0.5f : (current_tp / Driving.fps - (float)i);

			//print("current frame :" + i + " split :" + split);

			if(i > data.Count/STRIDE - 1){
				i = data.Count/STRIDE - 1;
			}

			if(j > data.Count/STRIDE - 1){
				j = data.Count/STRIDE - 1;
			}

			this.gameObject.transform.position = Vector3.Lerp(new Vector3(data[STRIDE*i], data[STRIDE*i+1], data[STRIDE*i+2]),
																												new Vector3(data[STRIDE*j], data[STRIDE*j+1], data[STRIDE*j+2]),
																												split);
			this.gameObject.transform.rotation = Quaternion.Slerp(new Quaternion(data[STRIDE*i+3], data[STRIDE*i+4], data[STRIDE*i+5], data[STRIDE*i+6]),
																												new Quaternion(data[STRIDE*j+3], data[STRIDE*j+4], data[STRIDE*j+5], data[STRIDE*j+6]),
																												split);

			//set the wheel
			float axisV = (new Vector3(data[STRIDE*i], data[STRIDE*i+1], data[STRIDE*i+2]) - new Vector3(data[STRIDE*j], data[STRIDE*j+1], data[STRIDE*j+2])).magnitude;
			WheelRotated(axisV, 0.0f);


			current_tp += Time.deltaTime;
		}else{
			this.gameObject.transform.position = new Vector3(data[0], data[1], data[2]);
			this.gameObject.transform.rotation =   new Quaternion(data[3], data[4], data[5], data[6]);
		}

	}

	private void WheelRotated(float axisV, float wheelAngle)
	{
			foreach (Transform wheel in wheelTrans)
			{
					wheel.Rotate(RPM * 6 * Time.deltaTime, 0, 0);
			}
			//flWheelTrans.localEulerAngles = new Vector3(RPM* Time.deltaTime * 6, wheelAngle * 3, 0);
			//frWheelTrans.localEulerAngles = new Vector3(RPM* Time.deltaTime * 6, wheelAngle * 3, 0);
	}






	void read_data(){
			data = new List<float>();
			if(!File.Exists(log_path)){
				//fill in the static data and return
				data.Add(this.gameObject.transform.position.x);
						data.Add(this.gameObject.transform.position.y);
								data.Add(this.gameObject.transform.position.z);
									data.Add(this.gameObject.transform.rotation.x);
										data.Add(this.gameObject.transform.rotation.y);
											data.Add(this.gameObject.transform.rotation.z);
												data.Add(this.gameObject.transform.rotation.w);
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
				if(pos.Length == STRIDE){
				data.Add((float)Convert.ToSingle(pos[0]));
				data.Add((float)Convert.ToSingle(pos[1]));
				data.Add((float)Convert.ToSingle(pos[2]));
				data.Add((float)Convert.ToSingle(pos[3]));
				data.Add((float)Convert.ToSingle(pos[4]));
				data.Add((float)Convert.ToSingle(pos[5]));
				data.Add((float)Convert.ToSingle(pos[6]));
			}
			}

			sr.Close();
			print("IN:" + data.Count);

	}
}
