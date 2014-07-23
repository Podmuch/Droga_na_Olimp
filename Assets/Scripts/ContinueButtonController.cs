//Continue Button after win or lose game
using UnityEngine;

public class ContinueButtonController : MonoBehaviour {
	void OnMouseDown(){
		Application.LoadLevel(2);
	}
}
