using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Player[] players;
  public GameObject[] playerOneShopOptions;

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
      p.sm = new ShopManager(p);
      p.sm.Init();
      if (p == players[0])
      {
        p.sm.SetShopOptions(playerOneShopOptions);
      }
      p.sm.RefreshShop();
    }
  }

  #endregion

  #region Buttons

  public void RefreshShopButton()
  {
    players[0].sm.RefreshShop();
  }

  #endregion
}
