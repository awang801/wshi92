using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MMMultiplayerButton : MonoBehaviour {

	public void LoadMultiplayerScene()
	{
		SceneManager.LoadScene ("Lobby");
	}
}
