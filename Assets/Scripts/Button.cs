using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
	public enum button {LEFT, RIGHT};
	public button b;
	public ControlPanel c;

	void OnTriggerEnter(Collider other) {
		Debug.Log ("BAP");
		if(other.tag == "Hand") {
			Debug.Log ("TWAS A HAND");
			if (b == button.LEFT)
				c.buttonState = ButtonState.LeftPressed;
			else
				c.buttonState = ButtonState.RightPressed;
		}
	}
}
