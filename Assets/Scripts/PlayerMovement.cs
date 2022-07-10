using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jump;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private Rigidbody2D body;
    private Animator anim;
    private BoxCollider2D boxCollider;
    private float wallJumpCooldown;
    private float horizontalInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Turn(horizontalInput);

        anim.SetBool("run", horizontalInput != 0);
        anim.SetBool("grounded", isGrounded());

        if (wallJumpCooldown > 0.2)
        {
            body.velocity = new Vector2(horizontalInput * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            } 
            else
            {
                body.gravityScale = 7;
            }

            Jump();
        }
        else
        {
            wallJumpCooldown += Time.deltaTime;
        }
    }

    private void Turn(float horizontalInput)
    {
        if (horizontalInput > 0.01)
            transform.localScale = Vector2.one;
        else if (horizontalInput < -0.01)
            transform.localScale = new Vector2(-1, 1);
    }

    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (isGrounded())
            {
                body.velocity = new Vector2(body.velocity.x, jump);
                anim.SetTrigger("jump");
            }
            else if (onWall() && !isGrounded())
            {
                if (horizontalInput == 0)
                {
                    body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 10, 3);
                    transform.localScale = new Vector2(-Mathf.Sign(transform.localScale.x), transform.localScale.y);
                }
                else
                {
                    body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 6);
                }

                wallJumpCooldown = 0;               
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }

    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.1f, wallLayer);
        return raycastHit.collider != null;
    }
}
