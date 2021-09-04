using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
  #region Properties

  [Header("Janky")]
  Player p0;

  [Header("EXP")]
  public GameObject expPanel;
  public TextMeshProUGUI playerLevelText;
  public Image expSpriteImage;
  public Button purchaseExpButton;
  public Dictionary<int, List<Sprite>> levelToExpSpritesDict;

  [Header("Timer")]
  public TextMeshProUGUI timerText;

  [Header("Players")]
  public GameObject[] playerPanels;
  private TextMeshProUGUI[] playerPanelText;

  [Header("Tool Tips")]
  private GameObject hovered;
  public GameObject toolTipPanel;
  public TextMeshProUGUI toolTipHeader;
  public TextMeshProUGUI toolTipBody;

  #endregion

  #region unity Methods

  private void Awake()
  {
    levelToExpSpritesDict = new Dictionary<int, List<Sprite>>();
    InitLevelToExpSpritesDict();

    InitExpPanel();

    InitPlayerPanels();
  }

  private void Update()
  {
    if(EventSystem.current.IsPointerOverGameObject())
    {
      hovered = GetHoveredObject(GetPointerRaycastResults());
      if(hovered && hovered.CompareTag("StoreOption"))
      {
        DisplayStoreOptionToolTip();
      }
    }
    else
    {
      toolTipPanel.SetActive(false);
    }
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

  private void InitPlayerPanels()
  {
    playerPanelText = new TextMeshProUGUI[playerPanels.Length];
    int idx = 0;
    foreach(GameObject go in playerPanels)
    {
      playerPanelText[idx] = go.GetComponent(typeof(TextMeshProUGUI)) as TextMeshProUGUI;
      idx++;
    }
  }

  #endregion

  #region player exp methods

  public void UpdateExp(int level, int exp)
  {
    playerLevelText.text = level.ToString();
    expSpriteImage.sprite = levelToExpSpritesDict[level][exp];
  }

  #endregion

  #region Timer methods

  public void UpdateTimer(float time)
  {
    timerText.text = Mathf.FloorToInt(time).ToString();
  }

  #endregion

  #region DPS Chart Methods

  #endregion

  #region Player Chart Methods

  public void UpdatePlayerHealthDisplay(Player p, int playerIndex)
  {
    playerPanelText[playerIndex].text = "Player " + playerIndex + ": " + p.health;
  }

  public void UpdatePlayerPanelOrder()
  {
    
  }

  #endregion

  #region Tool Tips
  //Find if we are hovering over an ability
  private GameObject GetHoveredObject(List<RaycastResult> raycasts)
  {
    for (int index = 0; index < raycasts.Count; index++)
    {
      if (raycasts[index].gameObject.layer == LayerMask.NameToLayer("UI"))
      {
        return raycasts[index].gameObject;
      }
    }

    return null;
  }

  private List<RaycastResult> GetPointerRaycastResults()
  {
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    eventData.position = Input.mousePosition;
    List<RaycastResult> result = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, result);
    return result;
  }

  private void DisplayStoreOptionToolTip()
  {
    toolTipPanel.SetActive(true);
    toolTipPanel.transform.position = Input.mousePosition;
  }
  #endregion
}
