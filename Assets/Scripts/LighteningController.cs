//Lightening Controller
using UnityEngine;

public class LighteningController : MonoBehaviour {
	private float lifeTime=0.5f;	
	// Update is called once per frame
	void Update () {
		lifeTime -= Time.deltaTime;
		if(lifeTime<0) Destroy(gameObject);
	}
}
