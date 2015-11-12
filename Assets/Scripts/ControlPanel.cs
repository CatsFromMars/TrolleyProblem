using UnityEngine;
using System.Collections;

public class ControlPanel : MonoBehaviour {
	private bool isTouching = false;
	public bool switchFlipped = false;

	void Update() {
		Debug.Log (isTouching);
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag == "Player" && !isTouching) {
			switchFlipped = !switchFlipped;
			Debug.Log("SWITCHED");
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag == "Player" && isTouching) {
			isTouching = false;
		}
	}
}
