using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject hit;
    private GameObject mine;
    private GameObject slowField;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        // Movement
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 10.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 10.0f;
        transform.Translate(x, y, 0);

        // Clamp position to borders
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -12.5f, 12.5f);
        pos.y = Mathf.Clamp(pos.y, -9.5f, 9.5f);
        pos.z = -1;
        transform.position = pos;

        // A Button
        bool Abtn = Input.GetKeyDown("joystick button 0");
        if (Abtn)
        {
            GameObject expl = Instantiate(Resources.Load("Prefabs/Hit"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            // Add explosion force
            Vector2 dir = player.transform.position - transform.position;
            dir = dir.normalized;
            float distance = Vector2.Distance(transform.position, player.transform.position);
            // TODO: Playtest this (if value is 2 then enemy can only push player away very close)
            if (distance < 15f)
            {
                player.GetComponent<Rigidbody2D>().AddForce((dir * (1 / (distance))) * 500);
            }
            Destroy(expl, 0.2f);
        }

        // B Button
        bool Bbtn = Input.GetKeyDown("joystick button 1");
        if (Bbtn)
        {
            GameObject slow = Instantiate(Resources.Load("Prefabs/SlowField"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            // TODO: logic of slowing down in player script
            Destroy(slow, 5f);
        }

        // X Button
        bool Xbtn = Input.GetKeyDown("joystick button 2");
        if (Xbtn)
        {
            GameObject obj = Instantiate(Resources.Load("Prefabs/Mine"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            // TODO: logic of mine stun in player script
        }

        // Y Button
        bool Ybtn = Input.GetKeyDown("joystick button 3");
        if (Ybtn)
        {
            Debug.Log("Y button pressed");
        }
    }
}