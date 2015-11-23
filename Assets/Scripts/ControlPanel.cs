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
	public Material leftMat;
	public Material rightMat;
	public Color leftPressed;
	public Color leftUnpressed;
	public Color rightPressed;
	public Color rightUnpressed;
	public Color disabled;

	// Is the control panel on (lit up and accepting input)?
	public bool Activated {
		get {
			return isActivated;
		}
		set {
			// TODO: light up switch when activated, otherwise power down
			isActivated = value;
			if (!isActivated) {
				leftMat.color = disabled;
				rightMat.color = disabled;
			}
			else {
				leftMat.color = leftUnpressed;
				rightMat.color = rightUnpressed;
			}
		}
	}

	private bool isTouching = false;
	private bool isActivated = false;

	void Awake() {
		leftMat.color = leftUnpressed;
		rightMat.color = rightUnpressed;
	}

	void Update() {
		// Space toggles in debug mode
		if (Activated && Config.Debug && Input.GetKeyDown(KeyCode.Alpha1)) {
			buttonState = ButtonState.LeftPressed;
			leftMat.color = leftPressed;
			rightMat.color = rightUnpressed;
			Debug.Log("Pressed left button");
		} else if (Activated && Config.Debug && Input.GetKeyDown(KeyCode.Alpha2)) {
			buttonState = ButtonState.RightPressed;
			leftMat.color = leftUnpressed;
			rightMat.color = rightPressed;
			Debug.Log("Pressed right button");
		}

		if (Activated && buttonState == ButtonState.LeftPressed) {
			buttonState = ButtonState.LeftPressed;
			leftMat.color = leftPressed;
			rightMat.color = rightUnpressed;
			Debug.Log("Pressed left button");
		} else if (Activated && buttonState == ButtonState.RightPressed) {
			buttonState = ButtonState.RightPressed;
			leftMat.color = leftUnpressed;
			rightMat.color = rightPressed;
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
