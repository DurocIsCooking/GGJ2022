using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject Chicken;
    [HideInInspector] public GameObject Egg;

    public GameObject CameraFocus; // Object the camera is focusing on

    private void Awake()
    {
        CameraFocus = Chicken;
        Chicken.GetComponent<Chicken>().Camera = this;
    }

    private void Update()
    {
        Vector3 focusPosition = CameraFocus.transform.position;
        transform.position = new Vector3(focusPosition.x, focusPosition.y, transform.position.z);
    }
}
