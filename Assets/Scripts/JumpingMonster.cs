//Jumping Monster - inherit from Monster
using UnityEngine;
using System.Collections;

public class JumpingMonster : Monster {
	//Animation
	private bool isFlight=false;
	//Animation and Movement Control
	private float jumpStart=3.49f, dieStart=9.0f, jumpTime=0.0f, maxJumpTime=0.8f, constHalf=0.5f, ControllerHeight;
    private new void Start()
    {
        rotationAfterStrokeX = 90.0f;
        rotationAfterStrokeZ = 0.0f;
        ControllerHeight = controller.height;
    }
	private new void Update () {
		Wrapping ();
		Animation ();
		Move ();
	}
    //Jump Animation
	private void Animation(){
		//Animation
		if(!isFlight){
			if(controller.isGrounded&&!isDead&&
			   (!isHurt||(timeToRescue<hurtTime*warrning))) {
				foreach(AnimationState state in animation){
					state.time=jumpStart;
				}
				animation.Play ();
				jumpTime=maxJumpTime;
				isFlight=true;
			}
		}
		else {
			jumpTime-=Time.deltaTime;
			if(jumpTime<0.0f) isFlight=false;
		}
	}
    //Movement - jumping
	private new void Move(){
		if(!isHurt){
            //Controller Height change - other rotation (Monster should touching surface, so controller must be reduced)
            controller.height = ControllerHeight;
            //Movement after double stroke
			if(timeToRescue>0.0f){
				controller.Move(new Vector3(speed, jumpForce, 0));
				timeToRescue-=Time.deltaTime;
			}
			//normal Movement
			else {
				if(jumpTime>maxJumpTime*constHalf){
					controller.Move(new Vector3(speed,Mathf.Abs(speed), 0));
				}
				else {
					if(controller.isGrounded) controller.Move(new Vector3(minMove, -gravityForce, 0));
					else controller.Move(new Vector3(speed, -gravityForce, 0));
				}
			}
		}
		else {
            //Controller Height change - other rotation (Monster should touching surface, so controller must be reduced)
            controller.height = 0;
			timeToRescue-=Time.deltaTime;
            //Initial flight after stroke
			if(timeToRescue>hurtTime-flightTime){
				controller.Move(new Vector3(speed, jumpForce, 0));
			}
			//Movement when monster is hurt - allows collect monster
			else {
				controller.Move(new Vector3(minMove, -gravityForce, 0));
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
	//Death
	protected new IEnumerator Lightening(){
		isDead = true;
		yield return new WaitForSeconds(Delay);
		speed = 0;
		foreach(AnimationState state in animation){
			state.time=dieStart;
		}
		animation.Play ();
		yield return new WaitForSeconds(DieDelay);
		FindObjectOfType<Player> ().points += points;
		FindObjectOfType<MonsterFactory> ().remainingMonsters--;
		Destroy(gameObject);
	}
}
