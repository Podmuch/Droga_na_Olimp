//obsługa włóczni
//pojawiająca się okresowo włócznia przelatująca w poprzek planszy
//należy jej unikać, gdyż przy kolizji straci się życie (jako ostrzeżenie usłyszymy dźwięk rzutu)
using UnityEngine;

public class Spear : MonoBehaviour {
	private bool lot=false, lewo=true;
	//pojawianie się włóczni
	public float czas=0.0f, interwal=1.0f;
	//wybór stron
	private System.Random rand=new System.Random();
	//obsługa lotu
	private float pozycjaZ=269.95f, srodekY=104, lewykraniec=960, prawykraniec=1000, przesuniecie=10, predkosc=-0.5f;
	private int polowawysokości = 8;
	// Update is called once per frame
	void Update () {
		if(!lot) {
			czas -= Time.deltaTime;
			if(czas<0.0f){
				czas=interwal;
				lot=true;
				audio.Play();
				//wybór strony pojawienia się włóczni
				if(rand.Next(2)<1) {
					transform.position=new Vector3(lewykraniec,(float)(srodekY+rand.Next(-polowawysokości,polowawysokości)),pozycjaZ);
					if(lewo) {
						lewo=false;
						//ustawienie w odpowiednią stronę
						transform.Rotate(0,0,180,0);
					}
				}
				else {
					transform.position=new Vector3(prawykraniec,(float)(srodekY+rand.Next(-polowawysokości,polowawysokości)),pozycjaZ);
					if(!lewo) {
						lewo=true;
						//ustawienie w odpowiednią stronę
						transform.Rotate(0,0,180,0);
					}
				}
			}
		}
		else {
			transform.Translate(predkosc,0,0);
			if(transform.position.x>prawykraniec+przesuniecie||transform.position.x<lewykraniec-przesuniecie) lot=false;
		}
	}
}
