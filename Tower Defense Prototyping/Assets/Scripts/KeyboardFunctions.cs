using UnityEngine;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class KeyboardFunctions : MonoBehaviour
{

    AudioClip UIBuildFX;
    AudioSource sourceSFX;

    MouseFunctions mFunc; //Reference to Mouse functions

    public string towerToBuild; //Contains name of Prefab to be instantiated

    public bool selectedTowerToBuild; //Boolean if tower has been selected

	public GameObject player;
	public GameObject spawnObject;
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

        UIBuildFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
        sourceSFX = this.gameObject.GetComponent<AudioSource>();

		spawner = spawnObject.GetComponent<SpawnUnit> ();
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
            mFunc.Selecting = false; //Shows mouse cursor
            Destroy(mFunc.SelHighlight); //Destroys the green highlight selection box
            sourceSFX.PlayOneShot(UIBuildFX);

        }
        else if (mode == 1 && selectedTowerToBuild == false)
        {
            //Cancels building mode
            buildButton.BuildToggle();
            Debug.Log("CANCEL BUILDING");
            mode = 0;
            sourceSFX.PlayOneShot(UIBuildFX);
        }
		else if (mode == 2)
		{
			//Cancels sending mode
			Debug.Log("CANCEL SENDING");
			mode = 0;
		}
    }


	//========================================================================================
	// Building Functions
	//========================================================================================
    public void Build()
    {
        if (mode == 0)
        {

            mode = 1;
            Debug.Log("BUILDING START");
            sourceSFX.PlayOneShot(UIBuildFX);

        }
    }

    public void CancelBuild()
    {
        if (selectedTowerToBuild == true)
        {
            selectedTowerToBuild = false;
            mFunc.Selecting = false;
            Destroy(mFunc.SelHighlight);
            sourceSFX.PlayOneShot(UIBuildFX);
        }

        if (mode == 1)
        {
            mode = 0;
            sourceSFX.PlayOneShot(UIBuildFX);
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
            sourceSFX.PlayOneShot(UIBuildFX);
            Debug.Log("BUILDING Z TOWER");
            towerToBuild = "Wall";
            SelectedTowerToBuild = true;
            mFunc.Selecting = true;
        }
    }

    public void BuildTower()
    {
        if (mode == 1)
        {
            if (SelectedTowerToBuild == false)
            {
                mFunc.SelHighlight = ((GameObject)(Instantiate(Resources.Load("UI/SelectionHighlight")))); //Creates green selection box
            }
            sourceSFX.PlayOneShot(UIBuildFX);
            Debug.Log("BUILDING X TOWER");
            towerToBuild = "Tower";
            SelectedTowerToBuild = true;
            mFunc.Selecting = true;

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
		} else if (mode == 2) {
			mode = 0;
			Debug.Log ("SENDING STOP");
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

        if (Input.GetButtonDown("Cancel"))
        { //If ESC is pressed

            //Cancels out of the layers of selection

            //Layers from shallow to deep are
            //1)Build Mode 
            // 2) Tower
            // 2) Wall

            //Cancels from deeper layers first, one layer per press

            Cancel();

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
				BuildTower ();
			} else if (mode == 2) {
				//Future different unit to send here
			}
		}
        else if (Input.GetButtonDown("T"))
        {
			SendToggle ();
        }


    }

    

}

