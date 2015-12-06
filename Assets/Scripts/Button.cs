using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	public enum button {LEFT, RIGHT};
	public button b;
	public ControlPanel c;
	public Material mat;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hand" && c.Activated) {
			Debug.Log("TWAS A HAND");
			if (b == button.LEFT && c.buttonState != ButtonState.LeftPressed) {
				c.buttonState = ButtonState.LeftPressed;
				c.leftAnimator.SetTrigger(Animator.StringToHash("Push"));
				mat.color = c.leftPressed;
			} else if (b == button.RIGHT && c.buttonState != ButtonState.RightPressed) {
				c.buttonState = ButtonState.RightPressed;
				c.rightAnimator.SetTrigger(Animator.StringToHash("Push"));
				mat.color = c.rightPressed;
			}
		}
	}
}
