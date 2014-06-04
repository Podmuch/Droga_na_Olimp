using UnityEngine;
using System.Collections;

public class Krzaki : MonoBehaviour {
	public Transform fundament;
	private bool podbicie=false;
	private float czas=0.0f, maxczas=0.3f, silapodbicia=0.3f;
	//Update
	void Update () {
		if (podbicie == true) {
			czas-=Time.deltaTime;
			if(czas<0.0f){
				Destroy(fundament.gameObject);
				Destroy(gameObject);
			}
		}
	}
	//podbicie przez gracza
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			transform.Translate (0, -silapodbicia, 0);
			podbicie=true;
			czas=maxczas;
		}
	}
}
