using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {
	SerialPort port = new SerialPort("COM3", 9600);
	StreamWriter writer;

	public string logDirectory = "Log";
	private bool writing = false;
	private Thread readerThread;

	// Set up Arduino and logger and return experiment number
	public int InitializeArduino() {
		// connect to port for Arduino
		port.ReadTimeout = 500;
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
			int fileCount = Directory.GetFiles(logDirectory, "*.txt", SearchOption.TopDirectoryOnly).Length + 1;
			writer = new StreamWriter(logDirectory + "/heartRate" + fileCount.ToString("D2") + ".txt");
			writer.WriteLine("Heart Rate Log - " + DateTime.Now.ToString("HH:mm:ss.ffff"));
			writer.WriteLine("Current time,time (ms),BPM,EDR");

			// set up reader thread
			readerThread = new Thread(ReadAndWriteData);
			readerThread.IsBackground = true;
			readerThread.Start();
			return fileCount;

		} catch (Exception) {
			Debug.Log("could not connect to Arduino port. aborting");
			writing = false;
			enabled = false;
			return 999;
		}
	}

	private void ReadAndWriteData() {
		while (writing && port.IsOpen) {
	        try {
		        string indata = port.ReadLine();
		        writer.Write(DateTime.Now.ToString("HH:mm:ss.ffff") + ",");
	    	    writer.WriteLine(indata);
	    	} catch (TimeoutException) {
			}
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
