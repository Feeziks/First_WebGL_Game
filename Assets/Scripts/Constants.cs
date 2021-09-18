using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Constants
{
  /*
   * Unit realted constants
   */
  public static int numUnitTiers = 4;
  public static int maxUnitLevel = 3;
  public static int unitsToLevelUp = 3;
  public static int cardsInShop = 5;
  public static int storeRefreshPrice = 2;

  public static string PlayerUnitTag = "Player_Unit";
  public static string PVETag = "PVE_Unit";

  public static List<Vector2Int> placeableHexesOrder = new List<Vector2Int>() //TODO Finish this list
  { new Vector2Int(3, 0), new Vector2Int(4,0), new Vector2Int(2,0), new Vector2Int(3,1), new Vector2Int(4,1)};

  /*
   * Player related constants
   */
  public static int numPlayers = 8;
  public static int maxPlayerLevel = 8;
  //                                            1 -> 2, 2 -> 3, 3 -> 4, 4 -> 5, 5 -> 6, 6 -> 7, 7 -> 8
  public static int[] expPerLevel = new int[] { 2, 3, 5, 8, 11, 15, 20 };
  public static int expPurchasePrice = 5;

  /*
   * Rounds / PVE constants
   */
  public static void InitPVERounds()
  {
    PveRounds = new List<int>() { 1, 2, 3, 8, 13, 18, 23, 28, 33, 38, 43, 48 };
    allPVERoundBoards = Resources.LoadAll("PVE_Rounds", typeof(SO_PVE_Board_State)).Cast<SO_PVE_Board_State>().ToList();
    pveRoundToBoardStateDict = new Dictionary<int, SO_PVE_Board_State>();
    for (int i = 0; i < PveRounds.Count; i++)
    {
      pveRoundToBoardStateDict[PveRounds[i]] = allPVERoundBoards[i];
    }
  }
  public static List<int> PveRounds;
  public static List<SO_PVE_Board_State> allPVERoundBoards;
  public static Dictionary<int, SO_PVE_Board_State> pveRoundToBoardStateDict;

  public static float paddingTime = 3f;
  public static string paddingTimeString = "Cleaning up...";
  public static float timeBetweenRounds = 11f;
  public static string timeBetweenRoundsString = "Prepare For Battle!";
  public static float roundTimeout = 10f;
  public static string roundOccuringString = "Battling!";

  public static LayerMask hexGridLayer = 8;

  public static int roundWon = 1;
  public static int roundLost = 0;
  //                                                   0  1  2  3  4  5  6  7  8  9  10+
  public static int[] winStreakMoneyGain = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 2, 2, 3 };
  //                                                    0  1  2  3  4  5  6  7  8  9  10+
  public static int[] lossStreakMoneyGain = new int[] { 0, 0, 0, 1, 1, 1, 2, 2, 3, 3, 4 };

  /*
   * Board Related Constants
   */
  public static int boardWidth = 7;
  public static int boardHeight = 7;
  public static int benchWidth = 9;
  public static int benchHeight = 1;

  public static float timeToMoveHex = 0.6f;

  /*
   * I dont know what to call these
   */
  public static float tickRate = 60f;
}
