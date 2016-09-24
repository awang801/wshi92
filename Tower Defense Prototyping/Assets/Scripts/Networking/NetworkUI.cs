using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class NetworkUI : NetworkBehaviour {

	void Start()
	{
		if (!localPlayerAuthority) {

			this.enabled = false;

		}
	}

}
