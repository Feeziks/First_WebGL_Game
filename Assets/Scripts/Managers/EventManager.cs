using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
  public delegate void ExpPurchaseAction();
  public static event ExpPurchaseAction expGainEvent;
  public void OnExpGain()
  {
    expGainEvent();
  }

}
