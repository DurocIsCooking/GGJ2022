using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chicken : MonoBehaviour
{
    // Pointers
    Rigidbody2D _rigidbody;

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
        // Swap controls if needed
        ManageControls();
        if (IsActive)
        {
            CollectMovementInput();
        }
    }

    private void CollectMovementInput()
    {
        // Get horizontal input with axis controls
        _horizontalInput = Input.GetAxis("Horizontal");

        // Check to see if player wants to jump
        if (Input.GetButtonDown("Jump") && _currentStamina > 0)
        {
            _wantsToJump = true;
        }

        if(Input.GetButtonDown("ThrowEgg") && _hasEgg)
        {
            ThrowEgg();
        }
    }

    private void ThrowEgg()
    {
        if(Egg.Health > 0)
        {
            _hasEgg = false;
            _currentHorizontalSpeed = HorizontalSpeedNoEgg;
            Egg.Health -= 1;

            // Get launch vector
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 chickenPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 launchDirection = (mousePos - chickenPos).normalized;

            // Instantiate egg
            _eggInstance = Instantiate(EggPrefab, chickenPos + launchDirection, Quaternion.identity);
            _eggInstance.GetComponent<Rigidbody2D>().velocity = launchDirection * EggLaunchForce;
            //_eggInstance.GetComponent<Egg>().Chicken = gameObject;
        }

    }

    private void FixedUpdate()
    {
        // Manage movement based on input
        if(IsActive)
        {
            ManageHorizontalMovement();
            ManageJump();
        }
        if(_inWater)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + FloatationForce);
        }
    }

    private void ManageControls()
    {
        if (Input.GetButtonDown("SwitchControls") && _eggInstance != null)
        {
            SwitchControls();
        }
    }

    public void SwitchControls()
    {
        // Stop movement
        if (IsActive)
        {
            _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        }
        else
        {
            Rigidbody2D eggrb = _eggInstance.GetComponent<Rigidbody2D>();
            eggrb.velocity = new Vector2(0, eggrb.velocity.y);
        }
        // Switch controls
        _eggInstance.GetComponent<Egg>().IsActive = IsActive;
        IsActive = !IsActive;
    }

    private void ManageHorizontalMovement()
    {
        // Move
        _rigidbody.velocity = new Vector2(_horizontalInput * _currentHorizontalSpeed, _rigidbody.velocity.y);
        
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
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpForce);
            if(!IsTouchingGround()) // If we just double jumped
            {
                _canDoubleJump = false;
            }
            _currentStamina -= 1;
            // VISUALS FOR STAMINA
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
        if(col.collider.gameObject.layer == LayerMask.NameToLayer("Egg"))
        {
            Destroy(col.collider.gameObject);
            if(!IsActive)
            {
                SwitchControls();
            }
            PickUpEgg();
        }

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
            }
            else
            {
                // PROMPT PLAYER TO FIND EGG
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

   
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - 0.6f), new Vector3(transform.position.x, transform.position.y - 0.5f - _groundCheckRaycastLength));
    }
}
