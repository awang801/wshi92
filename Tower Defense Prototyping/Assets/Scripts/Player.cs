using UnityEngine;
using System.Collections;

public class Player {

	Lives lives;
	Bank bank;

	public Player()
	{
		lives = new Lives(30);
		bank = new Bank(100, 10);
	}

	public Player(int startLives, int startMoney, int startIncome)
	{
		lives = new Lives(startLives);
		bank = new Bank(startMoney, startIncome);
	}


}
