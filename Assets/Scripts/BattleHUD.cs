using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{

	public Text nameTxt;
	public Text lvlTxt;
	public Slider health;

	public void SetHUD(Unit unit)
	{
		nameTxt.text = unit.unitName;
		lvlTxt.text = "Lvl " + unit.unitLevel;
		health.maxValue = unit.maxHP;
		health.value = unit.currentHP;
	}

	public void SetHP(int hp)
	{
		health.value = hp;
	}

}