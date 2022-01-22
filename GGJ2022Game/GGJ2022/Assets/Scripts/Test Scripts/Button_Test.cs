using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Test : MonoBehaviour
{
    [SerializeField] private bool m_ButtonPressed;
    [SerializeField] private GameObject m_Platform;
    [SerializeField] private GameObject m_PointA;
    [SerializeField] private GameObject m_PointB;

    [Range(1.0f, 5.0f)]
    [SerializeField] private float m_PlatformSpeed;

    [SerializeField] private Animator m_Animator;

    private void Update()
    {
        if (m_ButtonPressed)
        {
            m_Platform.transform.position = Vector2.MoveTowards(m_Platform.transform.position, m_PointB.transform.position, m_PlatformSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            m_ButtonPressed = true;
            m_Animator.SetBool("ButtonIsPressed", true); 
            Debug.Log("Button Pressed!");
        }
    }
}
