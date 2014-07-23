//Spear Controller
//Spear is throwing across map, Player should 
//Player should avoided it, becouse he die when collision occurs
//Throwing sounds is a warring
using UnityEngine;

public class SpearController : MonoBehaviour {
	private bool flight=false, left=true;
	public float timeCounter=0.0f, interval=1.0f;
    //random selection of parts
	private System.Random rand=new System.Random();
	//Flight Control
	private float startPositionZ=269.95f, mapHeightCenter=104, leftBroder=960, rightBorter=1000, translation=10, speed=-0.5f;
	private int polowawysokości = 8;
	// Update is called once per frame
	void Update () {
		if(!flight) {
			timeCounter -= Time.deltaTime;
			if(timeCounter<0.0f){
				timeCounter=interval;
				flight=true;
				audio.Play();
                //random selection of parts
				if(rand.Next(2)<1) {
					transform.position=new Vector3(leftBroder,(float)(mapHeightCenter+rand.Next(-polowawysokości,polowawysokości)),startPositionZ);
					if(left) {
						left=false;
						//RotationY
						transform.Rotate(0,0,180,0);
					}
				}
				else {
					transform.position=new Vector3(rightBorter,(float)(mapHeightCenter+rand.Next(-polowawysokości,polowawysokości)),startPositionZ);
					if(!left) {
						left=true;
                        //RotationY
						transform.Rotate(0,0,180,0);
					}
				}
			}
		}
		else {
			transform.Translate(speed,0,0);
			if(transform.position.x>rightBorter+translation||transform.position.x<leftBroder-translation) flight=false;
		}
	}
}
