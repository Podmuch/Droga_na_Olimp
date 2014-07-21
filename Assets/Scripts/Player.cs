//Gracz
using UnityEngine;

public class Player : MonoBehaviour {
	//ilosc dynamitu i punktów
	public int iloscdynamitu = 3,ilpunktow=0, ilzyc=3;
	//ruch gracza
	public CharacterController kontroler;
	//dzwieki
	public AudioSource skokaudio, smiercaudio;
	//wyswietlanie punktow i zycia
	private TextMesh text=null;
	//parametry ruchu gracza
	private CharacterMotor ChMotor;
	//ilosc zyc,maximum dynamitu i punkty za zebranie bonusu punktowego
	private int maxdynamit=3, punktyzabonus=1000;
	//obsluga czasu i bonusow
	private float czaslotu=0.0f, maxczaslotu=1.0f,
				  czasniewidzialnosci=0.0f, maxczasniewidzialnosci=3.0f,
				  czasprzyspieszenia=0.0f, czasspowolnienia=0.0f, maxczasbonusu=10.0f, mocbonusu=2;
	private bool lewo=false, prawo=false, ruch=false, nietykalny=false, animacjasmierci=false;
	//pozycja startowa i ruch do wykrywania kolizji gdy gracz nic nie robi
	private Vector3 pozycjastartowa=new Vector3(980,97,270),
					niewidocznyruch=new Vector3(0.0001f, 0, 0),
					kieruneklotuposmierci=new Vector3(0.1f, 0.1f, 0.0f); //poczatkowe wznoszenie się po śmierci
	//obrot postaci przy zmianie kierunku ruchu
	private Quaternion obrotwprawo=new Quaternion(0,0.7071068f,0,0.7071068f),
					   obrotwlewo=new Quaternion(0,0.7071068f,0,-0.7071068f);
	//krance mapy potrzebne do zwijania
	private float lewykraniec=966.5f ,prawykraniec=993.5f;
	//zablokowanie ruchu przy wygranej
	public bool wygrana=false;
	//pozwala pozostac obiektowi na drugą runde
	void Awake () {
		DontDestroyOnLoad (this);
		ChMotor=GetComponent<CharacterMotor> ();
	}
	//ustawienie tekstu z iloscią żyć i punktów
	void ustawienietekstu(){
		foreach( GameObject go in FindObjectsOfType<GameObject> ()){
			if(go.tag=="tekst"){
				text= go.GetComponent<TextMesh>();
				transform.position=pozycjastartowa;
				wygrana=false;
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if(FindObjectOfType<Przycisk>()!=null){
			if(text==null) ustawienietekstu();
			//zablokowanie ruchu przy wygranej
			if(wygrana) transform.position=pozycjastartowa;
			text.text="POINTS: "+ilpunktow.ToString()+"     LIVES: "+ilzyc.ToString();
			//niewidoczny ruch, aby móc wykryć kolizje gdy gracz nic nie robi
			kontroler.Move(niewidocznyruch);
			animacje ();
			zawijanie ();
			spadanie ();
			nietykalnosc ();
			wygaszaniebonusow ();
		}
	}
	//zmiana animacji podczas skrecania
	void animacje(){
		if((Input.GetKeyDown("up")||Przycisk.skok)&&kontroler.isGrounded) skokaudio.Play();
		if(prawo||Przycisk.prawo) transform.rotation= obrotwprawo;
		else prawo=Input.GetKeyDown("right");
		if(lewo||Przycisk.lewo) transform.rotation=obrotwlewo;
		else lewo=Input.GetKeyDown("left");
		if(!ruch){
			if(!animacjasmierci)animation.Play("walk");
			ruch=true;
		}
		if(Input.GetKeyUp("right")) prawo=false;
		if(Input.GetKeyUp("left")) lewo=false;
		if(!lewo&&!prawo&&ruch&&!Przycisk.lewo&&!Przycisk.prawo){
			ruch=false;
			if(!animacjasmierci)animation.Play("idle");
		}
	}
	//zawijanie na krancach mapy
	void zawijanie(){
		if (transform.position.x < lewykraniec) transform.position=new Vector3(prawykraniec, transform.position.y,transform.position.z);
		if (transform.position.x > prawykraniec) transform.position=new Vector3(lewykraniec, transform.position.y,transform.position.z);
	}
	//smierc
	public void smierc(){
		if(!nietykalny){
			ilzyc--;
			//wyswietlenie napisów końcowych
			if(ilzyc<0) FindObjectOfType<Fabryka>().SendMessage("koniec", false);
			czaslotu=maxczaslotu;
			//przesuniecie po za plansze w osi Z, pozwala na animacje spadania
			transform.Translate(3.0f,0,0);
			animacjasmierci=true;
		}
	}
	//spadanie po smierci
	void spadanie(){
		czaslotu-=Time.deltaTime;
		if(czaslotu>0.0f) {
			kontroler.Move(kieruneklotuposmierci);
			animation.Play("diehard");
		}
		if(transform.position.y<85&&ilzyc>-1){
			transform.position = pozycjastartowa;
			animation.Play("idle");
			nietykalny=true;
			animacjasmierci=false;
			czasniewidzialnosci=maxczasniewidzialnosci;
			if(ruch) animation.Play("walk");
		}
	}
	//nietykalnosc gracza gdy sie zrespawnuje, zeby mial szanse uciec
	void nietykalnosc(){
		if(nietykalny) {
			czasniewidzialnosci-=Time.deltaTime;
			if(czasniewidzialnosci<0.0f) {
				nietykalny=false;
				czasniewidzialnosci=0.0f;
			}
			//r=b=g=0.588, a=1.0 naturalny kolor
			//zmiana od czarnego do naturalnego koloru w zależności od pozostałego czasu niewidzialności
			float nowykolor=0.1f+(0.488f-(czasniewidzialnosci*0.488f)/maxczasniewidzialnosci);
			foreach( Renderer r in gameObject.GetComponentsInChildren<Renderer>()){
				//zmiana koloru tekstur po zrespieniu się gracza(z czarnego do naturalnego)
				r.material.color = new Color(nowykolor,nowykolor,nowykolor,1.0f);
			}

		}
	}
	//wygaszanie przyspieszenia i spowolnienia
	void wygaszaniebonusow (){
		if(czasprzyspieszenia>0.0f) {
			czasprzyspieszenia-=Time.deltaTime;
			if(czasprzyspieszenia<0.0f) {
				ChMotor.movement.maxForwardSpeed /= mocbonusu;
				ChMotor.movement.maxGroundAcceleration /= mocbonusu;
				ChMotor.movement.maxAirAcceleration /= mocbonusu;
				ChMotor.jumping.baseHeight /= mocbonusu;
			}
		}
		if(czasspowolnienia>0.0f){
			czasspowolnienia-=Time.deltaTime;
			if(czasspowolnienia<0.0f) {
				ChMotor.movement.maxForwardSpeed *= mocbonusu;
				ChMotor.movement.maxGroundAcceleration *= mocbonusu;
				ChMotor.movement.maxAirAcceleration *= mocbonusu;
				ChMotor.jumping.baseHeight *= mocbonusu;
			}
		}
	}
	//zbieranie bonusow
	public void bonus(string tagbonusu){
		switch (tagbonusu) {
			case "zycie":
				ilzyc++;
				break;
			case "kasa": 
				ilpunktow+=punktyzabonus;
				break;
			case "blyskawica":
				if(iloscdynamitu<maxdynamit) iloscdynamitu++;
				FindObjectOfType<LighteningButton>().SendMessage("zmianailoscidynamitu");
				break;
			case "przyspieszenie":
				if(czasprzyspieszenia<=0.0f){
					ChMotor.movement.maxForwardSpeed *= mocbonusu;
					ChMotor.movement.maxGroundAcceleration *= mocbonusu;
					ChMotor.movement.maxAirAcceleration *= mocbonusu;
					ChMotor.jumping.baseHeight *= mocbonusu;
					czasprzyspieszenia=maxczasbonusu;
				}
				break;
			case "spowolnienie":
				if(czasspowolnienia<=0.0f){
					ChMotor.movement.maxForwardSpeed /= mocbonusu;
					ChMotor.movement.maxGroundAcceleration /= mocbonusu;
					ChMotor.movement.maxAirAcceleration /= mocbonusu;
					ChMotor.jumping.baseHeight /= mocbonusu;
					czasspowolnienia=maxczasbonusu;
				}
				break;
		}
	}

	void OnTriggerEnter(Collider other) {
		if((other.tag=="potwor"||other.tag=="boss")&&!nietykalny) smiercaudio.Play();
		if(other.tag=="spear"&&!nietykalny){
			smiercaudio.Play();
			smierc();
		}
	}
}
