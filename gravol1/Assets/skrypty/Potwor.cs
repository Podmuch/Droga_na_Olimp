using UnityEngine;
using System.Collections;

public class Potwor : MonoBehaviour {
	//parametry ruchu
	public float predkosc, predkoscmax, deltapredkosc, grawitacja, skok;
	//ilosc punktów które dostarcza
	public int punkty;
	public CharacterController kontroler;
	//animacja po zebraniu potwora
	public Transform wybuch;
	//podbicie -wskazjuje czy potwór został podbity, wpływa na ruch, 
	//umiera - pozwala dokończyć animacje śmierci przed usunięciem
	protected bool podbicie = false, umiera = false;
	//obsluga czasu (minimalny ruch - pozwala zebrać potwora, 
	//momentostrzezenia - czaspodbicia*momentostrzezenia to moment od którego potwor zaczyna ruszac nogami)
	protected float czaspodbicia=12.0f, minczaspodbicia=5.0f, deltaczaspodbicia=0.5f,
					czasdowstania=0.0f, czasodbicia=0.1f, opoznienie=0.1f, opoznienieprzysmierci=1.0f,
					dlugosclotu=1.0f, minimalnyruch=0.0001f, momentostrzezenia=0.25f,
					//krance mapy potrzebne do zwijania
					granicazawijaniaY=100, granicaplanszyY=80, wysokosc=15,
					lewykraniec=966.5f ,prawykraniec=993.5f, przesuniecie=0.2f,
                    //obrót po śmierci - potwór przerwaca się na plecy
                    obrotposmierciX = 0.0f, obrotposmierciZ = 180.0f;
	protected void Start () {
		animation.Play();
	}
	// Update is called once per frame
	protected void Update () {
		zawijanie ();
		ruch ();
	}
	//ruch
	protected void ruch(){
		if(!podbicie){
			//ruch po powtórnym podbiciu
			if(czasdowstania>0.0f){
				kontroler.Move(new Vector3(predkosc, skok, 0));
				czasdowstania-=Time.deltaTime;
			}
			//normalny ruch
			else {
				kontroler.Move(new Vector3(predkosc, -grawitacja, 0));
			}
		}
		else {
			czasdowstania-=Time.deltaTime;
			//początkowy lot po podbiciu
			if(czasdowstania>czaspodbicia-dlugosclotu){
				kontroler.Move(new Vector3(predkosc, skok, 0));
			}
			//normalny ruch po podbiciu -dzialanie grawitacji oraz umożliwienie zebrania potwora
			else {
				kontroler.Move(new Vector3(minimalnyruch, -grawitacja, 0));
				if(czasdowstania<czaspodbicia*momentostrzezenia){
					animation.Play();
				}
			}
			if(czasdowstania<0.0f){
				podbicie=false;
				czasdowstania=0.0f;
				//obrot w osi X (podniesienie się potwora)
                transform.Rotate(-obrotposmierciZ, 0, -obrotposmierciX, 0);
			}
		}
		if(czasodbicia>0.0f) czasodbicia-=Time.deltaTime;
	}
	//zawijanie na krancach mapy
	protected void zawijanie(){
		if (transform.position.y < granicaplanszyY) {
			smierc();
		}
		if (transform.position.y > granicazawijaniaY) {
			if (transform.position.x <lewykraniec)  {
				transform.position=new Vector3(prawykraniec,transform.position.y,transform.position.z);
				czasodbicia=opoznienie;
			}
			else if(transform.position.x >prawykraniec){
				transform.position=new Vector3(lewykraniec,transform.position.y,transform.position.z);
				czasodbicia=opoznienie;
			}
		} else {
			if (transform.position.x < lewykraniec || transform.position.x > prawykraniec) {
				transform.Translate (0, wysokosc, 0);
				//obrot przesuniecie na szczyt i zmiana parametrów
				transform.Rotate (0, 180, 0);
				if (predkosc < predkoscmax&&predkosc>-predkoscmax) {
					if(predkosc>0)predkosc += deltapredkosc;
					else predkosc -=deltapredkosc;
				}
				if (czaspodbicia >minczaspodbicia) czaspodbicia -= deltaczaspodbicia;
				predkosc = -predkosc;
				czasodbicia=opoznienie;
			}
		}
	}
	//zderzenia
	protected void OnTriggerEnter(Collider other) {
		switch (other.tag){
			case "podloga":
				if(!podbicie){
					if(other.bounds.center.y<transform.position.y) {
						podbicie=true;
						czasdowstania=czaspodbicia;
						//obrót na plecy podczas podbicia
                        transform.Rotate(obrotposmierciZ, 0, obrotposmierciX, 0);
						animation.Stop();
					}
				}
				else {
					if(czasdowstania<czaspodbicia-dlugosclotu){
						podbicie=false;
						czasdowstania=dlugosclotu;
						//obrót do normalnej pozycji
                        transform.Rotate(-obrotposmierciZ, 0, -obrotposmierciX, 0);
						animation.Play();
					}
					else czasdowstania=czaspodbicia-dlugosclotu;
				}
				break;
			case "Player":
				if(podbicie) smierc();
				else if(!umiera)other.SendMessage("smierc");
				break;
			case "potwor":
                //bool predkoscdodatnia = other.GetComponent<Potwor>().predkosc > 0 ? true : false;
				if(czasodbicia<0.0f&&
			      ((predkosc>0&&other.bounds.center.x>transform.position.x)||
                    (predkosc < 0 && other.bounds.center.x < transform.position.x)))
                {
					predkosc = -predkosc;
					//obrót przy kolizji z innym potworem
					transform.Rotate (0, 180, 0);
					//odbicie się od siebie potworów
					if(predkosc>0)transform.Translate(0,0,przesuniecie);
					else transform.Translate(0,0,przesuniecie);
				}
				break;
		}
	}
	//smierc
	protected IEnumerator piorun(){
		umiera = true;
		yield return new WaitForSeconds(opoznienie);
		predkosc = 0;
		animation.Play("die");
		yield return new WaitForSeconds(opoznienieprzysmierci);
		FindObjectOfType<Gracz> ().ilpunktow += punkty;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
	protected void smierc(){
		Instantiate(wybuch, transform.position, Quaternion.identity);
		FindObjectOfType<Gracz> ().ilpunktow += punkty;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
}
