using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    // Pointers
    Rigidbody2D _rigidbody;

    // Horizontal movement
    private float _horizontalInput;
    public float HorizontalSpeed;

    // Other physics
    private float _gravityScale = 1;

    // Wall detection
    private float _wallCheckRaycastLength;

    // Chicken
    [HideInInspector] public bool IsActive = false;
    private bool _airborne = true; // Can't control egg during initial throw

    public static float MaxHealth = 2;
    public static float Health = 2;

    private void Awake()
    {
        // Pointers
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = _gravityScale;
    }

    private void Update()
    {
        if(IsActive)
        {
            CollectMovementInput();
        }
        
    }

    private void CollectMovementInput()
    {
        // Get horizontal input with axis controls
        _horizontalInput = Input.GetAxis("EggHorizontal");
    }

    private void FixedUpdate()
    {
        // Manage movement based on input
        if(!_airborne && IsActive)
        {
            ManageHorizontalMovement();
        }
    }

    private void ManageHorizontalMovement()
    {
        // Move
        _rigidbody.velocity = new Vector2(_horizontalInput * HorizontalSpeed, _rigidbody.velocity.y);

        // Wall check (to prevent sticking to walls)
        if (_rigidbody.velocity.x != 0)
        {
            RaycastHit2D topHit;
            RaycastHit2D midHit;
            RaycastHit2D botHit;

            // Moving left
            if (_rigidbody.velocity.x < 0)
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y + 0.25f), Vector2.left, _wallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.left, _wallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y - 0.24f), Vector2.left, _wallCheckRaycastLength);
            }
            // Moving right
            else
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y + 0.25f), Vector2.right, _wallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y), Vector2.left, _wallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y - 0.25f), Vector2.left, _wallCheckRaycastLength);
            }
            // If we hit something, stop horizontal movement
            if (topHit.collider != null)
            {
                if(topHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        _airborne = false;
        // CRACKED VISUALS
    }
}
