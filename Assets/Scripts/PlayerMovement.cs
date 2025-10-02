using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum PlayerId { P1, P2 }
    [Header("Identity")]
    public PlayerId playerId = PlayerId.P1;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;     // empty child at feet
    [SerializeField] float groundRadius = 0.15f;
    [SerializeField] LayerMask groundLayers;

    [Header("Visuals")]
    [SerializeField] SpriteRenderer sprite;     // assign in Inspector

    // Runtime
    Rigidbody2D rb;
    public bool isActive;        // set by TurnManager
    bool isGrounded;

    void Awake()
    {
        moveSpeed = 4.5f;
        jumpForce = 7.5f;
        rb = GetComponent<Rigidbody2D>();
        if (!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        // If it's not this player's turn: no input, invisible.
        if (!isActive) return;

        // --- Horizontal ---
        float x = 0f;
        if (playerId == PlayerId.P1)
        {
            if (Input.GetKey(KeyCode.A)) x = -1f;
            else if (Input.GetKey(KeyCode.D)) x = 1f;
        }
        else // P2
        {
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
        }

        // Apply horizontal velocity (preserve current y)
        rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

        // Optional: flip sprite to face movement
        if (x != 0 && sprite) sprite.flipX = x < 0;

        // --- Jump ---
        DoGroundCheck();
        bool jumpPressed = (playerId == PlayerId.P1 && Input.GetKeyDown(KeyCode.W))
                        || (playerId == PlayerId.P2 && Input.GetKeyDown(KeyCode.UpArrow));

        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);   // consistent jump
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void DoGroundCheck()
    {
        if (!groundCheck) return;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayers);
    }

    
    public void SetActiveState( bool active)
    {
        isActive = active;

        // Visible only when active
        if (sprite) sprite.enabled = active;


        if (!active)
        {
            // zero horizontal drift but keep gravity
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public bool IsActive() 
    { 
        return isActive; 
    
    }
}
