using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.IO.Ports;
using System.Threading;

public class Arduino : MonoBehaviour {
	public StreamWriter writer;
	public string logDirectory = "Log";
	public string logEvent = "";
	public bool writing = false;

	private Thread readerThread;
	private SerialPort port = new SerialPort("COM3", 9600);

	// Set up Arduino and logger and return file name
	public string InitializeArduino() {
		// connect to port for Arduino
		port.ReadTimeout = 500;
		port.Parity = Parity.None;
        port.DataBits = 8;
        port.StopBits = StopBits.One;
        port.RtsEnable = true;
        port.Handshake = Handshake.None;

		string filename = logDirectory + "/subject999.csv";

		try {
			port.Open();
			Debug.Log("connected successfully to Arduino");

			// set up writer log
			writing = true;
			int fileCount = Directory.GetFiles(logDirectory, "*.csv", SearchOption.TopDirectoryOnly).Length + 1;
			filename = logDirectory + "/subject" + fileCount.ToString("D2") + ".csv";
			writer = new StreamWriter(filename);
			writer.WriteLine("Log: Subject " + fileCount.ToString("D2"));
			writer.WriteLine("Current Time,Time (ms),Heart Rate (BPM),EDR,Event");

			// set up reader thread
			readerThread = new Thread(ReadAndWriteData);
			readerThread.IsBackground = true;
			readerThread.Start();
			return filename;

		} catch (Exception) {
			Debug.Log("could not connect to Arduino port. aborting");
			writing = false;
			enabled = false;
			return filename;
		}
	}

	private void ReadAndWriteData() {
		while (writing && port.IsOpen) {
	        try {
		        string indata = port.ReadLine();
		        writer.Write(DateTime.Now.ToString("HH:mm:ss.ffff") + ",");
	    	    writer.Write(indata);
	    	    writer.WriteLine("," + logEvent);
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
