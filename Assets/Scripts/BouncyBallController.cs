using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBallController : MonoBehaviour {

	public int speed;
	public Vector2 direction;
	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(direction * speed);
	}
	
	void Update () {
		
	}
}
