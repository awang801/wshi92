using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetManager : NetworkManager {

	GameObject[] players = new GameObject[4];

	int numConnected = 0;

	int prevSpawnLocation = 0;

	/*
	public override void OnServerAddPlayer (NetworkConnection conn, short playerControllerId)
	{
		GameObject player;

		if (playerSpawnMethod == PlayerSpawnMethod.RoundRobin) {

			NetworkStartPosition[] startPositions = FindObjectsOfType (typeof(NetworkStartPosition)) as NetworkStartPosition[];

			Vector3 pos = Vector3.zero;
			if (startPositions.Length > 0) {

				if (startPositions.Length < prevSpawnLocation + 1) {
					
					prevSpawnLocation = 0;
					pos = startPositions [prevSpawnLocation].transform.position;


				} else {

					pos = startPositions [prevSpawnLocation].transform.position;
					prevSpawnLocation += 1;

				}



			} 

			player = (GameObject)Instantiate (playerPrefab, pos, Quaternion.identity);

		} else {
			
			player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);

		}


		players [numConnected] = player;
		PlayerNetworking pn = player.GetComponent<PlayerNetworking> ();
		pn.myPlayerID = numConnected;
		numConnected += 1;

		NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);

	}
*/
}
