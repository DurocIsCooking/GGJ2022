using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_BGMAudioSource;
    [SerializeField] private AudioSource m_SFXAudioSource;

    [Header("BGM")]
    [SerializeField] private AudioClip m_BGM;           //The song playing in the background.

    [Header("SFX - Chicken")]
    [SerializeField] private AudioClip[] m_ChickenBok;  //Bok bok bok.
    [SerializeField] private AudioClip m_ChickenJump;   //A jumping sound? Feathers being ruffled?
    //Add more Chicken SFX as needed.

    [Header("SFX - Egg")]
    [SerializeField] private AudioClip m_EggRoll;       //A sound that plays when the egg is rolling.
    [SerializeField] private AudioClip m_EggImpact;     //A sound that plays when the egg hits a surface.
    //Add more Egg SFX as needed.

    [Header("SFX - Environmental")]
    [SerializeField] private AudioClip m_Nest;          //A sound that plays when the Chicken and Egg restore Stamina and Throws at the nest?
    [SerializeField] private AudioClip m_Button;        //A sound that plays when a button is pressed.
    [SerializeField] private AudioClip m_Platform;      //A sound that plays when a platform is moving.
    [SerializeField] private AudioClip m_Water;         //A sound that plays when the Egg travels across water.
    //Add more Environmental SFX as needed.


    //Singleton
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject();
                    singleton.AddComponent<AudioManager>();
                    singleton.name = "(Singleton) AudioManager";
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        m_BGMAudioSource.clip = m_BGM;
        m_BGMAudioSource.Play();
    }

    public void SFX_ChickenBok()
    {
        m_SFXAudioSource.PlayOneShot(m_ChickenBok[Random.Range(0, m_ChickenBok.Length)]);
    }

    public void SFX_ChickenJump()
    {
        m_SFXAudioSource.PlayOneShot(m_ChickenJump);
    }

    public void SFX_EggRoll()
    {
        m_SFXAudioSource.PlayOneShot(m_EggRoll);
    }

    public void SFX_EggImpact()
    {
        m_SFXAudioSource.PlayOneShot(m_EggImpact);
    }
    public void SFX_Nest()
    {
        m_SFXAudioSource.PlayOneShot(m_Nest);
    }

    public void SFX_Button()
    {
        m_SFXAudioSource.PlayOneShot(m_Button);
    }
    public void SFX_Platform()
    {
        m_SFXAudioSource.PlayOneShot(m_Platform);
    }
    public void SFX_Water()
    {
        m_SFXAudioSource.PlayOneShot(m_Water);
    }
}
