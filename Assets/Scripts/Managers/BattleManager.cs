using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
  #region Properties

  private GameObject gameBoard;
  private Player player1;
  private Player player2;

  private bool pvp;
  private bool pve;

  public List<GameObject> pveUnitsThisRound;

  #endregion

  #region Getters / Setters

  public Player[] GetPlayers()
  {
    Player[] ret = new Player[] { player1, player2 };
    return ret;
  }

  public void SetPlayer1(Player p)
  {
    player1 = p;
  }

  public void SetPlayer2(Player p)
  {
    player2 = p;
  }

  public bool IsPVE()
  {
    return pve;
  }

  public bool IsPVP()
  {
    return pvp;
  }

  public void SetPVP()
  {
    pvp = true;
    pve = false;
  }

  public void SetPVE()
  {
    pvp = false;
    pve = true;
  }

  public List<GameObject> GetPVEUnits()
  {
    if (pveUnitsThisRound != null && pveUnitsThisRound.Count != 0)
    {
      return pveUnitsThisRound;
    }

    pveUnitsThisRound = new List<GameObject>();
    foreach (GameObject go in GameManager.GetAllChildren(gameBoard))
    {
      if (go.CompareTag(Constants.PVETag))
      {
        pveUnitsThisRound.Add(go);
      }
    }
    return pveUnitsThisRound;
  }

  public void SetPVEEnemyUnits()
  {
    foreach (GameObject go in GetPVEUnits())
    {
      Unit thisUnit = go.GetComponent(typeof(Unit)) as Unit;
      thisUnit.enemyUnits = player1.deployedUnits;
    }
  }

  #endregion

  #region Constructors

  public BattleManager(GameObject board, Player p1, Player p2)
  {
    gameBoard = board;
    player1 = p1;
    player2 = p2;
    pvp = true;
    pve = false;
  }

  public BattleManager(GameObject board, Player p1)
  {
    gameBoard = board;
    player1 = p1;
    player2 = null;
    pvp = false;
    pve = true;
  }

  #endregion

  #region Battle Methods

  //The only thing that does stuff tbh
  public bool Tick()
  {
    bool player1AnyAlive = false;
    bool player2AnyAlive = false;
    bool pveAnyAlive = true;
    //Run the battle
    if (pvp)
    {
      player1AnyAlive = player1.BattleTick();
      player2AnyAlive = player2.BattleTick();

      if (!player1AnyAlive)
      {
        player1.lastRoundResult = Constants.roundLost;
        player2.lastRoundResult = Constants.roundWon;
      }
      else if (!player2AnyAlive)
      {
        player1.lastRoundResult = Constants.roundWon;
        player2.lastRoundResult = Constants.roundLost;
      }
      else
      {

      }
    }
    else
    {
      player1AnyAlive = player1.BattleTick();
      player2AnyAlive = PVEBattleTick();

      if(!player1AnyAlive)
      {
        player1.lastRoundResult = Constants.roundLost;
      }
      else
      {
        player1.lastRoundResult = Constants.roundWon;
      }
    }

    return player1AnyAlive & player2AnyAlive;

  }

  public bool PVEBattleTick()
  {
    bool ret = false;
    foreach (GameObject go in GetPVEUnits())
    {
      Unit u = go.GetComponent(typeof(Unit)) as Unit;
      ret |= u.BattleTick();
    }
    return ret;
  }

  public void RemovePVEUnits()
  {
    foreach (GameObject go in GetPVEUnits())
    {
      GameObject.Destroy(go);
    }
    pveUnitsThisRound.Clear();
  }

  #endregion
}
