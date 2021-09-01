using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
  public GameObject hexPrefab;
  public int gridWidth = 11;
  public int gridHeight = 11;

  private Vector3 hexScale = new Vector3(20f, 20f, 1f);
  private float gap = 0.1f;

  private Vector3 startPos;

  #region Unity Methods

  private void Awake()
  {
    InitStartPosition();
    CreateGrid();
  }

  private void Start()
  {

  }
  #endregion

  #region Grid Creation

  private void InitStartPosition()
  {
    startPos = Vector3.zero;

    float offset = 0f;
    if(gridHeight / 2 % 2 != 0)
    {
      offset = hexScale.x / 2f;
    }

    float x = -hexScale.x * (gridWidth / 2) - offset;
    float z = hexScale.y * 0.75f * (gridHeight / 2);

    startPos = new Vector3(x, 0f, z);
  }

  private void CreateGrid()
  {
    for(int x = 0; x < gridWidth; x++)
    {
      for (int y = 0; y < gridHeight; y++)
      {
        GameObject thisHex = Instantiate(hexPrefab);
        thisHex.name = "Hex[" + x.ToString() + ", " + y.ToString() + "]";
        thisHex.transform.parent = transform;
        thisHex.transform.position = CalculateWorldPosition(x, y);
      }
    }
  }

  private Vector3 CalculateWorldPosition(int x, int y)
  {
    Vector3 ret = Vector3.zero;

    float zOffset = 0f;
    if(x % 2 != 0)
    {
      zOffset = hexScale.y * 0.4375f;
    }

    float xPos = x * hexScale.x * 0.75f + gap;
    float zPos = y * hexScale.y * 0.875f + gap + zOffset;

    ret = new Vector3(xPos, 0f, zPos);
    return ret;
  }

  #endregion
}
