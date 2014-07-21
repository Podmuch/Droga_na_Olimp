//Base monster class(other monster classes intereht from this) -basic moving, death and colision detecting
using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	//Moving parameters
	public float speed, maxSpeed, deltaSpeed, gravityForce, jumpForce;
	//Monster points
	public int points;
	public CharacterController controller;
	//Animation after collected monster
	public Transform explosionAnimation;
	//podbicie -wskazjuje czy potwór został podbity, wpływa na ruch, 
	//umiera - pozwala dokończyć animacje śmierci przed usunięciem
	protected bool isHurt = false, isDead = false;
	//obsluga czasu (minimalny ruch - pozwala zebrać potwora, 
	//momentostrzezenia - czaspodbicia*momentostrzezenia to moment od którego potwor zaczyna ruszac nogami)
	protected float hurtTime=12.0f, minHurtTime=5.0f, deltaHurtTime=0.5f,
					czasdowstania=0.0f, czasodbicia=0.1f, opoznienie=0.1f, opoznienieprzysmierci=1.0f,
					flyingTime=0.2f, maxFlyingTime=1.0f, minMove=0.0001f, momentostrzezenia=0.25f,
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
		Move ();
	}
	//ruch
	protected void Move(){
		if(!isHurt){
			//ruch po powtórnym podbiciu
			if(czasdowstania>0.0f){
                //podbicie w zaleznosci od polozenia klocka
                controller.Move(new Vector3(0, jumpForce, 0));
				czasdowstania-=Time.deltaTime;
			}
			//normalny ruch
			else {
                //przy odbiciu predkosc musi byc mala, a nie ze zaczyna poruszac sie w poziomie jak przestanie sie wznosic
				controller.Move(new Vector3(speed, -gravityForce, 0));
                if (!animation.isPlaying && controller.isGrounded) animation.Play();
                else if (animation.isPlaying && !controller.isGrounded) animation.Stop();
			}
		}
		else {
			czasdowstania-=Time.deltaTime;
			//początkowy lot po podbiciu
			if(czasdowstania>hurtTime-flyingTime){
                //zmienna skok powinna byc usunieta, ruch po okregu odpowiednim (wedlug zmiennej predkosc)
                controller.Move(new Vector3(speed * Mathf.Cos(Mathf.PI * (hurtTime - czasdowstania) / (3.0f * flyingTime)), jumpForce, 0));
			}
			//normalny ruch po podbiciu - dzialanie grawitacji oraz umożliwienie zebrania potwora
			else {
				if(controller.isGrounded)controller.Move(new Vector3(minMove, -gravityForce, 0));
                else controller.Move(new Vector3(speed * Mathf.Sin(Mathf.PI * (hurtTime-czasdowstania - flyingTime)/ (2.0f/3.0f*hurtTime) + Mathf.PI/3.0f), -gravityForce, 0));
				if(czasdowstania<hurtTime*momentostrzezenia){
					animation.Play();
				}
			}
			if(czasdowstania<0.0f){
				isHurt=false;
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
				if (speed < maxSpeed&&speed>-maxSpeed) {
					if(speed>0)speed += deltaSpeed;
					else speed -=deltaSpeed;
				}
                if (flyingTime < maxFlyingTime) flyingTime += 0.1f;
				if (hurtTime >minHurtTime) hurtTime -= deltaHurtTime;
				speed = -speed;
				czasodbicia=opoznienie;
			}
		}
	}
	//zderzenia
	protected void OnTriggerEnter(Collider other) {
		switch (other.tag){
			case "podloga":
				if(!isHurt){
					if(other.bounds.center.y<transform.position.y) {
						isHurt=true;
						czasdowstania=hurtTime;
						//obrót na plecy podczas podbicia
                        transform.Rotate(obrotposmierciZ, 0, obrotposmierciX, 0);
						animation.Stop();
					}
				}
				else {
					if(czasdowstania<hurtTime-flyingTime){
						isHurt=false;
						czasdowstania=flyingTime;
						//obrót do normalnej pozycji
                        transform.Rotate(-obrotposmierciZ, 0, -obrotposmierciX, 0);
					}
					else czasdowstania=hurtTime-flyingTime;
				}
				break;
			case "Player":
				if(isHurt) smierc();
				else if(!isDead)other.SendMessage("smierc");
				break;
			case "potwor":
				if(czasodbicia<0.0f&&
			      ((speed>0&&other.bounds.center.x>transform.position.x)||
                    (speed < 0 && other.bounds.center.x < transform.position.x)))
                {
					speed = -speed;
					//obrót przy kolizji z innym potworem
					transform.Rotate (0, 180, 0);
					//odbicie się od siebie potworów
					if(speed>0)transform.Translate(0,0,przesuniecie);
					else transform.Translate(0,0,przesuniecie);
				}
				break;
		}
	}
	//smierc
	protected IEnumerator piorun(){
		isDead = true;
		yield return new WaitForSeconds(opoznienie);
		speed = 0;
		animation.Play("die");
		yield return new WaitForSeconds(opoznienieprzysmierci);
		FindObjectOfType<Player> ().ilpunktow += points;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
	protected void smierc(){
		Instantiate(explosionAnimation, transform.position, Quaternion.identity);
		FindObjectOfType<Player> ().ilpunktow += points;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
}
