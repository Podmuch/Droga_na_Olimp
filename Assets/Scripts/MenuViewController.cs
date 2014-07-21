//First Menu View buttons controller
using UnityEngine;

public class MenuViewController : MonoBehaviour {

	public int number;
 	void OnMouseEnter(){
		renderer.material.color = Color.black;
	}
	void OnMouseExit(){
		renderer.material.color = Color.red;
	}
	void OnMouseDown(){
		if(number==2){
			Application.Quit();
		}
		if(number==0) {
			Application.LoadLevel(2);
		}
	}
}
