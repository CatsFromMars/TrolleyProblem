using UnityEngine;
using System.Collections;

public class PlatformBehavior : MonoBehaviour {
    public float speed = 0.5f;

    public bool Activated {
        get {
            return isActivated;
        }
        set {
            // TODO: play sound effect when starting or stopping
            isActivated = value;

            if (movingAnimation) {
                // TODO: if (isActivated && not playing animation) start playing animation;
                // else if (!isActivated && playing animation) stop playing animation;
            }
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

    public float stopY = 3f; // y-position of pipe
    public float deadY = 0f;
    public bool reachedStopZone = false;
    public bool movingAnimation = false; // whether to descend with animation

    private bool isActivated;
    private bool isDead;

    // Update is called once per frame
    void Update() {
        if (Dead) {
            return;
        }

        if (!movingAnimation) {
            // Pre-experiment, moving smoothly via y-position
            if (!reachedStopZone && Activated) {
                transform.position += speed * Vector3.down * Time.deltaTime;
                if (transform.position.y <= stopY) {
                    Vector3 newPos = transform.position;
                    newPos.y = stopY;
                    transform.position = newPos;
                    reachedStopZone = true;
                }
            }
        } else {
            // During experiment, moving via animation
            // TODO: if animation has ended, set Dead = true
            /*if () {
                Dead = true;
            }*/
        }
    }
}
