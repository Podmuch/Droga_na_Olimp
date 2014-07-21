//Second menu view controller - provide choose stage to play
using UnityEngine;
public class ChooseViewController : MonoBehaviour {
	public int number;
	void OnMouseEnter(){
		if(number==-1)renderer.material.color = Color.black;
		else transform.localScale+=new Vector3(0.2f,0.2f,0.2f);
	}
	void OnMouseExit(){
		if(number==-1)renderer.material.color = Color.red;
        else transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
	}
	void OnMouseDown(){
		if(number==-1){
			Application.LoadLevel(1);
		}
		else {
			Application.LoadLevel(number);
		}
	}
}
