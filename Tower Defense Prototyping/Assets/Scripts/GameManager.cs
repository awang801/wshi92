using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	// Use this for initialization
	float incomeTimer;
	float incomeDelay = 5.00f; //Seconds
	public Text timerText;

	public GameObject[] player;
	Bank[] bank = new Bank[2];
	Lives[] lives = new Lives[2];


	void Start()
	{
		for (int i = 0; i < 2; i++) { //NUMBER OF PLAYERS HERE
			bank[i] = player[i].GetComponent<Bank>();
			lives [i] = player [i].GetComponent<Lives> ();
		}
		incomeTimer = incomeDelay;

	}

	// Update is called once per frame
	void Update () {
		incomeTimer = incomeTimer - Time.deltaTime;
		UpdateTimerText ();
		if (incomeTimer <= 0) {
			incomeTimer = incomeDelay;
			for (int i = 0; i < 2; i++) { //NUMBER OF PLAYERS HERE
				bank[i].giveIncome();
			}
				
		}
			
	}

	void UpdateTimerText()
	{
		timerText.text = incomeTimer.ToString ("F2");
	}
}
