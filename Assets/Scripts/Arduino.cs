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
		port.ReadTimeout = 500;

		try {
			port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
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

	private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
		if (!writing) {
			return;
		}
        SerialPort sp = (SerialPort)sender;
        string indata = sp.ReadExisting();
        writer.Write(indata);
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
