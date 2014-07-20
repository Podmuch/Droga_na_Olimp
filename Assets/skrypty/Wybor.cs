//Obsługa drugiego widoku menu (wybór rundy)
using UnityEngine;
public class Wybor : MonoBehaviour {
	public int numer;
	void OnMouseEnter(){
		if(numer==-1)renderer.material.color = Color.black;
		else transform.localScale+=new Vector3(0.2f,0.2f,0.2f);
	}
	void OnMouseExit(){
		if(numer==-1)renderer.material.color = Color.red;
        else transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
	}
	void OnMouseDown(){
		if(numer==-1){
			Application.LoadLevel(1);
		}
		else {
			Application.LoadLevel(numer);
		}
	}
}
