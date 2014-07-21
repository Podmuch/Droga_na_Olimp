//Fabryka zarządza respawnem potworów oraz decyduje kiedy następił koniec gry 
//(gracz stracił wszystkie życia, lub zabito wszystkie potwory)
using UnityEngine;
using System.Collections;

public class Fabryka : MonoBehaviour {
	//tekst koncowy
	public TextMesh text;
	//tablica gatunków potworów
	public Transform[] potwor;
	//ilość potworów danego gatunku
	public int[] iloscpotworow;
	//ilość gatunków
	//licznik sprawdzający ile potworów jeszcze jest/będzie na planszy. Jeśli 0 to koniec rundy
	public int iloscpozostalychpotworow, iloscgatunkow;
	//obsługa respu
	public float[] czasrespu, opoznienie;
	//z której strony ma się pojawić
	private bool lewa=true;
	//licznik sprawdzający ile potworów danego gatunku pozostało
	private int[] licznik;
	//przesuniecie końcowego napisu, aby był widoczny i opoznienie po jakim pojawia sie napis
	private float przesuniecie=50, opoznienienapisu=2.0f;
	//pozycje respawnów dla nowych potworów
	private Vector3 lewyresp=new Vector3(966.8765f, 110.5012f, 270),
					prawyresp=new Vector3(993.912f, 95.5f, 270);
	void Start(){
		licznik = new int[iloscgatunkow];
		for(int i=0;i<iloscgatunkow;i++){
			licznik[i]=0;
			iloscpozostalychpotworow+=iloscpotworow[i];
		}
		if(iloscpozostalychpotworow==0) iloscpozostalychpotworow=-1;
	}
	// Update is called once per frame
	void Update () {
		for(int i=0;i<iloscgatunkow;i++){
			opoznienie[i]-=Time.deltaTime;
			if(opoznienie[i]<0.0f){
				Transform nowy=null;
				if(licznik[i]<iloscpotworow[i]) {
					if(lewa)nowy=(Transform)Instantiate(potwor[i],lewyresp , Quaternion.identity);
					else nowy=(Transform)Instantiate(potwor[i], prawyresp, Quaternion.identity);
					licznik[i]++;
					lewa=!lewa;
					//obrot wokół osi Y, aby ustawić go zgodnie z ustawieniem platform
					nowy.Rotate(0,90,0,0);
					opoznienie[i]=czasrespu[i];
				}
			}
		}
		if (iloscpozostalychpotworow == 0) {
			SendMessage ("koniec", true);
			iloscpozostalychpotworow--;
		}
	}
	public IEnumerator koniec(bool wygrana){
		yield return new WaitForSeconds(opoznienienapisu);
		text.transform.Translate(new Vector3(0,0,przesuniecie));
		if(wygrana) {
			text.text="Wygrałes";
			FindObjectOfType<Player> ().wygrana = true;
		}
		else text.text="Przegrałeś";
	}
}
