//Funkcja uruchamiana przy powrocie gracza do menu - tymczasowa
using UnityEngine;

public class Reset : MonoBehaviour {
	void Start () {
		Player gracz = FindObjectOfType<Player> ();
		if(gracz!=null){
			gracz.ilpunktow = 0;
			gracz.iloscdynamitu = 3;
			gracz.ilzyc = 3;
		}
	}
}
