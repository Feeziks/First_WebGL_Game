using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IEndDragHandler
{
  [Header("Some Data IDK")]
  public SO_Unit soUnit;
  public int unitLevel;
  public UnitStatusType status;
  public Player owner;
  //TODO: Lots more

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
  }

  private void Start()
  {
    
  }

  private void Update()
  {
    if(toolTip)
    {
      ui.EnableUnitToolTip(this);
    }
  }

  private void FixedUpdate()
  {
    
  }

  #endregion

  #region Pointer Interfaces

  //Detect if a click occurs
  public void OnPointerClick(PointerEventData pointerEventData)
  {
    //If the user Left clicks the unit do nothing for now

    //If the user right clicks the unit sell it (After checking if they have the permissions to sell that unit)
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    //Begin displaying the tool tip for this unit
    ui.EnableUnitToolTip(this);
    toolTip = true;

    if(!dragging)
      positionPreDrag = transform.position;

    if(owner.benchedUnits.Contains(gameObject))
    {
      foreach(KeyValuePair<GameObject, GameObject> kvp in owner.benchChildToBenchedUnitDict)
      {
        if(kvp.Value == gameObject)
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
    if(owner.playerId == 0 && eventData.button == 0)
    {

      if(owner.roundOccuring)
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
              if(go.transform.position.x == hit.transform.position.x && go.transform.position.z == hit.transform.position.z)
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

      if(dragTarget.transform.parent.gameObject == owner.gameBoard)
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
    }

    if(dragTarget.transform.parent.gameObject == owner.bench)
    {
      owner.benchedUnits.Add(gameObject);
      owner.benchChildToBenchedUnitDict[dragTarget.gameObject] = gameObject;
    }

    dragTarget = null;
  }


  #endregion
}
