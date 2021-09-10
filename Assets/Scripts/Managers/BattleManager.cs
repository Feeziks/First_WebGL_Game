﻿using System.Collections;
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

  #endregion

  #region Getters / Setters

  public Player[] GetPlayers()
  {
    Player[] ret = new Player[]{ player1, player2};
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
    if(pvp)
    {
      player1AnyAlive = player1.BattleTick();
      player2AnyAlive = player2.BattleTick();

      if(!player1AnyAlive)
      {
        player1.lastRoundResult = Constants.roundLost;
        player2.lastRoundResult = Constants.roundWon;
      }
      else if(!player2AnyAlive)
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
      //TODO: PVE BattleTick
    }

    return player1AnyAlive & player2AnyAlive;

  }

  public void RemovePVEUnits()
  {
    foreach(GameObject go in GameManager.GetAllChildren(gameBoard))
    {
      if (go.CompareTag(Constants.PVETag))
      {
        GameObject.Destroy(go);
      }
    }
  }

  #endregion
}
