using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
  public delegate void ExpPurchaseAction();
  public static event ExpPurchaseAction expPurchaseEvent;
  public void OnExpPurchase()
  {
    expPurchaseEvent();
  }

}
