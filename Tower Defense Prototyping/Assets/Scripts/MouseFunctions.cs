using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//This class manages mouse functions including selection on the grid, cursor hiding/showing
public class MouseFunctions : MonoBehaviour
{

	//References
	//=============================================
	public GameObject player;
	KeyboardFunctions kFunc;
	Bank bank;

	//Selection
	//=============================================
	float camRayLength = 200f;
	Grid grid;
	bool buildSelecting;
	Node currentMouseNode;

	//Building
	//==============================================
	bool buildWallMode;
	Node startWallModeNode;
	Node endWallModeNode;
	string buildWallModeXY;
	GameObject wallGhost;
	float wallsToBuild;
    bool building;

	string buildStructure;
	Vector3 positionToBuildStart;
	Vector3 positionToBuildEnd;

	//Audio
	//==============================================
    AudioClip BuildFX;
    AudioSource sourceSFX;
    public GameObject newStructure;


	//Preloaded Objects
	//===============================================
	GameObject wall2Way;
	GameObject wall3Way;
	GameObject wall4Way;
	GameObject wallEnd;
	GameObject wallCorner;
	GameObject wallSolo;

	GameObject wallGhostLoaded;

	GameObject selHighlight;

	GameObject orbTower;
	GameObject cannonTower;

    NavMeshPath path;

	//Cursor
	//===============================================
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	//Other..?
	//===============================================
    Transform target; //Target of the enemies (need to change this in the future)
    int floorMask;





    void Awake()
    {
		kFunc = this.gameObject.GetComponent<KeyboardFunctions>();
        target = GameObject.Find("Destination 1").transform;
        grid = GetComponent<Grid>();

        bank = player.GetComponent<Bank>();


		wall2Way = (GameObject)(Resources.Load ("Walls/Wall2Way"));
		wall3Way = (GameObject)(Resources.Load ("Walls/Wall3Way"));
		wall4Way = (GameObject)(Resources.Load ("Walls/Wall4Way"));
		wallEnd = (GameObject)(Resources.Load ("Walls/WallEnd"));
		wallCorner = (GameObject)(Resources.Load ("Walls/WallCorner"));
		wallSolo = (GameObject)(Resources.Load ("Walls/WallSolo"));

		wallGhostLoaded = (GameObject)(Resources.Load ("Walls/WallGhost"));

		orbTower = (GameObject)(Resources.Load ("Towers/BasicOrbTower"));
		cannonTower = (GameObject)(Resources.Load ("Towers/CannonTower"));

        BuildFX = (AudioClip)(Resources.Load("Sounds/BuildingPlacement", typeof(AudioClip)));
        sourceSFX = this.gameObject.GetComponent<AudioSource>();

    }


    void Start()
    {
        floorMask = LayerMask.GetMask("Terrain");
        path = new NavMeshPath();
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

    }

    // Update is called once per frame
    void Update()
    {

        UpdateMouseNode();

        if (buildSelecting == true)
        {
			MoveBuildSelection();
            CheckBuildClick();
        }

        if (buildWallMode)
        {
            MoveWallGhost();
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
    /*
	 * Updates the node that the mouse is currently over
	 * 
	 * Saves information in "currentMouseNode"
	 */
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit nodeHit;

        //Debug.DrawRay (Camera.main.transform.position, Input.mousePosition);
        if (Physics.Raycast(camRay, out nodeHit, camRayLength, floorMask))
        {
            currentMouseNode = grid.NodeFromWorldPoint(nodeHit.point);
        }
    }

    void HandleBuildTower()
    {

        if (buildStructure == "OrbTower")
        {
            if (currentMouseNode.Wall != null)
            {
                if (currentMouseNode.Tower != null)
                {

                    Debug.Log("CANNOT BUILD TOWER, TOWER ALREADY EXISTS");

                }
                else if (bank.getMoney() - 20 < 0)
                {
                    Debug.Log("Not enough Money");
                }
                else
                {
                    bank.addMoney(-20);
                    sourceSFX.PlayOneShot(BuildFX);
                    positionToBuildStart = currentMouseNode.worldPosition + (Vector3.up * 1f);
                    currentMouseNode.Tower = ((GameObject)(Instantiate(orbTower, positionToBuildStart, Quaternion.identity)));
                }

            }
            else
            {
                Debug.Log("CANNOT BUILD TOWER WITHOUT WALL");
            }
        }
		else if (buildStructure == "CannonTower")
		{
			if (currentMouseNode.Wall != null)
			{
				if (currentMouseNode.Tower != null)
				{

					Debug.Log("CANNOT BUILD TOWER, TOWER ALREADY EXISTS");

				}
				else if (bank.getMoney() - 40 < 0)
				{
					Debug.Log("Not enough Money");
				}
				else
				{
					bank.addMoney(-40);
					sourceSFX.PlayOneShot(BuildFX);
					positionToBuildStart = currentMouseNode.worldPosition + (Vector3.up * 1f);
					currentMouseNode.Tower = ((GameObject)(Instantiate(cannonTower, positionToBuildStart, Quaternion.identity)));
				}

			}
			else
			{
				Debug.Log("CANNOT BUILD TOWER WITHOUT WALL");
			}
		}
    }

    /*
	  if (buildStructure == "Wall") {
			if (currentMouseNode.hasWall == false) {
				positionToBuildStart = currentMouseNode.worldPosition + (Vector3.up * 0.5f);
				newStructure = ((GameObject)(Instantiate (Resources.Load (buildStructure), positionToBuildStart, Quaternion.identity)));
				currentMouseNode.hasWall = true;
			} else {
				Debug.Log ("CANNOT BUILD HERE, WALL ALREADY EXISTS");
			}

		} else 
	  */
    void CheckBuildClick()
    {

        if (Input.GetButtonDown("Fire1"))
        {

            buildStructure = kFunc.TowerToBuild;
            positionToBuildStart = currentMouseNode.worldPosition;
            if (buildStructure == "Wall")
            {
                startWallModeNode = currentMouseNode; 
                buildWallMode = true;
				wallGhost = ((GameObject)(Instantiate(wallGhostLoaded)));
            }
            else
            {
                HandleBuildTower();
            }
            building = true;
        }
        if (Input.GetButton("Fire1"))
        {
            if (buildWallMode == true)
            {

                Vector2 difference = grid.NodeDifference(startWallModeNode, currentMouseNode);

                if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y))
                {

                    endWallModeNode = grid.NodeFromCoordinates(currentMouseNode.gridX, startWallModeNode.gridY);
                    //Debug.Log ("X ===" +endWallModeNode.worldPosition + "   Current Node = " + currentMouseNode.worldPosition + "     StartWall = " + startWallModeNode.worldPosition);
                    buildWallModeXY = "x";
                    wallsToBuild = difference.x;

                }
                else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
                {

                    endWallModeNode = grid.NodeFromCoordinates(startWallModeNode.gridX, currentMouseNode.gridY);
                    buildWallModeXY = "y";
                    wallsToBuild = difference.y;
                    //Debug.Log ("Y ===" +endWallModeNode.worldPosition + "   Current Node = " + currentMouseNode.worldPosition + "     StartWall = " + startWallModeNode.worldPosition);

                }
            }
        }

        if (Input.GetButtonUp("Fire1") && building == true)
        {

			List<Node> alreadyChecked = new List<Node> ();
			List<Node> toCheck = new List<Node> ();

            buildWallMode = false;
			if (bank.getMoney() >= ((Mathf.Abs(wallsToBuild) + 1) * 5 ))
            {
                if (grid.NodeLineContainsWall(startWallModeNode, endWallModeNode, buildWallModeXY) == false)
                {
					
                    Debug.Log("buildWallModeXY = " + buildWallModeXY);

					Node currentNode = startWallModeNode;
					Node nextNode = currentNode;
					Node[] neighbors;
					GameObject dummyWall = new GameObject();

					GameObject newWall;

                   
                    sourceSFX.PlayOneShot(BuildFX);

                    for (int i = 0; i <= Mathf.Abs(wallsToBuild); i++)
                    {
						neighbors = grid.GetNeighbors (currentNode);


						if (i != Mathf.Abs (wallsToBuild)) { //If statement so we don't try to go OVER the grid boundaries on last iteration

							if (buildWallModeXY == "x") {
								
								if (wallsToBuild > 0) {

									neighbors [1].Wall = dummyWall;
									nextNode = grid.NodeFromCoordinates (currentNode.gridX + 1, currentNode.gridY);
								} else {
									neighbors [3].Wall = dummyWall;
									nextNode = grid.NodeFromCoordinates (currentNode.gridX - 1, currentNode.gridY);
								}

							} else {
								
								if (wallsToBuild > 0) {
									neighbors [0].Wall = dummyWall;
									nextNode = grid.NodeFromCoordinates (currentNode.gridX, currentNode.gridY + 1);
								} else {
									neighbors [2].Wall = dummyWall;
									nextNode = grid.NodeFromCoordinates (currentNode.gridX, currentNode.gridY - 1);
								}

							}

						} 

						newWall = wallType (currentNode, neighbors);
						currentNode.Wall = newWall;
						bank.addMoney(-5);

						alreadyChecked.Add (currentNode);

						for(int j = 0; j < 4; j++)
						{
							if (neighbors [j] != null) {
								if (!alreadyChecked.Contains (neighbors [j]) && neighbors [j].Wall != null) {
									toCheck.Add (neighbors [j]);
								}
							}
						}

						currentNode = nextNode;

                    } // END FOR

					foreach (var node in toCheck) {
						neighbors = grid.GetNeighbors (node);
						Destroy (node.Wall);
						newWall = wallType (node, neighbors);
						node.Wall = newWall;

					}

                    Debug.Log(bank.getMoney());
					Destroy (dummyWall);

                }
                else
                {
                    Debug.Log("CANNOT PLACE, WALL IN LINE");
                }
            }
            else
            {
                Debug.Log("Not enough Money");
            }

            Destroy(wallGhost);


        }
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Fire2"))
        {
            for (int i = 0; i <= Mathf.Abs(wallsToBuild); i++)
            {
                Destroy(wallGhost);
            }
            building = false;
        }

    }

	GameObject wallType(Node n, Node[] neighbors)
	{
		//Returns the correct wall type, location, and rotation
		GameObject myWall;

		int number = 0;
		int sum = 0;
		int nullIndex = 0;

		int[] nonNullIndexes = new int[4];

		neighbors = grid.GetNeighbors (n);

		for (int i = 0; i < 4; i++) {
			if (neighbors [i] != null) {
				if (neighbors [i].Wall != null) {
					nonNullIndexes[number] = i;
					number++;
					sum += (i + 1);
				} else {
					nullIndex = i;
				}
			}

		}

		switch (number) {
		case 0:

			myWall = (GameObject)Instantiate(wallSolo);
			break;
		case 1:

			myWall = (GameObject)Instantiate(wallEnd);

			myWall.transform.Rotate(0, 90f * (nonNullIndexes[0] - 1), 0);

			break;

		case 2:
			if (sum % 2 == 0) { //Even means straight connector
				
				myWall = (GameObject)Instantiate(wall2Way);

				//wall2Way is vertical by default
				if (nonNullIndexes [0] == 1) {
					myWall.transform.Rotate(0, 90f, 0); //Rotate Horizontal
				} 

			} else { //Odd means corner

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

		case 3:

			myWall = (GameObject)Instantiate(wall3Way);
			switch (nullIndex) {
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

		case 4:

			myWall = (GameObject)Instantiate(wall4Way);
			break;

		default:
			myWall = (GameObject)Instantiate(wallSolo); //Default so unity will stop yelling at me
			break;
		}

		myWall.transform.position = n.worldPosition;

		return myWall;

	}

    void MoveBuildSelection()
    {
        if (SelHighlight != null && currentMouseNode.worldPosition != null)
        {
            if (currentMouseNode.Wall != null)
            {
                SelHighlight.transform.position = currentMouseNode.worldPosition + (Vector3.up * 1.1f);
            }
            else
            {
                SelHighlight.transform.position = currentMouseNode.worldPosition + (Vector3.up * 0.1f);
            }
        }
    }

    void MoveWallGhost()
    {
        //Moves the pre-building wall ghost to the proper location while drag building
        if (wallGhost != null && endWallModeNode != null)
        {

            wallGhost.transform.position = grid.CenterOfTwoNodes(startWallModeNode, endWallModeNode);

            if (buildWallModeXY == "x")
            {
                wallGhost.transform.localScale = new Vector3(grid.DistanceBetweenTwoNodesX(startWallModeNode, endWallModeNode) + 1, 1, 1);
            }
            else
            {
                wallGhost.transform.localScale = new Vector3(1, 1, grid.DistanceBetweenTwoNodesY(startWallModeNode, endWallModeNode) + 1);
            }
        }
    }

    //=====================================================
    //  PROPERTIES
    //=====================================================
    public GameObject SelHighlight
    {
        get
        {
            return selHighlight;
        }
        set
        {
            selHighlight = value;
        }
    }

    public bool BuildSelecting
    {
        get
        {
            return buildSelecting;
        }
        set
        {
            if (value == true)
            {
                HideMouse();
            }
            else
            {
                ShowMouse();
            }
            buildSelecting = value;
        }
    }


}
