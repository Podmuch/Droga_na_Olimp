//Monster Factory control monster respawns and receive end game singal
//(end game signal are sending when all monsters is dead or player lose all lifes)
using UnityEngine;
using System.Collections;

public class MonsterFactory : MonoBehaviour {
	//End Tekst
	public TextMesh text;
	//array of monster types
	public Transform[] monsters;
	//Number of monsters each type 
	public int[] numberOfMonsters;
	//Monsters Counter - end game if remainingMonsters==0
	public int remainingMonsters;
    //The interval between spawn monsters each type
	public float[] spawnTime, delay;
	//new monster movement direction
	private bool left=true;
	//Counter for each type
	private int[] counter;
	//Translation text from background to foreground
	private float textTranslation=50, textDelay=2.0f;
	//Respawns
	private Vector3 leftRespawn=new Vector3(966.8765f, 110.5012f, 270),
					rightRespawn=new Vector3(993.912f, 95.5f, 270);
	void Start(){
		counter = new int[monsters.Length];
		for(int i=0;i<monsters.Length;i++){
			counter[i]=0;
			remainingMonsters+=numberOfMonsters[i];
		}
		if(remainingMonsters==0) remainingMonsters=-1;
	}
	// Update is called once per frame
	void Update () {
		for(int i=0;i<monsters.Length;i++){
			delay[i]-=Time.deltaTime;
			if(delay[i]<0.0f){
				Transform newMonster=null;
				if(counter[i]<numberOfMonsters[i]) {
					if(left)newMonster=(Transform)Instantiate(monsters[i],leftRespawn , Quaternion.identity);
					else newMonster=(Transform)Instantiate(monsters[i], rightRespawn, Quaternion.identity);
					counter[i]++;
					left=!left;
                    //RotationY - parallel to platforms orientation
					newMonster.Rotate(0,90,0,0);
					delay[i]=spawnTime[i];
				}
			}
		}
		if (remainingMonsters == 0) {
			SendMessage ("EndGame", true);
			remainingMonsters--;
		}
	}
	public IEnumerator EndGame(bool isWin){
		yield return new WaitForSeconds(textDelay);
		text.transform.Translate(new Vector3(0,0,textTranslation));
		if(isWin) {
			text.text="You Win";
			FindObjectOfType<Player> ().isWin = true;
		}
		else text.text="You Lose";
	}
}
