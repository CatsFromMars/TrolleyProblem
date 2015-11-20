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

	private int state = 0;
	private bool panelControlsPlatforms = false;
	private ButtonState buttonState = ButtonState.NotPressed;
	private int timesFlipped = 0;
	private float decisionTime; // time decision is introduced
	private float actionTime; // time when player has acted

	void Start() {
		fatMan.SetActive(Config.Group != RGroup.LeverControl);
		singlePlatform.gameObject.SetActive(Config.Group == RGroup.LeverControl);
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
					Debug.Log("Hello, and welcome to the lab. In a few moments, I will guide you through a series of questions and ask you to perform a few simple actions.");
					yield return StartCoroutine(PlaySoundAndWait("RS-01", 10f)); // TODO: time how long it takes to say this

					controller.Activated = true; // control panel lights up
					controller.buttonState = ButtonState.NotPressed;
					Debug.Log("Please indicate your response via the control panel in front of you. Tap any button when you are ready to begin.");
					yield return StartCoroutine(PlaySoundAndWait("RS-02", 5f)); // TODO: time how long it takes to say this

					yield return StartCoroutine(WaitForButton(true)); // wait for button to be pressed
					break;

				// Questioning
				case 1:
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					controller.buttonState = ButtonState.NotPressed;
					// TODO: set text on control panel (YES/NO)
					Debug.Log("Question 1. Have you experienced virtual reality before today? Tap the control panel to answer.");
					yield return StartCoroutine(PlaySoundAndWait("RS-03", 7f)); // TODO: time how long it takes to say this

					yield return StartCoroutine(WaitForButton(true)); // wait for button to be pressed
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					controller.buttonState = ButtonState.NotPressed;
					// TODO: set text on control panel
					Debug.Log("Question 2. Have you heard of the virtual elevator problem?");
					yield return StartCoroutine(PlaySoundAndWait("RS-04", 4f)); // TODO: time how long it takes to say this

					yield return StartCoroutine(WaitForButton(true)); // wait for button to be pressed
					// TODO: Play positive (answer accepted) sound effect
					controller.Activated = false;
					yield return new WaitForSeconds(1f);

					controller.Activated = true;
					controller.buttonState = ButtonState.NotPressed;
					// TODO: set text on control panel
					Debug.Log("Doing great. Question 3: Imagine you are on a platform--");
					yield return StartCoroutine(PlaySoundAndWait("RS-05", 4f)); // TODO: time how long it takes to say this
					break;

				// Power outage
				case 2:
					// TODO: lights flicker out, with SE.
					controller.Activated = false;
					areaLight.SetActive(false);
					pLight1.SetActive(false);
					pLight2.SetActive(false);
					playerLighting.SetActive(false);
					// TODO: animate controller powering off
					yield return new WaitForSeconds(3f);
					// TODO: emergency lighting appears, with SE.
					yield return new WaitForSeconds(1f);

					Debug.Log("Analyzing system failure. Please hold.");
					yield return StartCoroutine(PlaySoundAndWait("AI-01", 6f)); // TODO: time how long it takes to say this

					Debug.Log("Looks like we are experiencing some technical difficulties. Please remain calm and in your seat! We will send personnel out to investigate.");
					yield return StartCoroutine(PlaySoundAndWait("RS-06", 10f)); // TODO: time how long it takes to say this

					// TODO: lights on subject's platform appear, with SE
					playerLighting.SetActive(true);
					yield return new WaitForSeconds(1f);
					Debug.Log("Lights in section C are now online.");
					yield return StartCoroutine(PlaySoundAndWait("AI-02", 6f)); // TODO: time how long it takes to say this

					// TODO: lights on 5-person platform light up, with SE
					yield return new WaitForSeconds(1f);
					pLight2.SetActive(true);
					Debug.Log("Section B, normal. Analysis: 32% complete.");
					yield return StartCoroutine(PlaySoundAndWait("AI-03", 6f)); // TODO: time how long it takes to say this
					break;

				// Introduce fat man or 1-person platform
				case 3:
					if (Config.Group == RGroup.LeverControl) {
						// TODO: lights on 1-person platform light up, with SE
						pLight1.SetActive(true);
						yield return new WaitForSeconds(1f);
						Debug.Log("Section D, normal. Analysis: 77% complete.");
						yield return StartCoroutine(PlaySoundAndWait("AI-04", 6f)); // TODO: time how long it takes to say this
					} else {
						// TODO: guy walks out onto platform, with footsteps
						yield return new WaitForSeconds(2f);
						if (Config.Group == RGroup.Haptic) {
							Debug.Log("(EXPERIMENTER: Pat subject on back.)");
						}
						yield return new WaitForSeconds(1f);
						Debug.Log("Don't worry, I'll get this fixed.");
						yield return StartCoroutine(PlaySoundAndWait("WK-01", 4f)); // TODO: time how long it takes to say this

						// TODO: guy walks over to edge and bends down, looking at floor
						yield return new WaitForSeconds(2f);

						Debug.Log("Section D, normal. Analysis: 77% complete.");
						yield return StartCoroutine(PlaySoundAndWait("AI-05", 4f)); // TODO: time how long it takes to say this
					}
					break;

				// Electricity! zap
				case 4:
					// TODO: electric death trap lights up
					yield return new WaitForSeconds(2f);
					Debug.Log("Analysis: 100% complete. The electric generator has malfunctioned. Warning: electricity has reached maximum levels. Please keep away from the ground floor.");
					yield return StartCoroutine(PlaySoundAndWait("AI-06", 10f)); // TODO: time how long it takes to say this

					// 5-person elevator begin to move
					groupPlatform.Activated = true;
					// TODO: guys on elevator appear alarmed
					yield return new WaitForSeconds(1f);
					Debug.Log("Warning: emergency lockdown has engaged. The elevators cannot be disabled.");
					yield return StartCoroutine(PlaySoundAndWait("AI-07", 5f)); // TODO: time how long it takes to say this
					break;

				// Introduce the decision
				case 5:
					Debug.Log("Oh man! The elevator is out of control! We don't have much time before it reaches the floor and electrocutes everyone on it!");
					yield return StartCoroutine(PlaySoundAndWait("RS-07", 10f)); // TODO: time how long it takes to say this

					if (Config.Group == RGroup.LeverControl) {
						controller.Activated = true;
						controller.buttonState = ButtonState.LeftPressed;
						// TODO: set text on control panel
						yield return new WaitForSeconds(2f);

						Debug.Log("The control panel has an emergency override that controls the platforms. By pressing a button, you can choose which elevator will descend. But no matter what, one of the elevators will hit the floor...");
						yield return StartCoroutine(PlaySoundAndWait("RS-08", 10f)); // TODO: time how long it takes to say this

						decisionTime = Time.time;
						panelControlsPlatforms = true;
						Debug.Log("Oh man oh man oh man. What do we do?");
						yield return StartCoroutine(PlaySoundAndWait("RS-09", 10f)); // TODO: time how long it takes to say this
					} else {
						Debug.Log("According to the computer, the electricity will short on contact with biological material, but there's nothing we can do! Unless...");
						yield return StartCoroutine(PlaySoundAndWait("RS-10", 10f)); // TODO: time how long it takes to say this

						Debug.Log("The only way I can think of... if you want to save the five people on the platform, you could give the worker a push.");
						yield return StartCoroutine(PlaySoundAndWait("RS-11", 10f)); // TODO: time how long it takes to say this

						decisionTime = Time.time;
						// TODO: enable fat man's collider
						//fatMan.GetComponent<Rigidbody>().enabled = true;
						Debug.Log("The decision is up to you.");
						yield return StartCoroutine(PlaySoundAndWait("RS-12", 10f)); // TODO: time how long it takes to say this
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
					yield return StartCoroutine(PlaySoundAndWait("AI-08", 10f)); // TODO: time how long it takes to say this

					// TODO: fade out scene
					Debug.Log("This concludes the research experiment. Please remove your headgear.");
					yield return StartCoroutine(PlaySoundAndWait("RS-13", 10f));
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
		yield return new WaitForSeconds(seconds);
	}

	private IEnumerator WaitForButton(bool right) {
		while (controller.buttonState == ButtonState.NotPressed) {
			yield return null;
		}
	}

	private IEnumerator WaitForDeath() {
		bool dead = false;

		while (!dead) {
			yield return null;
			dead = Config.Group == RGroup.LeverControl
			         ? (singlePlatform.Dead || groupPlatform.Dead)
			         : (groupPlatform.Dead || fatMan.transform.position.y <= 0f);
		}
	}

}
