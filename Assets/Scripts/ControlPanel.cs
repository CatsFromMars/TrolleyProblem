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
	public Animator leftAnimator;
	public Animator rightAnimator;
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

	private bool isActivated = false;

	void Awake() {
		leftMat.color = disabled;
		rightMat.color = disabled;
	}

	void Update() {
		// pressing 1, 2 activate buttons in debug mode
		if (Activated && Config.Debug && Input.GetKeyDown(KeyCode.Alpha1)) {
			buttonState = ButtonState.LeftPressed;
			leftMat.color = leftPressed;
			rightMat.color = rightUnpressed;
			rightAnimator.SetTrigger(Animator.StringToHash("Push"));
			Debug.Log("Pressed left button");
		} else if (Activated && Config.Debug && Input.GetKeyDown(KeyCode.Alpha2)) {
			buttonState = ButtonState.RightPressed;
			leftMat.color = leftUnpressed;
			rightMat.color = rightPressed;
			leftAnimator.SetTrigger(Animator.StringToHash("Push"));
			Debug.Log("Pressed right button");
		}

		if (Activated && buttonState == ButtonState.LeftPressed) {
			leftMat.color = leftPressed;
			rightMat.color = rightUnpressed;
		} else if (Activated && buttonState == ButtonState.RightPressed) {
			leftMat.color = leftUnpressed;
			rightMat.color = rightPressed;
		}
	}
}
