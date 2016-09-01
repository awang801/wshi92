using UnityEngine;
using System.Collections;

public class BuildHandler : MonoBehaviour {

	//References
	//====================================================
	public GameObject player;
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


	//Audio
	AudioSource sourceSFX;
	AudioClip BuildFX;
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
		bank = player.GetComponent<Bank>();

		//Pre-load Objects
		//============================================================
		//Towers
		wall2Way = (GameObject)(Resources.Load ("Walls/Wall2Way"));
		wall3Way = (GameObject)(Resources.Load ("Walls/Wall3Way"));
		wall4Way = (GameObject)(Resources.Load ("Walls/Wall4Way"));
		wallEnd = (GameObject)(Resources.Load ("Walls/WallEnd"));
		wallCorner = (GameObject)(Resources.Load ("Walls/WallCorner"));
		wallSolo = (GameObject)(Resources.Load ("Walls/WallSolo"));

		wallFloorOpen = (GameObject)(Resources.Load ("Walls/WallDoorOpen"));
		wallFloorClose = (GameObject)(Resources.Load ("Walls/WallDoorClose"));

		//Walls
		orbTower = (GameObject)(Resources.Load ("Towers/BasicOrbTower"));
		cannonTower = (GameObject)(Resources.Load ("Towers/CannonTower"));

		//Audio References
		sourceSFX = GetComponent<AudioSource> ();

		BuildFX = (AudioClip)(Resources.Load("Sounds/BuildingPlacement", typeof(AudioClip)));
		needMoneySound  = (AudioClip)(Resources.Load("Sounds/needMoney", typeof(AudioClip)));
		cannotBuildSound  = (AudioClip)(Resources.Load("Sounds/CannotBuild", typeof(AudioClip)));
		selectSound = Resources.Load<AudioClip> ("Sounds/CarDoorClose");


	}

	public void SellSelection()
	{
		int sellValue = 0;

		if (mf.CurrentObject != null) {

			if (mf.CurrentObject.CompareTag ("TowerSelector")) {
				
				Tower tempTower = mf.CurrentObject.transform.parent.GetComponent<Tower> ();
				sellValue = int.Parse (tempTower.Stats [5]);
				bank.addMoney (sellValue);
				Destroy (mf.CurrentObject.transform.parent.gameObject);
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

				switch (kf.ObjectToBuild) {

				case "OrbTower":

					if (bank.getMoney() - 20 < 0)
					{
						Debug.Log("Not enough Money");
						sourceSFX.PlayOneShot(needMoneySound);
					}
					else
					{
						bank.addMoney(-20);
						sourceSFX.PlayOneShot(BuildFX);
						positionToBuildStart = buildNode.worldPosition + (Vector3.up * 1f);


						StartCoroutine(BuildAfterTime (0.5f, orbTower, buildNode));
						CreateWallFloorAnimation (true);


					}



					break;

				case "CannonTower":

					if (bank.getMoney () - 40 < 0)
					{
						Debug.Log ("Not enough Money");
						sourceSFX.PlayOneShot(needMoneySound);
					}
					else {

						bank.addMoney (-40);
						sourceSFX.PlayOneShot (BuildFX);
						positionToBuildStart = buildNode.worldPosition + (Vector3.up * 1f);

						StartCoroutine(BuildAfterTime (0.5f, cannonTower, buildNode));
						CreateWallFloorAnimation (true);

					}

					break;

				default:

					break;
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

	void CreateWallFloorAnimation(bool isOpening)
	{
		if (isOpening) {
			Instantiate (wallFloorOpen, positionToBuildStart - Vector3.up, wallFloorOpen.transform.rotation);
		} else {
			Instantiate (wallFloorClose, positionToBuildStart - Vector3.up, wallFloorOpen.transform.rotation);
		}
	}

	IEnumerator BuildAfterTime(float time, GameObject towerToBuild, Node nodeToBuildOn)
	{		
		
		yield return new WaitForSeconds (time);

		nodeToBuildOn.Tower = ((GameObject)(Instantiate(towerToBuild, positionToBuildStart - Vector3.up, Quaternion.identity)));

		Tower tempTower = nodeToBuildOn.Tower.GetComponent<Tower> ();

		tempTower.startTargetAnimationPoint (positionToBuildStart);
		tempTower.node = nodeToBuildOn;


	}

}
