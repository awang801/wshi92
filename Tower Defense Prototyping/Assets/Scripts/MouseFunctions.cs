using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Networking;

//This class manages mouse functions including selection on the grid, cursor hiding/showing
public class MouseFunctions : NetworkBehaviour
{

	//References
	//=============================================
	KeyboardFunctions kf;
	BuildHandler bhandler;
	Bank bank;
	PathFind pathfind;
	GameManager gm;
	GameObject gmObject;
	PlayerNetworking pn;

	//Selection
	//=============================================
	float camRayLength = 200f;

	Grid grid;

	int mode;
	Node currentMouseNode;
	GameObject selectedObject;
	SphereCollider selectedObjectCollider;
	Renderer[] selectedObjectRenderer;
	Material[][] selectedObjectResetMaterial;
	int numberOfRenderers;
	int[] numberOfMaterials;
	GameObject currentMouseObject;

	GameObject rangeIndicator;
	GameObject rangeIndicatorInstance;

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
	Sprite iceIcon;
	Sprite lightIcon;
	Sprite magicIcon;


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
		target = GameObject.Find("Destination1").transform;

		grid = GetComponent<Grid>();
		bhandler = GetComponent<BuildHandler> ();
        bank = GetComponent<Bank>();
		pathfind = GetComponent<PathFind>();
		pn = GetComponent<PlayerNetworking> ();



		wallGhostLoaded = (GameObject)(Resources.Load ("Walls/WallGhost"));


		wallIcon = Resources.Load<Sprite> ("Sprites/WallIcon");
		orbIcon = Resources.Load<Sprite> ("Sprites/OrbIcon");
		cannonIcon = Resources.Load<Sprite> ("Sprites/CannonIcon");
		laserIcon = Resources.Load<Sprite> ("Sprites/LaserIcon");
		iceIcon = Resources.Load<Sprite> ("Sprites/IceIcon");
		lightIcon = Resources.Load<Sprite> ("Sprites/LightIcon");
		magicIcon = Resources.Load<Sprite> ("Sprites/MagicIcon");

		rangeIndicator = (GameObject)(Resources.Load ("UI/RangeIndicator"));

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

		selectedObjectRenderer = new Renderer[10];
		selectedObjectResetMaterial = new Material[10][];
		for (int i = 0; i < 10; i++) {
			selectedObjectResetMaterial [i] = new Material[10];
		}
		numberOfMaterials = new int[10];

		selectedValues = new string[7];




    }

    // Update is called once per frame
    void Update()
    {
		if (!isLocalPlayer) {
			return;
		}

		if (gmObject == null) {
			infoText = GameObject.Find ("Info Values").GetComponent<Text> ();
			nameText = GameObject.Find ("Title").GetComponent<Text> ();
			selectionImage = GameObject.Find ("InfoPicture").GetComponent<Image> ();;
			selectionPanel = GameObject.Find ("InfoPanel");
			gmObject = GameObject.Find ("GameManager");
			StopCoroutine ("animateSelectionPanel");
			StartCoroutine (animateSelectionPanel (-1));
			Debug.Log ("GM is NULL, setting in MF script");
		} else if (gm == null) {
			gm = gmObject.GetComponent<GameManager>();
		}

		if (gm != null) {
			if (gm.gameRunning) {
				

				if (mode == 0) {
					UpdateMouseObjects ();
					CheckSelectionClick ();

				} else if (mode == 1) {
					UpdateMouseNode ();

					MoveBuildSelection ();
					CheckBuildClick ();
					if (buildWallMode) {
						MoveWallGhost ();
					}
				}
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
	 * Saves information in "currentMouseObject"
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
					
				if (currentMouseObject.CompareTag ("TowerSelector")) {

					SelectTower ();

				} else if (currentMouseObject.CompareTag ("Wall")) {

					SelectWall ();

				}

			}
			updateInfoText ();

		} else if (Input.GetButtonDown ("Fire2")) {

			Deselect ();
			updateInfoText ();

		}

	}

	void SelectTower()
	{

		Deselect ();

		sourceSFX.PlayOneShot (selectSound);

		selectedObject = currentMouseObject;
		selectedObjectCollider = selectedObject.transform.parent.parent.GetComponent<SphereCollider> ();
		selectedObjectType = currentMouseObject.tag;

		selectedObjectRenderer = selectedObject.transform.parent.parent.GetComponentsInChildren<Renderer> ();

		numberOfRenderers = selectedObjectRenderer.Length;

		for (int i = 0; i < numberOfRenderers; i++) {

			if (!selectedObjectRenderer [i].CompareTag ("BulletPoint")) {
				selectedObjectResetMaterial [i] = selectedObjectRenderer [i].materials;
				numberOfMaterials [i] = selectedObjectRenderer [i].materials.Length;
				Material[] tempMaterials = new Material[numberOfMaterials [i]];
				for (int j = 0; j < numberOfMaterials [i]; j++) {
					tempMaterials [j] = highlightCastleMaterial;
				}
				selectedObjectRenderer [i].materials = tempMaterials;
			} 
		}

		ShowRangeIndicator ();

	}

	void SelectWall()
	{
		
		Deselect ();

		sourceSFX.PlayOneShot (selectSound);

		selectedObject = currentMouseObject;
		selectedObjectType = currentMouseObject.tag;

		selectedObjectRenderer = selectedObject.GetComponents<Renderer> ();

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
				} else if (stats [0] == "Ice") {
					selectionImage.sprite = iceIcon;
				} else if (stats [0] == "Light") {
					selectionImage.sprite = lightIcon;
				} else if (stats [0] == "Magic") {
					selectionImage.sprite = magicIcon;
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

	void ShowRangeIndicator()
	{
		rangeIndicatorInstance = (GameObject)Instantiate (rangeIndicator, selectedObject.transform.parent);


		float correctScale = selectedObjectCollider.radius * 2f;

		rangeIndicatorInstance.transform.localScale = new Vector3 (correctScale, 0.001f, correctScale);
		rangeIndicatorInstance.transform.position = selectedObject.transform.parent.parent.position;
	}

	void DeleteRangeIndicator()
	{
		if (rangeIndicatorInstance != null) {
			Destroy (rangeIndicatorInstance);
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
		DeleteRangeIndicator ();

		if (selectedObject != null) {
			for (int i = 0; i < numberOfRenderers; i++) {
				for (int j = 0; j < numberOfMaterials[i]; j++) {
					if (!selectedObjectRenderer [i].CompareTag ("BulletPoint")) {
						selectedObjectRenderer[i].materials = selectedObjectResetMaterial[i];
					}
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

		//On mouse DOWN
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

				//Updates the wall ghost as you drag the mouse along the X or Y axis
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



        if (Input.GetButtonUp("Fire1") && buildWallMode == true && building == true)
        {

			BuildWalls ();

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

	void BuildWalls()
	{
		
		kf.building = false;
		buildWallMode = false;

		if (bank.getMoney() >= ((Mathf.Abs(wallsToBuild) + 1) * 5 )) //Check if we have enough money
		{
			if (grid.NodeLineContainsWall(startWallModeNode, endWallModeNode, buildWallModeXY) == false) // Check for existing walls along this line
			{

				sourceSFX.PlayOneShot(BuildFX);
				bank.addMoney((int)((Mathf.Abs(wallsToBuild) + 1) * -5));
				CmdBuildWalls (startWallModeNode.gridX, startWallModeNode.gridY, wallsToBuild, buildWallModeXY, pn.playerUniqueIdentity);
				Debug.Log ("Cmd Build Walls");

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

	[Command]
	void CmdBuildWalls(int startNodeX, int startNodeY, float _wallsToBuild, string buildDirection, string playerID)
	{
		List<Node> alreadyChecked = new List<Node> ();
		List<Node> toCheck = new List<Node> ();
		Node currentNode = grid.NodeFromCoordinates(startNodeX, startNodeY);
		Node nextNode = currentNode;
		Node[] neighbors;
		GameObject dummyWall = new GameObject();
		GameObject newWall;
		Wall tempWall;

		Node startNode = new Node(new Vector3(12, 0, 30), 12, 30);
		Node endNode = new Node(new Vector3(12,0,5),12,5);

		for (int i = 0; i <= Mathf.Abs(_wallsToBuild); i++)
		{
			neighbors = grid.GetNeighbors (currentNode);

			if (i != Mathf.Abs (_wallsToBuild)) { //If statement so we don't try to go OVER the grid boundaries on last iteration

				if (buildDirection == "x") {

					if (_wallsToBuild > 0) {

						neighbors [1].Wall = dummyWall;
						nextNode = grid.NodeFromCoordinates (currentNode.gridX + 1, currentNode.gridY);
					} else {
						neighbors [3].Wall = dummyWall;
						nextNode = grid.NodeFromCoordinates (currentNode.gridX - 1, currentNode.gridY);
					}

				} else {

					if (_wallsToBuild > 0) {
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
			tempWall = newWall.GetComponent<Wall> ();
			tempWall.node = currentNode;
			tempWall.OwnerPlayerId = pn.playerUniqueIdentity;

			NetworkServer.Spawn(newWall);
			RpcSyncNode (currentNode.gridX, currentNode.gridY, 1, newWall, pn.playerUniqueIdentity);


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

		currentNode = grid.NodeFromCoordinates(startNodeX, startNodeY);;

		if(pathfind.pathFound(startNode,endNode))
		{
			foreach (var node in toCheck) {
				neighbors = grid.GetNeighbors (node);
				Destroy (node.Wall);
				newWall = bhandler.wallType (node, neighbors);
				node.Wall = newWall;
				tempWall = newWall.GetComponent<Wall> ();
				tempWall.node = node;
				tempWall.OwnerPlayerId = pn.playerUniqueIdentity;

				NetworkServer.Spawn(newWall);
				RpcSyncNode (node.gridX, node.gridY, 1, newWall, pn.playerUniqueIdentity);
			}
		}

		else
		{
			for(int i = 0; i <= Mathf.Abs(_wallsToBuild); i++) 
			{
				if (i != Mathf.Abs (_wallsToBuild)) {								
					if (buildDirection == "x") {

						if (_wallsToBuild > 0) {
							nextNode = grid.NodeFromCoordinates (currentNode.gridX + 1, currentNode.gridY);
						} else {
							nextNode = grid.NodeFromCoordinates (currentNode.gridX - 1, currentNode.gridY);
						}

					} else {

						if (_wallsToBuild > 0) {
							nextNode = grid.NodeFromCoordinates (currentNode.gridX, currentNode.gridY + 1);
						} else {
							nextNode = grid.NodeFromCoordinates (currentNode.gridX, currentNode.gridY - 1);
						}

					}
				}
				Destroy(currentNode.Wall);
				currentNode = nextNode;

			} 
			RpcPlaySound (playerID, "CannotBuild");
		}
		Destroy (dummyWall);


	}

	[ClientRpc]
	void RpcPlaySound(string playerID, string sound)
	{
		
		if (pn.playerUniqueIdentity == playerID && isLocalPlayer) {

			switch (sound) {

			case "CannotBuild":
				sourceSFX.PlayOneShot (cannotBuildSound);
				break;

			default:
				break;

			}

		}

	}



	[ClientRpc]
	public void RpcSyncNode(int nodeX, int nodeY, int tw, GameObject obj, string owner)
	{		
		if (obj != null) {
			Node myNode = grid.NodeFromCoordinates (nodeX, nodeY);

			if (tw == 0) {
				myNode.Tower = obj;
				Tower tempTower = obj.GetComponent<Tower>();
				tempTower.node = myNode;
				tempTower.OwnerPlayerId = owner;
			} else if (tw == 1) {
				myNode.Wall = obj;
				Wall tempWall = obj.GetComponent<Wall> ();
				tempWall.node = myNode;
				tempWall.OwnerPlayerId = owner;
			}

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
