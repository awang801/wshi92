using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Bank:MonoBehaviour{
    private int money = 100;
	private int income = 0;

	public Text bankText;
	public Text incomeText;

	void Start()
	{
		updateMoneyText ();
		updateIncomeText ();
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
			bankText.text = "Money: " + money;
		}
	}

	void updateIncomeText()
	{
		if (incomeText != null) {
			incomeText.text = "Income: " + income;
		}
	}
}
