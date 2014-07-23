//Surface
using UnityEngine;

public class Surface : MonoBehaviour {
	private bool isStroke=false;
	private float TimeCounter=0.0f, maxStrokeTime=0.3f, strokeForce=0.3f;
	//Update
	void Update () {
		if (isStroke == true) {
			TimeCounter-=Time.deltaTime;
			if(TimeCounter<0.0f){
				isStroke=false;
				TimeCounter=0.0f;
				transform.Translate (0, strokeForce, 0);
			}
		}
	}
	//Collision with player - stroke
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			transform.Translate (0, -strokeForce, 0);
			isStroke=true;
			TimeCounter=maxStrokeTime;
		}
	}
}
