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
	KeyboardFunctions kf;
	BuildHandler bhandler;
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
	public bool panelShowing;


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


	GameObject wallGhostLoaded;

	GameObject selHighlight;

	Sprite wallIcon;
	Sprite orbIcon;
	Sprite cannonIcon;
	Sprite laserIcon;

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

	public LayerMask objectUILayerMask;



    void Awake()
    {
		kf = GetComponent<KeyboardFunctions>();
        target = GameObject.Find("Destination 1").transform;
        grid = GetComponent<Grid>();
		bhandler = GetComponent<BuildHandler> ();
        bank = player.GetComponent<Bank>();


		wallGhostLoaded = (GameObject)(Resources.Load ("Walls/WallGhost"));


		wallIcon = Resources.Load<Sprite> ("Sprites/WallIcon");
		orbIcon = Resources.Load<Sprite> ("Sprites/OrbIcon");
		cannonIcon = Resources.Load<Sprite> ("Sprites/CannonIcon");
		laserIcon = Resources.Load<Sprite> ("Sprites/LaserIcon");


		highlightCastleMaterial = (Material)(Resources.Load ("Materials/Outlined_Object"));

        BuildFX = (AudioClip)(Resources.Load("Sounds/BuildingPlacement", typeof(AudioClip)));
		needMoneySound  = (AudioClip)(Resources.Load("Sounds/needMoney", typeof(AudioClip)));
		cannotBuildSound  = (AudioClip)(Resources.Load("Sounds/CannotBuild", typeof(AudioClip)));
		selectSound = Resources.Load<AudioClip> ("Sounds/CarDoorClose");
		sourceSFX = Camera.main.GetComponent<AudioSource>();

    }


    void Start()
    {
		
		terrainFloorMask = LayerMask.GetMask("BoardTerrain");
        path = new NavMeshPath();
		Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);

		selectedObjectRenderer = new Renderer[5];
		selectedObjectResetMaterial = new Material[5][];
		selectedObjectResetMaterial [0] = new Material[10];
		selectedObjectResetMaterial [1] = new Material[10];
		selectedObjectResetMaterial [2] = new Material[10];
		numberOfMaterials = new int[5];

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
		if (Physics.Raycast (camRay, out objHit, camRayLength, objectUILayerMask)) {
			
				currentMouseObject = objHit.transform.gameObject;

		} else {
			currentMouseObject = null;
		}
	}

    

	void CheckSelectionClick()
	//Checks how to handle clicks for selecting buildings and towers (maybe units too later on)
	{
		if (Input.GetButtonDown ("Fire1")) {
			
				

			if (currentMouseObject != null) {
					

				if (currentMouseObject.CompareTag ("TowerSelector") || currentMouseObject.CompareTag ("Wall")) {
					Deselect ();
					sourceSFX.PlayOneShot (selectSound);
					selectedObject = currentMouseObject;
					selectedObjectType = currentMouseObject.tag;
					Debug.Log (selectedObjectType);

					if (selectedObject.transform.parent != null) {
						selectedObjectRenderer = selectedObject.transform.parent.GetComponentsInChildren<Renderer> ();
					} else {
						selectedObjectRenderer = selectedObject.GetComponents<Renderer> ();
					}

					numberOfRenderers = selectedObjectRenderer.Length;

					for (int i = 0; i < numberOfRenderers; i++) {
						selectedObjectResetMaterial [i] = selectedObjectRenderer [i].materials;
						numberOfMaterials [i] = selectedObjectRenderer [i].materials.Length;
						Material[] tempMaterials = new Material[numberOfMaterials [i]];
						for (int j = 0; j < numberOfMaterials [i]; j++) {
							tempMaterials [j] = highlightCastleMaterial;
						}
						selectedObjectRenderer [i].materials = tempMaterials;
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
				Tower tower = selectedObject.transform.parent.GetComponentInParent<Tower> ();
				string[] stats = tower.Stats;

				nameText.text = stats [0];
				for (int i = 1; i < 7; i++) {
					displayMe = displayMe + stats [i] + "\n\n";
				}

				if (stats [0] == "Orb") {
					selectionImage.sprite = orbIcon;
				} else if (stats [0] == "Cannon") {
					selectionImage.sprite = cannonIcon;
				} else if (stats [0] == "Laser") {
					selectionImage.sprite = laserIcon;
				} 

				selectionImage.enabled = true;

				ShowPanel ();

				infoText.text = displayMe;

			}
		} else if (selectedObjectType == "Wall") {
			selectionImage.sprite = wallIcon;
			selectionImage.enabled = true;
			nameText.text = "Wall";
			infoText.text = "-\n\n-\n\n-\n\n5\n\n2\n\n-";
			ShowPanel ();
		}
		else {
			selectionImage.enabled = false;
			infoText.text = "";
			nameText.text = "";

			HidePanel ();
		}
	}

	public void HidePanel()
	{
		if (panelShowing == true) {
			StopCoroutine ("animateSelectionPanel");
			StartCoroutine (animateSelectionPanel (-1));
		}
	}

	public void ShowPanel()
	{
		if (panelShowing == false) {
			StopCoroutine ("animateSelectionPanel");
			StartCoroutine (animateSelectionPanel (1));
		}
	}

	IEnumerator animateSelectionPanel(int direction)
	{
		
		Vector3 targetPosition;
		if (direction > 0) {
			selectionPanel.transform.position.Set (-160f, 212.5f, 0f);
			targetPosition = new Vector3(40f, 212.5f, 0f);
			while (selectionPanel.transform.position.x < 40f) {
				selectionPanel.transform.position = Vector3.Lerp (selectionPanel.transform.position, targetPosition, 30f * Time.deltaTime);

				yield return null;
			}
			panelShowing = true;

		} else {
			selectionPanel.transform.position.Set (40f, 212.5f, 0f);
			targetPosition = new Vector3(-160f, 212.5f, 0f);
			while (selectionPanel.transform.position.x > -160f) {
				selectionPanel.transform.position = Vector3.Lerp (selectionPanel.transform.position, targetPosition, 30f * Time.deltaTime);

				yield return null;
			}
			panelShowing = false;

		}
	}


	public void Deselect()
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
            kf.building = false;
        }

        if (Input.GetButtonDown("Fire1"))
        {

            buildStructure = kf.ObjectToBuild;
            positionToBuildStart = currentMouseNode.worldPosition;

            if (buildStructure == "Wall")
            {
                startWallModeNode = currentMouseNode; 
                buildWallMode = true;
				wallGhost = ((GameObject)(Instantiate(wallGhostLoaded)));
                kf.building = true;
                building = true;
            }
            else
            {
                bhandler.HandleBuildTower(currentMouseNode);
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
            kf.building = false;
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

						newWall = bhandler.wallType (currentNode, neighbors);
						currentNode.Wall = newWall;
						newWall.GetComponent<Wall> ().node = currentNode;

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
						newWall = bhandler.wallType (node, neighbors);
						node.Wall = newWall;
						newWall.GetComponent<Wall> ().node = node;
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

	public Grid TheGrid
	{
		get
		{
			return grid;
		}
	}

	public Node CurrentNode
	{
		get {
			return currentMouseNode;
		}
	}

	public GameObject CurrentObject
	{
		get {
			return selectedObject;
		}
	}


}
