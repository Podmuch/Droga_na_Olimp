//Base monster class(other monster classes inherit from this) -basic moving, death and colision detecting
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
	//isHurt -true if monster was stroken.
	//isDead -allows end of animation
	protected bool isHurt = false, isDead = false;
	//Time control (minMove- allow collect a monster, 
	//warrning - hurtTime*warrning is a moment when monster start moving a legs - He could be rescue if player don't collect him )
	protected float hurtTime=12.0f, minHurtTime=5.0f, deltaHurtTime=0.5f,
					timeToRescue=0.0f, RescueDelay=0.1f, Delay=0.1f, DieDelay=1.0f,
					flightTime=0.2f, maxFlightTime=1.0f, minMove=0.0001f, warrning=0.25f,
					//map border ( need to wrapping)
					wrappingBorderY=100, mapBorderY=80, height=15,
					leftBorder=966.5f ,rightBorder=993.5f, translation=0.2f,
                    //rotation after die - monster rotates on the back
                    rotationAfterStrokeX = 0.0f, rotationAfterStrokeZ = 180.0f;
	protected void Start () {
		animation.Play();
	}
	// Update is called once per frame
	protected void Update () {
		Wrapping ();
		Move ();
	}
	//Movement
	protected void Move(){
		if(!isHurt){
			//Movement after double stroke
			if(timeToRescue>0.0f){
                //podbicie w zaleznosci od polozenia klocka
                controller.Move(new Vector3(0, jumpForce, 0));
				timeToRescue-=Time.deltaTime;
			}
			//Normal movement
			else {
                //przy odbiciu predkosc musi byc mala, a nie ze zaczyna poruszac sie w poziomie jak przestanie sie wznosic
				controller.Move(new Vector3(speed, -gravityForce, 0));
                if (!animation.isPlaying && controller.isGrounded) animation.Play();
                else if (animation.isPlaying && !controller.isGrounded) animation.Stop();
			}
		}
		else {
			timeToRescue-=Time.deltaTime;
			//Initial flight after stroke
			if(timeToRescue>hurtTime-flightTime){
                //zmienna skok powinna byc usunieta, ruch po okregu odpowiednim (wedlug zmiennej predkosc)
               /* x' = x * cos(fi) – y * sin(fi)
                y' = x * sin(fi) + y * cos(fi)
					*/
                controller.Move(new Vector3(speed * Mathf.Cos(Mathf.PI * (hurtTime - timeToRescue) / (3.0f * flightTime)), jumpForce, 0));
			}
			//movement after stroke - gravity and allows collect monster
			else {
				if(controller.isGrounded)controller.Move(new Vector3(minMove, -gravityForce, 0));
                else controller.Move(new Vector3(speed * Mathf.Sin(Mathf.PI * (hurtTime-timeToRescue - flightTime)/ (2.0f/3.0f*hurtTime) + Mathf.PI/3.0f), -gravityForce, 0));
				if(timeToRescue<hurtTime*warrning){
					animation.Play();
				}
			}
			if(timeToRescue<0.0f){
				isHurt=false;
				timeToRescue=0.0f;
                //RotationX -  monster rotates to normal rotation
                transform.Rotate(-rotationAfterStrokeZ, 0, -rotationAfterStrokeX, 0);
			}
		}
		if(RescueDelay>0.0f) RescueDelay-=Time.deltaTime;
	}
	//Wrapping on the map borders
	protected void Wrapping(){
		if (transform.position.y < mapBorderY) {
			Death();
		}
		if (transform.position.y > wrappingBorderY) {
			if (transform.position.x <leftBorder)  {
				transform.position=new Vector3(rightBorder,transform.position.y,transform.position.z);
				RescueDelay=Delay;
			}
			else if(transform.position.x >rightBorder){
				transform.position=new Vector3(leftBorder,transform.position.y,transform.position.z);
				RescueDelay=Delay;
			}
		} else {
			if (transform.position.x < leftBorder || transform.position.x > rightBorder) {
				transform.Translate (0, height, 0);
				//RotationY and translate to spawn
				transform.Rotate (0, 180, 0);
				if (speed < maxSpeed&&speed>-maxSpeed) {
					if(speed>0)speed += deltaSpeed;
					else speed -=deltaSpeed;
				}
                if (flightTime < maxFlightTime) flightTime += 0.1f;
				if (hurtTime >minHurtTime) hurtTime -= deltaHurtTime;
				speed = -speed;
				RescueDelay=Delay;
			}
		}
	}
	//Collisions
	protected void OnTriggerEnter(Collider other) {
		switch (other.tag){
			case "Surface":
				if(!isHurt){
					if(other.bounds.center.y<transform.position.y) {
						isHurt=true;
						timeToRescue=hurtTime;
                        //RotationAfterStroke -  monster rotates on the back
                        transform.Rotate(rotationAfterStrokeZ, 0, rotationAfterStrokeX, 0);
						animation.Stop();
					}
				}
				else {
					if(timeToRescue<hurtTime-flightTime){
						isHurt=false;
						timeToRescue=flightTime;
                        ////RotationX -  monster rotates to normal rotation
                        transform.Rotate(-rotationAfterStrokeZ, 0, -rotationAfterStrokeX, 0);
					}
					else timeToRescue=hurtTime-flightTime;
				}
				break;
			case "Player":
				if(isHurt) Death();
				else if(!isDead)other.SendMessage("Death");
				break;
			case "Monster":
				if(RescueDelay<0.0f&&
			      ((speed>0&&other.bounds.center.x>transform.position.x)||
                    (speed < 0 && other.bounds.center.x < transform.position.x)))
                {
					speed = -speed;
					//RotationY - collision with other monster
					transform.Rotate (0, 180, 0);
					//Translate monsters 
					if(speed>0)transform.Translate(0,0,translation);
					else transform.Translate(0,0,translation);
				}
				break;
		}
	}
	//Death
	protected IEnumerator Lightening(){
		isDead = true;
		yield return new WaitForSeconds(Delay);
		speed = 0;
		animation.Play("die");
		yield return new WaitForSeconds(DieDelay);
		FindObjectOfType<Player> ().points += points;
		FindObjectOfType<MonsterFactory> ().remainingMonsters--;
		Destroy(gameObject);
	}
	protected void Death(){
		Instantiate(explosionAnimation, transform.position, Quaternion.identity);
		FindObjectOfType<Player> ().points += points;
		FindObjectOfType<MonsterFactory> ().remainingMonsters--;
		Destroy(gameObject);
	}
}
