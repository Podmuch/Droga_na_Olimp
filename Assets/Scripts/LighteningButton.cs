//Lightening Button - kills all monster on the game board
using UnityEngine;

public class LighteningButton : MonoBehaviour {
	public Texture threeChargesView, twoChargesView, oneChargesView;
	public Player player;
	public Transform lightening;
	public Collider buttonBase;
	//Translating base behind the board
	private float translation=500;
	//Rotation for new Lightenings
	private Quaternion lighteningRotation=new Quaternion(0.7f,0.3f,-0.7f,0.3f);
	//Translation for new Lightenings
	private Vector3 lighteningTranslation=new Vector3(1,1,-1);
	//This objects never destroyed
	void Awake () {
		DontDestroyOnLoad (this);
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			audio.Play();
			if(player.lighteningCharges==1) {
				transform.Translate(translation,0,0);
				buttonBase.isTrigger=true;
				player.lighteningCharges--;
			}
			if(player.lighteningCharges==2) {
				renderer.material.SetTexture(0,oneChargesView);
				player.lighteningCharges--;
			}
			if(player.lighteningCharges==3) {
				renderer.material.SetTexture(0,twoChargesView);
				player.lighteningCharges--;
			}
			foreach(Monster p in FindObjectsOfType<Monster>()){
				//New Lightening near the monster
				Transform newLightening=(Transform)Instantiate(lightening, p.GetComponent<Transform>().position, Quaternion.identity);
				newLightening.Translate(lighteningTranslation);
				newLightening.rotation=lighteningRotation;
				p.SendMessage("Lightening");
			}

		}
	}
	void OnLighteningChargesChanged(){
		if(player.lighteningCharges==1) {
			transform.Translate(-translation,0,0);
			buttonBase.isTrigger=false;
			renderer.material.SetTexture(0,oneChargesView);
		}
		if(player.lighteningCharges==2) {
			renderer.material.SetTexture(0,twoChargesView);
		}
		if(player.lighteningCharges==3) {
			renderer.material.SetTexture(0,threeChargesView);
		}
	}
}
