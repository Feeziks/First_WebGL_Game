using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
  [Header("Some Data IDK")]
  public SO_Unit soUnit;
  public int unitLevel;
  public UnitStats currentStats;
  public UnitStatusType status;
  public Player owner;
  public List<GameObject> enemyUnits;

  [Header("Location")]
  public GameObject thisBattleGameBoard;
  public Vector2Int hexAtRoundStart;
  public Vector2Int currentHex;
  public Vector2Int nextHex;
  public bool moving;
  private float movingTime;
  private int failedMoveAttempts;

  [Header("Attack Information")]
  public float lastAttackTime;
  public float lastAbilityTime;
  public GameObject target;
  public Unit targetUnit;
  public float DPS;

  [Header("Display Information")]
  public Image hpBar;
  public RawImage manaBar;

  private UIManager ui;
  private bool toolTip;

  private GameObject dragTarget;
  private bool dragging;
  private Vector3 positionPreDrag;
  private GameObject benchGameObjectPreDrag;

  #region Unity Methods

  private void Awake()
  {
    ui = FindObjectOfType(typeof(UIManager)) as UIManager;
    toolTip = false;
    moving = false;
    currentStats = new UnitStats();

    hpBar = transform.GetChild(0).GetChild(0).gameObject.GetComponent(typeof(Image)) as Image;
    manaBar = transform.GetChild(0).GetChild(1).gameObject.GetComponent(typeof(RawImage)) as RawImage;
  }

  private void Start()
  {

  }

  private void Update()
  {
    if (toolTip)
    {
      ui.EnableUnitToolTip(this);
    }
  }

  private void FixedUpdate()
  {

  }

  #endregion

  #region Fighting things

  public bool BattleTick()
  {
    if (!gameObject.activeSelf)
    {
      return false;
    }

    HealthAndManaRegen();

    if (enemyUnits.Count > 0)
    {
      if (status != UnitStatusType.stunned)
      {
        if (target == null)
        {
          FindTarget();
        }

        if (TargetInRange())
        {
          AttackTarget();
          if (currentStats.mana[unitLevel] >= soUnit.baseStats.mana[unitLevel])
          {
            CastAbility();
          }
        }
        else
        {
          ApproachTarget();
        }
      }
    }

    return true;
  }

  private void FindTarget()
  {
    //TODO: Loop over enemy deployed units - Find closest enemy? or unit has some sort of targeting type (Lowest hp, highest hp, closest, furthest, etc)
    //target = newTarget;
    //targetUnit = newTarget.GetComponent(typeof(Unit)) as Unit;

    GameObject newTarget = null;
    Unit newTargetUnit = null;

    float thisDistance = 0f;
    float minDistance = float.MaxValue;
    float maxDistance = float.MinValue;

    float thisHP = 0;
    float minHP = float.MaxValue;
    float maxHP = float.MinValue;

    float thisDPS = 0;
    float minDPS = float.MaxValue;
    float maxDPS = float.MinValue;

    switch (soUnit.targetingType)
    {
      case UnitTargetingType.closest:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;

          thisDistance = Vector3.Distance(gameObject.transform.position, go.transform.position);
          if (thisDistance < minDistance)
          {
            minDistance = thisDistance;
            newTarget = go;
          }
        }
        break;
      case UnitTargetingType.furthest:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;

          thisDistance = Vector3.Distance(gameObject.transform.position, go.transform.position);
          if (thisDistance > maxDistance)
          {
            maxDistance = thisDistance;
            newTarget = go;
          }
        }
        break;
      case UnitTargetingType.lowestHp:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;

          Unit u = go.GetComponent(typeof(Unit)) as Unit;
          thisHP = u.currentStats.health[unitLevel];
          if (thisHP < minHP)
          {
            minHP = thisHP;
            newTarget = go;
          }
        }
        break;
      case UnitTargetingType.highestHp:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;
          Unit u = go.GetComponent(typeof(Unit)) as Unit;
          thisHP = u.currentStats.health[unitLevel];
          if (thisHP > maxHP)
          {
            maxHP = thisHP;
            newTarget = go;
          }
        }
        break;
      case UnitTargetingType.highestDps:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;
          Unit u = go.GetComponent(typeof(Unit)) as Unit;
          thisDPS = u.DPS;
          if (thisDPS < minDPS)
          {
            minDPS = thisDPS;
            newTarget = go;
          }
        }
        break;
      case UnitTargetingType.lowestDps:
        foreach (GameObject go in enemyUnits)
        {
          if (!go.activeSelf)
            continue;
          Unit u = go.GetComponent(typeof(Unit)) as Unit;
          thisDPS = u.DPS;
          if (thisDPS > maxDPS)
          {
            maxDPS = thisDPS;
            newTarget = go;
          }
        }
        break;
      default:
        Debug.Log("Undefined unit targeting type for unit" + soUnit.unitName);
        break;
    }

    newTargetUnit = newTarget.GetComponent(typeof(Unit)) as Unit;

    target = newTarget;
    targetUnit = newTargetUnit;
  }

  private bool TargetInRange()
  {
    //TODO: Check if the targeted unit is within our range - range is defined in hexes how to get this to work in the game object transform level?
    //Maybe we can get the gameboard hexes that are within range # tiles away and check if the distance between us and the target is less than the distance between us and the furthest hex or something

    float currentRange = currentStats.attackRange[unitLevel] * Vector3.Distance(thisBattleGameBoard.transform.GetChild(0).transform.position, thisBattleGameBoard.transform.GetChild(1).transform.position);
    float distance = Vector3.Distance(transform.position, targetUnit.transform.position);

    if (distance <= currentRange)
      return true;

    return false;
  }

  private void ApproachTarget()
  {
    //TODO: Move the unit's game object along the gameboard hexes if we are not close enough to our target to attack it
    //This will involve some sort of path finding, not entirely sure how to implement that

    Vector3 direction = target.transform.position - transform.position;
    float currentDistance = Vector3.Distance(transform.position, target.transform.position);
    float minDistance = float.MaxValue;
    float thisDistance;

    if (!moving)
    {
      //Get the closest hex in that direction which is not occluded
      for (int x = -1; x <= 1; x += 2)
      {
        for (int y = -1; y <= 1; y += 2)
        {
          //Make sure we arent wrapping around the edge of the board
          if(currentHex.x == 0)
          {
            if (x == -1)
              continue;
          }

          if(currentHex.x == Constants.boardWidth)
          {
            if (x == 1)
              continue;
          }

          //Get the hex at this index
          int hexIndex = currentHex.x + currentHex.y * Constants.boardWidth;
          hexIndex += x + y * Constants.boardWidth;
          if (hexIndex >= 0 && hexIndex < thisBattleGameBoard.transform.childCount)
          {
            GameObject hexGo = thisBattleGameBoard.transform.GetChild(hexIndex).gameObject;

            //Check if the spot is occluded
            bool occluded = false;
            foreach (GameObject go in owner.deployedUnits)
            {
              if (!go.activeSelf)
                continue;

              if (go.transform.position.x == hexGo.transform.position.x && go.transform.position.z == hexGo.transform.position.z)
              {
                occluded = true;
                break;
              }

              if (owner.gameObjectToUnitDict[go].nextHex == currentHex + new Vector2Int(x, y))
              {
                occluded = true;
                break;
              }
            }

            if (!occluded)
            {
              foreach (GameObject go in enemyUnits)
              {
                if (!go.activeSelf)
                  continue;

                Unit goUnit = go.GetComponent(typeof(Unit)) as Unit;

                if (go.transform.position.x == hexGo.transform.position.x && go.transform.position.z == hexGo.transform.position.z)
                {
                  occluded = true;
                  break;
                }

                if (goUnit.nextHex == currentHex + new Vector2Int(x, y))
                {
                  occluded = true;
                  break;
                }
              }
            }

            if (occluded)
              break;

            //Check if this hex is in the right direction and if it is not occluded
            thisDistance = Vector3.Distance(hexGo.transform.position, target.transform.position);
            if (failedMoveAttempts >= 30 || (thisDistance < currentDistance && thisDistance < minDistance))
            {
              minDistance = thisDistance;
              nextHex = new Vector2Int(currentHex.x + x, currentHex.y + y);
              moving = true;
              movingTime = 0f;
              failedMoveAttempts = 0;
            }
            else
            {
              failedMoveAttempts++;
            }
          }
        }
      }
    }
    else
    {
      Vector3 startPos = thisBattleGameBoard.transform.GetChild(currentHex.x + currentHex.y * Constants.boardWidth).position + new Vector3(0f, gameObject.transform.localScale.y + 1f, 0f);
      Vector3 nextPosition = thisBattleGameBoard.transform.GetChild(nextHex.x + nextHex.y * Constants.boardWidth).position + new Vector3(0f, gameObject.transform.localScale.y + 1f, 0f);
      if (gameObject.transform.position != nextPosition || movingTime < Constants.timeToMoveHex)
      {
        transform.position = Vector3.Lerp(startPos, nextPosition, Mathf.Min(movingTime / Constants.timeToMoveHex, 1f));
        movingTime += 1f / Constants.tickRate;
      }
      else
      {
        moving = false;
        movingTime = 0f;
        currentHex = nextHex;
        failedMoveAttempts = 0;
      }
    }
  }

  private void AttackTarget()
  {
    //TODO: Create an attack timer that keeps track of our last attack time, and the interval between attacks
    //If the time that has lapsed since the last attack time is greater than the interval between attack times then perform a new attack
    if (lastAttackTime + (1f / currentStats.attackSpeed[unitLevel]) >= Time.realtimeSinceStartup || lastAttackTime == 0f)
    {
      //Do the attack
      if (target)
      {
        Dictionary<AttackTypes, float> thisAttackDamageByTypeDict = new Dictionary<AttackTypes, float>();

        thisAttackDamageByTypeDict[AttackTypes.physical] = soUnit.attackTypes[unitLevel].percent[0];
        thisAttackDamageByTypeDict[AttackTypes.magical] = soUnit.attackTypes[unitLevel].percent[1];
        thisAttackDamageByTypeDict[AttackTypes.tru] = soUnit.attackTypes[unitLevel].percent[2];

        //Check for a crit
        float damageValue = currentStats.attackDamage[unitLevel];
        if (Random.Range(0f, 1f) < currentStats.critChance[unitLevel])
        {
          damageValue *= 2f;
        }

        UnitDamageDealtType thisAttackDamage = new UnitDamageDealtType(damageValue, thisAttackDamageByTypeDict);
        targetUnit.RecieveDamge(thisAttackDamage);

        currentStats.mana[unitLevel] = Mathf.Min(currentStats.mana[unitLevel] + currentStats.manaGainOnHit[unitLevel], soUnit.baseStats.mana[unitLevel]);

        lastAttackTime = Time.realtimeSinceStartup;

        //Reset our attack timer and apply any on hit affects to ourselves (life steal, stat change etc)
      }
    }
  }

  private void CastAbility()
  {
    if (lastAbilityTime + (1f / currentStats.abilityCooldown[unitLevel]) > Time.realtimeSinceStartup)
    {
      if (target)
      {

        lastAbilityTime = Time.realtimeSinceStartup;
      }
    }
  }

  private void Die()
  {
    //The unit has died
    gameObject.SetActive(false);

    currentHex = hexAtRoundStart;
    nextHex = currentHex;
  }

  private void UpdateHealthManaBars()
  {
    hpBar.rectTransform.sizeDelta = new Vector2((currentStats.health[unitLevel] / soUnit.baseStats.health[unitLevel]) * 20f, hpBar.rectTransform.sizeDelta.y);
    manaBar.rectTransform.sizeDelta = new Vector2((currentStats.mana[unitLevel] / soUnit.baseStats.mana[unitLevel]) * 20f, hpBar.rectTransform.sizeDelta.y);
  }

  private void HealthAndManaRegen()
  {
    currentStats.health[unitLevel] += currentStats.healthRegen[unitLevel] / Constants.tickRate;
    currentStats.mana[unitLevel] += currentStats.manaRegen[unitLevel] / Constants.tickRate;

    UpdateHealthManaBars();
  }

  public void RecieveDamge(UnitDamageDealtType data)
  {
    foreach (KeyValuePair<AttackTypes, float> kvp in data.damageByType)
    {
      float thisDmgAmount = kvp.Value * data.damageValue;
      switch (kvp.Key)
      {
        case AttackTypes.physical:
          thisDmgAmount = ApplyArmor(thisDmgAmount);
          currentStats.health[unitLevel] -= thisDmgAmount;
          break;
        case AttackTypes.magical:
          thisDmgAmount = ApplyMagicResist(thisDmgAmount);
          currentStats.health[unitLevel] -= thisDmgAmount;
          break;
        case AttackTypes.tru:
          //True damage cannot be blocked so apply it directly
          currentStats.health[unitLevel] -= thisDmgAmount;
          break;
        default:
          Debug.Log("Undefined attack type used in an attack on unit" + gameObject.name);
          break;
      }
    }

    UpdateHealthManaBars();

    if (currentStats.health[unitLevel] <= 0f)
    {
      Die();
    }
  }

  private float ApplyArmor(float damageAmount)
  {
    return (100f) / (100f + currentStats.armor[unitLevel]);
  }

  private float ApplyMagicResist(float damageAmount)
  {
    return (100f) / (100f + currentStats.magicResist[unitLevel]);
  }

  #endregion

  #region Pointer Interfaces

  //Detect if a click occurs
  public void OnPointerClick(PointerEventData pointerEventData)
  {
    //If the user Left clicks the unit do nothing for now

    //If the user right clicks the unit sell it (After checking if they have the permissions to sell that unit)
    if (pointerEventData.button == PointerEventData.InputButton.Right)
    {
      //Dont allow selling units mid fight
      if (owner.roundOccuring && owner.deployedUnits.Contains(gameObject))
        return;

      owner.SellUnit(gameObject);
    }
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    //Begin displaying the tool tip for this unit
    ui.EnableUnitToolTip(this);
    toolTip = true;

    if (!dragging)
      positionPreDrag = transform.position;

    if (owner.benchedUnits.Contains(gameObject))
    {
      foreach (KeyValuePair<GameObject, GameObject> kvp in owner.benchChildToBenchedUnitDict)
      {
        if (kvp.Value == gameObject)
        {
          benchGameObjectPreDrag = kvp.Key;
          break;
        }
      }
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    //Stop displaying the tool tip for this unit
    ui.DisableUnitToolTip();
    toolTip = false;
  }

  public void OnDrag(PointerEventData eventData)
  {
    //Begin Moving the unit around if we have the permission to do so
    if (owner.playerId == 0 && eventData.button == 0)
    {

      if (owner.roundOccuring)
      {
        if (owner.deployedUnits.Contains(gameObject))
          return;
      }

      dragging = true;

      RaycastHit[] hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(eventData.position));
      foreach (RaycastHit hit in hits)
      {
        if (hit.transform.gameObject.layer == Constants.hexGridLayer)
        {
          if (hit.transform.gameObject.CompareTag("Player" + owner.playerId.ToString() + "Placeable"))
          {
            //Dont place a unit on top of another unit
            bool occluded = false;
            foreach (GameObject go in GameManager.GetAllChildren(owner.unitParent))
            {
              if (go.transform.position.x == hit.transform.position.x && go.transform.position.z == hit.transform.position.z)
              {
                occluded = true;
                break;
              }
            }

            if (!occluded)
            {
              dragTarget = hit.transform.gameObject;
              transform.position = hit.transform.position + new Vector3(0f, transform.localScale.y + 1f, 0f);
            }
          }
        }
      }
    }
  }

  public void OnEndDrag(PointerEventData eventData)
  {

    dragging = false;

    if (owner.roundOccuring)
    {
      if (owner.deployedUnits.Contains(gameObject))
        return;

      if (dragTarget.transform.parent.gameObject == owner.gameBoard)
      {
        transform.position = positionPreDrag;
        return;
      }
    }


    //Place the unit onto the bench or the gameboard where appropriate
    if (owner.benchedUnits.Contains(gameObject))
    {
      owner.benchedUnits.Remove(gameObject);
      owner.benchChildToBenchedUnitDict[benchGameObjectPreDrag] = null;
    }
    else if (owner.deployedUnits.Contains(gameObject))
      owner.deployedUnits.Remove(gameObject);

    if (dragTarget.transform.parent.gameObject == owner.gameBoard)
    {
      owner.deployedUnits.Add(gameObject);
      SetRoundStartHex(dragTarget);
    }

    if (dragTarget.transform.parent.gameObject == owner.bench)
    {
      owner.benchedUnits.Add(gameObject);
      owner.benchChildToBenchedUnitDict[dragTarget.gameObject] = gameObject;
    }

    dragTarget = null;
  }


  #endregion

  #region Helpers
  public void SetRoundStartHex(GameObject hexGO)
  {
    int idx = hexGO.transform.GetSiblingIndex();
    int xPos = (int)(idx % Constants.boardWidth);
    int yPos = (int)(idx / Constants.boardWidth);
    hexAtRoundStart = new Vector2Int(xPos, yPos);
    currentHex = hexAtRoundStart;
    nextHex = hexAtRoundStart;
  }

  public void SetStatsAtRoundStart()
  {
    //TODO: Add Item stats
    //TODO: Add class stats
    //TODO: Add type stats
    currentStats = soUnit.baseStats.ShallowCopy();
  }

  #endregion

  #region Debugging

  public void OnDrawGizmos()
  {
    if (target)
    {
      Gizmos.color = Color.white;
      Gizmos.DrawLine(transform.position, target.transform.position);
    }

    if (thisBattleGameBoard)
    {
      Gizmos.color = Color.green;
      Gizmos.DrawLine(transform.position, thisBattleGameBoard.transform.GetChild(nextHex.x + nextHex.y * Constants.boardWidth).position + new Vector3(0f, gameObject.transform.localScale.y + 1f, 0f));

      Gizmos.DrawWireSphere(transform.position, currentStats.attackRange[unitLevel] * Vector3.Distance(thisBattleGameBoard.transform.GetChild(0).transform.position, thisBattleGameBoard.transform.GetChild(1).transform.position));
    }
  }

  #endregion
}
