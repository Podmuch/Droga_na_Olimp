//Bonus Controller - bonus activation (translate over surface)
// deleting collected or timeout bonuses
using UnityEngine;

public class BonusController : MonoBehaviour {
	private bool widoczny=false;
	//Activation Time and variable to control animation
	private float time=10.0f, resztaczasu, animationHeight=1/30, halfConst=0.5f;
	// Update is called once per frame
	void Update () {
		if(widoczny){
			if(time>0.0f) {
				time-=Time.deltaTime;
				resztaczasu=time-Mathf.Floor(time);
				//animation
				if(resztaczasu>halfConst) transform.Translate(0,0,-(resztaczasu-halfConst)*animationHeight);
				else transform.Translate(0,0,resztaczasu*animationHeight);
			}
			else Destroy(gameObject);
		}
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			if(!widoczny) {
				//translate over surface
				transform.Translate(new Vector3(0,0,-1));
				widoczny=true;
			}
			else {
				other.SendMessage("bonus",tag);
				Destroy(gameObject);
			}
		}
	}
}
