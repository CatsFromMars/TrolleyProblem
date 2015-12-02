using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {
	public ScenarioController controller;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hand") {
			controller.pushedFatMan = true;
		}
	}
}
