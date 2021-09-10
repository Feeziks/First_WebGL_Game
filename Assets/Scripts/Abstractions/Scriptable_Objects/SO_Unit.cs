using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Unit")]
public class SO_Unit : ScriptableObject
{
  [Header("Identifiers")]
  public string unitName;
  public UnitID ID;
  public string unitToolTipText;

  [Header("Types")]
  public List<UnitTypes> unitTypes;
  public List<UnitClasses> unitClasses;

  [Header("Visuals")]
  public Sprite storeSprite;
  public GameObject[] unitPrefab = new GameObject[Constants.maxUnitLevel];
  public Animator animator;

  [Header("Unit Stats")]
  public UnitStats baseStats;

  [Header("Audio")]
  public int audio;
}
