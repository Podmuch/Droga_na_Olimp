//Skaczący Potwór - klasa pochodna zwykłego potwora
//zmodyfikowany ruch i inna obsługa animacji
using UnityEngine;
using System.Collections;

public class SkaczacyPotwor : Monster {
	//potrzebny do animacji
	private bool lot=false;
	//obsluga animacji i ruchu
	private float poczatekskoku=3.49f, poczateksmierci=9.0f, czasskoku=0.0f, maxczasskoku=0.8f, pol=0.5f, wysokosckontrolera;
    private new void Start()
    {
        obrotposmierciX = 90.0f;
        obrotposmierciZ = 0.0f;
        wysokosckontrolera = controller.height;
    }
	private new void Update () {
		zawijanie ();
		animacja ();
		Move ();
	}
    //animacja skoku
	private void animacja(){
		//animacja
		if(!lot){
			if(controller.isGrounded&&!isDead&&
			   (!isHurt||(czasdowstania<hurtTime*momentostrzezenia))) {
				foreach(AnimationState state in animation){
					state.time=poczatekskoku;
				}
				animation.Play ();
				czasskoku=maxczasskoku;
				lot=true;
			}
		}
		else {
			czasskoku-=Time.deltaTime;
			if(czasskoku<0.0f) lot=false;
		}
	}
    //nowy sposób ruchu - skokowy
	private new void Move(){
		if(!isHurt){
            //zmiana wysokosci kontrolera przy powrocie do normalnego chodzenia
            controller.height = wysokosckontrolera;
			//ruch po powtórnym podbiciu
			if(czasdowstania>0.0f){
				controller.Move(new Vector3(speed, jumpForce, 0));
				czasdowstania-=Time.deltaTime;
			}
			//normalny ruch
			else {
				if(czasskoku>maxczasskoku*pol){
					controller.Move(new Vector3(speed,Mathf.Abs(speed), 0));
				}
				else {
					if(controller.isGrounded) controller.Move(new Vector3(minMove, -gravityForce, 0));
					else controller.Move(new Vector3(speed, -gravityForce, 0));
				}
			}
		}
		else {
            //zmniejszenie kontrolera, żeby potwór nie wisiał w powietrzu tylko upadł na ziemie
            controller.height = 0;
			czasdowstania-=Time.deltaTime;
			//początkowy lot po podbiciu
			if(czasdowstania>hurtTime-flyingTime){
				controller.Move(new Vector3(speed, jumpForce, 0));
			}
			//normalny ruch po podbiciu -dzialanie grawitacji oraz umożliwienie zebrania potwora
			else {
				controller.Move(new Vector3(minMove, -gravityForce, 0));
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
	//smierc
	protected new IEnumerator piorun(){
		isDead = true;
		yield return new WaitForSeconds(opoznienie);
		speed = 0;
		foreach(AnimationState state in animation){
			state.time=poczateksmierci;
		}
		animation.Play ();
		yield return new WaitForSeconds(opoznienieprzysmierci);
		FindObjectOfType<Player> ().ilpunktow += points;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
}
