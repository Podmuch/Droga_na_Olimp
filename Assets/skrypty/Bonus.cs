//obsługa bonusów - aktywacja bonusu (przesunięcie na powierchnie)
// i usunawnie bonusu po zebraniu lub minięciu odpowiedniego czasu
using UnityEngine;

public class Bonus : MonoBehaviour {
	private bool widoczny=false;
	//czas aktywnosci i zmienna do animacji ruchu
	private float czas=10.0f, resztaczasu, wysokoscanimacji=1/30, pol=0.5f;
	// Update is called once per frame
	void Update () {
		if(widoczny){
			if(czas>0.0f) {
				czas-=Time.deltaTime;
				resztaczasu=czas-Mathf.Floor(czas);
				//animacja ruchu
				if(resztaczasu>pol) transform.Translate(0,0,-(resztaczasu-pol)*wysokoscanimacji);
				else transform.Translate(0,0,resztaczasu*wysokoscanimacji);
			}
			else Destroy(gameObject);
		}
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			if(!widoczny) {
				//przesuniecie nad podłogę
				transform.Translate(new Vector3(0,0,-1));
				widoczny=true;
			}
			else {
				other.SendMessage("bonus",tag);
				Destroy(gameObject);
			}
		}
	}
}
