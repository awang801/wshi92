using UnityEngine;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class KeyboardFunctions : MonoBehaviour
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

	public GameObject player;
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

	SpawnUnit spawner;

	public int mode = 0; 
	// 0 = idle (nothing happening)
	// 1 = build mode
	// 2 = send mode

    BuildButtonPress buildButton;
	SendButtonPress sendButton;

    void Awake()
    {
        mFunc = GetComponent<MouseFunctions>();
		bhandler = GetComponent<BuildHandler> ();

        buildButton = GameObject.Find("BuildButtonText").GetComponent<BuildButtonPress>();
		sendButton = GameObject.Find("SendButtonText").GetComponent<SendButtonPress>();

        UIClickFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
		sourceSFX = Camera.main.GetComponent<AudioSource>();

		spawner = spawnObject.GetComponent<SpawnUnit> ();

		normalCursor = (Texture2D)Resources.Load ("Sprites/Arrow");
		buildCursor = (Texture2D)Resources.Load ("Sprites/BuildCursor");
		attackCursor = (Texture2D)Resources.Load ("Sprites/AttackCursor");

		orbGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/OrbGhost"));
		cannonGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/CannonGhost"));
		laserGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/LaserGhost"));
		lightGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/LightGhost"));
		magicGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/MagicGhost"));
		iceGhost = (GameObject)(Resources.Load("Towers/TowerGhosts/IceGhost"));

		bank = player.GetComponent<Bank>();

    }


    // Update is called once per frame
    void Update()
    {
        CheckButtons(); //Checks if any buttons are pressed
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
        else if (mode == 1 && selectedObjectToBuild == false)
        {
            //Cancels building mode
            buildButton.BuildToggle();
            Debug.Log("CANCEL BUILDING");
            mode = 0;
            sourceSFX.PlayOneShot(UIClickFX);
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
        }
		else if (mode == 2)
		{
			//Cancels sending mode
			sendButton.SendToggle();
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("CANCEL SENDING");
			mode = 0;
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
		}
    }


	//========================================================================================
	// Building Functions
	//========================================================================================
    public void Build()
    {
		if (mode == 0 || mode == 2)
        {
			Cursor.SetCursor(buildCursor, hotSpot, cursorMode);
            mode = 1;
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

        if (mode == 1)
        {
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
            mode = 0;
            sourceSFX.PlayOneShot(UIClickFX);
        }
    }

    public void BuildWall()
    {
        if (mode == 1)
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
        if (mode == 1)
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
		if (mode == 1)
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
		if (mode == 1)
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
		if (mode == 1)
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
		if (mode == 1)
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
		if (mode == 1)
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
		if (mode == 0 || mode == 1) {
			mode = 2;
			Debug.Log ("SENDING START");
			Cursor.SetCursor (attackCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot (UIClickFX);
		}
	}

	public void CancelSend()
	{
		if (mode == 2) {
			mode = 0;
			Debug.Log ("SENDING STOP");
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot(UIClickFX);
		}
	}

	public void SendUnit(string unitName)
	{
		if (mode == 2) {
			spawner.Spawn (unitName);
		}
	}

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
        { //If B is pressed, Enter Build mode
			buildButton.BuildToggle ();
        }
        else if (Input.GetButtonDown("Z"))
        { //If Z is pressed, and Building Mode is enabled, -- Build wall
			if (mode == 1) {
				BuildWall();
			} else if (mode == 2) {
				SendUnit("Potato");
			}
           

        }
		else if (Input.GetButtonDown("X"))
		{
			if (mode == 1) {
				BuildOrbTower ();
			} else if (mode == 2) {
				SendUnit("Cloud");
			}
		}
		else if (Input.GetButtonDown("C"))
		{
			if (mode == 1) {
				BuildCannonTower ();
			} else if (mode == 2) {
			}
		}
        else if (Input.GetButtonDown("T"))
        {
			sendButton.SendToggle ();
        }
		else if (Input.GetButtonDown("Q"))
		{
			Debug.Log ("You hacker.... sombra confirmed, here have some money");
			bank.addMoney (50);
		}
		else if (Input.GetButtonDown("V"))
		{
			if (mode == 1) {
				BuildLaserTower ();
			} else if (mode == 2) {
			}
		}
		else if (Input.GetButtonDown("F"))
		{
			if (mode == 1) {
				BuildIceTower ();
			} else if (mode == 2) {
			}
		}
		else if (Input.GetButtonDown("G"))
		{
			if (mode == 1) {
				BuildLightTower ();
			} else if (mode == 2) {
			}
		}
		else if (Input.GetButtonDown("H"))
		{
			if (mode == 1) {
				BuildMagicTower ();
			} else if (mode == 2) {
			}
		}


    }

    

}

