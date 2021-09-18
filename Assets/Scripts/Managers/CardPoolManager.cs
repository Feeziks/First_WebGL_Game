using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class CardPoolManager : MonoBehaviour
{

  #region Properties

  private Dictionary<UnitTier, Dictionary<SO_Unit, int>> deck;
  private Dictionary<UnitTier, int> numCardsByTier;
  private Dictionary<int, float[]> playerLevelToUnitTierChance;

  #endregion

  #region Singleton

  static CardPoolManager mInstance;

  public static CardPoolManager instance
  {
    get
    {
      if (mInstance == null)
      {
        mInstance = new GameObject("CardPoolManager").AddComponent(typeof(CardPoolManager)) as CardPoolManager;
      }
      return mInstance;
    }
  }

  #endregion

  #region Unity Methods

  private void Awake()
  {
    deck = new Dictionary<UnitTier, Dictionary<SO_Unit, int>>();
    numCardsByTier = new Dictionary<UnitTier, int>();
    playerLevelToUnitTierChance = new Dictionary<int, float[]>();
    InitializeDeckDict();
    InitializeNumCardsByTierDict();
    InitializePlayerLevelToUnitTierChanceDict();
    InitializePool();
  }

  private void Start()
  {

  }

  private void Update()
  {

  }

  #endregion

  #region Pool Initialization

  private void InitializeDeckDict()
  {
    foreach (UnitTier t in Enum.GetValues(typeof(UnitTier)))
    {
      deck[t] = new Dictionary<SO_Unit, int>();
    }
  }

  private void InitializeNumCardsByTierDict()
  {
    //Thought process for number of each unit that should be available throughout the game
    //Every player should be able to get to the max unit level for a common unit
    //Every player - 2 should be able to get to the max unit level for an uncommon unit
    //Half of players should be able to get to the max unit level for a rare unit
    //2 players should be able to get to the max unit level for a legendary unit

    numCardsByTier[UnitTier.common] = Constants.numPlayers * Mathf.CeilToInt(Mathf.Pow(3, Constants.maxUnitLevel - 1));
    numCardsByTier[UnitTier.uncommon] = (Constants.numPlayers - 2) * Mathf.CeilToInt(Mathf.Pow(3, Constants.maxUnitLevel - 1));
    numCardsByTier[UnitTier.rare] = (Constants.numPlayers / 2) * Mathf.CeilToInt(Mathf.Pow(3, Constants.maxUnitLevel - 1));
    numCardsByTier[UnitTier.legendary] = 2 * Mathf.CeilToInt(Mathf.Pow(3, Constants.maxUnitLevel - 1));
  }

  public void InitializePlayerLevelToUnitTierChanceDict()
  {
    playerLevelToUnitTierChance[1] = new float[] { 0f, 0.9f,    1f,      0f,    0f }; //0.9 + 0.1 + 0 + 0 = 1.0
    playerLevelToUnitTierChance[2] = new float[] { 0f, 0.8f,    1f,      0f,    0f }; //0.8 + 0.2 + 0 + 0 = 1.0
    playerLevelToUnitTierChance[3] = new float[] { 0f, 0.7f,  0.95f,     1f,    0f }; //0.7 + 0.25 + 0.05 + 0f = 1.0
    playerLevelToUnitTierChance[4] = new float[] { 0f, 0.5f,  0.85f,     1f,    0f }; //0.5 + 0.35 + 0.15 + 0 = 1.0
    playerLevelToUnitTierChance[5] = new float[] { 0f, 0.3f,   0.8f,  0.99f,    1f };
    playerLevelToUnitTierChance[6] = new float[] { 0f, 0.2f,   0.6f,  0.95f,    1f };
    playerLevelToUnitTierChance[7] = new float[] { 0f, 0.1f,   0.4f,   0.9f,    1f };
    playerLevelToUnitTierChance[8] = new float[] { 0f, 0.05f, 0.25f,  0.65f,    1f };
  }

  private void InitializePool()
  {
    List<SO_Unit> allPossibleUnits = Resources.LoadAll("Units/", typeof(SO_Unit)).Cast<SO_Unit>().ToList();

    foreach (SO_Unit unit in allPossibleUnits)
    {
      deck[unit.ID.GetTier()][unit] = numCardsByTier[unit.ID.GetTier()];
    }

  }


  #endregion

  #region Requesting Cards

  public SO_Unit[] RequestNewCards(Player p)
  {
    SO_Unit[] ret = new SO_Unit[Constants.cardsInShop];

    for(int i = 0; i < Constants.cardsInShop; i++)
    {
      UnitTier tier = GetRandomTierByLevel(p.level);
      SO_Unit temp;
      do
      {
        temp = GetRandomUnitByTier(tier);
      } while (p.maxLevelUnits.Contains(temp));
      ret[i] = temp;
    }

    return ret;
  }

  private UnitTier GetRandomTierByLevel(int level)
  {
    float randomTierChoice = UnityEngine.Random.Range(0f, 1f);

    UnitTier ret = UnitTier.common;

    for(int i = 0; i < playerLevelToUnitTierChance[level].Length; i ++)
    {
      if(playerLevelToUnitTierChance[level][i] != 0 && randomTierChoice < playerLevelToUnitTierChance[level][i])
      {
        ret = (UnitTier)i;
        break;
      }
    }

    return ret;
  }

  private SO_Unit GetRandomUnitByTier(UnitTier tier)
  {
    SO_Unit ret = null;

    List<SO_Unit> unitsInTier = new List<SO_Unit>(deck[tier].Keys);
    int randomUnitChoice;
    int availableUnitCount;
    int count = 0;
    do
    {
      randomUnitChoice = Mathf.RoundToInt(UnityEngine.Random.Range(0f, unitsInTier.Count - 1));
      availableUnitCount = deck[tier][unitsInTier[randomUnitChoice]];
      count++;
    } while (availableUnitCount <= 0 && count < 100);

    if (count == 100)
    {
      ret = null;
    }
    else
    {
      ret = unitsInTier[randomUnitChoice];
      deck[tier][unitsInTier[randomUnitChoice]] = deck[tier][unitsInTier[randomUnitChoice]] - 1;
    }

    return ret;
  }

  #endregion

  #region Returning Cards

  public void ReturnCard(SO_Unit unit)
  {
    deck[unit.ID.GetTier()][unit] += 1;
  }

  #endregion
}
