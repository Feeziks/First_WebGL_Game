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
  public int lastRoundResult;
  public List<int> roundHistory = new List<int>();
  private int currentWinStreak = 0;
  private int currentLossStreak = 0;
  private bool upgradeAfterRoundFinishes;
  private Dictionary<SO_Unit, int> unitsToUpgradeAfterRoundFinishes = new Dictionary<SO_Unit, int>();

  [Header("Units")]
  public GameObject unitParent;
  public List<GameObject> deployedUnits;
  public List<GameObject> benchedUnits;
  public Dictionary<GameObject, Unit> gameObjectToUnitDict = new Dictionary<GameObject, Unit>();
  public SO_Unit[] storeUnits = new SO_Unit[5];
  public List<SO_Unit> maxLevelUnits = new List<SO_Unit>();

  [Header("Bench & GameBoard")]
  public GameObject bench;
  public List<GameObject> benchChildren;
  public Dictionary<GameObject, GameObject> benchChildToBenchedUnitDict = new Dictionary<GameObject, GameObject>();
  public GameObject gameBoard;
  private List<GameObject> gameBoardChildren = new List<GameObject>();
  private Dictionary<Vector2Int, GameObject> placeableHexes = new Dictionary<Vector2Int, GameObject>();

  public ShopManager sm;
  public UIManager ui;
  public BattleManager bm;

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
    money += amount;
    if(ui)
      ui.UpdateMoney(money);
  }

  #endregion

  #region Units gained and leveling

  public void UnitGained(SO_Unit toAdd, int unitLevel)
  {
    UpdateMoney(-1 * (int)toAdd.ID.GetTier());

    GameObject newUnit = GameObject.Instantiate(toAdd.unitPrefab[0]);
    newUnit.transform.parent = unitParent.transform;
    gameObjectToUnitDict[newUnit] = newUnit.GetComponent(typeof(Unit)) as Unit;
    gameObjectToUnitDict[newUnit].soUnit = toAdd;
    gameObjectToUnitDict[newUnit].unitLevel = unitLevel;
    gameObjectToUnitDict[newUnit].status = UnitStatusType.normal;
    gameObjectToUnitDict[newUnit].owner = this;
    benchedUnits.Add(newUnit);
    PlaceUnitInFirstEmptyBenchPosition(newUnit);

    if (CheckForUnitUpgrade(toAdd, unitLevel))
    {
      UpgradeUnit(toAdd, unitLevel);
    }

  }

  public void SellUnit(GameObject unit)
  {
    //Calculate unit cost
    Unit u = unit.GetComponent(typeof(Unit)) as Unit;
    int moneyReturned = Mathf.Max(Mathf.FloorToInt(Mathf.Pow(Constants.unitsToLevelUp, u.unitLevel) * (int)u.soUnit.ID.GetTier() * 3f/4f), 1);
    UpdateMoney(moneyReturned);

    if(benchedUnits.Contains(unit))
    {
      benchedUnits.Remove(unit);
      //TODO: Remove from the benchedunit Dictionary
      foreach(GameObject go in benchChildren)
      {
        if(benchChildToBenchedUnitDict[go] == unit)
        {
          benchChildToBenchedUnitDict[go] = null;
          break;
        }
      }
    }

    if(deployedUnits.Contains(unit))
    {
      deployedUnits.Remove(unit);
    }

    //TODO: Return the correct number of cards to the card pool manager
    int numCardToReturn = (int)Mathf.Pow(Constants.unitsToLevelUp, u.unitLevel);

    for(; numCardToReturn > 0; numCardToReturn--)
    {
      CardPoolManager.instance.ReturnCard(u.soUnit);
    }

    if (maxLevelUnits.Contains(u.soUnit))
      maxLevelUnits.Remove(u.soUnit);

    GameObject.DestroyImmediate(unit);
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
        benchChildToBenchedUnitDict[go] = null;
        gameObjectToUnitDict.Remove(go);
        GameObject.DestroyImmediate(go);
      }

      GameObject newUnit = GameObject.Instantiate(toUpgrade.unitPrefab[unitLevel + 1]);
      newUnit.transform.parent = unitParent.transform;
      gameObjectToUnitDict[newUnit] = newUnit.GetComponent(typeof(Unit)) as Unit;
      gameObjectToUnitDict[newUnit].soUnit = toUpgrade;
      gameObjectToUnitDict[newUnit].unitLevel = unitLevel + 1;
      gameObjectToUnitDict[newUnit].status = UnitStatusType.normal;
      gameObjectToUnitDict[newUnit].owner = this;
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

        if(unitLevel + 1 == Constants.maxUnitLevel - 1)
        {
          maxLevelUnits.Add(toUpgrade);
        }

        return;
      }

      foreach (GameObject go in benchedUnitsToUpgradeWith)
      {
        benchedUnits.Remove(go);
        benchChildToBenchedUnitDict[go] = null;
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
      gameObjectToUnitDict[newUnit].owner = this;
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

    GainExp(1);

    UpdateWinLossHistoryAndStreaks();

    int moneyToEarn = 1; //Base gain is 1 gold; Win - Lose Streak values are in Constants; gain an additional 1 gold for every 5 gold you are holding as interest capped at 5
    moneyToEarn += (int)Mathf.Min(money / 5f, 5); // Interest
    if (currentWinStreak > 0)
    {
      moneyToEarn += Constants.winStreakMoneyGain[(int)Mathf.Min(currentWinStreak, 10)];
    }
    else if (currentLossStreak > 0)
    {
      moneyToEarn += Constants.lossStreakMoneyGain[(int)Mathf.Min(currentWinStreak, 10)];
    }

    UpdateMoney(moneyToEarn);

    //TODO: Return All units back to their deployed locations

    //TODO: Return any units that died
  }

  private void UpdateWinLossHistoryAndStreaks()
  {
    roundHistory.Add(lastRoundResult);
    int streakLength = GetStreakLength();
    //Check if we won or lost the last round
    if (lastRoundResult == Constants.roundWon)
    {
      currentWinStreak = streakLength;
      currentLossStreak = 0;
    }
    else if(lastRoundResult == Constants.roundLost)
    {
      currentWinStreak = 0;
      currentLossStreak = streakLength;
    }
  }

  private int GetStreakLength()
  {
    int ret = 0;
    for(int i = roundHistory.Count - 1; i > 0; i--)
    {
      if (roundHistory[i] == lastRoundResult)
      {
        ret++;
      }
      else
        break;
    }
    return ret;
  }

  public void RoundStart()
  {
    //Check how many units are deployed vs how many CAN be deployed
    if(deployedUnits.Count < level)
    {
      int diff = level - deployedUnits.Count;
      for(; diff > 0; diff--)
      {
        if(benchedUnits.Count > 0)
        {
          MoveUnitToBoard();
        }
      }
    }

    if(deployedUnits.Count > level)
    {
      for(int diff = deployedUnits.Count - level; diff > 0; diff--)
      {
        RemoveUnitFromBoard();
      }
    }

    //Tell our units about the enemy units
    List<GameObject> enemyUnits = GetEnemyUnitsList();
    foreach(GameObject go in deployedUnits)
    {
      gameObjectToUnitDict[go].enemyUnits = enemyUnits;
    }
  }

  private List<GameObject> GetEnemyUnitsList()
  {
    List<GameObject> ret = new List<GameObject>();

    if (bm.IsPVP())
    {
      Player[] players = bm.GetPlayers();
      Player opponent = players[0] == this ? players[1] : players[0];

      foreach (GameObject go in opponent.deployedUnits)
      {
        ret.Add(go);
      }
    }
    else
    {
      ret = bm.GetPVEUnits();
    }

    return ret;
  }

  private void MoveUnitToBoard()
  {
    //Get the hex that our benched unit should move to
    if(gameBoardChildren.Count == 0)
    {
      gameBoardChildren = GameManager.GetAllChildren(gameBoard);
      InitPlaceableHexes();
    }

    foreach(Vector2Int idx in Constants.placeableHexesOrder)
    {
      GameObject hexToPlace = placeableHexes[idx];
      bool empty = true;
      foreach(GameObject go in deployedUnits)
      {
        if(go.transform.position.x == hexToPlace.transform.position.x && go.transform.position.z == hexToPlace.transform.position.z)
        {
          empty = false;
          break;
        }
      }

      if(empty)
      {
        GameObject thisUnit = benchedUnits[0];
        benchedUnits.Remove(thisUnit);
        benchChildToBenchedUnitDict[thisUnit] = null;
        thisUnit.transform.position = hexToPlace.transform.position + new Vector3(0f, thisUnit.transform.localScale.y + 1f, 0f);
        deployedUnits.Add(thisUnit);
        gameObjectToUnitDict[thisUnit].SetRoundStartHex(hexToPlace);
        break;
      }
    }
  }

  private void RemoveUnitFromBoard()
  {
    if(benchedUnits.Count == Constants.benchWidth)
    {
      GameObject thisUnit = deployedUnits[deployedUnits.Count - 1];
      deployedUnits.Remove(thisUnit);
      PlaceUnitInFirstEmptyBenchPosition(thisUnit);
    }
    else
    {
      GameObject thisUnit = deployedUnits[deployedUnits.Count - 1];
      deployedUnits.Remove(thisUnit);
      SellUnit(thisUnit);
    }
  }

  private void InitPlaceableHexes()
  {
    for(int x = 0; x < Constants.boardWidth; x++)
    {
      for(int y = 0; y < 3; y++)
      {
        Vector2Int pos = new Vector2Int(x, y);
        placeableHexes[pos] = gameBoardChildren[x + y];
      }
    }
  }

  public bool BattleTick()
  {
    bool anyAlive = false;

    foreach(GameObject go in deployedUnits)
    {
      anyAlive |= gameObjectToUnitDict[go].BattleTick();
    }

    return anyAlive; 
  }

  #endregion

  public void ReturnToBoard()
  {
    //TODO: Not sure what this is for honestly
  }
}
