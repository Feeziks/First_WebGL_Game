using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/PVE Board")]
public class SO_PVE_Board_State : ScriptableObject
{
  public List<DeployedUnit> units;
}
