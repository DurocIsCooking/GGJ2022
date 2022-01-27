using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    [SerializeField] private enum TutorialType
    {
        Move, Jump, Throw, Swap
    }
    [SerializeField] private Sprite[] _tutorialSprites;
    [SerializeField] private SpriteRenderer _tutorialRenderer;
    public static bool HasMoved = false;
    public static bool HasJumped = false;
    public static bool HasThrown = false;
    public static bool HasSwapped = false;
    private bool _jumpAlreadyInvoked;
    private bool _throwAlreadyInvoked;
    private bool _swapAlreadyInvoked;
    public void DismissTutorial(GameObject ToDismiss)
    {
        ToDismiss.SetActive(false);
    }
    private void Start()
    {
        Invoke("MoveCheck", 2);
    }
    private void Update()
    {
        if (HasMoved && HasJumped && HasThrown && HasSwapped)
        {
            Destroy(gameObject);
        }
        else if (HasMoved && !HasJumped && !_jumpAlreadyInvoked)
        {
            _jumpAlreadyInvoked = true;
            Invoke("JumpCheck", 5);
        }
        else if (HasMoved && HasJumped && !HasThrown && !_throwAlreadyInvoked)
        {
            if (transform.position.x > 13)
            {
                _throwAlreadyInvoked = true;
                Invoke("ThrowCheck", 5);
            }
        }
        else if (HasMoved && HasJumped && HasThrown && !HasSwapped && !_swapAlreadyInvoked)
        {
            _swapAlreadyInvoked = true;
            SwapCheck();
        }
    }
    public void MoveCheck()
    {
        if (!HasMoved)
        {
            _tutorialRenderer.enabled = true;
            _tutorialRenderer.sprite = _tutorialSprites[0];
        }
    }
    public void JumpCheck()
    {
        if (!HasJumped)
        {
            _tutorialRenderer.enabled = true;
            _tutorialRenderer.sprite = _tutorialSprites[1];
        }   
    }
    public void ThrowCheck()
    {
        if (!HasThrown)
        {
            _tutorialRenderer.enabled = true;
            _tutorialRenderer.sprite = _tutorialSprites[2];
        }
    }
    public void SwapCheck()
    {
        if (!HasSwapped)
        {
            _tutorialRenderer.enabled = true;
            _tutorialRenderer.sprite = _tutorialSprites[3];
        }
    }
}
