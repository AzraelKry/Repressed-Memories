using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance;

    public float speed = 3f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private Vector2 lastMoveDir;

    public bool CanMove { get; set; } = true;

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!CanMove)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isWalking", false);
            animator.speed = 0f;
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        bool isWalking = moveInput.magnitude > 0.01f;

        animator.SetFloat("moveX", moveInput.x);
        animator.SetFloat("moveY", moveInput.y);
        animator.SetBool("isWalking", isWalking);

        if (moveInput.x > 0)
            spriteRenderer.flipX = true;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = false;

        if (isWalking)
            lastMoveDir = moveInput;
        else
        {
            animator.SetFloat("moveX", lastMoveDir.x);
            animator.SetFloat("moveY", lastMoveDir.y);
        }

        if (isWalking)
            animator.speed = 1f;
        else
            animator.speed = 0f;
    }

    void FixedUpdate()
    {
        if (!CanMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = moveInput * speed;
    }
}
