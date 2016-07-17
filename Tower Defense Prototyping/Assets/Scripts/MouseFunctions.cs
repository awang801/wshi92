using UnityEngine;
using System.Collections;

//This class manages mouse functions including selection on the grid, cursor hiding/showing
public class MouseFunctions : MonoBehaviour {


	public GameObject newStructure;


	Grid grid;
	BuildingFunctions bFunc;  

	float camRayLength = 200f;

	GameObject selHighlight;
	bool selecting;


	NavMeshAgent pathBlockChecker;
	NavMeshPath path;

	Node currentMousePosNode;

	Transform target; //Target of the enemies (need to change this in the future)

	int floorMask;

	bool buildWallMode;
	Node startWallModeNode;
	Node endWallModeNode;
	string buildWallModeXY;
	GameObject wallGhost;
    float wallsToBuild;

	string buildStructure;
	Vector3 positionToBuildStart;
	Vector3 positionToBuildEnd;

	void Awake()
	{
		bFunc = this.gameObject.GetComponent<BuildingFunctions> ();
		pathBlockChecker = GameObject.Find ("PathBlockCheck").GetComponent<NavMeshAgent> ();
		target = GameObject.Find ("Destination").transform;
		grid = GetComponent<Grid> ();
	}

	void Start () {
		floorMask = LayerMask.GetMask ("Terrain");
		path = new NavMeshPath ();
	}
	
	// Update is called once per frame
	void Update () {

		UpdateMouseNode ();

        if (Selecting == true) {
			MoveSelection ();
			CheckClick ();
		}

		if (buildWallMode) {
			MoveWallGhost ();
		}
			






	}

	//=====================================================
	//  PUBLIC FUNCTIONS HERE 
	//=====================================================

	public void HideMouse()
	{
		Cursor.visible = false;
	}

	public void ShowMouse()
	{
		Cursor.visible = true;
	}

	//=====================================================
	//  PRIVATE FUNCTIONS HERE 
	//=====================================================

	void UpdateMouseNode()
	{
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit tileHit;

		//Debug.DrawRay (Camera.main.transform.position, Input.mousePosition);
		if (Physics.Raycast (camRay, out tileHit, camRayLength, floorMask)) 
		{
			currentMousePosNode = grid.NodeFromWorldPoint (tileHit.point);
		}
	}

	void HandleBuildTower()
	{

		if (buildStructure == "Tower") {
			if (currentMousePosNode.Wall != null) {
				if (currentMousePosNode.Tower != null) {

					Debug.Log ("CANNOT BUILD TOWER, TOWER ALREADY EXISTS");

				} else {
					positionToBuildStart = currentMousePosNode.worldPosition + (Vector3.up * 1.25f);
					currentMousePosNode.Tower = ((GameObject)(Instantiate (Resources.Load (buildStructure), positionToBuildStart, Quaternion.identity)));
				}

			} else {
				Debug.Log ("CANNOT BUILD TOWER WITHOUT WALL");
			}
		}
	}

	/*
	  if (buildStructure == "Wall") {
			if (currentMousePosNode.hasWall == false) {
				positionToBuildStart = currentMousePosNode.worldPosition + (Vector3.up * 0.5f);
				newStructure = ((GameObject)(Instantiate (Resources.Load (buildStructure), positionToBuildStart, Quaternion.identity)));
				currentMousePosNode.hasWall = true;
			} else {
				Debug.Log ("CANNOT BUILD HERE, WALL ALREADY EXISTS");
			}

		} else 
	  */
	void CheckClick()
	{
		

		if (Input.GetButtonDown("Fire1")) {
					
			buildStructure = bFunc.TowerToBuild;
			positionToBuildStart = currentMousePosNode.worldPosition;
			if (buildStructure == "Wall") {
				startWallModeNode = currentMousePosNode;
				buildWallMode = true;
				wallGhost = ((GameObject)(Instantiate (Resources.Load ("WallGhost"))));
			} else {
				HandleBuildTower ();
			}
		}
		if (Input.GetButton ("Fire1")) 
		{
			if (buildWallMode == true) {
				
				Vector2 difference = grid.NodeDifference (startWallModeNode, currentMousePosNode);

				if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y)) {
					
					endWallModeNode = grid.NodeFromCoordinates (currentMousePosNode.gridX, startWallModeNode.gridY);
					//Debug.Log ("X ===" +endWallModeNode.worldPosition + "   Current Node = " + currentMousePosNode.worldPosition + "     StartWall = " + startWallModeNode.worldPosition);
					buildWallModeXY = "x";
                    wallsToBuild = difference.x;

				} else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x)) {

					endWallModeNode = grid.NodeFromCoordinates (startWallModeNode.gridX, currentMousePosNode.gridY);
					buildWallModeXY = "y";
                    wallsToBuild = difference.y;
					//Debug.Log ("Y ===" +endWallModeNode.worldPosition + "   Current Node = " + currentMousePosNode.worldPosition + "     StartWall = " + startWallModeNode.worldPosition);
	
				}
			}
		}

		if (Input.GetButtonUp("Fire1")) {
			
			buildWallMode = false;

			if (grid.NodeLineContainsWall (startWallModeNode, endWallModeNode, buildWallModeXY) == false) {
				Debug.Log ("buildWallModeXY = " + buildWallModeXY);
                Node tempStart = startWallModeNode;
				tempStart.Wall = ((GameObject)(Instantiate (Resources.Load (buildStructure), startWallModeNode.worldPosition, Quaternion.identity)));
                for (int i = 0; i<Mathf.Abs(wallsToBuild); i++)
                {
                    if (buildWallModeXY == "x")
                    {
                        if (wallsToBuild > 0)
                        {
                            tempStart = grid.NodeFromCoordinates(tempStart.gridX + 1, tempStart.gridY);
                        }
                        else
                        {
                            tempStart = grid.NodeFromCoordinates(tempStart.gridX - 1, tempStart.gridY);
                        }
                    }
                    else
                    {
                        if (wallsToBuild > 0)
                        {
                            tempStart = grid.NodeFromCoordinates(tempStart.gridX, tempStart.gridY + 1);
                        }
                        else
                        {
                            tempStart = grid.NodeFromCoordinates(tempStart.gridX, tempStart.gridY - 1);
                        }
                    }
                    tempStart.Wall = ((GameObject)(Instantiate(Resources.Load(buildStructure), tempStart.worldPosition, Quaternion.identity)));
                }
				//newStructure.transform.localScale = wallGhost.transform.lossyScale;
				//grid.setNodesAlongAxis (startWallModeNode, endWallModeNode, buildWallModeXY, true);


			} else {
				Debug.Log ("CANNOT PLACE, WALL IN LINE");
			}

			Destroy (wallGhost);


		}

	}

	void HandleWallBuild()
	{
		if (buildWallMode) {

		}
	}
		

	void MoveSelection()
	{
		if (SelHighlight != null && currentMousePosNode.worldPosition != null) {
			if (currentMousePosNode.Wall != null) {
				SelHighlight.transform.position = currentMousePosNode.worldPosition + (Vector3.up * 1.1f);
			} else {
				SelHighlight.transform.position = currentMousePosNode.worldPosition + (Vector3.up * 0.1f);
			}
		}
	}

	void MoveWallGhost()
	{
		if (wallGhost != null && endWallModeNode != null) {
			

			wallGhost.transform.position = grid.CenterOfTwoNodes (startWallModeNode, endWallModeNode);


			//Debug.Log (wallGhost.transform.position + "       SW " + startWallModeNode.worldPosition + "       EW " + endWallModeNode.worldPosition);

			if (buildWallModeXY == "x") {
				wallGhost.transform.localScale = new Vector3 (grid.DistanceBetweenTwoNodesX (startWallModeNode, endWallModeNode) + 1, 1, 1);
			} else {
				wallGhost.transform.localScale = new Vector3(1, 1, grid.DistanceBetweenTwoNodesY (startWallModeNode, endWallModeNode) + 1);
			}
		}
	}

	//=====================================================
	//  PROPERTIES
	//=====================================================
	public GameObject SelHighlight
	{
		get {
			return selHighlight;
		}
		set{
			selHighlight = value;
		}
	}

	public bool Selecting
	{
		get{
			return selecting;
			}
		set{
			if (value == true)
			{
				HideMouse();
			}
			else
			{
				ShowMouse();
			}
			selecting = value;
		}
	}


}
