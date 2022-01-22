using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenLeader : MonoBehaviour
{
    // Pointers
    Rigidbody2D _rigidbody;
    // Horizontal movement
    private float _horizontalInput;
    public float HorizontalSpeedMultiplier;

    // Jumping
    private bool _wantsToJump = false; // Stores player input for jumping. Needed since input is in Update and Jump is in FixedUpdate
    public float JumpVelocity; // How much to increase player's y velocity on jump
    public float ExtendedJumpVelocity; // Velocity continuously added to a jump if the player holds the jump key
    public float GravityScale;
    public float GroundCheckRaycastLength;
    public float WallCheckRaycastLength;
    
    // Respawn point
    public static Vector3 RespawnPoint;

    private void Awake()
    {
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
        _horizontalInput = Input.GetAxis("Horizontal");

        // Check to see if player wants to jump
        if (Input.GetButtonDown("Jump"))
        {
            _wantsToJump = true;
        }
    }

    private void FixedUpdate()
    {
        // Manage movement based on input
        ManageHorizontalMovement();
        ManageJump();
        _wantsToJump = false;
    }

    private void ManageHorizontalMovement()
    {
        // Move
        _rigidbody.velocity = new Vector2(_horizontalInput * HorizontalSpeedMultiplier, _rigidbody.velocity.y);
        
        // Wall check (to prevent sticking to walls)
        if (_rigidbody.velocity.x != 0)
        {
            RaycastHit2D hit;

            // Moving left
            if (_rigidbody.velocity.x < 0)
            {
                hit = Physics2D.Raycast(new Vector2(transform.position.x - 0.6f, transform.position.y), Vector2.left, WallCheckRaycastLength);
            }
            // Moving right
            else
            {
                hit = Physics2D.Raycast(new Vector2(transform.position.x + 0.6f, transform.position.y), Vector2.right, WallCheckRaycastLength);
            }
            // If we hit something, stop horizontal movement
            if (hit.collider != null)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            }
        }
        
    }

    private void ManageJump()
    {
        // Initial jump
        if (IsTouchingGround() && _wantsToJump)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, JumpVelocity);
        }

        // Add a bit of speed if holding space
        if(Input.GetButton("Jump"))
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + ExtendedJumpVelocity);
        }
    }
    
    private bool IsTouchingGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - 0.6f), Vector2.down, GroundCheckRaycastLength);
        if(hit.collider != null)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                return true;
            }
        }
        return false;       
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(transform.position.x, transform.position.y - 0.6f), new Vector3(transform.position.x, transform.position.y - 0.5f - GroundCheckRaycastLength));
    }
}
