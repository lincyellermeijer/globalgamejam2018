using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private float fireRate;
    private float nextFireSlowField;
    private float nextFireMine;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        // Movement
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 20f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 20f;
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
        if (Bbtn && Time.time > nextFireSlowField)
        {
            fireRate = 5f;
            //If the player fired, reset the NextFire time to a new point in the future
            nextFireSlowField = Time.time + fireRate;

            GameObject slow = Instantiate(Resources.Load("Prefabs/SlowField"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;

            // TODO: logic of slowing down in player script

            Destroy(slow, 3f);
        }

        // X Button
        bool Xbtn = Input.GetKeyDown("joystick button 2");
        if (Xbtn && Time.time > nextFireMine)
        {
            fireRate = 2f;
            //If the player fired, reset the NextFire time to a new point in the future
            nextFireMine = Time.time + fireRate;

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