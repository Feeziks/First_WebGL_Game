using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PVE Board")]
public class SO_PVE_Board_State : ScriptableObject
{
  public List<SO_Unit> units;
  public List<Vector2> positions;
  public List<int>     level;
}
