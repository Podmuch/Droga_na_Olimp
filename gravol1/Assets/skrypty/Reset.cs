//Funkcja uruchamiana przy powrocie gracza do menu - tymczasowa
using UnityEngine;

public class Reset : MonoBehaviour {
	void Start () {
		Gracz gracz = FindObjectOfType<Gracz> ();
		if(gracz!=null){
			gracz.ilpunktow = 0;
			gracz.iloscdynamitu = 3;
			gracz.ilzyc = 3;
		}
	}
}
