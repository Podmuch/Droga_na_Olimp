//Bonus Controller - bonus activation (translate over surface)
//deleting collected or timeout bonuses
using UnityEngine;

public class BonusController : MonoBehaviour {
	private bool visible=false;
	//Activation Time and variable to control animation
	private float time=10.0f, restoftime, animationHeight=1/30, halfConst=0.5f;
	// Update is called once per frame
	void Update () {
		if(visible){
			if(time>0.0f) {
				time-=Time.deltaTime;
				restoftime=time-Mathf.Floor(time);
				//animation
				if(restoftime>halfConst) transform.Translate(0,0,-(restoftime-halfConst)*animationHeight);
				else transform.Translate(0,0,restoftime*animationHeight);
			}
			else Destroy(gameObject);
		}
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			if(!visible) {
				//translate over surface
				transform.Translate(new Vector3(0,0,-1));
				visible=true;
			}
			else {
                other.SendMessage("OnBonusCollected", tag);
				Destroy(gameObject);
			}
		}
	}
}
