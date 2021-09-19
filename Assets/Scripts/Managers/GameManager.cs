using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Player[] players;
  public int numPlayersAlive;
  public GameObject[] playerOneShopOptions;

  public GameObject gameBoardParent;
  public GameObject benchParent;

  public GameObject cameraTarget;

  public int currentRound = 1;

  private bool tickUpdate = false;
  private bool roundOccuring = false;
  private bool endRoundEarly = false;
  private bool timeBetwenRoundOccuring = true;

  private List<BattleManager> battleManagers = new List<BattleManager>();

  private EventManager eManager;

  #region Unity Methods

  private void Awake()
  {
    eManager = FindObjectOfType(typeof(EventManager)) as EventManager;
    InitPlayerStores();
    Constants.InitPVERounds();

    UIManager ui = FindObjectOfType(typeof(UIManager)) as UIManager;
    ui.p0 = players[0];

    foreach(Player p in players)
    {
      GameObject unitParent = new GameObject("Player_" + p.playerName + "_Units");
      unitParent.transform.position = Vector3.zero;
      unitParent.transform.localScale = Vector3.one;
      unitParent.transform.parent = transform;
      p.unitParent = unitParent;
    }

    numPlayersAlive = Constants.numPlayers;
  }

  private void Start()
  {
    InitPlayerGameBoardsAndBenches();
    InitCameraPosition();

    InitBattleManagers();

    StartCoroutine("Tick");
    StartCoroutine("BetweenRoundTimer");
  }

  private void Update()
  {
    if(tickUpdate)
    {
      tickUpdate = false;
      HandleTick();
    }
  }

  #endregion

  #region The Bread And Butter
  private void HandleTick()
  {
    if(roundOccuring)
    {
      bool roundStillRunning = false;
      foreach(BattleManager bm in battleManagers)
      {
        roundStillRunning |= bm.Tick();
      }

      if(!roundStillRunning)
      {
        endRoundEarly = true;
      }
    }
    else if(timeBetwenRoundOccuring)
    {
      //TODO: Do we need to do anything
    }
  }

  #endregion

  #region helpers

  private void InitPlayerStores()
  {
    foreach(Player p in players)
    {
      p.sm = new ShopManager(p);
      p.sm.Init();
      if (p == players[0])
      {
        p.sm.SetShopOptions(playerOneShopOptions);
      }
      p.sm.RefreshShop();
    }
  }

  private void InitPlayerGameBoardsAndBenches()
  {
    for(int i = 0; i < players.Length; i++)
    {
      players[i].bench = benchParent.transform.GetChild(i).gameObject;
      players[i].gameBoard = gameBoardParent.transform.GetChild(i).gameObject;
    }
  }

  private void InitCameraPosition()
  {
    cameraTarget.transform.position = new Vector3(players[0].bench.transform.position.x - 200f, 0f, players[0].bench.transform.position.z);
  }

  private void LoadPVEBoards(SO_PVE_Board_State board)
  {
    foreach(Player p in players)
    {
      List<GameObject> enemyUnits = new List<GameObject>();
      GameObject playerBoard = p.gameBoard;

      for(int idx = 0; idx < board.units.Count; idx++)
      {
        GameObject thisEnemy = Instantiate(board.units[idx].unitPrefab[board.level[idx]]);
        thisEnemy.name = "PVE_Round_" + currentRound + "_Enemy_Unit_" + idx.ToString();
        thisEnemy.transform.SetParent(playerBoard.transform);
        thisEnemy.tag = Constants.PVETag;
        thisEnemy.transform.position = PVEUnitPositionToBoardPosition(board.positions[idx], playerBoard);
        Unit thisEnemyUnit = thisEnemy.GetComponent(typeof(Unit)) as Unit;
        thisEnemyUnit.soUnit = board.units[idx];
        thisEnemyUnit.unitLevel = board.level[idx];
        thisEnemyUnit.status = UnitStatusType.normal;
        thisEnemyUnit.SetRoundStartHex(playerBoard.transform.GetChild(playerBoard.transform.childCount - (int)(board.positions[idx].x + board.positions[idx].y * Constants.boardWidth)).gameObject);
        thisEnemyUnit.SetStatsAtRoundStart();
        enemyUnits.Add(thisEnemy);
      }
    }
  }

  public static List<GameObject> GetAllChildren(GameObject parent)
  {
    List<GameObject> ret = new List<GameObject>();
    foreach(Transform t in parent.transform)
    {
      ret.Add(t.gameObject);
    }
    return ret;
  }

  private Vector3 PVEUnitPositionToBoardPosition(Vector2 pos, GameObject gameBoard)
  {
    Vector3 ret = new Vector3();

    List<GameObject> gameBoardChildren = GetAllChildren(gameBoard);

    //TODO: This doesnt work on round 3 for some reason - seems like the X and Y values are backwards or something idk

    int index = (int)(pos.x + pos.y * Constants.boardWidth) + 2; // the + 2 accounts for the parents and stuff idk

    float xPos = gameBoardChildren[gameBoardChildren.Count - index].transform.position.x;
    float yPos = 1f;
    float zPos = gameBoardChildren[gameBoardChildren.Count - index].transform.position.z;

    ret = new Vector3(xPos, yPos, zPos);

    return ret;
  }

  private void InitBattleManagers()
  {
    foreach(Player p in players)
    {
      BattleManager thisBattleManager = new BattleManager(p.gameBoard, p);
      thisBattleManager.SetPVE();
      battleManagers.Add(thisBattleManager);
    }
  }

  private List<Player> GetPlayersForThisRound()
  {
    List<Player> ret = new List<Player>();

    foreach(Player p in players)
    {
      if (p.health > 0)
      {
        ret.Add(p);
      }
    }
    return ret;
  }

  #endregion

  #region Buttons

  public void RefreshShopButton()
  {
    if(players[0].money >= Constants.storeRefreshPrice)
      players[0].sm.RefreshShop();
  }

  public void PurchaseFromShop(int index)
  {
    players[0].sm.PurchaseUnitByIndex(index);
  }

  public void PurchaseExp()
  {
    if (players[0].money >= Constants.expPurchasePrice && players[0].level < Constants.maxPlayerLevel)
      players[0].sm.PurchaseExp();
  }

  #endregion

  #region Coroutines

  private IEnumerator Tick()
  {
    while(true)
    {
      tickUpdate = true;
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
    }
  }

  private IEnumerator BetweenRoundTimer()
  {
    timeBetwenRoundOccuring = true;
    float timer = Constants.timeBetweenRounds;

    players[0].ui.UpdateRoundState(RoundState.timeBetweenRounds);

    foreach(Player p in players)
    {
      p.UpdateMoney(Constants.storeRefreshPrice);
      p.sm.RefreshShop();
    }


    if (Constants.PveRounds.Contains(currentRound))
    {
      LoadPVEBoards(Constants.pveRoundToBoardStateDict[currentRound]);
      int idx = 0;
      foreach (BattleManager bm in battleManagers)
      {
        players[idx].bm = bm;
        bm.SetPlayer1(players[idx++]);
        bm.SetPVE();
        bm.pveUnitsThisRound = bm.GetPVEUnits();
      }
    }
    else
    {
      int numFights = Mathf.CeilToInt((float)numPlayersAlive / 2f);
      List<Player> playersForThisRound = GetPlayersForThisRound();

      //TODO: How to handle an odd number of players remaining

      foreach(BattleManager bm in battleManagers)
      {
        if (numFights > 0)
        {
          bm.SetPVP();
          bm.enabledThisRound = true;
          int player1Index = (int)Random.Range(0f, playersForThisRound.Count - 1);
          Player p1 = playersForThisRound[player1Index];
          playersForThisRound.Remove(p1);
          int player2Index = (int)Random.Range(0f, playersForThisRound.Count - 1);
          Player p2 = playersForThisRound[player2Index];
          playersForThisRound.Remove(p2);

          bm.SetPlayer1(p1);
          bm.SetPlayer2(p2);
          bm.SetGameBoard(p1.gameBoard);

          p1.bm = bm;
          p2.bm = bm;

          numFights--;
        }
        else
        {
          bm.enabledThisRound = false;
        }
      }
    }

    while (timer >= 1f / Constants.tickRate)
    {
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
      timer -= 1f / Constants.tickRate;
      players[0].ui.UpdateTimer(timer);
    }

    timeBetwenRoundOccuring = false;
    StartCoroutine("DuringRoundTimer");
    yield return null;
  }

  private IEnumerator DuringRoundTimer()
  {
    roundOccuring = true;
    float timer = Constants.roundTimeout;

    players[0].ui.UpdateRoundState(RoundState.roundOccuring);

    foreach (Player p in players)
    {
      p.roundOccuring = true;
      p.RoundStart();
    }

    if(Constants.PveRounds.Contains(currentRound))
    {
      int idx = 0;
      foreach(BattleManager bm in battleManagers)
      {
        bm.SetGameBoard(players[idx++].gameBoard);
        bm.SetPVEEnemyUnits();
        bm.SetPVEGameBoard();
      }
    }

    while (timer >= 1f / Constants.tickRate)
    {
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
      timer -= 1f / Constants.tickRate;
      players[0].ui.UpdateTimer(timer);

      if(endRoundEarly)
      {
        timer = 0f;
        endRoundEarly = false; //Reset flag
      }
    }

    if(Constants.PveRounds.Contains(currentRound))
    {
      foreach(BattleManager bm in battleManagers)
      {
        bm.RemovePVEUnits();
      }
    }

    roundOccuring = false;
    StartCoroutine("PaddingTimer");
    yield return null;
  }

  private IEnumerator PaddingTimer()
  {
    float timer = Constants.paddingTime;

    players[0].ui.UpdateRoundState(RoundState.paddingTime);

    foreach (Player p in players)
    {
      p.roundOccuring = false;
      p.RoundEnd();
    }

    while (timer >= 1f / Constants.tickRate)
    {
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
      timer -= 1f / Constants.tickRate;
      players[0].ui.UpdateTimer(timer);
    }

    currentRound++;
    StartCoroutine("BetweenRoundTimer");
  }

  #endregion
}
