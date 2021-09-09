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
  private Dictionary<SO_Unit, int> unitsToUpgradeAfterRoundFinishes = new Dictionary<SO_Unit, int>();

  [Header("Units")]
  public GameObject unitParent;
  public List<GameObject> deployedUnits;
  public List<GameObject> benchedUnits;
  private Dictionary<GameObject, Unit> gameObjectToUnitDict = new Dictionary<GameObject, Unit>();
  public SO_Unit[] storeUnits = new SO_Unit[5];
  public List<SO_Unit> maxLevelUnits = new List<SO_Unit>();

  [Header("Bench & GameBoard")]
  public GameObject bench;
  public List<GameObject> benchChildren;
  private Dictionary<GameObject, GameObject> benchChildToBenchedUnitDict = new Dictionary<GameObject, GameObject>();
  public GameObject gameBoard;
  private List<GameObject> gameBoardChildren;

  public ShopManager sm;
  public UIManager ui;

  public Player()
  {
  }

  #region EXP And Money
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

  #endregion

  #region Units gained and leveling

  public void UnitGained(SO_Unit toAdd, int unitLevel)
  {
    //TODO: How to add a new unit

    GameObject newUnit = GameObject.Instantiate(toAdd.unitPrefab[0]);
    newUnit.transform.parent = unitParent.transform;
    gameObjectToUnitDict[newUnit] = newUnit.GetComponent(typeof(Unit)) as Unit;
    gameObjectToUnitDict[newUnit].soUnit = toAdd;
    gameObjectToUnitDict[newUnit].unitLevel = unitLevel;
    gameObjectToUnitDict[newUnit].status = UnitStatusType.normal;
    benchedUnits.Add(newUnit);
    PlaceUnitInFirstEmptyBenchPosition(newUnit);

    if (CheckForUnitUpgrade(toAdd, unitLevel))
    {
      UpgradeUnit(toAdd, unitLevel);
    }

  }

  private void PlaceUnitInFirstEmptyBenchPosition(GameObject thisUnit)
  {
    if(benchChildren.Count == 0)
    {
      benchChildren = GameManager.GetAllChildren(bench);
      foreach(GameObject go in benchChildren)
      {
        benchChildToBenchedUnitDict[go] = null;
      }
    }

    foreach(GameObject go in benchChildren)
    {
      if(benchChildToBenchedUnitDict[go] == null)
      {
        benchChildToBenchedUnitDict[go] = thisUnit;
        thisUnit.transform.position = go.transform.position + new Vector3(0f, thisUnit.transform.localScale.y / 2f + 1f, 0f);
        return;
      }
    }
  }

  private void PlaceUnitAtDeployedPosition(GameObject thisUnit)
  {
    //thisUnit.unit.go.transform.position = gameBoardChildren[thisUnit.position.x + thisUnit.position.y * Constants.boardWidth].transform.position;
  }

  private bool CheckForUnitUpgrade(SO_Unit newUnit, int unitLevel)
  {
    int numSameUnits = 0;
    foreach(GameObject go in benchedUnits)
    {
      if(gameObjectToUnitDict[go].soUnit == newUnit && gameObjectToUnitDict[go].unitLevel == unitLevel)
      {
        numSameUnits++;
      }
    }
    foreach(GameObject go in deployedUnits)
    {
      if (gameObjectToUnitDict[go].soUnit == newUnit && gameObjectToUnitDict[go].unitLevel == unitLevel)
      {
        numSameUnits++;
      }
    }

    if (numSameUnits >= Constants.unitsToLevelUp)
      return true;

    return false;
  }

  private void UpgradeUnit(SO_Unit toUpgrade, int unitLevel)
  {
    List<GameObject> benchedUnitsToUpgradeWith = new List<GameObject>();
    List<GameObject> deployedUnitsToUpgradeWith = new List<GameObject>();

    //First check if we are upgrading benched units only
    foreach(GameObject go in benchedUnits)
    {
      if (gameObjectToUnitDict[go].soUnit == toUpgrade && gameObjectToUnitDict[go].unitLevel == unitLevel)
      {
        benchedUnitsToUpgradeWith.Add(go);
      }
    }

    if(benchedUnitsToUpgradeWith.Count == Constants.unitsToLevelUp)
    {
      foreach(GameObject go in benchedUnitsToUpgradeWith)
      {
        benchedUnits.Remove(go);
        benchChildToBenchedUnitDict.Remove(go);
        gameObjectToUnitDict.Remove(go);
        GameObject.DestroyImmediate(go);
      }

      GameObject newUnit = GameObject.Instantiate(toUpgrade.unitPrefab[unitLevel + 1]);
      newUnit.transform.parent = unitParent.transform;
      gameObjectToUnitDict[newUnit] = newUnit.GetComponent(typeof(Unit)) as Unit;
      gameObjectToUnitDict[newUnit].soUnit = toUpgrade;
      gameObjectToUnitDict[newUnit].unitLevel = unitLevel + 1;
      gameObjectToUnitDict[newUnit].status = UnitStatusType.normal;
      benchedUnits.Add(newUnit);
      PlaceUnitInFirstEmptyBenchPosition(newUnit);

      if (unitLevel + 1 == Constants.maxUnitLevel - 1)
      {
        maxLevelUnits.Add(toUpgrade);
      }
      else if (CheckForUnitUpgrade(toUpgrade, unitLevel + 1))
      {
        UpgradeUnit(toUpgrade, unitLevel + 1);
      }

      return;
    }

    //Next check if we need to consume any deployed units for the upgrade
    foreach(GameObject go in deployedUnits)
    {
      if(gameObjectToUnitDict[go].soUnit == toUpgrade && gameObjectToUnitDict[go].unitLevel == unitLevel)
      {
        deployedUnitsToUpgradeWith.Add(go);
      }
    }

    if(benchedUnitsToUpgradeWith.Count + deployedUnitsToUpgradeWith.Count == Constants.unitsToLevelUp)
    {
      if(roundOccuring)
      {
        upgradeAfterRoundFinishes = true;
        unitsToUpgradeAfterRoundFinishes[toUpgrade] = unitLevel;
        return;
      }

      foreach (GameObject go in benchedUnitsToUpgradeWith)
      {
        benchedUnits.Remove(go);
        benchChildToBenchedUnitDict.Remove(go);
        gameObjectToUnitDict.Remove(go);
        GameObject.DestroyImmediate(go);
      }

      Vector3 pos = deployedUnitsToUpgradeWith[0].transform.position;

      foreach(GameObject go in deployedUnitsToUpgradeWith)
      {
        deployedUnits.Remove(go);
        gameObjectToUnitDict.Remove(go);
        GameObject.DestroyImmediate(go);
      }

      GameObject newUnit = GameObject.Instantiate(toUpgrade.unitPrefab[unitLevel + 1]);
      newUnit.transform.parent = unitParent.transform;
      gameObjectToUnitDict[newUnit] = newUnit.GetComponent(typeof(Unit)) as Unit;
      gameObjectToUnitDict[newUnit].soUnit = toUpgrade;
      gameObjectToUnitDict[newUnit].unitLevel = unitLevel + 1;
      gameObjectToUnitDict[newUnit].status = UnitStatusType.normal;
      newUnit.transform.position = pos;
      deployedUnits.Add(newUnit);

      if (unitLevel + 1 == Constants.maxUnitLevel)
      {
        maxLevelUnits.Add(toUpgrade);
      }
      else if (CheckForUnitUpgrade(toUpgrade, unitLevel + 1))
      {
        UpgradeUnit(toUpgrade, unitLevel + 1);
      }

      return;
    }
  }

  #endregion

  #region Messages To Recieve

  public void RoundEnd()
  {
    if(upgradeAfterRoundFinishes)
    {
      foreach(KeyValuePair<SO_Unit, int> kvp in unitsToUpgradeAfterRoundFinishes)
      {
        UpgradeUnit(kvp.Key, kvp.Value);
      }

      unitsToUpgradeAfterRoundFinishes.Clear();
      upgradeAfterRoundFinishes = false;
    }
  }

  #endregion

  public void ReturnToBoard()
  {
    //TODO: Not sure what this is for honestly
  }
}
