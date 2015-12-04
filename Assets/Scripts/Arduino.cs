using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;

public class Arduino : MonoBehaviour {
	SerialPort port;
	StreamWriter writer;

	public string logDirectory = "Log";
	private bool writing = false;

	void Start() {

			int fileCount = Directory.GetFiles(logDirectory, "*.txt", SearchOption.TopDirectoryOnly).Length + 1;
			writer = new StreamWriter(logDirectory + "/heartRate" + fileCount.ToString("D2") + ".txt");
			writer.WriteLine("Heart Rate Log - " + DateTime.Now.ToString("HH:mm:ss.ffff"));
			writer.WriteLine("Current time,time (ms),BPM,EDR");

		// connect to port for Arduino
		port = new SerialPort("COM3", 9600);
		port.ReadTimeout = 10;
		port.Parity = Parity.None;
        port.DataBits = 8;
        port.StopBits = StopBits.One;
        port.RtsEnable = true;
        port.Handshake = Handshake.None;

		try {
			port.Open();
			Debug.Log("connected successfully to Arduino");

			// set up writer log
			writing = true;
			/*int fileCount = Directory.GetFiles(logDirectory, "*.txt", SearchOption.TopDirectoryOnly).Length + 1;
			writer = new StreamWriter(logDirectory + "/heartRate" + fileCount.ToString("D2") + ".txt");
			writer.WriteLine("Heart Rate Log - " + DateTime.Now.ToString("HH:mm:ss.ffff"));
			writer.WriteLine("Current time,time (ms),BPM,EDR");*/
		} catch (Exception e) {
			Debug.Log(e.ToString());
			Debug.Log("could not connect to Arduino port. aborting");
			writing = false;
			enabled = false;
		}
	}

	void Update() {
		if (!(writing && port.IsOpen)) {
			return;
		}

        try {
	        string indata = port.ReadLine();
	        writer.Write(DateTime.Now.ToString("HH:mm:ss.ffff") + ",");
    	    writer.WriteLine(indata);
    	} catch (TimeoutException e) {
		}
    }

	public void Finish() {
		if (writer != null) {
			writer.Close();
		}
		if (port.IsOpen) {
			port.Close();
		}
	}

	void OnDestroy() {
		Finish();
	}
}
