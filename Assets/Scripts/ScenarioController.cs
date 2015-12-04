using UnityEngine;
using System;
using System.Collections;
using System.IO;

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

	public AudioSource RSSource;
	public AudioSource AISource;
	public AudioClip[] voiceClips;
	public AudioClip[] soundEffects;

	public GameObject animationTrigger;
	public bool pushedFatMan = false; // did player push fat man???
	public Animator screenFader;
	public Arduino arduinoController;

	private int state = 0;
	private bool scenarioStarted = false;
	private bool panelControlsPlatforms = false;
	private ButtonState buttonState = ButtonState.NotPressed;
	private int timesFlipped = 0;
	private float decisionTime; // time decision is introduced
	private float actionTime; // time when player has acted

	private string filename;

	void Start() {
		Debug.Log("Scene loaded. Press any key to begin.");
		animationTrigger.SetActive (false);
	}

	// Update is called once per frame
	void Update() {
		// start state when key is pressed
		if (!scenarioStarted && Input.anyKey) {
			StartCoroutine(RunState());
			scenarioStarted = true;
		}

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

			arduinoController.logEvent = "Button pressed: " + (buttonState == ButtonState.RightPressed ? "right" : "left");
		}
	}

	// Script-specific details, updating state of game
	public IEnumerator RunState() {

		while (state < 8) {
			switch (state++) {
				// Intro phase
				case 0:
					filename = arduinoController.InitializeArduino();
					yield return new WaitForSeconds(1f);

					arduinoController.logEvent = "Experiment started";

					Debug.Log("Hello, and welcome to the facility. Today, you will be assisting a few construction workers as they revamp the lab.");
					yield return StartCoroutine(PlaySoundAndWait(0, RSSource, 7f)); // RS-01

					controller.Activated = true; // control panel lights up
					controller.buttonState = ButtonState.NotPressed;
					panelControlsPlatforms = true;

					// Platform lights come on
					// TODO: sound effects?
					pLight1.SetActive(true);
					pLight2.SetActive(true);

					Debug.Log("The control panel in front of you controls the workers' platforms. Please use the buttons to lower the platforms to the pipe. Note that only one platform can move at a time.");
					yield return StartCoroutine(PlaySoundAndWait(1, RSSource, 0f)); // RS-02
					yield return StartCoroutine(WaitForElevators()); // wait for elevators to reach right level
					break;

				// Post-intro
				case 1:
					controller.Activated = false;
					panelControlsPlatforms = false;
					if (Config.Group == RGroup.LeverControl) {
						singlePlatform.speed *= 2f;
						groupPlatform.speed *= 2f;
					}
					yield return new WaitForSeconds(1f);
					controller.buttonState = ButtonState.NotPressed;
					Debug.Log("Good work. Now that the platforms are in the correct place--");
					yield return StartCoroutine(PlaySoundAndWait(2, RSSource, 2.8f)); // RS-03
					break;

				// Power outage
				case 2:
					areaLight.SetActive(false);
					pLight1.SetActive(false);
					pLight2.SetActive(false);
					playerLighting.SetActive(false);
					yield return StartCoroutine(PlaySoundAndWait(soundEffects[0], AISource, 2f)); // lights-off
					arduinoController.logEvent = "Lights out";

					if (Config.Group != RGroup.LeverControl) {
						// TODO: whirring sound effect for disappearing control panel.
						controller.gameObject.SetActive(false);
					}

					// TODO: emergency lighting appears, with SE.
					yield return new WaitForSeconds(1f);

					Debug.Log("Analyzing system failure. Please hold.");
					yield return StartCoroutine(PlaySoundAndWait(3, AISource, 4f)); // AI-01

					Debug.Log("Looks like we are experiencing some technical difficulties. Please stay calm as we investigate.");
					yield return StartCoroutine(PlaySoundAndWait(4, RSSource, 6.5f)); // RS-04
					break;

				// Lights restored
				case 3:
					// Lights on subject's platform appear, with SE
					playerLighting.SetActive(true);
					yield return StartCoroutine(PlaySoundAndWait(soundEffects[1], AISource, 1f)); // lights-on
					Debug.Log("Lights in section C are now online. Restoring power...");
					yield return StartCoroutine(PlaySoundAndWait(5, AISource, 6f)); // AI-02

					// Lights on platforms return
					areaLight.SetActive(false);
					pLight1.SetActive(true);
					pLight2.SetActive(true);
					yield return new WaitForSeconds(1f);

					if (Config.Group != RGroup.LeverControl) {
						fatMan.SetActive(true);
						Debug.Log("Please sit tight-- we're sending a guy to fix the control panel.");
						yield return StartCoroutine(PlaySoundAndWait(6, RSSource, 4f)); // RS-05

						// fat man walks over to edge and bends down, looking at floor
						// TODO: play footstep SE?
						yield return new WaitForSeconds(1f);
					}

					Debug.Log("All power restored. Rebooting generator...");
					yield return StartCoroutine(PlaySoundAndWait(7, AISource, 5f)); // AI-03
					yield return StartCoroutine(PlaySoundAndWait(soundEffects[2], AISource, 4.5f)); // generator-charging
					break;

				// Electricity! zap
				case 4:
					electricity.SetActive(true);
					yield return new WaitForSeconds(2f);
					arduinoController.logEvent = "Electricity activated";
					Debug.Log("Warning: the electric generator has malfunctioned. Please keep away from the ground floor.");
					yield return StartCoroutine(PlaySoundAndWait(8, AISource, 7f)); // AI-04

					// 5-person elevator begin to shake
					groupPlatform.movingAnimation = true;
					// TODO: guys on elevator appear alarmed
					yield return new WaitForSeconds(1f);
					Debug.Log("Danger! Platform A unstable. Brake failure in 20 seconds.");
					yield return StartCoroutine(PlaySoundAndWait(9, AISource, 6f)); // AI-05
					break;

				// Introduce the decision
				case 5:
					Debug.Log("Oh man! The elevator is going to fall! We don't have much time before it hits the floor and electrocutes everyone on it!");
					animationTrigger.SetActive(true);
					yield return StartCoroutine(PlaySoundAndWait(10, RSSource, 6f)); // RS-06

					groupPlatform.Activated = true;
					if (Config.Group == RGroup.LeverControl) {
						controller.Activated = true;
						controller.buttonState = ButtonState.LeftPressed;
						buttonState = ButtonState.LeftPressed;
						singlePlatform.movingAnimation = true;
						singlePlatform.Activated = false;
						panelControlsPlatforms = true;
						decisionTime = Time.time;
						actionTime = -1f;
						timesFlipped = 0;
						arduinoController.logEvent = "Decision time";

						Debug.Log("Using the control panel, you can switch the falling platforms. But no matter what, we can only save one of them...");
						yield return StartCoroutine(PlaySoundAndWait(11, RSSource, 6f)); // RS-07

						Debug.Log("The decision is up to you.");
						yield return StartCoroutine(PlaySoundAndWait(12, RSSource, 2f)); // RS-08
					} else {
						Debug.Log("The emergency brakes won't kick in unless the generator shorts. But the only way to short the electricity... The only way to stop the elevator is to push the worker in front of you onto the floor.");
						yield return StartCoroutine(PlaySoundAndWait(13, RSSource, 10f)); // RS-09

						decisionTime = Time.time;
						arduinoController.logEvent = "Decision time";
						// Enable pushing fat man

						Debug.Log("The decision is up to you.");
						yield return StartCoroutine(PlaySoundAndWait(12, RSSource, 2f)); // RS-08
					}

					yield return StartCoroutine(WaitForDeath());

					if (Config.Group == RGroup.LeverControl) {
						controller.Activated = false;
						panelControlsPlatforms = false;
						singlePlatform.Activated = false;
						groupPlatform.Activated = false;

						if (actionTime < 0) {
							actionTime = Time.time;
						}
						if (singlePlatform.Dead) {
							yield return StartCoroutine(PlaySoundAndWait(soundEffects[3], singlePlatform.GetComponent<AudioSource>(), 1f)); // scream
							arduinoController.logEvent = "Single elevator death";
						} else {
							yield return StartCoroutine(PlaySoundAndWait(soundEffects[4], groupPlatform.GetComponent<AudioSource>(), 1f)); // scream
							arduinoController.logEvent = "Group elevator death";
						}
					} else {
						actionTime = Time.time;
						if (pushedFatMan) {
							groupPlatform.Activated = false;
							yield return StartCoroutine(PlaySoundAndWait(soundEffects[3], fatMan.GetComponent<AudioSource>(), 1f)); // scream
							arduinoController.logEvent = "Fat man pushed";
						} else {
							yield return StartCoroutine(PlaySoundAndWait(soundEffects[4], groupPlatform.GetComponent<AudioSource>(), 1f)); // scream
							arduinoController.logEvent = "Group elevator death";
						}
					}
					break;

				// Someone has died. Ending.
				case 6:
					Debug.Log("Bio material detected. Disengaging power supply.");
					yield return StartCoroutine(PlaySoundAndWait(14, AISource, 5f)); // AI-06
					electricity.SetActive(false);

					yield return new WaitForSeconds(2f);

					// Fade out scene
					screenFader.SetTrigger(Animator.StringToHash("Fade"));
					Debug.Log("This concludes the research experiment. You may now remove your headgear.");
					yield return StartCoroutine(PlaySoundAndWait(15, RSSource, 5f)); // RS-10
					break;

				// Results
				default:
					arduinoController.writing = false;
					arduinoController.writer.WriteLine();
					arduinoController.writer.WriteLine("==== RESULTS ====");
					Debug.Log("==== RESULTS ====");
					arduinoController.writer.WriteLine("Research Group: " + Config.Group.ToString());
					Debug.Log("Research Group: " + Config.Group.ToString());
					arduinoController.writer.WriteLine("Time to make decision (last action - decision time): " + (actionTime - decisionTime).ToString() + " s");
					Debug.Log("Time to make decision (last action - decision time): " + (actionTime - decisionTime).ToString() + " s");

					if (Config.Group == RGroup.LeverControl) {
						if (controller.buttonState == ButtonState.RightPressed) {
							arduinoController.writer.WriteLine("Results: killed single person with " + timesFlipped.ToString() + " flips");
							Debug.Log("Results: killed single person with " + timesFlipped.ToString() + " flips");
						} else {
							arduinoController.writer.WriteLine("Results: killed 5 people with " + timesFlipped.ToString() + " flips");
							Debug.Log("Results: killed 5 people with " + timesFlipped.ToString() + " flips");
						}
					} else {
						if (pushedFatMan) {
							arduinoController.writer.WriteLine("Results: pushed fat man");
							Debug.Log("Results: pushed fat man");
						} else {
							arduinoController.writer.WriteLine("Results: did not push fat man");
							Debug.Log("Results: did not push fat man");
						}
					}
					arduinoController.Finish();
					Debug.Log("Scenario complete, data saved to " + filename);
					break;
			}
		}
	}

	private IEnumerator PlaySoundAndWait(int sound, AudioSource source, float seconds) {
		return PlaySoundAndWait(voiceClips[sound], source, seconds);
	}

	private IEnumerator PlaySoundAndWait(AudioClip sound, AudioSource source, float seconds) {
		source.PlayOneShot(sound);

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
			         : (groupPlatform.Dead || pushedFatMan);
		}
	}

}
