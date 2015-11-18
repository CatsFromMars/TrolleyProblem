using UnityEngine;
using System.Collections;

[System.Serializable]
public enum ButtonState {
	NotPressed,
	LeftPressed,
	RightPressed
};

public class ControlPanel : MonoBehaviour {
	public ButtonState buttonState = ButtonState.NotPressed;

	// Is the control panel on (lit up and accepting input)?
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
		if (Activated && Config.Debug && Input.GetButtonDown("1")) {
			buttonState = ButtonState.LeftPressed;
			Debug.Log("Pressed left button");
		} else if (Activated && Config.Debug && Input.GetButtonDown("2")) {
			buttonState = ButtonState.RightPressed;
			Debug.Log("Pressed right button");
		}
	}

	void OnTriggerEnter(Collider other) {
		// TODO: currently only sets left button. should case on which button is pressed
		if (Activated && other.gameObject.tag == "Player" && !isTouching) {
			buttonState = ButtonState.LeftPressed;
			Debug.Log("SWITCHED");
		}
	}

	void OnTriggerExit(Collider other) {
		if (Activated && other.gameObject.tag == "Player" && isTouching) {
			isTouching = false;
		}
	}
}
