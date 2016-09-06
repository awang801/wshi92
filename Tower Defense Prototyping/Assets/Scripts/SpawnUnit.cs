using UnityEngine;
using System.Collections;

public class SpawnUnit : MonoBehaviour
{

    public GameObject spawnUnit;
	public Transform target;

	public GameObject attackPlayer;
	public GameObject sendPlayer;

	Bank bank;

	GameObject enemyType1;
	GameObject potato;
	GameObject cloud;
	GameObject reinhardt;

	public string unitToSpawn;

	public bool playTestAutoSend;
	public float spawnDelay;
	private float timeSinceSpawn;

	void Awake()
	{
		enemyType1 = (GameObject)Resources.Load ("Enemies/EnemyType1");
		potato = (GameObject)Resources.Load ("Enemies/Potato");
		cloud = (GameObject)Resources.Load ("Enemies/Cloud");
		reinhardt = (GameObject)Resources.Load ("Enemies/Reinhardt");

		unitToSpawn = "Potato";

		bank = sendPlayer.GetComponent<Bank> ();
	}

    // Update is called once per frame
    void Update()
    {
		if (playTestAutoSend == true) {
			timeSinceSpawn += Time.deltaTime;

			if (timeSinceSpawn >= spawnDelay)
			{
				Spawn(unitToSpawn);
				timeSinceSpawn = 0;
			}
		}
        
    }

	public void Spawn(string unitName) //For Spawning specific units by name
	{
		int unitCost = 0;
		int incomeGain = 0;

		switch (unitName) {

		case "EnemyType1":
			unitCost = 5;
			incomeGain = 1;
			spawnUnit = enemyType1;
			break;
		case "Potato":
			unitCost = 5;
			incomeGain = 1;
			spawnUnit = potato;
			break;
		case "Cloud":
			unitCost = 30;
			incomeGain = 8;
			spawnUnit = cloud;
			break;
		case "Reinhardt":
			unitCost = 30;
			incomeGain = 8;
			spawnUnit = reinhardt;
			break;
		default:
			break;

		}
			
		if (bank.getMoney() >= unitCost) {
			GameObject newUnitObject = ((GameObject)(Instantiate(spawnUnit, transform.position, transform.rotation)));
			Unit newUnit = newUnitObject.GetComponent<Unit> ();
			newUnitObject.transform.SetParent (this.transform);
			newUnit.setTarget (target);
			newUnit.attackPlayer = attackPlayer;
			newUnit.sendPlayer = sendPlayer;
			bank.subtractMoney (unitCost);
			bank.addIncome (incomeGain);
		} else {
			Debug.Log ("Not enough money to send!");
		}

	}
		
}
