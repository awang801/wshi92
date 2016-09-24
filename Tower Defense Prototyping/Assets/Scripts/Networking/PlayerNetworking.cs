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

	GameManager gm;

	GameObject myCanvas;
		
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

		}

	}

	public override void OnStartLocalPlayer ()
	{
		GetNetIdentity ();
		CmdTellServerToAddPlayer(playerNetID);
		UpdateMyTimerText ();
	}

	[Command]
	void CmdTellServerToAddPlayer(NetworkInstanceId myID)
	{
		gm.AddPlayer (myID);
	}

	[Client]
	void GetNetIdentity()
	{
		playerNetID = GetComponent<NetworkIdentity> ().netId;
		CmdTellServerMyIdentity (MakeUniqueIdentity());
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
	public void RpcAssignEnemies(NetworkInstanceId enemyID)
	{
		if (isLocalPlayer) {
			myEnemy = NetworkServer.FindLocalObject (enemyID);
			Debug.Log ("Enemy Assigned!");
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
}
