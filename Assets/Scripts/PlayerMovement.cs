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
    [Range(0, 1)]
    public float drag;
    public float jumpForce;
    public float wallJumpForce;
    public float wallJumpSidewaysForce;
    public float wallSlideSpeed;

    public float dashVelocity;
    public float dashCooldown;
    public float dashTimeStamp;

    public float stunTime;

    public bool grounded = false;
    public bool onWall = false;
    public bool canMove = true;

    Rigidbody2D rb;
    public Vector2 currentVelocity;

    public float fallingGrav;
    public float lowJumpMulitplier;

    BoxCollider2D boxColl;
    TrailRenderer trail;
    float h;

    private GroundState groundState;

    void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
        trail = GetComponent<TrailRenderer>();

        groundState = new GroundState(transform.gameObject);
    }	

	void FixedUpdate ()
    {
        h = Input.GetAxis("Horizontal");

        if (canMove)
        {
            Movement();
            Dash();
        }
	}

    void Dash()
    {
        switch (state)
        {
            case States.Ready:

                if (Input.GetButtonDown("X") && h != 0)
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
        trail.time = 1f;

        rb.velocity = new Vector2(dashVelocity * h, 0);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.1f);        
        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(maxVelocity * h, 0);
        rb.gravityScale = 1;

        trail.time = .1f;
    }

    IEnumerator Stun()
    {
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(stunTime);
        canMove = true;
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

        if (Input.GetAxis("Horizontal") != 0)
        {
            rb.AddForce(new Vector2(h * speed, 0));
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x * drag, rb.velocity.y);
        }

        //Jumping/Falling
        if (Input.GetButton("A") && groundState.isGround() && !groundState.isWall())
        {            
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        if (rb.velocity.y < 0 && !groundState.isWall())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallingGrav) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("A") && !groundState.isWall())
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMulitplier) * Time.deltaTime;
        }


        //RaycastHit2D leftRay = Physics2D.Raycast(transform.position, Vector2.left);
        //RaycastHit2D rightRay = Physics2D.Raycast(transform.position, Vector2.right);

        //if (leftRay.collider != null && rightRay.collider != null)
        //{
        //    float distanceLeft = Mathf.Abs(leftRay.point.x - transform.position.x);
        //    float distanceRight = Mathf.Abs(rightRay.point.x - transform.position.x);

        //    if (distanceLeft < distanceRight)
        //    {

        //    }
        //    else if (distanceLeft > distanceRight)
        //    {
        //        wallJumpSidewaysForce *= -1;
        //    }
        //}

        //WallJump
        if (Input.GetButton("A") && !groundState.isGround() && groundState.isWall())
        {
            onWall = false;

            //RaycastHit2D leftRay = Physics2D.Raycast(transform.position, Vector2.left);
            //RaycastHit2D rightRay = Physics2D.Raycast(transform.position, Vector2.right);

            //if (leftRay.collider != null && rightRay.collider != null)
            //{
            //    float distanceLeft = Mathf.Abs(leftRay.point.x - transform.position.x);
            //    float distanceRight = Mathf.Abs(rightRay.point.x - transform.position.x);

            //    if (distanceLeft < distanceRight)
            //    {
                    
            //    }
            //    else if (distanceLeft > distanceRight)
            //    {
            //        wallJumpSidewaysForce *= -1;
            //    }
            //}
            rb.velocity = new Vector2(groundState.wallDirection() * -wallJumpSidewaysForce, wallJumpForce);
            //rb.AddForce(new Vector2(wallJumpSidewaysForce, wallJumpForce));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Collider2D coll = collision.collider;

        //if (collision.gameObject.CompareTag("Ground"))
        //{
        //    grounded = true;
        //    onWall = false;
        //}
        //if (collision.gameObject.CompareTag("Wall"))
        //{
        //    if (!grounded)
        //    {
        //        Vector2 contactPoint = collision.contacts[0].point;
        //        Vector2 center = coll.bounds.center;
        //        if (contactPoint.x > center.x)
        //        {
        //            wallJumpSidewaysForce = -20f;
        //        }
        //        else if (contactPoint.x < center.x)
        //        {
        //            wallJumpSidewaysForce = 20f; 
        //        }
        //        onWall = true;
        //    }
        //}

        if (collision.gameObject.CompareTag("Mine"))
        {
            StartCoroutine(Stun());
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (collision.gameObject.CompareTag("Wall"))
        //{
        //    onWall = false;
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SlowField"))
        {
            rb.mass *= 2;
            rb.velocity /= 2;
            rb.angularDrag /= 2;
            fallingGrav /= 2;
            lowJumpMulitplier /= 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SlowField"))
        {
            rb.mass /= 2;
            rb.velocity *= 2;
            rb.angularDrag *= 2;
            fallingGrav *= 2;
            lowJumpMulitplier *= 2;
        }
    }
}

public class GroundState
{
    private GameObject player;
    private float width;
    private float height;
    private float length;

    //GroundState constructor.  Sets offsets for raycasting.
    public GroundState(GameObject playerRef)
    {
        player = playerRef;
        width = player.GetComponent<BoxCollider2D>().bounds.extents.x + 0.1f;
        height = player.GetComponent<BoxCollider2D>().bounds.extents.y + 0.2f;
        length = 0.05f;
    }

    //Returns whether or not player is touching wall.
    public bool isWall()
    {
        bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y), -Vector2.right, length);
        bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y), Vector2.right, length);

        if (left || right)
            return true;
        else
            return false;
    }

    //Returns whether or not player is touching ground.
    public bool isGround()
    {
        bool bottom1 = Physics2D.Raycast(new Vector2(player.transform.position.x, player.transform.position.y - height), -Vector2.up, length);
        bool bottom2 = Physics2D.Raycast(new Vector2(player.transform.position.x + (width - 0.2f), player.transform.position.y - height), -Vector2.up, length);
        bool bottom3 = Physics2D.Raycast(new Vector2(player.transform.position.x - (width - 0.2f), player.transform.position.y - height), -Vector2.up, length);
        if (bottom1 || bottom2 || bottom3)
            return true;
        else
            return false;
    }

    //Returns whether or not player is touching wall or ground.
    public bool isTouching()
    {
        if (isGround() || isWall())
            return true;
        else
            return false;
    }

    //Returns direction of wall.
    public int wallDirection()
    {
        bool left = Physics2D.Raycast(new Vector2(player.transform.position.x - width, player.transform.position.y), -Vector2.right, length);
        bool right = Physics2D.Raycast(new Vector2(player.transform.position.x + width, player.transform.position.y), Vector2.right, length);

        if (left)
            return -1;
        else if (right)
            return 1;
        else
            return 0;
    }
}
