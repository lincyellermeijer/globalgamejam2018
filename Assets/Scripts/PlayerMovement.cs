using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public enum States
    {
        Ready,
        Dashing,
        Cooldown
    }

    public States state;

    public float speed;
    public float maxVelocity;
    public float jumpForce;
    public float wallJumpForce;
    public float wallJumpSidewaysForce;
    public float wallSlideSpeed;

    public float dashVelocity;
    public float dashCooldown;
    public float dashTimeStamp;

    public bool grounded = false;
    public bool onWall = false;

    Rigidbody2D rb;
    public Vector2 currentVelocity;

    public float fallingGrav;
    public float lowJumpMulitplier;

    BoxCollider2D boxColl;
    float h;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
    }	

	void FixedUpdate ()
    {
        h = Input.GetAxis("Move");

        Debug.Log(h);
        Movement();
        Dash();
	}

    void Dash()
    {
        switch (state)
        {
            case States.Ready:

                if (Input.GetButtonDown("Dash"))
                {
                    state = States.Dashing;
                }
                break;

            case States.Dashing:

                StartCoroutine(PlayerDash());
                dashTimeStamp = Time.time + dashCooldown;
                state = States.Cooldown;
                break;

            case States.Cooldown:

                if (dashTimeStamp <= Time.time)
                {
                    state = States.Ready;
                }
                break;
        }
    }

    IEnumerator PlayerDash()
    {
        h = Input.GetAxis("Move");
        rb.velocity = new Vector2(dashVelocity * h, 0);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.1f);
        rb.velocity = Vector2.zero;
        rb.gravityScale = 1;
    }

    void Movement()
    {

        //Player Movement        
        currentVelocity = rb.velocity;

        //MaxVelocity Settings
        if (currentVelocity.x >= maxVelocity)
        {
            currentVelocity.x = maxVelocity;
        }
        if (currentVelocity.x <= -maxVelocity)
        {
            currentVelocity.x = -maxVelocity;
        }
        if (currentVelocity.y <= -10)
        {
            currentVelocity.y = 10;
        }

        if (Input.GetAxis("Move") != 0)
        {
            rb.AddForce(new Vector2(h * speed, 0));
        }

        //Jumping/Falling
        if (Input.GetButton("Jump") && grounded && !onWall)
        {            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            grounded = false;
        }
        if (rb.velocity.y < 0 && !onWall)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallingGrav) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump") && !onWall)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMulitplier) * Time.deltaTime;
        }

        //WallJump
        if (Input.GetButton("Jump") && !grounded && onWall)
        {
            onWall = false;

            RaycastHit2D leftRay = Physics2D.Raycast(transform.position, Vector2.left);
            RaycastHit2D rightRay = Physics2D.Raycast(transform.position, Vector2.right);

            if (leftRay.collider != null && rightRay.collider != null)
            {
                float distanceLeft = Mathf.Abs(leftRay.point.x - transform.position.x);
                float distanceRight = Mathf.Abs(rightRay.point.x - transform.position.x);

                if (distanceLeft < distanceRight)
                {
                    wallJumpSidewaysForce = 10f;
                }
                else if (distanceLeft > distanceRight)
                {
                    wallJumpSidewaysForce = -10f;
                }
            }
            rb.velocity = new Vector2(wallJumpSidewaysForce, wallJumpForce);

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            onWall = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (!grounded)
            {
                onWall = true;
            }
        }
    }
}
