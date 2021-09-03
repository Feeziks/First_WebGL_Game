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
  private bool roundTimerRunning = false;
  private bool timeBetwenRoundOccuring = true;
  private bool betweenRoundTimerRunning = false;

  private bool nextRoundStart = false;
  private bool thisRoundEnd = false;

  private EventManager eManager;

  #region Unity Methods

  private void Awake()
  {
    eManager = FindObjectOfType(typeof(EventManager)) as EventManager;
    InitPlayerStores();

    Constants.InitPVERounds();
  }

  private void Start()
  {
    InitPlayerGameBoardsAndBenches();
    InitCameraPosition();

    StartCoroutine("Tick");
    StartCoroutine("BetweenRoundTimer");
    LoadPVEBoards(Constants.pveRoundToBoardStateDict[currentRound]);
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

      if(thisRoundEnd)
      {
        thisRoundEnd = false;
        currentRound++;

        if (Constants.PveRounds.Contains(currentRound))
        {
          LoadPVEBoards(Constants.pveRoundToBoardStateDict[currentRound]);
        }
      }

    }
    else if(timeBetwenRoundOccuring)
    {
      if (nextRoundStart)
      {
        nextRoundStart = false;
      }
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

      foreach (DeployedUnit u in board.units)
      {
        enemyUnits.Add(Instantiate(u.unit.soUnit.unitPrefab[u.unit.unitLevel]));
        enemyUnits[enemyUnits.Count - 1].transform.position = DeployedUnitPositionToBoardPosition(u.position, playerBoard);
        enemyUnits[enemyUnits.Count - 1].name = "PVE_Round_" + currentRound + "_Enemy_Unit_" + (enemyUnits.Count - 1).ToString();
        enemyUnits[enemyUnits.Count - 1].transform.SetParent(playerBoard.transform);
      }
    }
  }

  private List<GameObject> GetAllChildren(GameObject parent)
  {
    List<GameObject> ret = new List<GameObject>();
    foreach(Transform t in parent.transform)
    {
      ret.Add(t.gameObject);
    }
    return ret;
  }

  private Vector3 DeployedUnitPositionToBoardPosition(Vector2 pos, GameObject gameBoard)
  {
    Vector3 ret = new Vector3();

    List<GameObject> gameBoardChildren = GetAllChildren(gameBoard);

    float xPos = gameBoardChildren[gameBoardChildren.Count - (int)(pos.x + pos.y * Constants.boardHeight)].transform.position.x;
    float yPos = 1f;
    float zPos = gameBoardChildren[gameBoardChildren.Count - (int)(pos.x + pos.y * Constants.boardHeight)].transform.position.z;

    ret = new Vector3(xPos, yPos, zPos);

    return ret;
  }
  #endregion

  #region Buttons

  public void RefreshShopButton()
  {
    players[0].sm.RefreshShop();
  }

  public void PurchaseFromShop(int index)
  {
    players[0].sm.PurchaseUnitByIndex(index);
  }

  public void PurchaseExp()
  {
    if (players[0].level < Constants.maxPlayerLevel)
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
    betweenRoundTimerRunning = true;
    timeBetwenRoundOccuring = true;
    float timer = Constants.timeBetweenRounds;

    while(timer > 0f)
    {
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
      timer -= 1f / Constants.tickRate;
      players[0].ui.UpdateTimer(timer);
    }

    betweenRoundTimerRunning = false;
    timeBetwenRoundOccuring = false;
    nextRoundStart = true;
    StartCoroutine("DuringRoundTimer");
    yield return null;
  }

  private IEnumerator DuringRoundTimer()
  {
    roundTimerRunning = true;
    roundOccuring = true;
    float timer = Constants.roundTimeout;

    //TODO: Round ends early if all enemies on all gameboard are defeated

    while (timer > 0f)
    {
      yield return new WaitForSecondsRealtime(1f / Constants.tickRate);
      timer -= 1f / Constants.tickRate;
      players[0].ui.UpdateTimer(timer);
    }

    roundTimerRunning = false;
    thisRoundEnd = true;
    roundOccuring = false;
    StartCoroutine("BetweenRoundTimer");
    yield return null;
  }

  #endregion
}
