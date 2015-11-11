using UnityEngine;
using System.Collections;

public class ScenarioController : MonoBehaviour {

	public bool controllerFlipped = false;
	public PlatformBehavior singlePlatform;
	public PlatformBehavior groupPlatform;

	// Update is called once per frame
	void Update () {
		if (controllerFlipped) {
			singlePlatform.activated = true;
			groupPlatform.activated = false;
		}
		else {
			singlePlatform.activated = false;
			groupPlatform.activated = true;
		}
	}
}
