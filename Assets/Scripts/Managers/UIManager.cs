using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
  #region Properties

  [Header("EXP")]
  public GameObject expPanel;
  public TextMeshProUGUI playerLevelText;
  public Image expSpriteImage;
  public Button purchaseExpButton;
  public Dictionary<int, List<Sprite>> levelToExpSpritesDict;

  #endregion

  #region unity Methods

  private void Awake()
  {
    levelToExpSpritesDict = new Dictionary<int, List<Sprite>>();
    InitLevelToExpSpritesDict();

    InitExpPanel();
  }

  #endregion

  #region Helpers

  private void InitLevelToExpSpritesDict()
  {
    for (int level = 1; level <= Constants.maxPlayerLevel; level++)
    {
      List<Sprite> thisLevelSprites = Resources.LoadAll("Sprites/EXP_Sprites/Level_" + level.ToString() + "_Sprites", typeof(Sprite)).Cast<Sprite>().ToList();
      levelToExpSpritesDict[level] = thisLevelSprites;
    }
  }

  private void InitExpPanel()
  {
    playerLevelText.text = "1";
    expSpriteImage.sprite = levelToExpSpritesDict[1][0];
  }

  #endregion

  #region player exp methods

  public void UpdateExp(int level, int exp)
  {
    playerLevelText.text = level.ToString();
    expSpriteImage.sprite = levelToExpSpritesDict[level][exp];
  }

  #endregion
}
