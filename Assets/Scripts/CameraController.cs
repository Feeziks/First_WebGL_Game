using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public InputController inputs;
  public float moveSpeed;

  #region Unity Methods

  private void Awake()
  {
    
  }

  private void Start()
  {
    
  }

  private void Update()
  {
    HandleInputs();
  }

  private void FixedUpdate()
  {
    
  }

  #endregion


  #region Movement

  private void HandleInputs()
  {
    if(inputs.movement != Vector2.zero)
    {
      gameObject.transform.position += -1f * transform.forward * inputs.movement.x * moveSpeed * Time.deltaTime;
      gameObject.transform.position += transform.right * inputs.movement.y * moveSpeed * Time.deltaTime;
      //gameObject.transform.position += new Vector3(inputs.movement.x, 0f, inputs.movement.y) * moveSpeed * Time.deltaTime;
    }
  }

  #endregion
}
