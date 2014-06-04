//Obsługa podłoża - podnoszenie przy uderzeniu i samoczynne opadanie po odpowiednim czasie
using UnityEngine;

public class Podloga : MonoBehaviour {
	private bool podbicie=false;
	private float czas=0.0f, maxczas=0.3f, silapodbicia=0.3f;
	//Update
	void Update () {
		if (podbicie == true) {
			czas-=Time.deltaTime;
			if(czas<0.0f){
				podbicie=false;
				czas=0.0f;
				transform.Translate (0, silapodbicia, 0);
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
