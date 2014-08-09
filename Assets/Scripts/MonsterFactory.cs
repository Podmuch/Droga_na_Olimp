//Monster Factory control monster respawns and receive end game singal
//(end game signal are sending when all monsters is dead or player lose all lifes)
using UnityEngine;
using System.Collections;

public class MonsterFactory : MonoBehaviour {
    private struct MonsterFactoryParams {
        
        //Number of monsters and counter
        public int numberOfMonsters, counter;
        //The interval between spawn monsters and tmp Time counter
        public float spawnPeriod, delay;
    }
	//End Tekst
	public TextMesh text;
	//Monsters Counter - end game if remainingMonsters==0
	public int remainingMonsters;
    //Monsters
    public Transform[] monstersType;
    public int scene;
    private MonsterFactoryParams[] monsters;
	//new monster movement direction
	private bool left=true;
	//Translation text from background to foreground
	private float textTranslation=50, textDelay=2.0f;
	//Respawns
	private Vector3 leftRespawn=new Vector3(966.8765f, 110.5012f, 270),
					rightRespawn=new Vector3(993.912f, 95.5f, 270);
	void Start(){
        monsters = new MonsterFactoryParams[monstersType.Length];
        switch(scene)
        {
            case 1:
                monsters[0].numberOfMonsters = 10;
                monsters[1].numberOfMonsters = 5;
                monsters[0].spawnPeriod = 12;
                monsters[1].spawnPeriod = 16;
                monsters[0].delay = 2;
                monsters[1].delay = 30;
                break;
            case 2:
                monsters[0].numberOfMonsters = 10;
                monsters[1].numberOfMonsters = 10;
                monsters[0].spawnPeriod = 16;
                monsters[1].spawnPeriod = 16;
                monsters[0].delay = 5;
                monsters[1].delay = 12;
                break;
            case 3:
                monsters[0].numberOfMonsters = 5;
                monsters[1].numberOfMonsters = 10;
                monsters[2].numberOfMonsters = 5;
                monsters[0].spawnPeriod = 16;
                monsters[1].spawnPeriod = 16;
                monsters[2].spawnPeriod = 16;
                monsters[0].delay = 5;
                monsters[1].delay = 13;
                monsters[2].delay = 85;
                break;
            case 4:
                monsters[0].numberOfMonsters = 5;
                monsters[1].numberOfMonsters = 10;
                monsters[2].numberOfMonsters = 5;
                monsters[0].spawnPeriod = 16;
                monsters[1].spawnPeriod = 16;
                monsters[2].spawnPeriod = 16;
                monsters[0].delay = 5;
                monsters[1].delay = 85;
                monsters[2].delay = 13;
                break;
        }
		remainingMonsters = 0;
		for(int i=0;i<monsters.Length;i++) {
			monsters[i].counter=0;
            remainingMonsters += monsters[i].numberOfMonsters;
		}
		if(remainingMonsters==0) remainingMonsters=-1;
	}
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < monsters.Length; i++) {
			if(monsters[i].counter<monsters[i].numberOfMonsters) {
				monsters[i].delay-=Time.deltaTime;
				if(monsters[i].delay<0.0f){
					Transform newMonster=null;
					if(left)newMonster=(Transform)Instantiate(monstersType[i],leftRespawn , Quaternion.identity);
                    else newMonster = (Transform)Instantiate(monstersType[i], rightRespawn, Quaternion.identity);
					monsters[i].counter++;
					left=!left;
		    		//RotationY - parallel to platformonsters[i] orientation
					newMonster.Rotate(0,90,0,0);
					monsters[i].delay=monsters[i].spawnPeriod;
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
