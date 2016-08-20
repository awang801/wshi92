using UnityEngine;
using System.Collections;

//This class checks if user has pressed keys
//ATTACHED TO: GameManager

public class BuildingFunctions : MonoBehaviour
{

    AudioClip UIBuildFX;
    AudioSource sourceSFX;

    MouseFunctions mFunc; //Reference to Mouse functions

    public bool isBuilding; //Toggles Build Mode

    public string towerToBuild; //Contains name of Prefab to be instantiated

    public bool selectedTowerToBuild; //Boolean if tower has been selected

    BuildButtonPress buildButton;
    void Awake()
    {
        //Set any references here
        mFunc = this.gameObject.GetComponent<MouseFunctions>();
        buildButton = GameObject.Find("BuildButtonText").GetComponent<BuildButtonPress>();

        UIBuildFX = (AudioClip)(Resources.Load("Sounds/UIButtonclick", typeof(AudioClip)));
        sourceSFX = this.gameObject.GetComponent<AudioSource>();
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
        else if (isBuilding == true && selectedTowerToBuild == false)
        {
            //Cancels building mode
            buildButton.BuildToggle();
            Debug.Log("CANCEL BUILDING");
            isBuilding = false;
            sourceSFX.PlayOneShot(UIBuildFX);

        }
    }

    public void Build()
    {
        if (isBuilding == false)
        {

            isBuilding = true;
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

        if (isBuilding == true)
        {
            isBuilding = false;
            sourceSFX.PlayOneShot(UIBuildFX);
        }
    }

    public void BuildWall()
    {
        if (isBuilding == true)
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
        if (isBuilding == true)
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

            buildButton.BuildToggle();

        }
        else if (Input.GetButtonDown("Z"))
        { //If Z is pressed, and Building Mode is enabled, -- Build wall

            BuildWall();

        }
        else if (Input.GetButtonDown("X"))
        { //If X is pressed, and Building Mode is enabled, -- Build basic tower

            BuildTower();
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

}

