//temporary
using UnityEngine;

public class ResetController : MonoBehaviour {
	void Start () {
		Player player = FindObjectOfType<Player> ();
		if(player!=null){
			player.points = 0;
			player.lighteningCharges = 3;
			player.lives = 3;
		}
	}
}
