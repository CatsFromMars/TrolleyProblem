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
    public bool Dead {
        get {
            return isDead;
        }
        set {
            // TODO: play dying animation on all characters, zap sound effect
            isDead = value;
        }
    }
    public float deadY = 0f;

    private bool isActivated;
    private bool isDead;

	// Update is called once per frame
	void Update() {
        if (Dead) {
            return;
        }

		if (Activated) {
			transform.position += speed * Vector3.down * Time.deltaTime;
		}

        if (transform.position.y <= deadY) {
            Dead = true;
        }
	}
}
