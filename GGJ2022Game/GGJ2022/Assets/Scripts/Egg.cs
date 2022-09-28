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
    private float _floatationForce = 0.4f;
    private float _floatationDelay = 0.1f; // A short delay before water applies floatation to the egg, to create a bobbing motion.
    private float _waterEntrySpeed = -1; // Egg's y-velocity upon entering water
    private float _floatationTimer = 0;
    private bool _inWater = false;

    // Wall detection
    private float _wallCheckRaycastLength;

    // Chicken
    [HideInInspector] public bool IsActive = false;
    private bool _airborne = true; // Can't control egg during initial throw

    public static float MaxHealth = 2;
    public static float Health = 2;

    // Animation
    [SerializeField] private Animator _eggAnimator;
    [SerializeField] private SpriteRenderer _eggRenderer;

    private void Awake()
    {
        switch (Health)
        {
            case 1:
                {
                    _eggAnimator.Play("2ER");
                    break;
                }
            case 2:
                {
                    _eggAnimator.Play("1ER");
                    break;
                }
        }
        Health -= 1;
        // Pointers
        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _rigidbody.gravityScale = _gravityScale;
    }

    private void Update()
    {
        _eggAnimator.speed = (Mathf.Abs(_rigidbody.velocity.x) / HorizontalSpeed);
        if (IsActive)
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
        if (_inWater)
        {
            _floatationTimer += Time.deltaTime;
            if(_floatationTimer > _floatationDelay)
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y + _floatationForce);
            }
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
                _eggRenderer.flipX = true;
                topHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y + 0.25f), Vector2.left, _wallCheckRaycastLength);
                midHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y), Vector2.left, _wallCheckRaycastLength);
                botHit = Physics2D.Raycast(new Vector2(transform.position.x - 0.3f, transform.position.y - 0.24f), Vector2.left, _wallCheckRaycastLength);
            }
            // Moving right
            else
            {
                _eggRenderer.flipX = false;
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
        switch (Health)
        {
            case 0:
                {
                    _eggAnimator.Play("3ER");
                    break;
                }
            case 1:
                {
                    _eggAnimator.Play("2ER");
                    break;
                }
            case 2:
                {
                    _eggAnimator.Play("1ER");
                    break;
                }
        }

        //Parent to moving platform
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
            Debug.Log("Water");
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _waterEntrySpeed);
            _inWater = true;
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Nest") || col.gameObject.layer == LayerMask.NameToLayer("Goal"))
        {
            // PROMPT PLAYER TO FIND CHICKEN   
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            _inWater = false;
            _floatationTimer = 0;
        }
    }
}
