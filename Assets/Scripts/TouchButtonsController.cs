//Control Buttons - touch devices
//Each Button have own static variable
using UnityEngine;

public class TouchButtonsController : MonoBehaviour {
	public static bool left=false, right=false, jump=false;
	public int number;

	void OnMouseEnter(){
		switch (number){
			case 1:
				left=true;
				break;
			case 2:
				right=true;
				break;
			case 3:
				jump=true;
				break;
		}
	}
	void OnMouseExit(){
		switch (number){
		case 1:
			left=false;
			break;
		case 2:
			right=false;
			break;
		case 3:
			jump=false;
			break;
		}
	}
}
