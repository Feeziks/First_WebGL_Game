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


  public ShopManager sm;
}
