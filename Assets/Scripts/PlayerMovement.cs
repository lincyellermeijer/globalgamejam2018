using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	//TODO: put this in its own script
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

	//TODO: feels like slowmotion, which is not cool... could increase if size of world and edit world speed? Unity physics engine is slow or something
	public float speed = 14f;
	public float accel = 6f;
	public float airAccel = 3f;
	public float jump = 14f; 

	private GroundState groundState;
	private Rigidbody2D rb;

	void Start()
	{
		groundState = new GroundState(transform.gameObject);
		rb = GetComponent<Rigidbody2D>();
	}

	private Vector2 input;

	void Update()
	{
		//Handle input
		if (Input.GetAxis("Horizontal") < 0)
			input.x = -1;
		else if (Input.GetAxis("Horizontal") > 0)
			input.x = 1;
		else
			input.x = 0;

		if (Input.GetButton("A"))
			input.y = 1;

	}

	void FixedUpdate()
	{
		rb.AddForce(new Vector2(((input.x * speed) - rb.velocity.x) * (groundState.isGround() ? accel : airAccel), 0)); //Move player.
		rb.velocity = new Vector2((input.x == 0 && groundState.isGround()) ? 0 : rb.velocity.x,
								(input.y == 1 && groundState.isTouching()) ? jump : rb.velocity.y); //Stop player if input.x is 0 (and grounded) and jump if input.y is 1

		if (groundState.isWall() && !groundState.isGround() && input.y == 1)
			rb.velocity = new Vector2(-groundState.wallDirection() * speed * 0.75f, rb.velocity.y); //Add force negative to wall direction (with speed reduction)

		input.y = 0;
	}
}