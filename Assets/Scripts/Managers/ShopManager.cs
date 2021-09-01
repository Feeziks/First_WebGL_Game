using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
  public Player p;

  public GameObject[] shopOptions;
  private Dictionary<GameObject, SO_Unit> shopOptionToUnit;
  private Dictionary<GameObject, Image> shopOptionsToImage;

  #region Singleton

  static ShopManager mInstance;

  public static ShopManager instance
  {
    get
    {
      if (mInstance == null)
      {
        mInstance = new GameObject("ShopManager").AddComponent(typeof(ShopManager)) as ShopManager;
      }
      return mInstance;
    }
  }

  #endregion

  #region Unity Methods

  private void Awake()
  {
    shopOptionsToImage = new Dictionary<GameObject, Image>();
    InitializeShopOptionsToImageDict();

    shopOptionToUnit = new Dictionary<GameObject, SO_Unit>();
    InitializeShopOptionsToUnitDict();
  }

  void Start()
  {

  }


  void Update()
  {

  }

  #endregion

  #region Helpers

  private void InitializeShopOptionsToImageDict()
  {
    foreach(GameObject go in shopOptions)
    {
      shopOptionsToImage[go] = go.GetComponent(typeof(Image)) as Image;
    }
  }

  private void InitializeShopOptionsToUnitDict()
  {
    foreach(GameObject go in shopOptions)
    {
      shopOptionToUnit[go] = null;
    }
  }

  private void UpdateShopOptionsToUnitDict(SO_Unit[] newUnits)
  {
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
    foreach(GameObject go in shopOptions)
    {
      shopOptionsToImage[go].sprite = shopOptionToUnit[go].storeSprite;
    }
  }

  public void RefreshShop()
  {
    SO_Unit[] newUnitsForShop = CardPoolManager.instance.RequestNewCards(p);
    UpdateShopOptionsToUnitDict(newUnitsForShop);
    UpdateShopDisplay();
  }

  public void PurchaseUnitByIndex(int index)
  {

  }

  #endregion
}
