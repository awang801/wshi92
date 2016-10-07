using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class KeyboardFunctions : NetworkBehaviour
{
	GameObject gmObject;
	GameManager gm;

    AudioClip UIClickFX;
    AudioSource sourceSFX;
	AudioSource BGMusic;
	AudioClip needMoneySound;

    MouseFunctions mFunc; //Reference to Mouse functions
	//BuildHandler bhandler;
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

	bool cursorReset = false;

	bool Key_ctrl = false;

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
		//bhandler = GetComponent<BuildHandler> ();

        UIClickFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
		needMoneySound  = (AudioClip)(Resources.Load("Sounds/needMoney", typeof(AudioClip)));
		sourceSFX = Camera.main.GetComponent<AudioSource>();
		BGMusic = Camera.main.transform.GetComponentInChildren<AudioSource> ();

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
		if (gmObject == null) {
			gmObject = GameObject.Find ("GameManager");
			buildButton = GameObject.Find("BuildButtonText").GetComponent<BuildButtonPress>();
			sendButton = GameObject.Find("SendButtonText").GetComponent<SendButtonPress>();
		} else if (gm == null) {
			gm = gmObject.GetComponent<GameManager>();
		}

		if (myID == "" || myID == "Player(Clone)") {

			myID = myPN.playerUniqueIdentity;
			Debug.Log (myID);

		} else if (mySpawner == null) {
			if (gm != null) {

				int myIdx = gm.MyPlayerIndex (myID);
					
				if (myIdx == -1) {
					Debug.Log ("My index is not found yet");
				} else {
					if (myIdx == 1) {
						Debug.Log ("Set Spawner");
						mySpawner = GameObject.Find ("EnemySpawn2").GetComponent<SpawnUnit>();
						enemySpawner = GameObject.Find("EnemySpawn1").GetComponent<SpawnUnit>();
						myFinish = GameObject.Find ("FinishLine1").GetComponent<FinishLine> ();
						enemyFinish = GameObject.Find ("FinishLine2").GetComponent<FinishLine> ();
					} else if (myIdx == 0) {
						Debug.Log ("Set Spawner");
						mySpawner = GameObject.Find ("EnemySpawn1").GetComponent<SpawnUnit>();
						enemySpawner = GameObject.Find("EnemySpawn2").GetComponent<SpawnUnit>();
						myFinish = GameObject.Find ("FinishLine2").GetComponent<FinishLine> ();
						enemyFinish = GameObject.Find ("FinishLine1").GetComponent<FinishLine> ();
					}
				}
			}


		}

		if (mySpawnID.IsEmpty()) {

			if (mySpawner != null) {
				Debug.Log ("Set Spawn ID");
				mySpawnID = mySpawner.GetComponent<NetworkIdentity> ().netId;

				mySpawner.sendPlayer = gameObject;

				enemySpawner.attackPlayer = gameObject;

				myFinish.SetPlayerToHurt (gameObject, myID);
				myFinish.SetMyPlayer (myID);
				enemyFinish.setOtherPlayer (gameObject, myID);
			}

		}

		if (!isLocalPlayer) {
			return;

		} else if (gm != null) {

			if (gm.gameRunning) {

				if (cursorReset == true) {
					cursorReset = false;
				}
				CheckButtons (); //Checks if any buttons are pressed

			} else {
				
				if (cursorReset == false) {
					cursorReset = true;
					Cursor.SetCursor(normalCursor, hotSpot, CursorMode.Auto);
					Cursor.visible = true;
				}

			}

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
				unitCost = 500;
				incomeGain = 50;
				break;
			case "Mercy":
				unitCost = 300;
				incomeGain = 35;
				break;
			default:
				break;

			}
			if (bank.getMoney () >= unitCost) {
				
				bank.subtractMoney (unitCost);
				bank.addIncome (incomeGain);

				sourceSFX.PlayOneShot (UIClickFX);

				CmdSendUnit (mySpawnID, unitName);

			} else {
				Debug.Log ("Not enough money to send!");
				sourceSFX.PlayOneShot(needMoneySound);
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
		if (Input.GetButtonDown ("Ctrl")) {
			Key_ctrl = true;
		}

		if (Input.GetButtonUp ("Ctrl")) {
			Key_ctrl = false;
		}

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
        if (Input.GetButtonDown("B"))
        { //If B is pressed, Enter Build this.mode
			buildButton.BuildToggle ();
        }

        if (Input.GetButtonDown("Z"))
        { //If Z is pressed, and Building Mode is enabled, -- Build wall
			if (this.mode == 1) {
				BuildWall ();
			} else if (this.mode == 2) {
				CheckSendUnit ("Potato");
			} else {
				Debug.Log ("Z was pressed but build mode is not right");
			}
           

        }

		if (Input.GetButtonDown("X"))
		{
			if (this.mode == 1) {
				BuildOrbTower ();
			} else if (this.mode == 2) {
				CheckSendUnit("Cloud");
			}
		}

		if (Input.GetButtonDown("C"))
		{
			if (this.mode == 1) {
				BuildCannonTower ();
			} else if (this.mode == 2) {
				
			}
		}
        
		if (Input.GetButtonDown("T"))
        {
			sendButton.SendToggle ();
        }

		if (Input.GetButtonDown("Q"))
		{
			bank.addMoney (5000);
		}

		if (Input.GetButtonDown("V"))
		{
			if (this.mode == 1) {
				BuildLaserTower ();
			} else if (this.mode == 2) {
			}
		}

		if (Input.GetButtonDown("F"))
		{
			if (this.mode == 1) {
				BuildIceTower ();
			} else if (this.mode == 2) {
				CheckSendUnit ("Mercy");
				
			}
		}

		if (Input.GetButtonDown("G"))
		{
			if (this.mode == 1) {
				BuildLightTower ();
			} else if (this.mode == 2) {
				CheckSendUnit ("Reinhardt");
			}
		}

		if (Input.GetButtonDown("H"))
		{
			if (this.mode == 1) {
				BuildMagicTower ();
			} else if (this.mode == 2) {
				
			}
		}

		if (Input.GetButtonDown ("M")) {

			if (Key_ctrl) {
				ToggleSounds ();
			}

		}


    }

	void ToggleSounds()
	{
		if (BGMusic.mute == true) {
			BGMusic.mute = false;
		} else {
			BGMusic.mute = true;
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

