using UnityEngine;
using System.Collections;

public class PlatformBehavior : MonoBehaviour {

	public bool activated = true;
	public float speed = 2f;
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			transform.position += speed*Vector3.down*Time.deltaTime;
		}
	}
}
