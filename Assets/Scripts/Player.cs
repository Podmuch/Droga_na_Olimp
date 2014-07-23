//Player
using UnityEngine;

public class Player : MonoBehaviour {
    //lighteningCharges, points, lives
	public int lighteningCharges = 3,points=0, lives=3;
	//Player Controller
	public CharacterController controller;
	//Sounds
	public AudioSource jump, death;
	//points and lives display
	private TextMesh text=null;
	//Character Motor
	private CharacterMotor chMotor;
	//Maxiumum number Lightening Charges and points for money bonus collect
	private int maxLighteningCharges=3, bonusPoints=1000;
	//Time and Bonuses control
	private float flightTime=0.0f, maxFlightTime=1.0f,
				  invisibilityTime=0.0f, maxInvisibilityTime=3.0f,
				  accelerationTime=0.0f, slowdownTime=0.0f, maxBonusEffectTime=10.0f, BonusForce=2;
	private bool isTurnedLeft=false, isTurnedRight=false, isMoving=false, isInviolable=false, isDead=false;
	//Respawn, and base move directions
	private Vector3 Respawn=new Vector3(980,97,270),
					minMove=new Vector3(0.0001f, 0, 0),
                    flightAfterDie = new Vector3(0.1f, 0.1f, 0.0f); //Initial flight after die
	//Left and Right Rotation
	private Quaternion rightRotation=new Quaternion(0,0.7071068f,0,0.7071068f),
					   leftRotation=new Quaternion(0,0.7071068f,0,-0.7071068f);
    //map border ( need to wrapping)
    private float leftBorder = 966.5f, rightBorder = 993.5f;
	//disable control after Win
	public bool isWin=false;
    //This objects never destroyed
	void Awake () {
		DontDestroyOnLoad (this);
		chMotor=GetComponent<CharacterMotor> ();
	}
	//Text setting
	void setText(){
		foreach( GameObject go in FindObjectsOfType<GameObject> ()){
			if(go.tag=="Text"){
				text= go.GetComponent<TextMesh>();
				transform.position=Respawn;
				isWin=false;
			}
		}
	}
	// Update is called once per frame
	void Update () {
		if(FindObjectOfType<TouchButtonsController>()!=null){
			if(text==null) setText();
            //disable control after Win
			if(isWin) transform.position=Respawn;
			text.text="POINTS: "+points.ToString()+"     LIVES: "+lives.ToString();
			//minMove - allows collision detection
			controller.Move(minMove);
			Animations ();
			Wrapping ();
			FallingDown ();
			Inviolability ();
			BonusesExpiration ();
		}
	}
	//Animations
	void Animations(){
		if((Input.GetKeyDown("up")||TouchButtonsController.jump)&&controller.isGrounded) jump.Play();
		if(isTurnedRight||TouchButtonsController.right) transform.rotation= rightRotation;
		else isTurnedRight=Input.GetKeyDown("right");
		if(isTurnedLeft||TouchButtonsController.left) transform.rotation=leftRotation;
		else isTurnedLeft=Input.GetKeyDown("left");
		if(!isMoving){
			if(!isDead)animation.Play("walk");
			isMoving=true;
		}
		if(Input.GetKeyUp("right")) isTurnedRight=false;
		if(Input.GetKeyUp("left")) isTurnedLeft=false;
		if(!isTurnedLeft&&!isTurnedRight&&isMoving&&!TouchButtonsController.left&&!TouchButtonsController.right){
			isMoving=false;
			if(!isDead)animation.Play("idle");
		}
	}
	//Wrapping
	void Wrapping(){
		if (transform.position.x < leftBorder) transform.position=new Vector3(rightBorder, transform.position.y,transform.position.z);
		if (transform.position.x > rightBorder) transform.position=new Vector3(leftBorder, transform.position.y,transform.position.z);
	}
	//Death
	public void Death(){
		if(!isInviolable){
			lives--;
			//End Game text display
			if(lives<0) FindObjectOfType<MonsterFactory>().SendMessage("EndGame", false);
			flightTime=maxFlightTime;
			//Translate behind platforms - allows falling down animation
			transform.Translate(3.0f,0,0);
			isDead=true;
		}
	}
	//Falling down animation
	void FallingDown(){
		flightTime-=Time.deltaTime;
		if(flightTime>0.0f) {
			controller.Move(flightAfterDie);
			animation.Play("diehard");
		}
		if(transform.position.y<85&&lives>-1){
			transform.position = Respawn;
			animation.Play("idle");
			isInviolable=true;
			isDead=false;
			invisibilityTime=maxInvisibilityTime;
			if(isMoving) animation.Play("walk");
		}
	}
    //Inviolability when player spawns again
	void Inviolability(){
		if(isInviolable) {
			invisibilityTime-=Time.deltaTime;
			if(invisibilityTime<0.0f) {
				isInviolable=false;
				invisibilityTime=0.0f;
			}
			//r=b=g=0.588, a=1.0 natural color
            //change from black to a normal color in depending on the time
			float newColor=0.1f+(0.488f-(invisibilityTime*0.488f)/maxInvisibilityTime);
			foreach( Renderer r in gameObject.GetComponentsInChildren<Renderer>()){
                //change textures colors
				r.material.color = new Color(newColor,newColor,newColor,1.0f);
			}

		}
	}
    //Acceleration and SlowDown bonuses expiration
	void BonusesExpiration (){
		if(accelerationTime>0.0f) {
			accelerationTime-=Time.deltaTime;
			if(accelerationTime<0.0f) {
				chMotor.movement.maxForwardSpeed /= BonusForce;
				chMotor.movement.maxGroundAcceleration /= BonusForce;
				chMotor.movement.maxAirAcceleration /= BonusForce;
				chMotor.jumping.baseHeight /= BonusForce;
			}
		}
		if(slowdownTime>0.0f){
			slowdownTime-=Time.deltaTime;
			if(slowdownTime<0.0f) {
				chMotor.movement.maxForwardSpeed *= BonusForce;
				chMotor.movement.maxGroundAcceleration *= BonusForce;
				chMotor.movement.maxAirAcceleration *= BonusForce;
				chMotor.jumping.baseHeight *= BonusForce;
			}
		}
	}
	//Bonus Collecting
	public void OnBonusCollected(string bonusTag){
		switch (bonusTag) {
			case "LifeBonus":
				lives++;
				break;
			case "MoneyBonus": 
				points+=bonusPoints;
				break;
			case "LighteningBonus":
				if(lighteningCharges<maxLighteningCharges) lighteningCharges++;
                FindObjectOfType<LighteningButton>().SendMessage("OnLighteningChargesChanged");
				break;
			case "AccelerationBonus":
				if(accelerationTime<=0.0f){
					chMotor.movement.maxForwardSpeed *= BonusForce;
					chMotor.movement.maxGroundAcceleration *= BonusForce;
					chMotor.movement.maxAirAcceleration *= BonusForce;
					chMotor.jumping.baseHeight *= BonusForce;
					accelerationTime=maxBonusEffectTime;
				}
				break;
            case "SlowdownBonus":
				if(slowdownTime<=0.0f){
					chMotor.movement.maxForwardSpeed /= BonusForce;
					chMotor.movement.maxGroundAcceleration /= BonusForce;
					chMotor.movement.maxAirAcceleration /= BonusForce;
					chMotor.jumping.baseHeight /= BonusForce;
					slowdownTime=maxBonusEffectTime;
				}
				break;
		}
	}

	void OnTriggerEnter(Collider other) {
        if ((other.tag == "Monster" || other.tag == "Boss" || other.tag == "Spear") && !isInviolable)
        {
            death.Play();
            //When monsters, player die before (monster send massage)
            if(other.tag=="Spear")Death();
        }
	}
}
