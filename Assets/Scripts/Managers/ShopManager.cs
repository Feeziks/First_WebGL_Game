using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager
{
  private Player p;

  private GameObject[] shopOptions;
  private Dictionary<GameObject, SO_Unit> shopOptionToUnit;
  private Dictionary<GameObject, Image> shopOptionsToImage;

  #region Constructor / Getters / Setters

  public ShopManager(Player play)
  {
    p = play;
  }

  public ShopManager(Player play, GameObject[] playerShopOptions)
  {
    p = play;
    shopOptions = playerShopOptions;
  }

  public void SetShopOptions(GameObject[] options)
  {
    shopOptions = options;
    Init();
  }

  #endregion

  #region Helpers

  public void Init()
  {
    if(p == null)
    {
      Debug.LogError("ShopManager cannot be initialized without a player!");
    }
    shopOptionsToImage = new Dictionary<GameObject, Image>();
    InitializeShopOptionsToImageDict();

    shopOptionToUnit = new Dictionary<GameObject, SO_Unit>();
    InitializeShopOptionsToUnitDict();
  }

  private void InitializeShopOptionsToImageDict()
  {
    if (shopOptions == null)
      return;

    foreach(GameObject go in shopOptions)
    {
      shopOptionsToImage[go] = go.GetComponent(typeof(Image)) as Image;
    }
  }

  private void InitializeShopOptionsToUnitDict()
  {
    if (shopOptions == null)
      return;

    foreach (GameObject go in shopOptions)
    {
      shopOptionToUnit[go] = null;
    }
  }

  private void UpdateShopOptionsToUnitDict(SO_Unit[] newUnits)
  {
    if (shopOptions == null)
      return;

    int idx = 0;
    foreach(GameObject go in shopOptions)
    {
      shopOptionToUnit[go] = newUnits[idx];
      idx++;
    }
  }

  #endregion

  #region Card / Shop Methods

  public void UpdateShopDisplay()
  {
    if (shopOptions == null)
      return;

    foreach (GameObject go in shopOptions)
    {
      shopOptionsToImage[go].sprite = shopOptionToUnit[go].storeSprite;
    }
  }

  public void RefreshShop()
  {
    ReturnStoreUnits();
    ReactivateShopOptions();
    SO_Unit[] newUnitsForShop = CardPoolManager.instance.RequestNewCards(p);
    UpdateShopOptionsToUnitDict(newUnitsForShop);
    UpdateShopDisplay();
    UpdatePlayerStoreUnits(newUnitsForShop);
  }

  public void PurchaseUnitByIndex(int index)
  {
    if (p.storeUnits[index] == null)
      return;

    if (p.benchedUnits.Count == Constants.benchWidth)
      return;

    //TODO: Fill in unit with the SO_Unit stuff
    Unit newUnit = new Unit();
    newUnit.soUnit = p.storeUnits[index];
    newUnit.unitLevel = 1;
    newUnit.status = UnitStatusType.normal;
    newUnit.go = GameObject.Instantiate(p.storeUnits[index].unitPrefab[0]);

    p.AddToBench(newUnit);


    if(shopOptions != null)
    {
      shopOptions[index].SetActive(false);
    }

    p.storeUnits[index] = null;
  }

  public void UpdatePlayerStoreUnits(SO_Unit[] units)
  {
    int idx = 0;
    foreach(SO_Unit u in units)
    {
      p.storeUnits[idx] = u;
      idx++;
    }
  }

  public void ReturnStoreUnits()
  {
    foreach(SO_Unit u in p.storeUnits)
    { 
      if(u != null)
        CardPoolManager.instance.ReturnCard(u);
    }
  }

  public void ReactivateShopOptions()
  {
    if (shopOptions == null)
      return;

    foreach(GameObject go in shopOptions)
    {
      go.SetActive(true);
    }
  }

  #endregion

  #region EXP

  public void PurchaseExp()
  {
    p.GainExp(1);
  }

  #endregion
}
