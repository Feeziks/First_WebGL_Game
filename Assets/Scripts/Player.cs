using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
  [Header("Identifiers")]
  public string playerName;
  public int playerId;

  [Header("Data")]
  public int exp;
  public int level;
  public int money;

  [Header("Units")]
  public List<DeployedUnit> deployedUnits;
  public List<Unit> benchedUnits;
  public SO_Unit[] storeUnits = new SO_Unit[5];

  public ShopManager sm;
  public UIManager ui;

  public void GainExp(int amount)
  {
    if (level == Constants.maxPlayerLevel)
      return;

    exp += amount;

    if(exp == Constants.expPerLevel[level - 1])
    {
      exp = 0;
      level++;
    }

    if(ui)
    {
      ui.UpdateExp(level, exp);
    }
  }

  public void UpdateMoney(int amount)
  {

  }
}
