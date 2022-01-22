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
    public float JumpVelocity; // How much to increase player's y velocity on jump
    public float ExtendedJumpVelocity; // Velocity continuously added to a jump if the player holds the jump key
    
    // Other physics
    public float GravityScale;

    // Platform detection
    public float GroundCheckRaycastLength;
    public float WallCheckRaycastLength;

    // Respawn point
    public static Vector3 RespawnPoint;

    // Egg
    public GameObject EggPrefab;
    private bool _hasEgg = true;
    public float EggLaunchForce;

    private void Awake()
    {
        // Set speed
        _currentHorizontalSpeed = HorizontalSpeedEgg;
        // Pointers
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = GravityScale;
    }

    private void Update()
    {
        CollectInput();
    }

    private void CollectInput()
    {
        // Get horizontal input with axis controls
        _horizontalInput = Input.GetAxis("ChickenHorizontal");

        // Check to see if player wants to jump
        if (Input.GetButtonDown("Jump"))
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
        _hasEgg = false;
        _currentHorizontalSpeed = HorizontalSpeedNoEgg;

        // Get launch vector
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 chickenPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 launchDirection = (mousePos - chickenPos).normalized;

        // Instantiate egg
        GameObject egg = Instantiate(EggPrefab, chickenPos + launchDirection, Quaternion.identity);
        egg.GetComponent<Rigidbody2D>().velocity = launchDirection * EggLaunchForce;
    }

    private void FixedUpdate()
    {
        // Manage movement based on input
        ManageHorizontalMovement();
        ManageJump();
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
                topHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y + 0.5f), Vector2.left, WallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y), Vector2.left, WallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y - 0.5f), Vector2.left, WallCheckRaycastLength);
            }
            // Moving right
            else
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y + 0.5f), Vector2.right, WallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y), Vector2.left, WallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y - 0.5f), Vector2.left, WallCheckRaycastLength);
            }
            // If we hit something, stop horizontal movement
            if (topHit.collider != null || midHit.collider != null || botHit.collider != null)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
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
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpVelocity);
            if(!IsTouchingGround()) // If we just double jumped
            {
                _canDoubleJump = false;
            }
        }

        // Add a bit of speed if holding space
        if(Input.GetButton("Jump"))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + ExtendedJumpVelocity);
        }

        _wantsToJump = false;
    }
    
    private bool IsTouchingGround()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.5f, transform.position.y - 0.6f), Vector2.down, GroundCheckRaycastLength);
        RaycastHit2D rightHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.5f, transform.position.y - 0.6f), Vector2.down, GroundCheckRaycastLength);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                return true;
            }
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
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
            _hasEgg = true;
            _canDoubleJump = false;
            _currentHorizontalSpeed = HorizontalSpeedEgg;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - 0.6f), new Vector3(transform.position.x, transform.position.y - 0.5f - GroundCheckRaycastLength));
    }
}
