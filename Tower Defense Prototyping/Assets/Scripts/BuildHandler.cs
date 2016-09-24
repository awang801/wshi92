using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BuildHandler : NetworkBehaviour {

	//References
	//====================================================
	MouseFunctions mf;
	KeyboardFunctions kf;
	Bank bank;

	//Pre-Loaded Objects
	//====================================================
	//Walls
	GameObject wall2Way;
	GameObject wall3Way;
	GameObject wall4Way;
	GameObject wallEnd;
	GameObject wallCorner;
	GameObject wallSolo;

	GameObject wallFloorOpen;
	GameObject wallFloorClose;

	//Towers
	GameObject orbTower;
	GameObject cannonTower;
	GameObject laserTower;
	GameObject iceTower;
	GameObject lightTower;
	GameObject magicTower;

	//Audio
	AudioSource sourceSFX;
	AudioClip needMoneySound;
	AudioClip cannotBuildSound;
	AudioClip selectSound;

	//Building
	//==============================================
	bool buildWallMode;
	Node startWallModeNode;
	Node endWallModeNode;
	string buildWallModeXY;
	GameObject wallGhost;
	float wallsToBuild;
	public bool building;

	string buildStructure;
	Vector3 positionToBuildStart;
	Vector3 positionToBuildEnd;

	void Awake()
	{
		//Set References
		//============================================================
		kf = GetComponent<KeyboardFunctions> ();
		mf = GetComponent<MouseFunctions> ();
		bank = GetComponent<Bank>();

		//Pre-load Objects
		//============================================================
		//Walls
		wall2Way = (GameObject)(Resources.Load ("Walls/Wall2Way"));
		wall3Way = (GameObject)(Resources.Load ("Walls/Wall3Way"));
		wall4Way = (GameObject)(Resources.Load ("Walls/Wall4Way"));
		wallEnd = (GameObject)(Resources.Load ("Walls/WallEnd"));
		wallCorner = (GameObject)(Resources.Load ("Walls/WallCorner"));
		wallSolo = (GameObject)(Resources.Load ("Walls/WallSolo"));

		wallFloorOpen = (GameObject)(Resources.Load ("Walls/WallDoorOpen"));
		wallFloorClose = (GameObject)(Resources.Load ("Walls/WallDoorClose"));

		//Towers
		orbTower = (GameObject)(Resources.Load ("Towers/BasicOrbTower"));
		cannonTower = (GameObject)(Resources.Load ("Towers/CannonTower"));
		laserTower = (GameObject)(Resources.Load ("Towers/LaserTower"));
		iceTower = (GameObject)(Resources.Load ("Towers/IceTower"));
		lightTower = (GameObject)(Resources.Load ("Towers/LightTower"));
		magicTower = (GameObject)(Resources.Load ("Towers/MagicTower"));

		//Audio References
		sourceSFX = GetComponent<AudioSource> ();

		needMoneySound  = (AudioClip)(Resources.Load("Sounds/needMoney", typeof(AudioClip)));
		cannotBuildSound  = (AudioClip)(Resources.Load("Sounds/CannotBuild", typeof(AudioClip)));
		selectSound = Resources.Load<AudioClip> ("Sounds/CarDoorClose");


	}

	public void SellSelection()
	{
		int sellValue = 0;

		if (mf.CurrentObject != null) {

			if (mf.CurrentObject.CompareTag ("TowerSelector")) {
				
				Tower tempTower = mf.CurrentObject.transform.parent.parent.GetComponent<Tower> ();
				sellValue = int.Parse (tempTower.Stats [5]);
				bank.addMoney (sellValue);
				Destroy (mf.CurrentObject.transform.parent.parent.gameObject);
				mf.HidePanel();

			} else if (mf.CurrentObject.CompareTag ("Wall")) {
				
				Wall tempWall = mf.CurrentObject.GetComponent<Wall> ();

				if (tempWall.node.Tower == null) {
					
					sellValue = 2;
					bank.addMoney (sellValue);
					Destroy (mf.CurrentObject);
					mf.HidePanel ();

				} else {
					Debug.Log ("CANNOT SELL WALL WITH TOWER ON IT");
				}

			}

		} else {
			Debug.Log ("Nothing to sell");
		}
	}


	public void HandleBuildTower(Node buildNode) 
	//Checks if it is possible to build a tower in current node
	//If it IS possible, handle the correct type of tower
	{


		if (buildNode.Wall == null) {

			//Play Cannot Build Sound
			Debug.Log("CANNOT BUILD TOWER WITHOUT WALL");
			sourceSFX.PlayOneShot(cannotBuildSound);
		} 

		else {

			if (buildNode.Tower != null) {

				//Play Cannot Build Sound
				Debug.Log("CANNOT BUILD TOWER, TOWER ALREADY EXISTS");
				sourceSFX.PlayOneShot(cannotBuildSound);

			} else {

				int moneyCost = 0;

				switch (kf.ObjectToBuild) {

				case "Orb":

					moneyCost = 20;

					break;

				case "Cannon":

					moneyCost = 40;

					break;

				case "Laser":

					moneyCost = 50;

					break;

				case "Ice":

					moneyCost = 50;

					break;

				case "Light":

					moneyCost = 80;

					break;

				case "Magic":

					moneyCost = 50;

					break;

				default:

					break;
				}

				if (bank.getMoney () - moneyCost < 0)
				{
					Debug.Log ("Not enough Money");
					sourceSFX.PlayOneShot(needMoneySound);
				}
				else {

					bank.addMoney (-moneyCost);
					positionToBuildStart = buildNode.worldPosition + (Vector3.up * 1f);

					CmdBuildTower (1.5f, kf.ObjectToBuild, buildNode.gridX, buildNode.gridY, positionToBuildStart);
					CmdCreateWallFloorAnimation (true, positionToBuildStart);

				}


			}

		}

	}


	public GameObject wallType(Node n, Node[] neighbors)
	{
		//Returns the correct wall type, location, and rotation, for MODULAR WALL BUILDING
		GameObject myWall;

		int numberOfNeighbors = 0; //numberOfNeighbors of Neighbors
		int sum = 0; //See below in the for loop
		int nullIndex = 0; //Stores the location of the null neighbor (starts at 0) - - ONLY USED FOR 3 NEIGHBOR CASE

		int[] nonNullIndexes = new int[4]; //Stores the indexes of the neighbors

		neighbors = mf.TheGrid.GetNeighbors (n); //Gets neighbors in an array

		//Gets neighbor indexes, and other information for calculating the necessary wall
		for (int i = 0; i < 4; i++) {
			if (neighbors [i] != null) {
				if (neighbors [i].Wall != null) {
					nonNullIndexes[numberOfNeighbors] = i;
					numberOfNeighbors++;
					sum += (i + 1); //A numbering system for neighbors (1 = North, 2 = East, 3 = South, 4 = West) - - ONLY USED FOR 2 NEIGHBOR CASE
				} else {
					nullIndex = i;
				}
			}

		}

		switch (numberOfNeighbors) {
		case 0: // No neighbors case

			myWall = (GameObject)Instantiate(wallSolo); //Use solo wall

			break;
		case 1: //1 Neighbor case

			myWall = (GameObject)Instantiate(wallEnd); //Use end wall piece

			myWall.transform.Rotate(0, 90f * (nonNullIndexes[0] - 1), 0); //Gives correct rotation relative to the one existing neighbor

			break;

		case 2: //2 Neighbor case
			if (sum % 2 == 0) { //Even means neighbors are across from each other, straight connector

				myWall = (GameObject)Instantiate(wall2Way);

				//wall2Way is vertical by default, so if our first neighbor (index 0 of nonNullIndexes) is 1 (East node), then rotate
				if (nonNullIndexes [0] == 1) {
					myWall.transform.Rotate(0, 90f, 0); //Rotate Horizontal
				} 

			} else { //Odd means neighbors are next to each other, use corner piece

				myWall = (GameObject)Instantiate(wallCorner);

				switch (nonNullIndexes [0]) {

				case 0:
					if (nonNullIndexes [1] == 1) {
						myWall.transform.Rotate(0, 180f, 0);
					} else {
						myWall.transform.Rotate(0, 90f, 0);
					}
					break;
				case 1:
					myWall.transform.Rotate(0, 270f, 0);
					break;
				case 2:
					//No rotation needed
					break;
				default:					
					break;
				}
			}
			break;

		case 3: //3 Neighbor case

			myWall = (GameObject)Instantiate(wall3Way);
			switch (nullIndex) { //If 3 neighbors, find the NULL neighbor, and rotate appropriately
			case 0:
				myWall.transform.Rotate(0, 270f, 0);
				break;
			case 1:
				//No rotation needed
				break;
			case 2:
				myWall.transform.Rotate(0, 90f, 0);
				break;
			case 3:
				myWall.transform.Rotate(0, 180f, 0);
				break;
			}
			break;

		case 4://4 Neighbor case

			myWall = (GameObject)Instantiate(wall4Way);
			break;

		default:
			myWall = (GameObject)Instantiate(wallSolo); //Default so unity will stop yelling at me
			break;
		}

		myWall.transform.position = n.worldPosition; //Set correct position for new wall


		return myWall;

	}

	[Command]
	void CmdCreateWallFloorAnimation(bool isOpening, Vector3 position)
	{
		if (isOpening) {
			GameObject floorOpen = (GameObject) Instantiate (wallFloorOpen, position - Vector3.up, wallFloorOpen.transform.rotation);
			NetworkServer.Spawn(floorOpen);
		} else {
			GameObject floorClose = (GameObject)Instantiate (wallFloorClose, position - Vector3.up, wallFloorOpen.transform.rotation);
			NetworkServer.Spawn(floorClose);
		}
	}

	[Command]
	void CmdBuildTower(float time, string towerToBuild, int _nodeX, int _nodeY, Vector3 positionToBuild)
	{
		GameObject buildThisTower;
		switch (towerToBuild) {

		case "Orb":
			buildThisTower = ((GameObject)(Instantiate(orbTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;
		case "Cannon":
			buildThisTower = ((GameObject)(Instantiate(cannonTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;
		case "Laser":
			buildThisTower = ((GameObject)(Instantiate(laserTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;
		case "Ice":
			buildThisTower = ((GameObject)(Instantiate(iceTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;
		case "Light":
			buildThisTower = ((GameObject)(Instantiate(lightTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;
		case "Magic":
			buildThisTower = ((GameObject)(Instantiate(magicTower, positionToBuild - Vector3.up*2f, Quaternion.identity)));
			break;

		default: 
			buildThisTower = null;
			break;
		}

		Node myNode = mf.TheGrid.NodeFromCoordinates (_nodeX, _nodeY);

		myNode.Tower = buildThisTower;
		NetworkServer.Spawn(buildThisTower);

		Tower tempTower = buildThisTower.GetComponent<Tower> ();

		tempTower.node = myNode;

		StartCoroutine(startTargetAnimationPoint (tempTower, positionToBuild, time));

		mf.RpcSyncNode (_nodeX, _nodeY, 0, myNode.Tower);

	}


	IEnumerator startTargetAnimationPoint(Tower tower, Vector3 moveTo, float _timedelay)
	{
		yield return new WaitForSeconds (_timedelay);

		StartCoroutine (buildAnimation(tower, moveTo));
	}

	IEnumerator buildAnimation(Tower tower, Vector3 buildAt)
	{
		
		while (Vector3.Distance (tower.transform.position, buildAt) > 0.05) {

			tower.transform.position = Vector3.Lerp (tower.transform.position, buildAt, 4 * Time.deltaTime);

			yield return new WaitForSeconds (Time.deltaTime);
		}

		tower.transform.position = buildAt;
		tower.isBeingBuilt = false;
	}

}
