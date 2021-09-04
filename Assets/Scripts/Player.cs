using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
  [Header("Identifiers")]
  public string playerName;
  public int playerId;
  public bool human;

  [Header("Data")]
  public int health = 100;
  public int exp;
  public int level;
  public int money;

  [Header("Units")]
  public List<DeployedUnit> deployedUnits;
  public List<Unit> benchedUnits;
  public SO_Unit[] storeUnits = new SO_Unit[5];

  [Header("Bench & GameBoard")]
  public GameObject bench;
  public List<GameObject> benchChildren;
  public GameObject gameBoard;
  private List<GameObject> gameBoardChildren;

  public ShopManager sm;
  public UIManager ui;

  public void GainExp(int amount)
  {
    if (level == Constants.maxPlayerLevel)
      return;

    exp += amount;

    if(exp == Constants.expPerLevel[level - 1])
    {
      exp = 0;
      level++;
    }

    if(ui)
    {
      ui.UpdateExp(level, exp);
    }
  }

  public void UpdateMoney(int amount)
  {

  }

  public void AddToBench(Unit toAdd)
  {
    if(benchChildren.Count == 0)
    {
      benchChildren = GameManager.GetAllChildren(bench);
    }

    benchedUnits.Add(toAdd);

    PlaceUnitInFirstEmptyBenchPosition();
  }

  private void PlaceUnitInFirstEmptyBenchPosition()
  {
    Unit thisUnit = benchedUnits[benchedUnits.Count - 1];
    bool empty = false;
    foreach(GameObject go in benchChildren)
    {
      empty = true;
      foreach(Transform t in go.transform)
      {
        if(t.CompareTag(Constants.PlayerUnitTag))
        {
          empty = false;
        }
      }

      if(empty)
      {
        thisUnit.go.transform.parent = go.transform;
        thisUnit.go.transform.position = go.transform.position + new Vector3(0f, 1f, 0f);
      }
    }
  }

  public void ReturnToBoard()
  {
    foreach (DeployedUnit u in deployedUnits)
    {
      if (gameBoardChildren == null)
      {
        gameBoardChildren = GameManager.GetAllChildren(gameBoard);
      }

      float xPos = gameBoardChildren[gameBoardChildren.Count - (int)(u.position.x + u.position.y * Constants.boardHeight)].transform.position.x;
      float yPos = 1f;
      float zPos = gameBoardChildren[gameBoardChildren.Count - (int)(u.position.x + u.position.y * Constants.boardHeight)].transform.position.z;

      u.unit.go.transform.position = new Vector3(xPos, yPos, zPos);   
    }
  }
}
