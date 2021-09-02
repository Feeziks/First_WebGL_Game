using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public Player[] players;
  public GameObject[] playerOneShopOptions;

  private EventManager eManager;

  #region Unity Methods

  private void Awake()
  {
    eManager = FindObjectOfType(typeof(EventManager)) as EventManager;
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
}
