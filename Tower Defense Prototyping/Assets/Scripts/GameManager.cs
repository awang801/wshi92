using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
	// Use this for initialization

	static int MaxNumPlayers = 2;
	float incomeTimer;
	float incomeDelay = 10.00f; //Seconds
	public Text timerText;

	AudioSource mainAudioSource;
	AudioClip moneySFX;

	public GameObject[] player = new GameObject[MaxNumPlayers];

	public PlayerNetworking[] playerPN = new PlayerNetworking[MaxNumPlayers];

	public string[] playerIDs = new string[MaxNumPlayers];

	public int playerNumber = 0;

	Bank[] bank = new Bank[MaxNumPlayers];
	Lives[] lives = new Lives[MaxNumPlayers];

	public bool gameRunning = false;
	public bool playerLost = false;

	bool IDsSynced = false;

	GameObject fireworks;

	void Awake()
	{
		mainAudioSource = Camera.main.GetComponent<AudioSource> ();
		moneySFX = Resources.Load<AudioClip> ("Sounds/money");
		fireworks = Resources.Load<GameObject> ("Fireworks");
	}


	// Update is called once per frame
	void Update () {

		if (gameRunning == true) {
			incomeTimer = incomeTimer - Time.deltaTime;
			UpdateTimerText ();

				if (incomeTimer <= 0) {
					incomeTimer = incomeDelay;
					for (int i = 0; i < MaxNumPlayers; i++) { //NUMBER OF PLAYERS HERE
						bank [i].giveIncome ();
						mainAudioSource.PlayOneShot (moneySFX);
					}
				}
			if (playerNumber < 2) {

				if (isServer) {

					RpcStopGame ();

				}

			}
		} else {

			if (playerNumber == 2 && !playerLost) {

				if (isServer) {
					IDsSynced = true;
					Debug.Log ("Calling RpcSyncPlayers and passing " + playerIDs[0] + " " + playerIDs[1]);
					RpcSyncPlayers (playerIDs);
					RpcStartGame ();

					UpdatePlayerEnemies ();
				}
			}

		}

			
	}

	[ClientRpc]
	void RpcStartGame()
	{		
		gameRunning = true;
		Initialize ();
		Debug.Log ("Found two players, Starting game");
	}

	[ClientRpc]
	void RpcStopGame()
	{		
		gameRunning = false;
		Debug.Log ("A player has left, stopping game");
	}

	[ClientRpc]
	void RpcPlayerLostWon(string playerIDLost, string playerIDWon)
	{
		playerLost = true;
		gameRunning = false;
		Debug.Log (playerIDLost + " has lost the game!");
		timerText.text = "GG!";
		fireworks = (GameObject)Instantiate (fireworks, PlayerFromID (playerIDWon).transform.position + new Vector3(13f, 0f, 17.5f), fireworks.transform.rotation);

	}

	void Initialize()
	{

		for (int i = 0; i < MaxNumPlayers; i++) { //NUMBER OF PLAYERS HERE
			bank[i] = player[i].GetComponent<Bank>();
			lives [i] = player [i].GetComponent<Lives> ();
		}
		incomeTimer = incomeDelay;
	}

	void UpdateTimerText()
	{
		timerText.text = incomeTimer.ToString ("F2");
	}

	void UpdatePlayerEnemies()
	{		
		
		playerPN [0].RpcAssignEnemies(playerPN[1].playerUniqueIdentity);
		playerPN [1].RpcAssignEnemies(playerPN[0].playerUniqueIdentity);

	}
		
	public void PlayerLoseWin(string loserID, string winnerID)
	{

		RpcPlayerLostWon (loserID, winnerID);

	}


	public void AddPlayer(string newID)
	{
		playerIDs [playerNumber] = newID;

		player [playerNumber] = PlayerFromID (newID);
		playerPN [playerNumber] = player [playerNumber].GetComponent<PlayerNetworking> ();
		playerNumber += 1;
		Debug.Log ("Added new ID " + newID);
		//StartCoroutine (AddPlayerWithDelay (newID, playerNumber));

		//playerNumber += 1;
	}


	IEnumerator AddPlayerWithDelay(string _newID, int _playerNumber)
	{
		
		while (true) {
			if (player [_playerNumber] == null) {
				Debug.Log ("Add player tick " + _newID);
				player [_playerNumber] = PlayerFromID (_newID);
				yield return new WaitForSeconds (0.1f);
			} else if (playerPN [_playerNumber] == null) {
				Debug.Log ("Add player tick 2" + _newID);
				playerPN [_playerNumber] = player [_playerNumber].GetComponent<PlayerNetworking> ();
				yield return new WaitForSeconds (0.1f);

			} else {
				
				Debug.Log ("Done adding new ID " + _newID);
				break;
			}
		}
	}

	/*
	[ClientRpc]
	void RpcAddPlayer(NetworkInstanceId _newID)
	{
		Debug.Log ("Adding Player " + _newID.ToString ());
		playerIDs [playerNumber] = _newID;
		player [playerNumber] = PlayerFromID (_newID);
		playerPN [playerNumber] = player [playerNumber].GetComponent<PlayerNetworking> ();
		playerNumber += 1;
	}
	*/


	[ClientRpc]
	public void RpcSyncPlayers(string[] _playerIDs)
	{
		if (isServer) {
			return;
		} else {

			StartCoroutine (SyncPlayersWithDelay (_playerIDs));

		}

	}

	IEnumerator SyncPlayersWithDelay(string[] _playerIDs)
	{

		bool done = false;
		int i = 0;
		playerIDs = _playerIDs;

		while (!done) {

			Debug.Log ("Coroutine running on playerNumber " + playerNumber + " playerID " + playerIDs[i]);
			player [playerNumber] = PlayerFromID (playerIDs [i]);

			if (player [playerNumber] != null) {
				playerPN [playerNumber] = player [playerNumber].GetComponent<PlayerNetworking> ();
				playerNumber += 1;
				i += 1;

				if (i >= MaxNumPlayers) {
					IDsSynced = true;
					done = true;
				}
			} else {
				yield return new WaitForSeconds (0.2f);
			}

		}

	}

	GameObject PlayerFromID(string id)
	{
		return GameObject.Find (id);
	}

	public int MyPlayerIndex(string id)
	{
		if (IDsSynced) {
			for (int i = 0; i < MaxNumPlayers; i++) {
				if (playerIDs [i] == id) {
					return i;
				}
			}
		}


		return -1;
	}

	public GameObject MyLocalPlayer()
	{

		for (int i = 0; i < MaxNumPlayers; i++) {
			if (playerPN [i] != null) {
				if (playerPN [i].IAmTheLocalPlayer ()) {
					return player [i];
				}
			}
		}

		return null;

	}
}
