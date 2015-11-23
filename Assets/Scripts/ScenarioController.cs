using UnityEngine;
using System.Collections;

public class ScenarioController : MonoBehaviour {
	public PlatformBehavior singlePlatform;
	public PlatformBehavior groupPlatform;
	public ControlPanel controller;
	public GameObject fatMan;
	public GameObject playerLighting;
	public GameObject pLight1;
	public GameObject pLight2;
	public GameObject areaLight;
	public GameObject electricity;

	private int state = 0;
	private bool panelControlsPlatforms = false;
	private ButtonState buttonState = ButtonState.NotPressed;
	private int timesFlipped = 0;
	private float decisionTime; // time decision is introduced
	private float actionTime; // time when player has acted

	void Start() {
		StartCoroutine(RunState());
	}

	// Update is called once per frame
	void Update() {
		// Check if control panel affects platforms
		if (panelControlsPlatforms && controller.buttonState != buttonState && controller.buttonState != ButtonState.NotPressed) {
			actionTime = Time.time;
			buttonState = controller.buttonState;
			timesFlipped++;
			if (buttonState == ButtonState.RightPressed) {
				singlePlatform.Activated = true;
				groupPlatform.Activated = false;
			} else {
				singlePlatform.Activated = false;
				groupPlatform.Activated = true;
			}
		}
	}

	// Script-specific details, updating state of game
	public IEnumerator RunState() {
		while (state < 8) {
			switch (state++) {
				// Intro phase
				case 0:
					yield return new WaitForSeconds(3f); // wait 3 seconds before starting script
					Debug.Log("Hello, and welcome to the facility. Today, you will be assisting a few construction workers as they revamp the lab.");
					yield return StartCoroutine(PlaySoundAndWait("RS-01", 8f));

					controller.Activated = true; // control panel lights up
					controller.buttonState = ButtonState.NotPressed;
					panelControlsPlatforms = true;

					// Platform lights come on
					// TODO: sound effects?
					pLight1.SetActive(true);
					pLight2.SetActive(true);

					Debug.Log("The control panel in front of you controls the workers' platforms. Please use the buttons to lower the platforms to the pipe. Note that only one platform can move at a time.");
					yield return StartCoroutine(PlaySoundAndWait("RS-02", 0f));
					yield return StartCoroutine(WaitForElevators()); // wait for elevators to reach right level
					break;

				// Post-intro
				case 1:
					controller.Activated = false;
					panelControlsPlatforms = false;
					yield return new WaitForSeconds(1f);
					controller.buttonState = ButtonState.NotPressed;
					Debug.Log("Good work. Now that the platforms are in the correct place--");
					yield return StartCoroutine(PlaySoundAndWait("RS-03", 3f));
					break;

				// Power outage
				case 2:
					// TODO: add sound effects for lights flickering out.
					areaLight.SetActive(false);
					pLight1.SetActive(false);
					pLight2.SetActive(false);
					playerLighting.SetActive(false);
					yield return new WaitForSeconds(3f);

					if (Config.Group != RGroup.LeverControl) {
						// TODO: whirring sound effect for disappearing control panel.
						controller.gameObject.SetActive(false);
					}

					// TODO: emergency lighting appears, with SE.
					yield return new WaitForSeconds(1f);

					Debug.Log("Analyzing system failure. Please hold.");
					yield return StartCoroutine(PlaySoundAndWait("AI-01", 4f));

					Debug.Log("Looks like we are experiencing some technical difficulties. Please stay calm! We will send personnel out to investigate.");
					yield return StartCoroutine(PlaySoundAndWait("RS-04", 9f));

					// TODO: lights on subject's platform appear, with SE
					playerLighting.SetActive(true);
					yield return new WaitForSeconds(1f);
					Debug.Log("Lights in section C are now online.");
					yield return StartCoroutine(PlaySoundAndWait("AI-02", 4f));

					// TODO: lights on 5-person platform light up, with SE
					yield return new WaitForSeconds(1f);
					pLight2.SetActive(true);
					Debug.Log("Section B, normal. Analysis: 32% complete.");
					yield return StartCoroutine(PlaySoundAndWait("AI-03", 5f));
					break;

				// Introduce fat man or 1-person platform
				case 3:
					if (Config.Group == RGroup.LeverControl) {
						// TODO: lights on 1-person platform light up, with SE
						pLight1.SetActive(true);
						yield return new WaitForSeconds(1f);
						Debug.Log("Section D, normal. Analysis: 77% complete.");
						yield return StartCoroutine(PlaySoundAndWait("AI-04", 6f));
					} else {
						// TODO: guy walks out onto platform, with footsteps
						yield return new WaitForSeconds(2f);
						if (Config.Group == RGroup.Haptic) {
							Debug.Log("(EXPERIMENTER: Pat subject on back.)");
						}
						yield return new WaitForSeconds(1f);
						Debug.Log("Don't worry, I'll get this fixed.");
						yield return StartCoroutine(PlaySoundAndWait("WK-01", 4f));

						// TODO: guy walks over to edge and bends down, looking at floor
						fatMan.SetActive(true);
						yield return new WaitForSeconds(2f);

						Debug.Log("Section D, normal. Analysis: 77% complete.");
						yield return StartCoroutine(PlaySoundAndWait("AI-05", 4f));
					}
					break;

				// Electricity! zap
				case 4:
					// TODO: play sound effect for electric death trap
					electricity.SetActive(true);
					yield return new WaitForSeconds(2f);
					Debug.Log("Warning: the electric generator has malfunctioned. Please keep away from the ground floor.");
					yield return StartCoroutine(PlaySoundAndWait("AI-06", 7f));

					// 5-person elevator begin to move
					groupPlatform.Activated = true;
					groupPlatform.movingAnimation = true;
					// TODO: guys on elevator appear alarmed
					yield return new WaitForSeconds(1f);
					Debug.Log("Warning: platform A unstable. Brake failure in 20 seconds.");
					yield return StartCoroutine(PlaySoundAndWait("AI-07", 5f));
					break;

				// Introduce the decision
				case 5:
					Debug.Log("Oh man! The elevator is going to fall! We don't have much time before it hits the floor and electrocutes everyone on it!");
					yield return StartCoroutine(PlaySoundAndWait("RS-04", 7.5f));

					if (Config.Group == RGroup.LeverControl) {
						controller.Activated = true;
						controller.buttonState = ButtonState.LeftPressed;
						singlePlatform.movingAnimation = true;
						panelControlsPlatforms = true;
						yield return new WaitForSeconds(2f);

						Debug.Log("The emergency brakes won't kick in unless the generator shorts. Using the control panel, you can choose which elevator will fall. But no matter what, one of the elevators will hit the floor...");
						yield return StartCoroutine(PlaySoundAndWait("RS-05", 10f));
						decisionTime = Time.time;
						Debug.Log("Oh man oh man oh man. What do we do?");
						yield return StartCoroutine(PlaySoundAndWait("RS-06", 3f));
					} else {
						Debug.Log("The emergency brakes won't kick in unless the generator shorts. But we can't disable the electricity... unless...");
						yield return StartCoroutine(PlaySoundAndWait("RS-07", 8f));

						Debug.Log("The only way to stop the platform is to push the worker in front of you and short the generator.");
						yield return StartCoroutine(PlaySoundAndWait("RS-08", 6f));

						decisionTime = Time.time;
						// TODO: enable fat man's collider
						fatMan.GetComponent<Animator>().enabled = false;
						Debug.Log("The decision is up to you.");
						yield return StartCoroutine(PlaySoundAndWait("RS-09", 3f));
					}

					yield return StartCoroutine(WaitForDeath());

					if (Config.Group == RGroup.LeverControl) {
						controller.Activated = false;
						panelControlsPlatforms = false;
						singlePlatform.Activated = false;
						groupPlatform.Activated = false;
					} else {
						groupPlatform.Activated = false;
					}
					break;

				// Someone has died. Ending.
				case 6:
					// TODO: play SE, animation of floor crackling
					Debug.Log("Bio material detected. Disengaging power supply.");
					yield return StartCoroutine(PlaySoundAndWait("AI-08", 4f));

					// TODO: fade out scene
					Debug.Log("This concludes the research experiment. Please remove your headgear.");
					yield return StartCoroutine(PlaySoundAndWait("RS-13", 5f));
					break;

				// Results
				default:
					// TODO: save these to a file somewhere?
					Debug.Log("==== RESULTS ====");
					Debug.Log("Research Group: " + Config.Group.ToString());
					Debug.Log("Time to make decision: " + (actionTime - decisionTime).ToString() + " ms");

					if (Config.Group == RGroup.LeverControl) {
						if (controller.buttonState == ButtonState.RightPressed) {
							Debug.Log("Results: killed single person, with " + timesFlipped.ToString() + " flips");
						} else {
							Debug.Log("Results: killed 5 people, with " + timesFlipped.ToString() + " flips");
						}
					} else {
						if (groupPlatform.Dead) {
							Debug.Log("Results: did not push fat man");
						} else {
							Debug.Log("Results: pushed fat man");
						}
					}

					Debug.Log("gg");
					break;
			}
			Debug.Log("switching state: " + state.ToString());
		}
	}

	private IEnumerator PlaySoundAndWait(string soundEffect, float seconds) {
		// TODO: play sound effect
		if (seconds > 0) {
			yield return new WaitForSeconds(seconds);
		}
	}

	private IEnumerator WaitForElevators() {
		while (!(singlePlatform.reachedStopZone && groupPlatform.reachedStopZone)) {
			yield return null;
		}
	}

	private IEnumerator WaitForDeath() {
		bool dead = false;

		while (!dead) {
			yield return null;
			dead = Config.Group == RGroup.LeverControl
			         ? (singlePlatform.Dead || groupPlatform.Dead)
			         : (groupPlatform.Dead || fatMan.transform.position.y <= -2f);
		}
	}

}
