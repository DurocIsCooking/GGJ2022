using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    // Pointers
    private Rigidbody2D _rigidbody;
    [HideInInspector] public CameraMovement Camera;

    // Horizontal movement
    private float _horizontalInput;
    private float _currentHorizontalSpeed;
    public float HorizontalSpeedEgg;
    public float HorizontalSpeedNoEgg;

    // Jumping
    private bool _wantsToJump = false; // Stores player input for jumping. Needed since input is in Update and Jump is in FixedUpdate
    private bool _canDoubleJump = false;
    public float JumpForce; // How much to increase player's y velocity on jump
    private float _extendedJumpVelocity = 0.1f; // Velocity continuously added to a jump if the player holds the jump key
    
    // Other physics
    private float _gravityScale = 1;
    public float FloatationForce = -0.1f;
    private bool _inWater = false;

    // Platform detection
    private float _groundCheckRaycastLength;
    private float _wallCheckRaycastLength;

    // Respawn point
    public static Vector3 RespawnPoint;

    // Egg
    public GameObject EggPrefab;
    private GameObject _eggInstance;
    private bool _hasEgg = true;
    public float EggLaunchForce;
    [HideInInspector] public bool IsActive = true;

    // Health
    [SerializeField] private int _maxStamina;
    private int _currentStamina;

    // Animation
    [SerializeField] private Animator _chickenAnimator;
    [SerializeField] private SpriteRenderer _chickenRenderer;
    [SerializeField] private GameObject _carriedEggLeft;
    [SerializeField] private GameObject _carriedEggRight;
    [SerializeField] private Sprite[] _backEggSprites;
    [SerializeField] private SpriteRenderer _tutorialRenderer;
    [SerializeField] private SpriteRenderer _staminaRenderer;
    [SerializeField] private Sprite[] _staminaSprites;
    private bool _hasFoundAnimation;

    private void Awake()
    {
        // Set values
        _currentHorizontalSpeed = HorizontalSpeedEgg;
        _currentStamina = _maxStamina;
        // Pointers
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = _gravityScale;
    }

    private void Update()
    {
        // Input that is possible all the time
        CollectInput();
        // Input only when the chicken is active
        if (IsActive)
        {
            CollectMovementInput();
        }
    }

    private void CollectInput()
    {
        // Switch between chicken and egg
        if (Input.GetButtonDown("SwitchControls") && _eggInstance != null)
        {
            SwitchControls();
        }
        // Reset to last nest
        if (Input.GetKeyDown(KeyCode.R))
        {
            
        }

    }

    private void CollectMovementInput()
    {
        // Get horizontal input with axis controls
        _horizontalInput = Input.GetAxis("Horizontal");

        // Check to see if player wants to jump
        if (Input.GetButtonDown("Jump"))
        {
            if(_currentStamina > 0)
            {
                _wantsToJump = true;
            }
        }

        if(Input.GetButtonDown("ThrowEgg") && _hasEgg)
        {
            if ((!TutorialSystem.HasThrown) && TutorialSystem.HasJumped && TutorialSystem.HasMoved)
            {
                _tutorialRenderer.enabled = false;
                TutorialSystem.HasThrown = true;
            }
            
            ThrowEgg();
        }
    }

    private void ThrowEgg()
    {
        if(Egg.Health > 0)
        {
            _hasEgg = false;

            _carriedEggLeft.SetActive(false);
            _carriedEggRight.SetActive(false);

            _currentHorizontalSpeed = HorizontalSpeedNoEgg;

            // Get launch vector
            Vector2 mousePos = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 chickenPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 launchDirection = (mousePos - chickenPos).normalized;

            // Instantiate egg
            _eggInstance = Instantiate(EggPrefab, chickenPos + launchDirection, Quaternion.identity);
            _eggInstance.GetComponent<Rigidbody2D>().velocity = launchDirection * EggLaunchForce;

            Debug.Log(_eggInstance.name);

            Camera.Egg = _eggInstance;
        }

    }

    private void FixedUpdate()
    {
        _hasFoundAnimation = false;
        // Manage movement based on input
        if(_rigidbody.velocity.y < 0)
        {
            _hasFoundAnimation = true;
            _chickenAnimator.Play("ChickenFlutter");
        }
        if(IsActive)
        {
            ManageHorizontalMovement();
            ManageJump();
        }
        if(_inWater)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + FloatationForce);
        }
        if (!_hasFoundAnimation && !_chickenAnimator.GetCurrentAnimatorStateInfo(0).IsName("ChickenJump"))
        {
            _chickenAnimator.Play("ChickenIdle");
        }
    }


    public void SwitchControls()
    {
        // Stop movement and change camera focus
        if ((!TutorialSystem.HasSwapped) && TutorialSystem.HasThrown && TutorialSystem.HasJumped && TutorialSystem.HasMoved)
        {
            _tutorialRenderer.enabled = false;
            TutorialSystem.HasSwapped = true;
        }
        if (IsActive)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            Camera.CameraFocus = _eggInstance;
        }
        else
        {
            Rigidbody2D eggrb = _eggInstance.GetComponent<Rigidbody2D>();
            eggrb.velocity = new Vector2(0, eggrb.velocity.y);
            Camera.CameraFocus = gameObject;
        }
        // Switch controls
        _eggInstance.GetComponent<Egg>().IsActive = IsActive;
        IsActive = !IsActive;
    }

    private void ManageHorizontalMovement()
    {
        // Move
        _rigidbody.velocity = new Vector2(_horizontalInput * _currentHorizontalSpeed, _rigidbody.velocity.y);
        if (_horizontalInput > 0)
        {
            if (!TutorialSystem.HasMoved)
            {
                _tutorialRenderer.enabled = false;
                TutorialSystem.HasMoved = true;
            }
            
            if (IsTouchingGround())
            {
                _hasFoundAnimation = true;
                _chickenAnimator.Play("ChickenWalk");
            }
            _chickenRenderer.flipX = false;
            if (_hasEgg)
            {
                BackEggUpdate();
            }
        }
        else if (_horizontalInput < 0)
        {
            if (!TutorialSystem.HasMoved)
            {
                _tutorialRenderer.enabled = false;
                TutorialSystem.HasMoved = true;
            }
            if (IsTouchingGround())
            {
                _hasFoundAnimation = true;
                _chickenAnimator.Play("ChickenWalk");
            }
            _chickenRenderer.flipX = true;
            if (_hasEgg)
            {
                BackEggUpdate();
            }
        }
        
        // Wall check (to prevent sticking to walls)
        if (_rigidbody.velocity.x != 0)
        {
            RaycastHit2D topHit;
            RaycastHit2D midHit;
            RaycastHit2D botHit;

            // Moving left
            if (_rigidbody.velocity.x < 0)
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y + 0.5f), Vector2.left, _wallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y), Vector2.left, _wallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y - 0.5f), Vector2.left, _wallCheckRaycastLength);
            }
            // Moving right
            else
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y + 0.5f), Vector2.right, _wallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y), Vector2.left, _wallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y - 0.5f), Vector2.left, _wallCheckRaycastLength);
            }
            // If we hit something, stop horizontal movement
            if (topHit.collider != null)
            {
                if (topHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                }
            }
            if (midHit.collider != null)
            {
                if (midHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                }
            }
            if (botHit.collider != null)
            {
                if (botHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                {
                    _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
                }
            }
        }
        
    }

    private void ManageJump()
    {
        // Reset double jump
        if (!_hasEgg && IsTouchingGround())
        {
            _canDoubleJump = true;
        }

        // Initial jump
        if (_wantsToJump && (IsTouchingGround() || _canDoubleJump))
        {
            if ((!TutorialSystem.HasJumped) && TutorialSystem.HasMoved)
            {
                _tutorialRenderer.enabled = false;
                TutorialSystem.HasJumped = true;
            }
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            _chickenAnimator.Play("ChickenJump");
            if(!IsTouchingGround()) // If we just double jumped
            {
                _canDoubleJump = false;
            }
            _currentStamina -= 1;
            _staminaRenderer.sprite = _staminaSprites[_currentStamina];
        }
        
        // Add a bit of speed if holding space
        if(Input.GetButton("Jump"))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + _extendedJumpVelocity);
        }

        _wantsToJump = false;
    }
    
    private bool IsTouchingGround()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.6f), Vector2.down, _groundCheckRaycastLength);
        RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y - 0.6f), Vector2.down, _groundCheckRaycastLength);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform") || leftHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return true;
            }
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform") || rightHit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // Pick up egg
        if(col.collider.gameObject.layer == LayerMask.NameToLayer("Egg"))
        {
            Destroy(col.collider.gameObject);
            if(!IsActive)
            {
                SwitchControls();
            }
            PickUpEgg();
            BackEggUpdate();
        }
        // Parent to moving platform
        if (col.gameObject.tag == "Platform")
        {
            gameObject.transform.parent = col.gameObject.transform;
        }

    }

    private void OnCollisionExit2D(Collision2D col)
    {
        // Deparent from moving platform
        gameObject.transform.parent = null;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            _inWater = true;
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Nest"))
        {
            if (_hasEgg)
            {
                _currentStamina = _maxStamina;
                Egg.Health = Egg.MaxHealth;
                BackEggUpdate();
                _staminaRenderer.sprite = _staminaSprites[_currentStamina];
            }
            else
            {
                // PROMPT PLAYER TO FIND EGG
            }
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Goal"))
        {
            if (_hasEgg)
            {
                if (col.gameObject.GetComponent<Goal>().StartOfNextLevel != null)
                {
                    transform.position = col.gameObject.GetComponent<Goal>().StartOfNextLevel.transform.position;
                }
                else
                {
                    // Victory
                    MenuManager.Instance.LoadGameEndMenu();
                }
            }
            else
            {
                // PROMPT PLAYER TO FIND EGG
            }
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Nest"))
        {
            if (_hasEgg)
            {
                _currentStamina = _maxStamina;
                Egg.Health = Egg.MaxHealth;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            _inWater = false;
        }
    }

    public void PickUpEgg()
    {
        _hasEgg = true;
        _canDoubleJump = false;
        _currentHorizontalSpeed = HorizontalSpeedEgg;
    }
    private void BackEggUpdate()
    {
        BackEggVisuals();
        if (_chickenRenderer.flipX)
        {
            _carriedEggLeft.SetActive(true);
            _carriedEggRight.SetActive(false);
        }
        else
        {
            _carriedEggLeft.SetActive(false);
            _carriedEggRight.SetActive(true);
        }
    }
    private void BackEggVisuals()
    {
        GameObject _activeEgg;
        if (_carriedEggLeft.activeSelf)
        {
            _activeEgg = _carriedEggLeft;
        }
        else
        {
            _activeEgg = _carriedEggRight;
        }
        switch (Egg.Health)
        {
            case 2:
                {
                    _activeEgg.transform.Find("ERL").GetComponent<SpriteRenderer>().sprite = _backEggSprites[0];
                    _activeEgg.transform.Find("ERR").GetComponent<SpriteRenderer>().sprite = _backEggSprites[1];
                    break;
                }
            case 1:
                {
                    _activeEgg.transform.Find("ERL").GetComponent<SpriteRenderer>().sprite = _backEggSprites[2];
                    _activeEgg.transform.Find("ERR").GetComponent<SpriteRenderer>().sprite = _backEggSprites[3];
                    break;
                }
            case 0:
                {
                    _activeEgg.transform.Find("ERL").GetComponent<SpriteRenderer>().sprite = _backEggSprites[4];
                    _activeEgg.transform.Find("ERR").GetComponent<SpriteRenderer>().sprite = _backEggSprites[5];
                    break;
                }
        }
    }
    protected void HideStamina()
    {
        _staminaRenderer.enabled = false;
    }
}
