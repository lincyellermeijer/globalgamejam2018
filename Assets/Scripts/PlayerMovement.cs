using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public enum States
    {
        InAir,
        Dashing,
        WallCling
    }

    public float speed;
    public float maxVelocity;
    public float jumpForce;
    public float wallJumpForce;
    public float wallJumpSidewaysForce;
    public float wallSlideSpeed;

    public bool grounded = false;
    public bool onWall = false;

    Rigidbody2D rb;
    public Vector2 currentVelocity;

    public float fallingGrav;
    public float lowJumpMulitplier;

    BoxCollider2D boxColl;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
    }	

	void FixedUpdate ()
    {
        Movement();
	}

    void Movement()
    {
        //Player Movement
        float h = Input.GetAxis("Horizontal");
        currentVelocity = rb.velocity;

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

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddForce(new Vector2(h * speed, 0));
        }

        //Jumping/Falling
        if (Input.GetButton("A") && grounded && !onWall)
        {            
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y + jumpForce);
            grounded = false;
        }
        if (rb.velocity.y < 0 && !onWall)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallingGrav) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("A") && !onWall)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMulitplier) * Time.deltaTime;
        }

        if (Input.GetButton("A") && !grounded && onWall)
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
            rb.velocity = new Vector2(rb.velocity.x + wallJumpSidewaysForce, rb.velocity.y + wallJumpForce);

        }
    }

    void WallJump()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            grounded = true;
            onWall = false;
        }

        //if (collision.gameObject.CompareTag("Wall"))
        //{

        //}
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
