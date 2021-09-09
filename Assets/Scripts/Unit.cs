using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IDropHandler
{
  [Header("Some Data IDK")]
  public SO_Unit soUnit;
  public int unitLevel;
  public UnitStatusType status;
  public Player owner;
  //TODO: Lots more

  private UIManager ui;
  private bool toolTip;
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
  }

  public void OnDrop(PointerEventData eventData)
  {
    //Place the unit onto the bench or the gameboard where appropriate
  }


  #endregion
}
