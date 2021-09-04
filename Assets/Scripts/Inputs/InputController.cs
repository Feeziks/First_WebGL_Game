using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
  public Vector2 movement;


  //Input Action Events
  public void OnCameraMovement(InputAction.CallbackContext context)
  {
    movement = context.ReadValue<Vector2>();
  }

  public void OnQuit(InputAction.CallbackContext context)
  {
    Application.Quit();    
  }
}
