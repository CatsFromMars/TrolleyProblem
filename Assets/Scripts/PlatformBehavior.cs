using UnityEngine;
using System.Collections;

public class PlatformBehavior : MonoBehaviour {
    public float speed = 0.5f;
	public float deadY = 0f;
	public Animator elevatorAnimator;
    public AudioClip elevatorStart;
    public AudioClip elevatorStop;
    public AudioClip scream;

    public bool Activated {
        get {
            return isActivated;
        }
        set {
            isActivated = value;
            AudioSource source = GetComponent<AudioSource>();
            if (isActivated) {
                source.PlayOneShot(elevatorStart);
                source.Play();
            } else {
                source.PlayOneShot(elevatorStop);
                source.Pause();
            }

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
            isDead = value;
            if (isDead) {
                // play dying sound effect
                AudioSource source = GetComponent<AudioSource>();
                source.PlayOneShot(scream);
                // TODO: play dying animation on all characters
            }
        }
    }

    public float stopY = 3f; // y-position of pipe
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

                    // play stop sound effect
                    AudioSource source = GetComponent<AudioSource>();
                    source.PlayOneShot(elevatorStop);
                    source.Pause();
                }
            }
        } else {
            // During experiment, moving via animation
			elevatorAnimator.SetBool(Animator.StringToHash("Shaking"), true);
            if (Activated) {
    			transform.position += speed / 4f * Vector3.down * Time.deltaTime;
    			if (transform.position.y <= deadY) isDead = true;
            }
        }
    }
}
