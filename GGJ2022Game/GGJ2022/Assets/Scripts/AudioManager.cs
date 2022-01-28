using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource m_BGMChickenAudioSource;
    [SerializeField] private AudioSource m_BGMEggAudioSource;
    [SerializeField] private AudioMixer m_BGMMixer;
    [SerializeField] private AudioSource m_SFXAudioSource;

    [Header("BGM")]
    [SerializeField] private AudioClip m_BGMChicken;    //The song that shou in the background while playing as the Chicken.
    private bool m_BGMChickenPlaying;
    [SerializeField] private AudioClip m_BGMEgg;        //The song playing in the background while playing as the Egg.

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
        //Set both clips.
        m_BGMChickenAudioSource.clip = m_BGMChicken;
        m_BGMEggAudioSource.clip = m_BGMEgg;
        //Set both volumes.
        m_BGMMixer.SetFloat("m_BGMChickenVol", -5.0f);
        m_BGMMixer.SetFloat("m_BGMEggVol", -80.0f);
        //Check if the first BGM is playing/audible.
        m_BGMChickenPlaying = true;
        //Both play.
        m_BGMChickenAudioSource.Play();
        m_BGMEggAudioSource.Play();
    }

    public void BGM_Switch()
    {
        if (m_BGMChickenPlaying)
        {
            m_BGMMixer.SetFloat("m_BGMChickenVol", -80.0f);
            m_BGMMixer.SetFloat("m_BGMEggVol", -10.0f);
            m_BGMChickenPlaying = false;
        }
        else
        {
            m_BGMMixer.SetFloat("m_BGMChickenVol", -5.0f);
            m_BGMMixer.SetFloat("m_BGMEggVol", -80.0f);
            m_BGMChickenPlaying = true;
        }
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
