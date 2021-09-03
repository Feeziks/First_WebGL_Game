using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Item")]
public class SO_Item : ScriptableObject
{
  [Header("Identifiers")]
  public string itemName;
  public ItemID ID;

  [Header("Visuals")]
  public Sprite itemSprite;

  [Header("Affects")]
  public int temp;
}
