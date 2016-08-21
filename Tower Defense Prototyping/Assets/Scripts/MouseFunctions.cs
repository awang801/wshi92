using UnityEngine;
using System.Collections;

//This class manages mouse functions including selection on the grid, cursor hiding/showing
public class MouseFunctions : MonoBehaviour
{

    AudioClip BuildFX;
    AudioSource sourceSFX;
    public GameObject newStructure;

    Grid grid;

	public GameObject player;
    Bank bank;

    KeyboardFunctions kFunc;

    float camRayLength = 200f;

    GameObject selHighlight;
    bool selecting;

    NavMeshPath path;

    Node currentMouseNode;

    Transform target; //Target of the enemies (need to change this in the future)

    int floorMask;

    bool buildWallMode;
    Node startWallModeNode;
    Node endWallModeNode;
    string buildWallModeXY;
    GameObject wallGhost;
    float wallsToBuild;

    string buildStructure;
    Vector3 positionToBuildStart;
    Vector3 positionToBuildEnd;

    void Awake()
    {
		kFunc = this.gameObject.GetComponent<KeyboardFunctions>();
        target = GameObject.Find("Destination 1").transform;
        grid = GetComponent<Grid>();

        bank = player.GetComponent<Bank>();

        BuildFX = (AudioClip)(Resources.Load("Sounds/BuildingPlacement", typeof(AudioClip)));
        sourceSFX = this.gameObject.GetComponent<AudioSource>();

    }


    void Start()
    {
        floorMask = LayerMask.GetMask("Terrain");
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {

        UpdateMouseNode();

        if (Selecting == true)
        {
            MoveSelection();
            CheckClick();
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

        if (buildStructure == "Tower")
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
                    currentMouseNode.Tower = ((GameObject)(Instantiate(Resources.Load("Towers/BasicOrbTower"), positionToBuildStart, Quaternion.identity)));
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
    void CheckClick()
    {


        if (Input.GetButtonDown("Fire1"))
        {

            buildStructure = kFunc.TowerToBuild;
            positionToBuildStart = currentMouseNode.worldPosition;
            if (buildStructure == "Wall")
            {
                startWallModeNode = currentMouseNode; 
                buildWallMode = true;
                wallGhost = ((GameObject)(Instantiate(Resources.Load("Walls/WallGhost"))));
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

        if (Input.GetButtonUp("Fire1"))
        {

            buildWallMode = false;
            if (bank.getMoney() - ((wallsToBuild+1) * 5 )>= 0)
            {
                if (grid.NodeLineContainsWall(startWallModeNode, endWallModeNode, buildWallModeXY) == false)
                {
                    Debug.Log("buildWallModeXY = " + buildWallModeXY);
                    Node tempStart = startWallModeNode;
                    tempStart.Wall = ((GameObject)(Instantiate(Resources.Load("Walls/WallConnector"), startWallModeNode.worldPosition, Quaternion.identity)));
                    bank.addMoney(-5);
                    sourceSFX.PlayOneShot(BuildFX);
                    for (int i = 0; i < Mathf.Abs(wallsToBuild); i++)
                    {
                        if (buildWallModeXY == "x")
                        {
                            if (wallsToBuild > 0)
                            {
                                tempStart = grid.NodeFromCoordinates(tempStart.gridX + 1, tempStart.gridY);
                            }
                            else
                            {
                                tempStart = grid.NodeFromCoordinates(tempStart.gridX - 1, tempStart.gridY);
                            }
                        }
                        else
                        {
                            if (wallsToBuild > 0)
                            {
                                tempStart = grid.NodeFromCoordinates(tempStart.gridX, tempStart.gridY + 1);
                            }
                            else
                            {
                                tempStart = grid.NodeFromCoordinates(tempStart.gridX, tempStart.gridY - 1);

                            }
                        }
                        tempStart.Wall = ((GameObject)(Instantiate(Resources.Load("Walls/WallConnector"), tempStart.worldPosition, Quaternion.identity)));
                        bank.addMoney(-5);
                    }
                    Debug.Log(bank.getMoney());
                    //newStructure.transform.localScale = wallGhost.transform.lossyScale;
                    //grid.setNodesAlongAxis (startWallModeNode, endWallModeNode, buildWallModeXY, true);


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

    }

    void MoveSelection()
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

    public bool Selecting
    {
        get
        {
            return selecting;
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
            selecting = value;
        }
    }


}
