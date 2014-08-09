//First Main Monster (Boss) - moving, death, control small monsters spawns
using UnityEngine;
using System.Collections;

public class FirstMainMonster : Monster {
	//Sounds
	public AudioSource jump, death, newMonster, fallingDown;
	//Lives
	public int lives;
	//Monsters
	public Transform[] monsters;
	public Vector3[] spawns;
	//Moving control
	private float movingTime=0.0f, maxMovingTime=20.0f, seeingRange=7.0f;
	//Data about Player
	private Player player;
	private Vector3 playerPosition;
	//Falling down and sounds control
	private bool isLeft=false, isFlight=true;
	private Quaternion rotation_Right=new Quaternion(0,0.7071068f,0,0.7071068f),
	rotation_Left=new Quaternion(0,0.7071068f,0,-0.7071068f);
	new void Start(){
		player = FindObjectOfType<Player> ();
		animation.Play();
		leftBorder = 966.5f;
		rightBorder=993.5f;
	}

	new void Update(){
		Wrapping ();
		Move ();
	}
	//Creating new monsters after stroke
	void CreateMonsters(){
		for(int i=0;i<spawns.Length;i++){
			Transform nowy=null;
			nowy=(Transform)Instantiate(monsters[i%monsters.Length],spawns[i] , Quaternion.identity);
			Instantiate(explosionAnimation, spawns[i], Quaternion.identity);
			//RotationY - parallel to platforms orientation
			nowy.Rotate(0,90,0,0);
		}
		newMonster.Play ();
	}
	//ruch
	new void Move(){
		playerPosition = player.transform.position;
		movingTime += Time.deltaTime;
		if(movingTime<maxMovingTime*0.5f){
			if(playerPosition.x-seeingRange>transform.position.x||
			   playerPosition.x+seeingRange<transform.position.x) 
			{
				if(playerPosition.x<transform.position.x) {
					if(!isLeft) {
						transform.rotation=rotation_Left;
						isLeft=true;
						speed=-speed;
					}
				}
				else {
					if(isLeft) {
						transform.rotation=rotation_Right;
						isLeft=false;
						speed=-speed;
					}
				}
			}
		}
		else {
			if(movingTime>maxMovingTime) movingTime=0;
		}
		if (movingTime > maxMovingTime*0.75f && movingTime < maxMovingTime*0.75f+flightTime) {
			if(controller.isGrounded) jump.Play();
			if(transform.position.z==270.0f) transform.Translate(2.0f,0,0);
			controller.Move(new Vector3(speed, jumpForce, 0));
			isFlight=true;
		}
		else {
			if(transform.position.z!=270.0f) transform.position=new Vector3(transform.position.x,transform.position.y,270);
			if(isFlight&&controller.isGrounded){
				isFlight=false;
				fallingDown.Play();
			}
			controller.Move(new Vector3(speed, -gravityForce, 0));
		}
	}
	//Wrapping
	new void Wrapping(){
		if(movingTime<maxMovingTime*0.5f)movingTime = maxMovingTime * 0.5f;
		if (transform.position.x < leftBorder) transform.position=new Vector3(rightBorder, transform.position.y,transform.position.z);
		if (transform.position.x > rightBorder) transform.position=new Vector3(leftBorder, transform.position.y,transform.position.z);
	}
	//Collisions
	new void OnTriggerEnter(Collider other) {
		switch (other.tag){
		case "Surface":
			if(other.bounds.center.y<transform.position.y&&!isFlight) {
				lives--;
				if(lives==0) 
					SendMessage("Death");
				else {
					CreateMonsters();
					movingTime=maxMovingTime*0.75f;
				}
			}
			break;
		case "Player":
			if(!isDead)other.SendMessage("Death");
			break;
		}
	}
	//Death
	new IEnumerator Lightening(){
		float tmp = speed;
		speed=0;
		animation.Play("resist");
		yield return new WaitForSeconds(DieDelay);
		speed = tmp;
		animation.Play ("walk");
	}
	//smierc
	new IEnumerator Death(){
		death.Play ();
		isDead = true;
		foreach(Monster p in FindObjectsOfType<Monster>()){
			if(p.tag!="Boss")p.SendMessage("Death");
		}
		speed = 0;
		movingTime = 0;
		animation.Play("die");
		yield return new WaitForSeconds(2*DieDelay);
		FindObjectOfType<Player> ().points += points;
		FindObjectOfType<MonsterFactory> ().SendMessage ("EndGame", true);
		Destroy(gameObject);
	}
}
