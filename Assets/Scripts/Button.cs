using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	public enum button {LEFT, RIGHT};
	public button b;
	public ControlPanel c;
	public Material mat;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hand") {
			Debug.Log("TWAS A HAND");
			if (b == button.LEFT) {
				c.buttonState = ButtonState.LeftPressed;
				c.leftAnimator.SetTrigger(Animator.StringToHash("Push"));
				mat.color = c.leftPressed;
			} else {
				c.buttonState = ButtonState.RightPressed;
				c.rightAnimator.SetTrigger(Animator.StringToHash("Push"));
				mat.color = c.rightPressed;
			}
		}
	}
}
