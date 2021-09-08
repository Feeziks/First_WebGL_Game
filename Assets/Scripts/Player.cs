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
  public bool roundOccuring;
  private bool upgradeAfterRoundFinishes;

  [Header("Units")]
  public List<DeployedUnit> deployedUnits;
  public List<Unit> benchedUnits;
  public SO_Unit[] storeUnits = new SO_Unit[5];
  public List<SO_Unit> maxLevelUnits = new List<SO_Unit>();

  [Header("Bench & GameBoard")]
  public GameObject bench;
  public List<GameObject> benchChildren;
  public GameObject gameBoard;
  private List<GameObject> gameBoardChildren;

  public ShopManager sm;
  public UIManager ui;

  public Player()
  {
  }

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

  public void UnitPurchased(Unit toAdd)
  {
    if(benchChildren.Count == 0)
    {
      benchChildren = GameManager.GetAllChildren(bench);
    }

    //Check if we have enough units to upgrade to the next tier
    if(CheckForUnitUpgrade(toAdd))
    {
      UpgradeUnit(toAdd);
    }
    else
    {
      benchedUnits.Add(toAdd);
      PlaceUnitInFirstEmptyBenchPosition(toAdd);
    }
  }

  private void PlaceUnitInFirstEmptyBenchPosition(Unit thisUnit)
  {
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

  private void PlaceUnitAtDeployedPosition(DeployedUnit thisUnit)
  {
    thisUnit.unit.go.transform.position = gameBoardChildren[thisUnit.position.x + thisUnit.position.y * Constants.boardWidth].transform.position;
  }

  private bool CheckForUnitUpgrade(Unit newUnit)
  {
    int numSameUnits = 1;
    foreach(DeployedUnit u in deployedUnits)
    {
      if (u.unit.soUnit == newUnit.soUnit && u.unit.unitLevel == newUnit.unitLevel)
        numSameUnits++;
    }

    foreach(Unit u in benchedUnits)
    {
      if(u.soUnit == newUnit.soUnit && u.unitLevel == newUnit.unitLevel)
      {
        numSameUnits++;
      }
    }

    if(numSameUnits >= Constants.unitsToLevelUp)
    {
      return true;
    }

    return false;
  }

  private void UpgradeUnit(Unit toUpgrade)
  {
    List<Unit> benchedUnitsToReplace = new List<Unit>();
    List<DeployedUnit> deployedUnitsToReplace = new List<DeployedUnit>();
    benchedUnitsToReplace.Add(toUpgrade);
    //Collect the units that will be "consumed" in this upgrade
    foreach(Unit u in benchedUnits)
    {
      if(u.soUnit == toUpgrade.soUnit && u.unitLevel == toUpgrade.unitLevel)
      {
        benchedUnitsToReplace.Add(u);
      }
    }

    //If we have enough just in the benched units reaplce them all right now
    if (benchedUnitsToReplace.Count == Constants.unitsToLevelUp)
    {
      foreach (Unit u in benchedUnitsToReplace)
      {
        GameObject.DestroyImmediate(u.go);
        benchedUnits.Remove(u);
      }

      Unit newUnit = new Unit();
      newUnit.soUnit = toUpgrade.soUnit;
      newUnit.unitLevel = toUpgrade.unitLevel + 1;
      if (newUnit.unitLevel == Constants.maxUnitLevel)
        maxLevelUnits.Add(newUnit.soUnit);
      newUnit.go = GameObject.Instantiate(toUpgrade.soUnit.unitPrefab[newUnit.unitLevel - 1]);
      //benchedUnits.Add(newUnit);
      PlaceUnitInFirstEmptyBenchPosition(newUnit);
      UnitPurchased(newUnit);
      return;
    }

    //If we have enough but it 'uses' any number of deployed units must check if we are currently in a round
    foreach(DeployedUnit u in deployedUnits)
    {
      if(u.unit.soUnit == toUpgrade.soUnit && u.unit.unitLevel == toUpgrade.unitLevel)
      {
        deployedUnitsToReplace.Add(u);
      }
    }

    if(!roundOccuring)
    {
      //If not currently in a round do the upgrade right away with mix of deployed and benched units
      if(benchedUnitsToReplace.Count + deployedUnitsToReplace.Count == Constants.unitsToLevelUp)
      {
        Unit newUnit = new Unit();
        newUnit.soUnit = toUpgrade.soUnit;
        newUnit.unitLevel = toUpgrade.unitLevel + 1;
        if (newUnit.unitLevel == Constants.maxUnitLevel)
          maxLevelUnits.Add(newUnit.soUnit);
        newUnit.go = GameObject.Instantiate(toUpgrade.soUnit.unitPrefab[newUnit.unitLevel - 1]);
        //Put this unit in the position of the first deployed unit we are replacing
        newUnit.go.transform.position = deployedUnitsToReplace[0].unit.go.transform.position;
        DeployedUnit newDeployedUnit = new DeployedUnit();
        newDeployedUnit.unit = newUnit;
        newDeployedUnit.position = deployedUnitsToReplace[0].position;
        deployedUnits.Add(newDeployedUnit);

        foreach(Unit u in benchedUnitsToReplace)
        {
          GameObject.Destroy(u.go);
          benchedUnits.Remove(u);
        }

        foreach(DeployedUnit du in deployedUnitsToReplace)
        {
          GameObject.Destroy(du.unit.go);
          deployedUnitsToReplace.Remove(du);
        }

        UnitPurchased(newUnit);
      }
    }

    //If currently in a round "consume" the benched units and set a flag to perform an upgrade at the end of the round
    //TODO: This

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
