using UnityEngine;
using System.Collections;

public class PlatformBehavior : MonoBehaviour {
    public float speed = 2f;
	public bool Activated {
        get {
            return isActivated;
        }
        set {
            // TODO: play sound effect when starting
            isActivated = value;
        }
    }

    private bool isActivated;

	// Update is called once per frame
	void Update () {
		if (Activated) {
			transform.position += speed * Vector3.down * Time.deltaTime;
		}
	}
}
