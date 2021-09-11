using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region Unit related types
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
public class UnitStats
{
  [Header("Health and Mana")]
  public float[] health = new float[Constants.maxUnitLevel];
  public float[] healthRegen = new float[Constants.maxUnitLevel];
  public float[] mana = new float[Constants.maxUnitLevel];
  public float[] manaRegen = new float[Constants.maxUnitLevel];

  [Header("Defense")]
  public float[] armor = new float[Constants.maxUnitLevel];
  public float[] magicResist = new float[Constants.maxUnitLevel];

  [Header("Attack")]
  public float[] attackDamage = new float[Constants.maxUnitLevel];
  public float[] attackSpeed = new float[Constants.maxUnitLevel];
  public int[]   attackRange = new int[Constants.maxUnitLevel];
  public float[] critChance = new float[Constants.maxUnitLevel];

  [Header("Magic")]
  public float[] magicDamage = new float[Constants.maxUnitLevel];
  public float[] manaGainOnHit = new float[Constants.maxUnitLevel];  
  public float[] abilityCooldown = new float[Constants.maxUnitLevel];
}

[System.Serializable]
public class AttackTypeToFloat
{
  public AttackTypes[] attackType = new AttackTypes[3];
  public float[] percent = new float[3];
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

#region Battle related Types

[System.Serializable]
public enum UnitTargetingType : int
{
  closest = 0,
  furthest = 1,
  highestHp = 2,
  lowestHp = 3,
  highestDps = 4,
  lowestDps = 5
}

public enum AttackTypes
{
  physical,
  magical,
  tru
}

public class UnitDamageDealtType
{
  public float damageValue;
  public Dictionary<AttackTypes, float> damageByType = new Dictionary<AttackTypes, float>();

  public UnitDamageDealtType(float dVal, Dictionary<AttackTypes, float> dByType)
  {
    damageValue = dVal;
    damageByType = dByType;
  }
}

#endregion