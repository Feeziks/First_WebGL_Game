using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
  /*
   * Unit realted constants
   */
  public static int numUnitTiers = 4;
  public static int maxUnitLevel = 3;
  public static int cardsInShop = 5;

  /*
   * Player related constants
   */
  public static int numPlayers = 8;
  public static int maxPlayerLevel = 8;
  //                                            1 -> 2, 2 -> 3, 3 -> 4, 4 -> 5, 5 -> 6, 6 -> 7, 7 -> 8
  public static int[] expPerLevel = new int[] { 2,      3,      5,      8,      11,     15,     20 };
}
