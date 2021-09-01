using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Player[] players;


  #region Unity Methods

  private void Awake()
  {
    InitPlayerStores();
  }

  #endregion


  #region helpers

  private void InitPlayerStores()
  {
    foreach(Player p in players)
    {
      p.sm.RefreshShop();
    }
  }

  #endregion
}
