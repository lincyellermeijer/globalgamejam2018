using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float maxSpeed;
	private Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		var i = Input.GetAxis("Horizontal");

		if(i != 0)
		{
			rb.AddForce(new Vector2(i * speed, 0));
		}

		if(rb.velocity.x >= maxSpeed)
		{
			rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
		}
		if(rb.velocity.x <= -maxSpeed)
		{
			rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
		}

		if(Input.GetKey(KeyCode.Space))
		{
			rb.velocity = new Vector2(rb.velocity.x, 10);
		}
	}
}
