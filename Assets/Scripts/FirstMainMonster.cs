//Pierwszy Boss - obsługa pojawiających się potworów, ruchu bossa oraz śmierci
using UnityEngine;
using System.Collections;

public class BossPotwor : Monster {
	//dzwieki
	public AudioSource skokaudio, smiercaudio, nowepotworyaudio, upadekaudio;
	//resp
	public int zycia;
	//potwory
	public Transform[] potwor;
	public int iloscgatunkow, iloscrespow;
	public Vector3[] respy;
	//obsluga ruchu
	private float czaschodzenia=0.0f, maxczaschodzenia=20.0f, zasiegwidoku=7.0f;
	//dane na temat gracza
	private Player gracz;
	private Vector3 pozycjagracza;
	//obsluga obrotów i dźwięku upadania
	private bool lewo=false, lot=true;
	private Quaternion obrotwprawo=new Quaternion(0,0.7071068f,0,0.7071068f),
	obrotwlewo=new Quaternion(0,0.7071068f,0,-0.7071068f);
	new void Start(){
		gracz = FindObjectOfType<Player> ();
		animation.Play();
		lewykraniec = 966.5f;
		prawykraniec=993.5f;
	}

	new void Update(){
		zawijanie ();
		Move ();
	}
	//tworzenie nowych potworów, gdy zostanie podbity
	void tworzeniepotworow(){
		for(int i=0;i<iloscrespow;i++){
			Transform nowy=null;
			nowy=(Transform)Instantiate(potwor[i%iloscgatunkow],respy[i] , Quaternion.identity);
			Instantiate(explosionAnimation, respy[i], Quaternion.identity);
			//obrot wokół osi Y, aby ustawić go zgodnie z ustawieniem platform
			nowy.Rotate(0,90,0,0);
		}
		nowepotworyaudio.Play ();
	}
	//ruch
	new void Move(){
		pozycjagracza = gracz.transform.position;
		czaschodzenia += Time.deltaTime;
		if(czaschodzenia<maxczaschodzenia*0.5f){
			if(pozycjagracza.x-zasiegwidoku>transform.position.x||
			   pozycjagracza.x+zasiegwidoku<transform.position.x) 
			{
				if(pozycjagracza.x<transform.position.x) {
					if(!lewo) {
						transform.rotation=obrotwlewo;
						lewo=true;
						speed=-speed;
					}
				}
				else {
					if(lewo) {
						transform.rotation=obrotwprawo;
						lewo=false;
						speed=-speed;
					}
				}
			}
		}
		else {
			if(czaschodzenia>maxczaschodzenia) czaschodzenia=0;
		}
		if (czaschodzenia > maxczaschodzenia*0.75f && czaschodzenia < maxczaschodzenia*0.75f+flyingTime) {
			if(controller.isGrounded) skokaudio.Play();
			if(transform.position.z==270.0f) transform.Translate(2.0f,0,0);
			controller.Move(new Vector3(speed, jumpForce, 0));
			lot=true;
		}
		else {
			if(transform.position.z!=270.0f) transform.position=new Vector3(transform.position.x,transform.position.y,270);
			if(lot&&controller.isGrounded){
				lot=false;
				upadekaudio.Play();
			}
			controller.Move(new Vector3(speed, -gravityForce, 0));
		}
	}
	//zawijanie
	new void zawijanie(){
		if(czaschodzenia<maxczaschodzenia*0.5f)czaschodzenia = maxczaschodzenia * 0.5f;
		if (transform.position.x < lewykraniec) transform.position=new Vector3(prawykraniec, transform.position.y,transform.position.z);
		if (transform.position.x > prawykraniec) transform.position=new Vector3(lewykraniec, transform.position.y,transform.position.z);
	}
	//kolizje
	new void OnTriggerEnter(Collider other) {
		switch (other.tag){
		case "podloga":
			if(other.bounds.center.y<transform.position.y&&!lot) {
				zycia--;
				if(zycia==0) 
					SendMessage("smierc");
				else {
					tworzeniepotworow();
					czaschodzenia=maxczaschodzenia*0.75f;
				}
			}
			break;
		case "Player":
			if(!isDead)other.SendMessage("smierc");
			break;
		}
	}
	//smierc
	new IEnumerator piorun(){
		float tmp = speed;
		speed=0;
		animation.Play("resist");
		yield return new WaitForSeconds(opoznienieprzysmierci);
		speed = tmp;
		animation.Play ("walk");
	}
	//smierc
	new IEnumerator smierc(){
		smiercaudio.Play ();
		isDead = true;
		foreach(Monster p in FindObjectsOfType<Monster>()){
			if(p.tag!="boss")p.SendMessage("smierc");
		}
		speed = 0;
		czaschodzenia = 0;
		animation.Play("die");
		yield return new WaitForSeconds(2*opoznienieprzysmierci);
		FindObjectOfType<Player> ().ilpunktow += points;
		FindObjectOfType<Fabryka> ().SendMessage ("koniec", true);
		Destroy(gameObject);
	}
}
