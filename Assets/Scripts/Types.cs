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
public class UnitBaseStats
{
  [Header("Health and Mana")]
  public float[] baseHealth = new float[3];
  public float[] baseHealthRegen = new float[3];
  public float[] baseMana = new float[3];
  public float[] baseManaRegen = new float[3];

  [Header("Defense")]
  public float[] baseArmor = new float[3];
  public float[] baseMagicResist = new float[3];

  [Header("Attack")]
  public float[] baseAttackDamage = new float[3];
  public float[] baseAttackSpeed = new float[3];
  public int[] baseAttackRange = new int[3];
  public float[] baseCritChance = new float[3];

  [Header("Magic")]
  public float[] baseMagicDamage = new float[3];
  public float[] baseManaGainOnHit = new float[3];  
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