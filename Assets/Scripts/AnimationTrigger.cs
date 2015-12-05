using UnityEngine;
using System.Collections;

public class AnimationTrigger : MonoBehaviour {
	public ScenarioController controller;
	public Animator fmAnimator;

    void Update() {
        // press 3 to trigger push
        if (Config.Debug && Input.GetKeyDown(KeyCode.Alpha3)) {
            controller.pushedFatMan = true;
			fmAnimator.SetBool(Animator.StringToHash("Push"), true);
        }
    }

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Hand" && Config.Group != RGroup.Haptic) {
			controller.pushedFatMan = true;
			fmAnimator.SetBool(Animator.StringToHash("Push"), true);
		}
	}
}
