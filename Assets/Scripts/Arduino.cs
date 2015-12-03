using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;

public class Arduino : MonoBehaviour {
	SerialPort port;
	StreamWriter writer;

	private bool writing = false;

	void Start() {
		// connect to port for Arduino
		port = new SerialPort("COM3", 9600);
		try {
			port.Open();
			Debug.Log("connected successfully to Arduino");

			// set up writer log
			writing = true;
			writer = new StreamWriter("Log/test.txt");
			writer.WriteLine("Heart Rate Log - " + DateTime.Now.ToString());
		} catch (Exception e) {
			Debug.Log(e.ToString());
			Debug.Log("could not connect to Arduino port. aborting");
			writing = false;
			enabled = false;
		}
	}

	void Update() {
		if (writing && port.IsOpen) {
			String message = port.ReadLine();
			if (message.Length > 0) {
				writer.WriteLine(message);
			}
		}
	}

	public void Finish() {
		if (writer != null) {
			writer.Close();
		}
	}

	void OnDestroy() {
		Finish();
	}
}
