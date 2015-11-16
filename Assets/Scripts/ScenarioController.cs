using UnityEngine;
using System.Collections;

public class ScenarioController : MonoBehaviour {
	public bool controllerFlipped = false;
	public PlatformBehavior singlePlatform;
	public PlatformBehavior groupPlatform;
	public ControlPanel controller;

	private int state = 0;

	void Start() {
		StartCoroutine(RunState()); // wait three seconds before starting script
	}

	// Update is called once per frame
	void Update() {
		/*controllerFlipped = controller.switchFlipped;
		if (controllerFlipped) {
			singlePlatform.Activated = true;
			groupPlatform.Activated = false;
		}
		else {
			singlePlatform.Activated = false;
			groupPlatform.Activated = true;
		}*/
	}

	// Script-specific details, updating state of game
	public IEnumerator RunState() {
		while (state < 100) {
			switch (state++) {
				// Intro phase
				case 0:
					yield return new WaitForSeconds(3f); // wait 3 seconds before starting script
					Debug.Log("Hello, and welcome to the lab. In a few moments, I will guide you through a series of questions and ask you to perform a few simple actions.");
					yield return StartCoroutine(PlaySoundAndWait("RS1", 10f)); // TODO: time how long it takes to say this

					controller.Activated = true; // control panel and lever light up
					Debug.Log("Please indicate your response by sliding the lever in front of you. When you are ready to begin, slide the lever to the right.");
					yield return StartCoroutine(PlaySoundAndWait("RS2", 5f)); // TODO: time how long it takes to say this

					yield return StartCoroutine(WaitForLever(true)); // wait for lever to be toggled
					break;

				// Questioning
				case 1:
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					// TODO: set text on control panel
					Debug.Log("Question 1. Have you experienced virtual reality before today? Slide the lever right for YES, left for NO.");
					yield return StartCoroutine(PlaySoundAndWait("RS3", 10f)); // TODO: time how long it takes to say this

					// TODO: start countdown timer on controller
					yield return new WaitForSeconds(10f);
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					// TODO: set text on control panel
					Debug.Log("Question 2. Have you heard of the virtual elevator problem? Slide the lever right for YES, left for NO.");
					yield return StartCoroutine(PlaySoundAndWait("RS4", 10f)); // TODO: time how long it takes to say this

					// TODO: start countdown timer on controller
					yield return new WaitForSeconds(10f);
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					// TODO: set text on control panel
					Debug.Log("Doing great. Question 3: Imagine you are on a platform--");
					yield return StartCoroutine(PlaySoundAndWait("RS5", 4f)); // TODO: time how long it takes to say this
					break;

				// Power outage
				case 2:
					// TODO: lights flicker out, with SE.
					controller.Activated = false;
					// TODO: animate controller powering off
					yield return new WaitForSeconds(3f);
					// TODO: emergency lighting appears, with SE.
					yield return new WaitForSeconds(1f);

					Debug.Log("Analyzing system failure. Please hold.");
					yield return StartCoroutine(PlaySoundAndWait("AI1", 6f)); // TODO: time how long it takes to say this

					Debug.Log("Looks like we are experiencing some technical difficulties. Please remain calm and in your seat! We will send personnel out to investigate.");
					yield return StartCoroutine(PlaySoundAndWait("RS6", 10f)); // TODO: time how long it takes to say this

					// TODO: lights on subject's platform appear, with SE
					yield return new WaitForSeconds(1f);
					Debug.Log("Lights in section C are now online.");
					yield return StartCoroutine(PlaySoundAndWait("AI2", 6f)); // TODO: time how long it takes to say this

					// TODO: lights on 5-person platform light up, with SE
					yield return new WaitForSeconds(1f);
					Debug.Log("Section B, normal. Analysis: 32% complete.");
					yield return StartCoroutine(PlaySoundAndWait("AI3", 6f)); // TODO: time how long it takes to say this
					break;

				// Introduce fat man or 1-person platform
				case 3:
					break;

				default:
					break;
			}
			Debug.Log("switching state: " + state.ToString());
		}
	}

	private IEnumerator PlaySoundAndWait(string soundEffect, float seconds) {
		// TODO: play sound effect
		yield return new WaitForSeconds(seconds);
	}

	private IEnumerator WaitForLever(bool right) {
		while (controller.switchFlipped != right) {
			yield return null;
		}
	}

}
