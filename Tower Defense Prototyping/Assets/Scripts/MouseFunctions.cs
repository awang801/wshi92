using UnityEngine;
using System.Collections;

public class MouseFunctions : MonoBehaviour {

	//This class manages mouse functions including selection on the grid, cursor hiding/showing
	BuildingFunctions bFunc;
	float camRayLength = 200f;

	GameObject selHighlight;
	bool selecting;

	public bool recentChangeInNavMesh;
	float recentChangeTimer;
	Transform recentChangePosition;

	Transform currentMouseGridLocation;


	int floorMask;

	void Awake()
	{
		bFunc = this.gameObject.GetComponent<BuildingFunctions> ();
	}

	void Start () {
		floorMask = LayerMask.GetMask ("TerrainBlocks");
	}
	
	// Update is called once per frame
	void Update () {
		
		CheckClick ();

	}

	void FixedUpdate()
	{
		
		CheckGrid ();

		if (Selecting == true) {
			MoveSelection ();
		}

		if (recentChangeInNavMesh == true) {
			recentChangeTimer += Time.fixedDeltaTime;
		}

		if (recentChangeTimer > .1f) {
			recentChangeInNavMesh = false;
			recentChangeTimer = 0;
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

	void CheckGrid()
	{
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit tileHit;

		Debug.DrawRay (Camera.main.transform.position, Input.mousePosition);
		if (Physics.Raycast (camRay, out tileHit, camRayLength, floorMask)) 
		{
			if (tileHit.transform.CompareTag ("TerrainBlock")) {
				currentMouseGridLocation = tileHit.transform;
			}
		}
	}

	void CheckClick()
	{
		if (Input.GetButtonDown("Fire1")) {
			if (Selecting == true) {
				string buildStructure = bFunc.TowerToBuild;
				Vector3 positionToBuild = currentMouseGridLocation.position;
				if (buildStructure == "Wall") {
					positionToBuild = currentMouseGridLocation.position;
				} else if (buildStructure == "Tower") {
					positionToBuild = currentMouseGridLocation.position + (Vector3.up*1.25f);
				}
				GameObject newStructure = ((GameObject)(Instantiate (Resources.Load (buildStructure), positionToBuild, Quaternion.identity)));
				recentChangeInNavMesh = true;
				recentChangeTimer = 0;
			}
		}
	}
	void MoveSelection()
	{
		if (SelHighlight != null && currentMouseGridLocation != null) {
			SelHighlight.transform.position = currentMouseGridLocation.position + (Vector3.up * 0.1f);
		}
	}

	public GameObject SelHighlight
	{
		get {
			return selHighlight;
		}
		set{
			selHighlight = value;
		}
	}

	public bool Selecting
	{
		get{
			return selecting;
			}
		set{
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
