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

	public NetworkInstanceId[] playerIDs = new NetworkInstanceId[MaxNumPlayers];

	public int playerNumber = 0;

	Bank[] bank = new Bank[MaxNumPlayers];
	Lives[] lives = new Lives[MaxNumPlayers];

	public bool gameRunning = false;

	void Awake()
	{
		mainAudioSource = Camera.main.GetComponent<AudioSource> ();
		moneySFX = Resources.Load<AudioClip> ("Sounds/money");
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

			if (playerNumber == 2) {

				if (isServer) {
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
		
		playerPN [0].RpcAssignEnemies(playerPN[1].playerNetID);
		playerPN [1].RpcAssignEnemies(playerPN[0].playerNetID);

	}
		



	public void AddPlayer(NetworkInstanceId newID)
	{
		playerIDs [playerNumber] = newID;
		player [playerNumber] = PlayerFromID (newID);
		playerPN [playerNumber] = player [playerNumber].GetComponent<PlayerNetworking> ();
		playerNumber += 1;
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
	public void RpcSyncPlayers(NetworkInstanceId[] _playerIDs)
	{
		if (isServer) {
			return;
		}
		playerIDs = _playerIDs;

		for (int i = 0; i < _playerIDs.Length; i++) {
			player [playerNumber] = PlayerFromID (playerIDs [i]);
			playerPN [playerNumber] = player [playerNumber].GetComponent<PlayerNetworking> ();
			playerNumber += 1;
		}
	}

	GameObject PlayerFromID(NetworkInstanceId id)
	{
		if (isServer) {
			return NetworkServer.FindLocalObject (id);
		} else {
			return ClientScene.FindLocalObject (id);
		}

	}
}
