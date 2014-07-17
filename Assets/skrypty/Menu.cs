//obsługa przycisków w pierwszym widoku menu
using UnityEngine;

public class Menu : MonoBehaviour {

	public int numer;
 	void OnMouseEnter(){
		renderer.material.color = Color.black;
	}
	void OnMouseExit(){
		renderer.material.color = Color.red;
	}
	void OnMouseDown(){
		if(numer==2){
			Application.Quit();
		}
		if(numer==0) {
			Application.LoadLevel(2);
		}
	}
}
