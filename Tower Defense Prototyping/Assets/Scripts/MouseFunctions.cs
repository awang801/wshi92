using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
	int mode;
	Node currentMouseNode;
	GameObject selectedObject;
	Renderer[] selectedObjectRenderer;
	Material[][] selectedObjectResetMaterial;
	int numberOfRenderers;
	int[] numberOfMaterials;
	GameObject currentMouseObject;

	string selectedObjectType;

	string[] selectedValues;
	public Text infoText;
	public Text nameText;
	public Image selectionImage;
	public GameObject selectionPanel;


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

	//Audio
	//==============================================
    AudioClip BuildFX;
	AudioClip needMoneySound;
	AudioClip cannotBuildSound;
	AudioClip selectSound;
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

	Sprite cannonIcon;
	Sprite orbIcon;
	Sprite wallIcon;

    NavMeshPath path;

	Material highlightCastleMaterial;

	//Cursor
	//===============================================
	public Texture2D cursorTexture;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	//Other..?
	//===============================================
    Transform target; //Target of the enemies (need to change this in the future)
    int terrainFloorMask;
	int objectFloorMask;





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

		orbIcon = Resources.Load<Sprite> ("Sprites/OrbIcon");
		cannonIcon = Resources.Load<Sprite> ("Sprites/CannonIcon");
		wallIcon = Resources.Load<Sprite> ("Sprites/WallIcon");

		highlightCastleMaterial = (Material)(Resources.Load ("Materials/Outlined_Object"));

        BuildFX = (AudioClip)(Resources.Load("Sounds/BuildingPlacement", typeof(AudioClip)));
		needMoneySound  = (AudioClip)(Resources.Load("Sounds/needMoney", typeof(AudioClip)));
		cannotBuildSound  = (AudioClip)(Resources.Load("Sounds/CannotBuild", typeof(AudioClip)));

		selectSound = Resources.Load<AudioClip> ("Sounds/CarDoorClose");
		sourceSFX = Camera.main.GetComponent<AudioSource>();

    }


    void Start()
    {
		
		terrainFloorMask = LayerMask.GetMask("Terrain");
		objectFloorMask = LayerMask.GetMask("Objects");
        path = new NavMeshPath();
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

		selectedObjectRenderer = new Renderer[3];
		selectedObjectResetMaterial = new Material[3][];
		selectedObjectResetMaterial [0] = new Material[10];
		selectedObjectResetMaterial [1] = new Material[10];
		selectedObjectResetMaterial [2] = new Material[10];
		numberOfMaterials = new int[3];

		selectedValues = new string[7];

		StartCoroutine (animateSelectionPanel (-1));

    }

    // Update is called once per frame
    void Update()
    {

		if (mode == 0) {
			UpdateMouseObjects ();
			CheckSelectionClick ();

		}
		else if (mode == 1)
        {
			UpdateMouseNode();

			MoveBuildSelection();
            CheckBuildClick();
			if (buildWallMode)
			{
				MoveWallGhost();
			}
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
		if (Physics.Raycast(camRay, out nodeHit, camRayLength, terrainFloorMask))
        {
            currentMouseNode = grid.NodeFromWorldPoint(nodeHit.point);
        }
    }

	void UpdateMouseObjects()
	/*
	 * Updates the object (wall/tower/unit) that the mouse is currently over
	 * 
	 * Saves information in "currentMouseNode"
	 */
	{
		Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit objHit;

		//Debug.DrawRay (Camera.main.transform.position, Input.mousePosition);
		if (Physics.Raycast (camRay, out objHit, camRayLength, objectFloorMask)) {
			currentMouseObject = objHit.transform.gameObject;
		} else {
			currentMouseObject = null;
		}
	}



    void HandleBuildTower() 
	//Checks if it is possible to build a tower in current node
	//If it IS possible, handle the correct type of tower
    {
		

		if (currentMouseNode.Wall == null) {

			//Play Cannot Build Sound
			Debug.Log("CANNOT BUILD TOWER WITHOUT WALL");
			sourceSFX.PlayOneShot(cannotBuildSound);
		} 

		else {

			if (currentMouseNode.Tower != null) {

				//Play Cannot Build Sound
				Debug.Log("CANNOT BUILD TOWER, TOWER ALREADY EXISTS");
				sourceSFX.PlayOneShot(cannotBuildSound);

			} else {

				switch (buildStructure) {

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
						positionToBuildStart = currentMouseNode.worldPosition + (Vector3.up * 1f);
						currentMouseNode.Tower = ((GameObject)(Instantiate(orbTower, positionToBuildStart, Quaternion.identity)));
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
						positionToBuildStart = currentMouseNode.worldPosition + (Vector3.up * 1f);
						currentMouseNode.Tower = ((GameObject)(Instantiate (cannonTower, positionToBuildStart, Quaternion.identity)));
					}

					break;

				default:

					break;
				}

			}

		}

    }

	void CheckSelectionClick()
	//Checks how to handle clicks for selecting buildings and towers (maybe units too later on)
	{
			if (Input.GetButtonDown ("Fire1")) {
			
				Deselect ();

				if (currentMouseObject != null) {
					
					if(currentMouseObject.CompareTag("TowerSelector") || currentMouseObject.CompareTag("Wall"))
					{
						sourceSFX.PlayOneShot (selectSound);
						selectedObject = currentMouseObject;
						selectedObjectType = currentMouseObject.tag;
						Debug.Log (selectedObjectType);

						if (selectedObject.transform.parent != null) {
							selectedObjectRenderer  = selectedObject.transform.parent.GetComponentsInChildren<Renderer> ();
						} else {
							selectedObjectRenderer  = selectedObject.GetComponents<Renderer> ();
						}

						numberOfRenderers = selectedObjectRenderer.Length;

						for (int i = 0; i < numberOfRenderers; i++) {
							selectedObjectResetMaterial[i] = selectedObjectRenderer[i].materials;
							numberOfMaterials [i] = selectedObjectRenderer [i].materials.Length;
							Material[] tempMaterials = new Material[numberOfMaterials[i]];
							for (int j = 0; j < numberOfMaterials[i]; j++) {
								tempMaterials[j] = highlightCastleMaterial;
							}
							selectedObjectRenderer[i].materials = tempMaterials;

						}

						

					}
				}

				updateInfoText ();
				
			} 

	}

	void updateInfoText()
	{
		if (selectedObjectType == "TowerSelector") {
			if (infoText != null) {
				string displayMe = "";
				Tower tower = selectedObject.transform.parent.gameObject.GetComponent<Tower> ();
				string[] stats = tower.Stats;

				nameText.text = stats [0];
				for (int i = 1; i < 7; i++) {
					displayMe = displayMe + stats [i] + "\n\n";
				}
				if (stats [0] == "Orb Tower") {
					selectionImage.sprite = orbIcon;
					selectionImage.enabled = true;
					StartCoroutine (animateSelectionPanel (1));
				} else if (stats [0] == "Cannon") {
					selectionImage.sprite = cannonIcon;
					selectionImage.enabled = true;
					StartCoroutine (animateSelectionPanel (1));
				} 

				infoText.text = displayMe;



			}
		} else {
			selectionImage.enabled = false;
			infoText.text = "";
			nameText.text = "";
			StartCoroutine (animateSelectionPanel (-1));
		}
	}

	IEnumerator animateSelectionPanel(int direction)
	{
		
		Vector3 targetPosition;
		if (direction > 0) {
			targetPosition = new Vector3(40f, 212.5f, 0f);
			while (selectionPanel.transform.position.x < 40f) {
				selectionPanel.transform.position = Vector3.Lerp (selectionPanel.transform.position, targetPosition, 30f * Time.deltaTime);

				yield return null;
			}

		} else {
			targetPosition = new Vector3(-160f, 212.5f, 0f);
			while (selectionPanel.transform.position.x > -160f) {
				selectionPanel.transform.position = Vector3.Lerp (selectionPanel.transform.position, targetPosition, 30f * Time.deltaTime);

				yield return null;
			}

		}



	}

	void Deselect()
	{
		if (selectedObject != null) {
			for (int i = 0; i < numberOfRenderers; i++) {
				for (int j = 0; j < numberOfMaterials[i]; j++) {
					selectedObjectRenderer[i].materials = selectedObjectResetMaterial[i];
				}
			}
		}

		selectedObject = null;
		selectedObjectType = null;
	}

	void CheckBuildClick()
	//Checks how to handle clicks for building walls and towers
    {
		
        if(building == false)
        {
            kFunc.building = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {

            buildStructure = kFunc.TowerToBuild;
            positionToBuildStart = currentMouseNode.worldPosition;

            if (buildStructure == "Wall")
            {
                startWallModeNode = currentMouseNode; 
                buildWallMode = true;
				wallGhost = ((GameObject)(Instantiate(wallGhostLoaded)));
                kFunc.building = true;
                building = true;
            }
            else
            {
                HandleBuildTower();
            }

        }
        if (Input.GetButton("Fire1"))
        {
            if (buildWallMode == true)
            {

                Vector2 difference = grid.NodeDifference(startWallModeNode, currentMouseNode);

                if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y))
                {

                    endWallModeNode = grid.NodeFromCoordinates(currentMouseNode.gridX, startWallModeNode.gridY);
                    buildWallModeXY = "x";
                    wallsToBuild = difference.x;

                }
                else if (Mathf.Abs(difference.y) > Mathf.Abs(difference.x))
                {

                    endWallModeNode = grid.NodeFromCoordinates(startWallModeNode.gridX, currentMouseNode.gridY);
                    buildWallModeXY = "y";
                    wallsToBuild = difference.y;

                }
            }
        }

        if (Input.GetButtonUp("Fire1") && buildWallMode == true)
        {

			List<Node> alreadyChecked = new List<Node> ();
			List<Node> toCheck = new List<Node> ();
            kFunc.building = false;
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
					sourceSFX.PlayOneShot(cannotBuildSound);
                }
            }
            else
            {
                Debug.Log("Not enough Money");
				sourceSFX.PlayOneShot(needMoneySound);
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
		//Returns the correct wall type, location, and rotation, for MODULAR WALL BUILDING
		GameObject myWall;

		int numberOfNeighbors = 0; //numberOfNeighbors of Neighbors
		int sum = 0; //See below in the for loop
		int nullIndex = 0; //Stores the location of the null neighbor (starts at 0) - - ONLY USED FOR 3 NEIGHBOR CASE

		int[] nonNullIndexes = new int[4]; //Stores the indexes of the neighbors

		neighbors = grid.GetNeighbors (n); //Gets neighbors in an array

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

    void MoveBuildSelection()
    {
		//Moves the green highlight box around with the mouse
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

	public int Mode
	{
		get
		{
			return mode;
		}
		set
		{
			if (value == 1)
			{
				HideMouse();
				Deselect ();
			}
			else
			{
				ShowMouse();
			}
			mode = value;
		}
	}



}
