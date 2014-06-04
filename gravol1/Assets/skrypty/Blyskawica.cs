using UnityEngine;
using System.Collections;

public class Blyskawica : MonoBehaviour {
	private float czas=0.5f;	
	// Update is called once per frame
	void Update () {
		czas -= Time.deltaTime;
		if(czas<0) Destroy(gameObject);
	}
}
