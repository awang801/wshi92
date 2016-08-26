﻿using UnityEngine;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class KeyboardFunctions : MonoBehaviour
{

    AudioClip UIClickFX;
    AudioSource sourceSFX;

    MouseFunctions mFunc; //Reference to Mouse functions

    public string towerToBuild; //Contains name of Prefab to be instantiated

    public bool selectedTowerToBuild; //Boolean if tower has been selected

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

	SpawnUnit spawner;

	public int mode = 0; 
	// 0 = idle (nothing happening)
	// 1 = build mode
	// 2 = send mode

    BuildButtonPress buildButton;
    void Awake()
    {
        //Set any references here
        mFunc = this.gameObject.GetComponent<MouseFunctions>();
        buildButton = GameObject.Find("BuildButtonText").GetComponent<BuildButtonPress>();

        UIClickFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
        sourceSFX = this.gameObject.GetComponent<AudioSource>();

		spawner = spawnObject.GetComponent<SpawnUnit> ();

		normalCursor = (Texture2D)Resources.Load ("Sprites/Arrow");
		buildCursor = (Texture2D)Resources.Load ("Sprites/BuildCursor");
		attackCursor = (Texture2D)Resources.Load ("Sprites/AttackCursor");

    }


    // Update is called once per frame
    void Update()
    {
        CheckButtons(); //Checks if any buttons are pressed
	}

    //Public functions for keypress actions so that they can be reused for the UI button presses

    public void Cancel()
    {
        if (selectedTowerToBuild == true)
        {
            //Cancels tower/wall location selection

            Debug.Log("CANCEL SELECTED TOWER");
            selectedTowerToBuild = false;
            mFunc.BuildSelecting = false; //Shows mouse cursor
            Destroy(mFunc.SelHighlight); //Destroys the green highlight selection box
            sourceSFX.PlayOneShot(UIClickFX);

        }
        else if (mode == 1 && selectedTowerToBuild == false)
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
        if (mode == 0)
        {
			Cursor.SetCursor(buildCursor, hotSpot, cursorMode);
            mode = 1;
            Debug.Log("BUILDING START");
            sourceSFX.PlayOneShot(UIClickFX);

        }
    }

    public void CancelBuild()
    {
        if (selectedTowerToBuild == true)
        {
            selectedTowerToBuild = false;
            mFunc.BuildSelecting = false;
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
            if (SelectedTowerToBuild == false)
            {
                mFunc.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
            }
            sourceSFX.PlayOneShot(UIClickFX);
            Debug.Log("BUILDING Z TOWER");
            towerToBuild = "Wall";
            SelectedTowerToBuild = true;
            mFunc.BuildSelecting = true;
        }
    }

    public void BuildOrbTower()
    {
        if (mode == 1)
        {
            if (SelectedTowerToBuild == false)
            {
                mFunc.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
            }
            sourceSFX.PlayOneShot(UIClickFX);
            Debug.Log("BUILDING X TOWER");
            towerToBuild = "OrbTower";
            SelectedTowerToBuild = true;
			mFunc.BuildSelecting = true;

        }
    }

	public void BuildCannonTower()
	{
		if (mode == 1)
		{
			if (SelectedTowerToBuild == false)
			{
				mFunc.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
			}
			sourceSFX.PlayOneShot(UIClickFX);
			Debug.Log("BUILDING C TOWER");
			towerToBuild = "CannonTower";
			SelectedTowerToBuild = true;
			mFunc.BuildSelecting = true;

		}
	}

	public bool SelectedTowerToBuild
	{
		get
		{
			return selectedTowerToBuild;
		}
		set
		{
			selectedTowerToBuild = value;
		}
	}

	public string TowerToBuild

	{
		get
		{
			return towerToBuild;
		}
		set
		{
			towerToBuild = value;
		}
	}

	//========================================================================================
	// Sending Functions
	//========================================================================================

	void SendToggle()
	{
		if (mode == 0) {
			mode = 2;
			Debug.Log ("SENDING START");
			Cursor.SetCursor(attackCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot(UIClickFX);
		} else if (mode == 2) {
			mode = 0;
			Debug.Log ("SENDING STOP");
			Cursor.SetCursor(normalCursor, hotSpot, cursorMode);
			sourceSFX.PlayOneShot(UIClickFX);
		}
	}

	void SendUnit(string unitName)
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
        else if (Input.GetButtonDown("Build"))
        { //If B is pressed, Enter Build mode
			if (mode != 2) {
				buildButton.BuildToggle ();
			}

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
			SendToggle ();
        }


    }

    

}

