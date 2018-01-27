using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBallController : MonoBehaviour {

	public Transform[] path;
	public float speed;

	private int index = 0;
	private int nextIndex = 1;

	private float lerp = 0.0f;

	void Start ()
	{
		transform.position = path[0].transform.position;
	}

	void Update()
	{
		lerp += Time.deltaTime * speed;

		if(lerp >= 1)
		{
			lerp = 0;
			index++;
			nextIndex++;

			if (index == path.Length)
			{
				index = 0;
			}

			if (nextIndex == path.Length)
			{
				nextIndex = 0;
			}

		}
		transform.position = Vector2.Lerp(path[index].position, path[nextIndex].position, lerp);
	}

}
