using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class KeyboardFunctions : NetworkBehaviour
{

    AudioClip UIClickFX;
    AudioSource sourceSFX;


    MouseFunctions mFunc; //Reference to Mouse functions
	BuildHandler bhandler;
	Bank bank;

    string objectToBuild; //Contains name of Prefab to be instantiated

    bool selectedObjectToBuild; //Boolean if tower has been selected

    public bool building = false; //mfunc reconciles the right click before it even moves 
                                  //to the update for keybardfunc so i have to have its own variable 
                                  //that decides when to reconcile the change forits value;

	public GameObject spawnObject;

	public Texture2D normalCursor;
	public Texture2D buildCursor;
	public Texture2D attackCursor;
	public CursorMode cursorMode = CursorMode.Auto;
	public Vector2 hotSpot = Vector2.zero;

	GameObject orbGhost;
	GameObject cannonGhost;
	GameObject laserGhost;
	GameObject lightGhost;
	GameObject magicGhost;
	GameObject iceGhost;

	PlayerNetworking myPN;
	string myID = "";


	SpawnUnit mySpawner;
	NetworkInstanceId mySpawnID;
	SpawnUnit enemySpawner;

	FinishLine myFinish;
	FinishLine enemyFinish;

	[SerializeField]
	int mode = 0; 
	// 0 = idle (nothing happening)
	// 1 = build this.mode
	// 2 = send this.mode

    BuildButtonPress buildButton;
	SendButtonPress sendButton;

    void Awake()
    {
        mFunc = GetComponent<MouseFunctions>();
		bhandler = GetComponent<BuildHandler> ();

		buildButton = GetComponentInChildren<BuildButtonPress>();
		sendButton = GetComponentInChildren<SendButtonPress>();

        UIClickFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
		sourceSFX = Camera.main.GetComponent<AudioSource>();

		//spawner = spawnObject.GetComponent<SpawnUnit> ();

		normalCursor = (Texture2D)Resources.Load ("Sprites/Arrow");
		buildCursor = (Texture2D)Resources.Load ("Sprites/BuildCursor");
		attackCursor = (Texture2D)Resources.Load ("Sprites/AttackCursor");

		orbGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/OrbGhost"));
		cannonGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/CannonGhost"));
		laserGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/LaserGhost"));
		lightGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/LightGhost"));
		magicGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/MagicGhost"));
		iceGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/IceGhost"));

		myPN = GetComponent<PlayerNetworking> ();
		bank = GetComponent<Bank>();

    }



    // Update is called once per frame
    void Update()
    {




		if (myID == "" || myID == "Player(Clone)") {

			myID = myPN.playerUniqueIdentity;
			Debug.Log (myID);

		} else if (mySpawner == null) {

			if (myID == "Player 5") {
				Debug.Log ("Set Spawner");
				mySpawner = GameObject.Find ("EnemySpawn2").GetComponent<SpawnUnit>();
				enemySpawner = GameObject.Find("EnemySpawn1").GetComponent<SpawnUnit>();
				myFinish = GameObject.Find ("FinishLine1").GetComponent<FinishLine> ();

			} else if (myID == "Player 4") {
				Debug.Log ("Set Spawner");
				mySpawner = GameObject.Find ("EnemySpawn1").GetComponent<SpawnUnit>();
				enemySpawner = GameObject.Find("EnemySpawn2").GetComponent<SpawnUnit>();
				myFinish = GameObject.Find ("FinishLine2").GetComponent<FinishLine> ();
			}

		}

		if (mySpawnID.IsEmpty()) {
			Debug.Log ("Set Spawn ID");
			mySpawnID = mySpawner.GetComponent<NetworkIdentity> ().netId;

			mySpawner.sendPlayer = gameObject;

			enemySpawner.attackPlayer = gameObject;
			mySpawner.lateInit ();

			myFinish.SetPlayerToHurt (gameObject);
		}
			

		



		if (!isLocalPlayer) {
			return;

		} else {

			CheckButtons(); //Checks if any buttons are pressed

		}

        
	}

    //Public functions for keypress actions so that they can be reused for the UI button presses

    public void Cancel()
    {
        if (selectedObjectToBuild == true)
        {
            //Cancels tower/wall location selection

            Debug.Log("CANCEL SELECTED TOWER");
            selectedObjectToBuild = false;
            mFunc.Mode = 0; //Shows mouse cursor
            Destroy(mFunc.SelHighlight); //Destroys the green highlight selection box
            sourceSFX.PlayOneShot(UIClickFX);

        }
        else if (this.mode == 1 && selectedObjectToBuild == false)
        {
            //Cancels building this.mode
            buildButton.BuildToggle();
            Debug.Log("CANCEL BUILDING");
            this.mode = 0;
            sourceSFX.PlayOneShot(UIClickFX);
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
        }
		else if (this.mode == 2)
		{
			//Cancels sending this.mode
			sendButton.SendToggle();
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("CANCEL SENDING");
			this.mode = 0;
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
		}
    }


	//========================================================================================
	// Building Functions
	//========================================================================================
    public void Build()
    {
		Debug.Log ("Player netID hit build button " + netId);
		if (this.mode == 0 || this.mode == 2)
        {
			Cursor.SetCursor(buildCursor, hotSpot, cursorMode);
            this.mode = 1;
            Debug.Log("BUILDING START");
            sourceSFX.PlayOneShot(UIClickFX);

        }
    }

    public void CancelBuild()
    {
        if (selectedObjectToBuild == true)
        {
            selectedObjectToBuild = false;
            mFunc.Mode = 0;
            Destroy(mFunc.SelHighlight);
            sourceSFX.PlayOneShot(UIClickFX);
        }

        if (this.mode == 1)
        {
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
            this.mode = 0;
            sourceSFX.PlayOneShot(UIClickFX);
        }
    }

    public void BuildWall()
    {
        if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
            mFunc.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
            sourceSFX.PlayOneShot(UIClickFX);
            Debug.Log("BUILDING Z TOWER");
            objectToBuild = "Wall";
            SelectedObjectToBuild = true;
            mFunc.Mode = 1;
        }
    }

    public void BuildOrbTower()
    {
        if (this.mode == 1)
        {
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(orbGhost))); //Creates green selection box
            sourceSFX.PlayOneShot(UIClickFX);
            Debug.Log("BUILDING X TOWER");
            objectToBuild = "Orb";
            SelectedObjectToBuild = true;
			mFunc.Mode = 1;

        }
    }

	public void BuildCannonTower()
	{
		if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(cannonGhost))); //Creates green selection box
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING C TOWER");
			objectToBuild = "Cannon";
			SelectedObjectToBuild = true;
			mFunc.Mode = 1;

		}
	}

	public void BuildLaserTower()
	{
		if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(laserGhost))); //Creates green selection box
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING V TOWER");
			objectToBuild = "Laser";
			SelectedObjectToBuild = true;
			mFunc.Mode = 1;

		}
	}

	public void BuildIceTower()
	{
		if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(iceGhost))); //Creates green selection box
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING F TOWER");
			objectToBuild = "Ice";
			SelectedObjectToBuild = true;
			mFunc.Mode = 1;

		}
	}

	public void BuildLightTower()
	{
		if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(lightGhost))); //Creates green selection box
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING G TOWER");
			objectToBuild = "Light";
			SelectedObjectToBuild = true;
			mFunc.Mode = 1;

		}
	}

	public void BuildMagicTower()
	{
		if (this.mode == 1)
		{
			Destroy(mFunc.SelHighlight);
			mFunc.SelHighlight = ((GameObject)(Instantiate(magicGhost))); //Creates green selection box
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING H TOWER");
			objectToBuild = "Magic";
			SelectedObjectToBuild = true;
			mFunc.Mode = 1;

		}
	}

	public bool SelectedObjectToBuild
	{
		get
		{
			return selectedObjectToBuild;
		}
		set
		{
			selectedObjectToBuild = value;
		}
	}

	public string ObjectToBuild

	{
		get
		{
			return objectToBuild;
		}
		set
		{
			objectToBuild = value;
		}
	}

	//========================================================================================
	// Sending Functions
	//========================================================================================

	public void Send()
	{
		if (this.mode == 0 || this.mode == 1) {
			this.mode = 2;
			Debug.Log ("SENDING START");
			Cursor.SetCursor (attackCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot (UIClickFX);
		}
	}

	public void CancelSend()
	{
		if (this.mode == 2) {
			this.mode = 0;
			Debug.Log ("SENDING STOP");
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot(UIClickFX);
		}
	}



	public void CheckSendUnit(string unitName)
	{
		if (this.mode == 2) {

			int unitCost = 0;
			int incomeGain = 0;
			switch (unitName) {

			case "EnemyType1":
				unitCost = 5;
				incomeGain = 1;
				break;
			case "Potato":
				unitCost = 5;
				incomeGain = 1;
				break;
			case "Cloud":
				unitCost = 30;
				incomeGain = 8;
				break;
			case "Reinhardt":
				unitCost = 30;
				incomeGain = 8;
				break;
			default:
				break;

			}
			if (bank.getMoney () >= unitCost) {
				
				bank.subtractMoney (unitCost);
				bank.addIncome (incomeGain);

				CmdSendUnit (mySpawnID, unitName);

			} else {
				Debug.Log ("Not enough money to send!");
			}

		}
	}



	[Command]
	void CmdSendUnit(NetworkInstanceId spawnID, string _unitName)
	{		
		SpawnUnit spawner = NetworkServer.FindLocalObject (spawnID).GetComponent<SpawnUnit> ();
		spawner.Spawn (_unitName);
	}

	[Client]
    void CheckButtons()
    {
        if (Input.GetButtonDown("Cancel") || Input.GetButtonDown("Fire2"))
        { //If ESC is pressed

            //Cancels out of the layers of selection

            //Layers from shallow to deep are
            //1)Build Mode 
            // 2) Tower
            // 2) Wall

            //Cancels from deeper layers first, one layer per press'
            if (building == false)
            {
                Cancel();
            }
        }
        else if (Input.GetButtonDown("B"))
        { //If B is pressed, Enter Build this.mode
			buildButton.BuildToggle ();
        }
        else if (Input.GetButtonDown("Z"))
        { //If Z is pressed, and Building Mode is enabled, -- Build wall
			if (this.mode == 1) {
				BuildWall ();
			} else if (this.mode == 2) {
				CheckSendUnit ("Potato");
			} else {
				Debug.Log ("Z was pressed but build mode is not right");
			}
           

        }
		else if (Input.GetButtonDown("X"))
		{
			if (this.mode == 1) {
				BuildOrbTower ();
			} else if (this.mode == 2) {
				CheckSendUnit("Cloud");
			}
		}
		else if (Input.GetButtonDown("C"))
		{
			if (this.mode == 1) {
				BuildCannonTower ();
			} else if (this.mode == 2) {
			}
		}
        else if (Input.GetButtonDown("T"))
        {
			sendButton.SendToggle ();
        }
		else if (Input.GetButtonDown("Q"))
		{
			bank.addMoney (50);
		}
		else if (Input.GetButtonDown("V"))
		{
			if (this.mode == 1) {
				BuildLaserTower ();
			} else if (this.mode == 2) {
			}
		}
		else if (Input.GetButtonDown("F"))
		{
			if (this.mode == 1) {
				BuildIceTower ();
			} else if (this.mode == 2) {
			}
		}
		else if (Input.GetButtonDown("G"))
		{
			if (this.mode == 1) {
				BuildLightTower ();
			} else if (this.mode == 2) {
			}
		}
		else if (Input.GetButtonDown("H"))
		{
			if (this.mode == 1) {
				BuildMagicTower ();
			} else if (this.mode == 2) {
			}
		}


    }

    
	public int Mode{

		get {
			return this.mode;
		}
		set {
			this.mode = value;
		}
	}

}

