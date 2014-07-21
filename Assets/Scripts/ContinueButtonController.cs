//Przycisk powrotu do menu po wygranej lub przegranej rundzie
using UnityEngine;

public class PrzyciskDalej : MonoBehaviour {
	void OnMouseDown(){
		Application.LoadLevel(2);
	}
}
