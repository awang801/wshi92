using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnUnit : NetworkBehaviour
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
	GameObject mercy;

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
		mercy = (GameObject)Resources.Load ("Enemies/Mercy");

		unitToSpawn = "Potato";


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

	public void lateInit()
	{
		bank = sendPlayer.GetComponent<Bank> ();
	}

	public void Spawn (string unitName) //For Spawning specific units by name
	{
		if (!isServer) {
			return;
		}

		switch (unitName) {

		case "EnemyType1":
			spawnUnit = enemyType1;
			break;
		case "Potato":
			spawnUnit = potato;
			break;
		case "Cloud":
			spawnUnit = cloud;
			break;
		case "Reinhardt":
			spawnUnit = reinhardt;
			break;
		case "Mercy":
			spawnUnit = mercy;
			break;
		default:
			break;

		}

		GameObject newUnitObject = ((GameObject)(Instantiate (spawnUnit, transform.position, transform.rotation)));
		Unit newUnit = newUnitObject.GetComponent<Unit> ();
		newUnitObject.transform.SetParent (this.transform);
		newUnit.attackPlayer = attackPlayer;
		newUnit.sendPlayer = sendPlayer;

		NetworkServer.Spawn (newUnitObject);
		newUnit.RpcSetTarget (target.position);
		newUnit.RpcSetPlayer (attackPlayer.transform.name, sendPlayer.transform.name);

	}



		
}
