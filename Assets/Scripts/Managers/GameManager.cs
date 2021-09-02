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

  private EventManager eManager;

  #region Unity Methods

  private void Awake()
  {
    eManager = FindObjectOfType(typeof(EventManager)) as EventManager;
    InitPlayerStores();
  }

  private void Start()
  {
    InitPlayerGameBoardsAndBenches();
    InitCameraPosition();
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
