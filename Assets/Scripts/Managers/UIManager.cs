﻿using System.Collections;
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
  public Player p0;

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

  private GameManager gm;
  #endregion

  #region unity Methods

  private void Awake()
  {
    levelToExpSpritesDict = new Dictionary<int, List<Sprite>>();
    InitLevelToExpSpritesDict();

    InitExpPanel();

    InitPlayerPanels();

    gm = FindObjectOfType(typeof(GameManager)) as GameManager;
  }

  private void Update()
  {
    if (EventSystem.current.IsPointerOverGameObject())
    {
      hovered = GetHoveredUIObject(GetPointerUIRaycastResults());
      if (hovered && hovered.CompareTag("StoreOption"))
      {
        DisplayStoreOptionToolTip();
      }
    }
    else if (hovered = GetHoveredNonUIObject(GetPointerNonUIRaycastResults()))
    {
      if (hovered.CompareTag("Player_Unit"))
      {
          DisplayPlayerUnitToolTip();
      }
      else if (hovered.CompareTag("PVE_Unit"))
        DisplayPVEUnitToolTip();
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
    foreach (GameObject go in playerPanels)
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
  private GameObject GetHoveredUIObject(List<RaycastResult> raycasts)
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

  private GameObject GetHoveredNonUIObject(List<RaycastHit> raycasts)
  {
    for (int index = 0; index < raycasts.Count; index++)
    {
      if (raycasts[index].transform.gameObject.layer == LayerMask.NameToLayer("Unit"))
      {
        return raycasts[index].transform.gameObject;
      }
    }

    return null;
  }

  private List<RaycastResult> GetPointerUIRaycastResults()
  {
    PointerEventData eventData = new PointerEventData(EventSystem.current);
    eventData.position = Input.mousePosition;
    List<RaycastResult> result = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventData, result);
    return result;
  }

  private List<RaycastHit> GetPointerNonUIRaycastResults()
  {
    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
    List<RaycastHit> ret = new List<RaycastHit>();

    ret = Physics.RaycastAll(r).ToList();
    return ret;
  }

  private void DisplayStoreOptionToolTip()
  {
    SO_Unit unit = p0.sm.shopOptionToUnit[hovered];

    toolTipHeader.text = unit.unitName;
    string body = "";
    foreach (UnitTypes t in unit.unitTypes)
    {
      body += t.ToString();
      body += "\n";
    }
    foreach (UnitClasses t in unit.unitClasses)
    {
      body += t.ToString();
      body += "\n";
    }

    toolTipBody.text = body;

    toolTipPanel.SetActive(true);
    toolTipPanel.transform.position = Input.mousePosition + new Vector3(2f, 2f, 0f);
  }

  private void DisplayPlayerUnitToolTip()
  {
    //TODO: GameObject To Unit information
    Unit u = GetHoveredPlayerUnitInformation();

    toolTipHeader.text = u.soUnit.unitName;
    toolTipBody.text = UnitToToolTopText(u);

    toolTipPanel.SetActive(true);
    toolTipPanel.transform.position = Input.mousePosition + new Vector3(2f, 2f, 0f);
  }

  private string UnitToToolTopText(Unit u)
  {
    string ret = "";

    ret += u.soUnit.unitToolTipText;
    ret += "\n";
    ret += "Armor: " + u.soUnit.baseStats.baseArmor[u.unitLevel - 1] + "\t";
    ret += "Magic Resist: " + u.soUnit.baseStats.baseMagicResist[u.unitLevel - 1] + "\n";
    ret += "Attack Damage: " + u.soUnit.baseStats.baseAttackDamage[u.unitLevel - 1] + "\t";
    ret += "Attack Speed: " + u.soUnit.baseStats.baseAttackSpeed[u.unitLevel - 1] + "\n";
    ret += "Attack Range: " + u.soUnit.baseStats.baseAttackRange[u.unitLevel - 1] + "\t";
    ret += "Crit Chance: " + u.soUnit.baseStats.baseCritChance[u.unitLevel - 1] + "\n";
    ret += "Magic Damage: " + u.soUnit.baseStats.baseMagicDamage[u.unitLevel - 1] + "\t";

    return ret;
  }

  private Unit GetHoveredPlayerUnitInformation()
  {
    Unit ret = null;

    GameObject gameBoardOrBench = hovered.transform.parent.gameObject;
    Player owner = null;

    foreach (Player p in gm.players)
    {
      if (p.gameBoard == gameBoardOrBench)
      {
        owner = p;
        break;
      }

      if (p.bench == gameBoardOrBench.transform.parent.gameObject)
      {
        owner = p;
        break;
      }
    }

    foreach (DeployedUnit du in owner.deployedUnits)
    {
      //if (du.unit.go == hovered)
      {
        ret = du.unit;
        return ret;
      }
    }

    foreach (Unit u in owner.benchedUnits)
    {
      //if (u.go == hovered)
      {
        ret = u;
        return ret;
      }
    }

    return ret;
  }

  private void DisplayPVEUnitToolTip()
  {
    //TODO: GameObject to PVE Unit Information

    toolTipHeader.text = "This PVE Unit wants to kick your butt";
    toolTipBody.text = "Try not to let him do that";

    toolTipPanel.SetActive(true);
    toolTipPanel.transform.position = Input.mousePosition + new Vector3(2f, 2f, 0f);
  }
  #endregion
}
