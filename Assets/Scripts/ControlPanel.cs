using UnityEngine;
using System.Collections;

public class ControlPanel : MonoBehaviour {
	public bool switchFlipped = false; // false is left, true is right
	public bool Activated {
		get {
			return isActivated;
		}
		set {
			// TODO: light up switch when activated, otherwise power down
			isActivated = value;
		}
	}

	private bool isTouching = false;
	private bool isActivated = false;

	void Update() {
		// Space toggles in debug mode
		if (Activated && Config.Debug && Input.GetButtonDown("Jump")) {
			switchFlipped = !switchFlipped;
		}
	}

	void OnTriggerEnter(Collider other) {
		if (Activated && other.gameObject.tag == "Player" && !isTouching) {
			switchFlipped = !switchFlipped;
			Debug.Log("SWITCHED");
		}
	}

	void OnTriggerExit(Collider other) {
		if (Activated && other.gameObject.tag == "Player" && isTouching) {
			isTouching = false;
		}
	}
}
