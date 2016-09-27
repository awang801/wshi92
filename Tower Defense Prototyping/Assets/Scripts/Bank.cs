using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Bank : NetworkBehaviour{

    private int money;

	private int income;

	public Text bankText;
	public Text incomeText;


	public Bank()
	{
		money = 100;
		income = 10;
	}

	public Bank(int startMoney, int startIncome)
	{
		money = startMoney;
		income = startIncome;
	}

	void Update()
	{
		if (isLocalPlayer) {
			if (bankText == null) {
				bankText = GameObject.Find ("MoneyText").GetComponent<Text> ();
				updateMoneyText ();

				Debug.Log ("Bank Textset to local player!");
			}
				
			if (incomeText == null) {
				incomeText = GameObject.Find ("IncomeText").GetComponent<Text> ();
				updateIncomeText ();
				Debug.Log ("Income Text set to local player!");
			}

		}

	}

	//===============================================================
	//MONEY FUNCTIONS
	//===============================================================
    public void setMoney(int amount)
    {
        money = amount;
		updateMoneyText ();
    }
    public void addMoney(int amount)
    {
        money = money + amount;
		updateMoneyText ();
    }


    public void subtractMoney(int amount)
    {
        money = money - amount;
		updateMoneyText ();
    }

    public int getMoney()
    {
        return money;
    }

	//===============================================================
	//INCOME FUNCTIONS
	//===============================================================
	public void setIncome(int amount)
	{
		income = amount;
		updateIncomeText ();
	}
	public void addIncome(int amount)
	{
		income = income + amount;
		updateIncomeText ();
	}


	public void subtractIncome(int amount)
	{
		income = income - amount;
		updateIncomeText ();
	}

	public int getIncome()
	{
		return income;
	}

	public void giveIncome()
	{
		money += income;
		updateMoneyText ();
	}

	//===============================================================
	//TEXT FUNCTIONS
	//===============================================================
	void updateMoneyText()
	{
		if (bankText != null) {
			bankText.text = money.ToString();
		}
	}

	void updateIncomeText()
	{
		if (incomeText != null) {
			incomeText.text = income.ToString();
		}
	}
}
