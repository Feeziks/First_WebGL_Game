using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Unit related types
[System.Serializable]
public class Unit
{
  public SO_Unit soUnit;
  public int unitLevel;
  public GameObject go;
  public Unit target;
  public UnitStatusType status;
  //TODO: Lots more
}


[System.Serializable]
public class UnitID
{
  [SerializeField]
  private UnitTier tier;
  [SerializeField]
  private int uniqueID;

  public UnitID(UnitTier unitTier, int ID)
  {
    tier = unitTier;
    uniqueID = ID;
  }

  public UnitTier GetTier()
  {
    return tier;
  }

  public int GetUniqueID()
  {
    return uniqueID;
  }
}

public enum UnitTier : int
{
  common    = 1,
  uncommon  = UnitTier.common + 1,
  rare      = UnitTier.uncommon + 1,
  legendary = UnitTier.rare + 1
}

public enum UnitClasses : int
{
  class_1,
  class_2
}
public enum UnitTypes : int
{
  type_1,
  type_2
}

[System.Serializable]
public class UnitBaseStats
{
  [Header("Health and Mana")]
  public float[] baseHealth = new float[Constants.maxUnitLevel];
  public float[] baseHealthRegen = new float[Constants.maxUnitLevel];
  public float[] baseMana = new float[Constants.maxUnitLevel];
  public float[] baseManaRegen = new float[Constants.maxUnitLevel];

  [Header("Defense")]
  public float[] baseArmor = new float[Constants.maxUnitLevel];
  public float[] baseMagicResist = new float[Constants.maxUnitLevel];

  [Header("Attack")]
  public float[] baseAttackDamage = new float[Constants.maxUnitLevel];
  public float[] baseAttackSpeed = new float[Constants.maxUnitLevel];
  public int[] baseAttackRange = new int[Constants.maxUnitLevel];
  public float[] baseCritChance = new float[Constants.maxUnitLevel];

  [Header("Magic")]
  public float[] baseMagicDamage = new float[Constants.maxUnitLevel];
  public float[] baseManaGainOnHit = new float[Constants.maxUnitLevel];  
}

[System.Serializable]
public class DeployedUnit
{
  public Vector2Int position;
  public Unit unit;
}

[System.Flags]
public enum UnitStatusType : int
{
  normal = 0,
  stunned = 1,
  silenced = 2,
  someOtherStatus = 3
}

#endregion

#region Item related types

[System.Serializable]
public class ItemID
{
  [SerializeField]
  private ItemTier tier;
  [SerializeField]
  private int uniqueID;

  public ItemID(ItemTier itemTier, int ID)
  {
    tier = itemTier;
    uniqueID = ID;
  }

  public ItemTier GetTier()
  {
    return tier;
  }

  public int GetUniqueID()
  {
    return uniqueID;
  }
}

public enum ItemTier : int
{
  common = 1,
  uncommon = ItemTier.common + 1,
  rare = ItemTier.uncommon + 1,
  legendary = ItemTier.rare + 1
}

#endregion