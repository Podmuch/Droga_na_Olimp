using UnityEngine;
using System.Collections;

public class Przycisk : MonoBehaviour {
	public static bool lewo=false, prawo=false, skok=false;
	public int numer;

	void OnMouseEnter(){
		switch (numer){
			case 1:
				lewo=true;
				break;
			case 2:
				prawo=true;
				break;
			case 3:
				skok=true;
				break;
		}
	}
	void OnMouseExit(){
		switch (numer){
		case 1:
			lewo=false;
			break;
		case 2:
			prawo=false;
			break;
		case 3:
			skok=false;
			break;
		}
	}
}
