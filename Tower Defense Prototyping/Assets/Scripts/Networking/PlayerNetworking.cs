using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerNetworking : NetworkBehaviour {

	[SyncVar] public string playerUniqueIdentity;
	public NetworkInstanceId playerNetID;
	private Transform myTransform;

	public GameObject myEnemy;

	public Text myTimerText;
	public Text myEnemyLivesText;

	GameManager gm;

	GameObject myCanvas;

	bool cmdAddPlayerSent = false;
		
	void Awake()
	{
		myTransform = transform;
		playerUniqueIdentity = myTransform.name;
		gm = GameObject.Find ("GameManager").GetComponent<GameManager>();

	}

	void Start () {

		myCanvas = transform.GetChild (0).gameObject;

		if (!isLocalPlayer) {
			
			myCanvas.SetActive (false);

			return;
		}

		Camera.main.transform.position = transform.position + new Vector3 (13.15f, 20f, 7.5f);

	}

	void Update()
	{

		if (myTransform.name == "" || myTransform.name == "Player(Clone)") {
			
			SetIdentity ();

		} else {
			if (isLocalPlayer) {

				if (cmdAddPlayerSent == false) {
					CmdTellServerToAddPlayer(myTransform.name);
					cmdAddPlayerSent = true;
				}

			}
		}

	}

	public override void OnStartLocalPlayer ()
	{
		GetNetIdentity ();
		UpdateMyTimerText ();
	}

	[Command]
	void CmdTellServerToAddPlayer(string myID)
	{
		gm.AddPlayer (myID);
	}

	[Client]
	void GetNetIdentity()
	{
		playerNetID = GetComponent<NetworkIdentity> ().netId;
		Debug.Log ("My Netid is : " + playerNetID);
		playerUniqueIdentity = MakeUniqueIdentity ();
		CmdTellServerMyIdentity (playerUniqueIdentity);
	}

	void SetIdentity()
	{
		if (!isLocalPlayer) {
			myTransform.name = playerUniqueIdentity;
		} else {
			myTransform.name = MakeUniqueIdentity();
		}

	}

	[ClientRpc]
	public void RpcAssignEnemies(string enemyID)
	{
		
		Debug.Log ("Enemy ID to find is : " + enemyID);
		if (isLocalPlayer) {
			myEnemy = GameObject.Find(enemyID);
			Debug.Log ("Enemy Assigned! " + myEnemy.transform.name);
			UpdateMyEnemyLivesText ();
		} else {
			Debug.Log ("Not local player, not assigning");
		}

	}

	[Command]
	void CmdTellServerMyIdentity(string name)
	{
		playerUniqueIdentity = name;
	}
		
	string MakeUniqueIdentity()
	{
		return "Player " + playerNetID.ToString();
	}

	void UpdateMyTimerText()
	{

		gm.timerText = myTimerText;

	}

	public void UpdateMyEnemyLivesText()
	{

		myEnemy.GetComponent<Lives> ().livesText = myEnemyLivesText;

	}
}
