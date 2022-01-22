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
    public float GravityScale;

    // Wall detection
    public float WallCheckRaycastLength;

    // Respawn point
    public static Vector3 RespawnPoint;

    private bool _airborne = true;

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
        _horizontalInput = Input.GetAxis("EggHorizontal");
    }

    private void FixedUpdate()
    {
        // Manage movement based on input
        if(!_airborne)
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
                topHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y + 0.25f), Vector2.left, WallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.left, WallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y - 0.25f), Vector2.left, WallCheckRaycastLength);
            }
            // Moving right
            else
            {
                topHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y + 0.25f), Vector2.right, WallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y), Vector2.left, WallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x + 0.3f, transform.position.y - 0.25f), Vector2.left, WallCheckRaycastLength);
            }
            // If we hit something, stop horizontal movement
            if (topHit.collider != null || midHit.collider != null || botHit.collider != null)
            {
                _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        _airborne = false;
    }
}
