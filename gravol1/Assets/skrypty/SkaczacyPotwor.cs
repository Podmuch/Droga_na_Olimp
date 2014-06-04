using UnityEngine;
using System.Collections;

public class SkaczacyPotwor : Potwor {
	//potrzebny do animacji
	private bool lot=false;
	//obsluga animacji i ruchu
	private float poczatekskoku=3.49f, poczateksmierci=9.0f, czasskoku=0.0f, maxczasskoku=0.8f, pol=0.5f, wysokosckontrolera;
    private new void Start()
    {
        obrotposmierciX = 90.0f;
        obrotposmierciZ = 0.0f;
        wysokosckontrolera = kontroler.height;
    }
	private new void Update () {
		zawijanie ();
		animacja ();
		ruch ();
	}
    //animacja skoku
	private void animacja(){
		//animacja
		if(!lot){
			if(kontroler.isGrounded&&!umiera&&
			   (!podbicie||(czasdowstania<czaspodbicia*momentostrzezenia))) {
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
	private new void ruch(){
		if(!podbicie){
            //zmiana wysokosci kontrolera przy powrocie do normalnego chodzenia
            kontroler.height = wysokosckontrolera;
			//ruch po powtórnym podbiciu
			if(czasdowstania>0.0f){
				kontroler.Move(new Vector3(predkosc, skok, 0));
				czasdowstania-=Time.deltaTime;
			}
			//normalny ruch
			else {
				if(czasskoku>maxczasskoku*pol){
					kontroler.Move(new Vector3(predkosc,Mathf.Abs(predkosc), 0));
				}
				else {
					if(kontroler.isGrounded) kontroler.Move(new Vector3(minimalnyruch, -grawitacja, 0));
					else kontroler.Move(new Vector3(predkosc, -grawitacja, 0));
				}
			}
		}
		else {
            //zmniejszenie kontrolera, żeby potwór nie wisiał w powietrzu tylko upadł na ziemie
            kontroler.height = 0;
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
	//smierc
	protected new IEnumerator piorun(){
		umiera = true;
		yield return new WaitForSeconds(opoznienie);
		predkosc = 0;
		foreach(AnimationState state in animation){
			state.time=poczateksmierci;
		}
		animation.Play ();
		yield return new WaitForSeconds(opoznienieprzysmierci);
		FindObjectOfType<Gracz> ().ilpunktow += punkty;
		FindObjectOfType<Fabryka> ().iloscpozostalychpotworow--;
		Destroy(gameObject);
	}
}
