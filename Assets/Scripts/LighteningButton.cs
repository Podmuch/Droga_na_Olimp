//Dynamit - obsługa użycia - nieszczenie potworów, pojawianie się i znikanie przy doładowaniu lub zużyciu
using UnityEngine;

public class LighteningButton : MonoBehaviour {
	public Texture trzy, dwa, jeden;
	public Player player;
	public Transform blyskawica;
	public Collider fundament;
	//przesuniecie za ekran wraz z fundamentem, gdy znika
	private float przesuniecie=500;
	//ustawienie obrotu dla powstajacych piorunów
	private Quaternion obrot=new Quaternion(0.7f,0.3f,-0.7f,0.3f);
	//przesunięcie pioruna względem potwora w którego ma trafic
	private Vector3 przesunieciepioruna=new Vector3(1,1,-1);
	//pozwala pozostac obiektowi na drugą runde
	void Awake () {
		DontDestroyOnLoad (this);
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag=="Player") {
			audio.Play();
			if(player.iloscdynamitu==1) {
				transform.Translate(przesuniecie,0,0);
				fundament.isTrigger=true;
				player.iloscdynamitu--;
			}
			if(player.iloscdynamitu==2) {
				renderer.material.SetTexture(0,jeden);
				player.iloscdynamitu--;
			}
			if(player.iloscdynamitu==3) {
				renderer.material.SetTexture(0,dwa);
				player.iloscdynamitu--;
			}
			foreach(Monster p in FindObjectsOfType<Monster>()){
				//tworzenie pioruna w miejscu potwora
				Transform nowy=(Transform)Instantiate(blyskawica, p.GetComponent<Transform>().position, Quaternion.identity);
				nowy.Translate(przesunieciepioruna);
				nowy.rotation=obrot;
				p.SendMessage("piorun");
			}

		}
	}
	void zmianailoscidynamitu(){
		if(player.iloscdynamitu==1) {
			transform.Translate(-przesuniecie,0,0);
			fundament.isTrigger=false;
			renderer.material.SetTexture(0,jeden);
		}
		if(player.iloscdynamitu==2) {
			renderer.material.SetTexture(0,dwa);
		}
		if(player.iloscdynamitu==3) {
			renderer.material.SetTexture(0,trzy);
		}
	}
}
