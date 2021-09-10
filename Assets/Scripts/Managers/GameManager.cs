using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Player[] players;
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

    if (Constants.PveRounds.Contains(currentRound))
    {
      LoadPVEBoards(Constants.pveRoundToBoardStateDict[currentRound]);
      int idx = 0;
      foreach (BattleManager bm in battleManagers)
      {
        bm.SetPlayer1(players[idx++]);
        bm.SetPVE();
      }
    }
    else
    {
      //TODO Determine which 2 players are in this fight
      foreach(BattleManager bm in battleManagers)
      {
        bm.SetPVP();
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

    //TODO: Round ends early if all enemies on all gameboard are defeated
    foreach (Player p in players)
    {
      p.roundOccuring = true;
      p.RoundStart();
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
