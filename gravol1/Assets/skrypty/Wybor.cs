//Obsługa drugiego widoku menu (wybór rundy)
using UnityEngine;

public class Wybor : MonoBehaviour {
	public int numer;
	void OnMouseEnter(){
		if(numer==-1)renderer.material.color = Color.black;
		else transform.up=new Vector3(2.5f,2.5f,1);
	}
	void OnMouseExit(){
		if(numer==-1)renderer.material.color = Color.red;
		else transform.up=new Vector3(0,0,0);
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
