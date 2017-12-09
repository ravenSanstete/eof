using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Ghost : MonoBehaviour {
	private string log_path = "/Users/morino/Desktop/bp/log.csv";
	private List<float> data;
	private GameControls gc;
	private int count = 0;
	// Use this for initialization
	void Start () {
			read_data();

	}

	// Update is called once per frame
	void Update () {
		gc = GameObject.Find("Finish").GetComponent<GameControls>();
		if(gc.isStart && 6*count < data.Count-6){
			count ++;
		}

		this.gameObject.transform.position = new Vector3(data[6*count], data[6*count+1], data[6*count + 2]);
		this.gameObject.transform.rotation = Quaternion.Euler(data[6*count+3], data[6*count+4], data[6*count + 5]);

	}


	void read_data(){
			data = new List<float>();
			if(!File.Exists(log_path)){
				data.Add(this.gameObject.transform.position.x);
						data.Add(this.gameObject.transform.position.y);
								data.Add(this.gameObject.transform.position.z);
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
