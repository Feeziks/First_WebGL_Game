using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCanvas : MonoBehaviour
{
  private Camera cam;
  private Canvas canvas;
  public float cutOffDistance = 500f;

  private void Start()
  {
    cam = Camera.main;
    canvas = gameObject.GetComponent(typeof(Canvas)) as Canvas;
  }

  // Update is called once per frame
  void Update()
  {
    if (Vector3.Distance(transform.position, cam.transform.position) <= cutOffDistance)
    {
      canvas.enabled = true;
      transform.LookAt(cam.transform);
    }
    else
    {
      canvas.enabled = false;
    }
  }
}
