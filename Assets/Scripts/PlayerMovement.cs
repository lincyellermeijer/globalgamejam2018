using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public enum States
    {
        Ready,
        Dashing,
        Cooldown
    }

    public States state;

    public float speed = 14f;
    public float jumpForce = 14f;
    public float accel = 6f;
    public float airAccel = 3f;

    public float dashVelocity;
    public float dashCooldown;
    public float dashTimeStamp;

    public float stunTime;
    public bool canMove = true;

    Rigidbody2D rb;
    public Vector2 currentVelocity;

    public float fallingGrav;
    public float lowJumpMulitplier;

    BoxCollider2D boxColl;
    TrailRenderer trail;
    ParticleSystem particle;
    SpriteRenderer rend;
    float h;

    int randomIndex;
    public int collectibles;
    public GameObject[] spawnLocation;

    private GroundState groundState;
    private Vector3 input; // z is dash 

    public Sprite[] sprite;
    public Sprite currentSprite;

    void Start()
    {
        canMove = true;

        rb = GetComponent<Rigidbody2D>();
        boxColl = GetComponent<BoxCollider2D>();
        trail = GetComponent<TrailRenderer>();
        particle = GetComponent<ParticleSystem>();
        rend = GetComponent<SpriteRenderer>();

        particle.Stop();
        currentSprite = sprite[0];

        groundState = new GroundState(transform.gameObject);

        randomIndex = Random.Range(0, spawnLocation.Length);
        GameObject item = Instantiate(Resources.Load("Prefabs/Collectible"), spawnLocation[randomIndex].transform.position, spawnLocation[randomIndex].transform.rotation) as GameObject;
    }

    void Update()
    {
        input.x = Input.GetAxis("Move");


        if (Input.GetButton("Jump"))
            input.y = 1;

        if (Input.GetButtonDown("Dash"))
        {
            input.z = 1;
        }

        rend.sprite = currentSprite;
    }



    void FixedUpdate()
    {
        if (!canMove)
        {
            return;
        }

        Dash();

        rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * (groundState.isGround() ? accel : airAccel), 0)); //Move player.
        rb.velocity = new Vector2((input.x == 0 && groundState.isGround()) ? 0 : rb.velocity.x,
                                (input.y == 1 && groundState.isTouching()) ? jumpForce : rb.velocity.y); //Stop player if input.x is 0 (and grounded) and jump if input.y is 1

        if (groundState.isWall() && !groundState.isGround() && input.y == 1)
            rb.velocity = new Vector2(-groundState.wallDirection() * speed * 0.75f, rb.velocity.y); //Add force negative to wall direction (with speed reduction)

        input.y = 0;
    }

    void Dash()
    {
        switch (state)
        {
            case States.Ready:

                if (input.z == 1 && input.x != 0)
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
        input.z = 0;
    }

    IEnumerator PlayerDash()
    {
        //trail.time = 1f;
        particle.Play();
        currentSprite = sprite[1];

        rb.velocity = new Vector2(dashVelocity * input.x, 0);
        rb.gravityScale = 0;
        yield return new WaitForSeconds(0.1f);
        currentSprite = sprite[0];
        rb.velocity = Vector2.zero;
        rb.velocity = new Vector2(speed * input.x, 0);
        rb.gravityScale = 3;

        //trail.time = .1f;
        particle.Stop();
    }

    IEnumerator Stun()
    {
        canMove = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        yield return new WaitForSeconds(stunTime);
        canMove = true;
    }

    IEnumerator PlayerDead()
    {
        canMove = false;
        currentSprite = sprite[2];
        Debug.Log(rend.sprite);
        yield return new WaitForSeconds(1f);
        //go back to menu
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mine"))
        {
            StartCoroutine(Stun());
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(PlayerDead());
        }
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

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(PlayerDead());
        }

        if (collision.gameObject.CompareTag("Collectible"))
        {
            collectibles++;
            Destroy(collision.gameObject);
            randomIndex = Random.Range(0, spawnLocation.Length);
            GameObject item = Instantiate(Resources.Load("Prefabs/Collectible"), spawnLocation[randomIndex].transform.position, spawnLocation[randomIndex].transform.rotation) as GameObject;
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