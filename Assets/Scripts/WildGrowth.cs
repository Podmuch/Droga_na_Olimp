//One of the surface type - disappears after hitting
using UnityEngine;

public class WildGrowth : MonoBehaviour {
	public Transform surfaceBase;
	private bool isStroke=false;
	private float TimeCounter=0.0f, maxStrokeTime=0.3f, strokeForce=0.3f;
	//Update
	void Update () {
		if (isStroke == true) {
			TimeCounter-=Time.deltaTime;
			if(TimeCounter<0.0f){
				Destroy(surfaceBase.gameObject);
				Destroy(gameObject);
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
