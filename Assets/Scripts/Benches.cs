using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Benches : MonoBehaviour
{
  public List<Vector3> gameBoardPositions;
  public int benchWidth;

  public GameObject benchPrefab;

  #region unity Methods

  private void Awake()
  {
    int count = 0;
    foreach(Vector3 v3 in gameBoardPositions)
    {
      CreateBench(v3, count);
      count++;
    }
  }

  #endregion

  #region Helpers

  private void CreateBench(Vector3 v3, int count)
  {
    float xPos = v3.x * 2.33f;
    float zPos = v3.z;

    Vector3 centerHexMiddlePoint = new Vector3(xPos, 0f, zPos);

    GameObject benchGo = new GameObject("Bench_" + count.ToString());
    benchGo.transform.SetParent(transform);
    benchGo.transform.position = v3;
    benchGo.layer = LayerMask.NameToLayer("HexGrid");
    for(int i = 0; i < benchWidth; i++)
    {
      GameObject thisBenchHexGo = Instantiate(benchPrefab);
      thisBenchHexGo.name = "Bench_" + count.ToString() + "_Spot_" + i.ToString();
      thisBenchHexGo.transform.SetParent(benchGo.transform);

      float thisHexPosX = xPos;//(i - (benchWidth - 1) / 2f) * (benchPrefab.transform.localScale.x / 2f) + centerHexMiddlePoint.x;
      float thisHexPosZ = (i - (benchWidth - 1) / 2f) * benchPrefab.transform.localScale.y * 1.8f + zPos;

      thisBenchHexGo.transform.position = new Vector3(thisHexPosX, 0f, thisHexPosZ);

      thisBenchHexGo.layer = LayerMask.NameToLayer("HexGrid");
      thisBenchHexGo.tag = "Player" + count.ToString() + "Placeable";
    }
  }

  #endregion
}
