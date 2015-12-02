using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {
	public ScenarioController controller;

    void Update() {
        // press 3 to trigger push
        if (Config.Debug && Input.GetKeyDown(KeyCode.Alpha3)) {
            controller.pushedFatMan = true;
        }
    }

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hand") {
			controller.pushedFatMan = true;
		}
	}
}
