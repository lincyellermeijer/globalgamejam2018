using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    private GameObject player;
    private AudioSource source;
    public AudioClip clip;
    private float fireRate;
    private float nextFireExplosion;
    private float nextFireSlowField;
    private float nextFireMine;
    public Image[] cdImages;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Movement
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 20f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 20f;
        transform.Translate(x, y, 0);

        // Clamp position to borders
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -24.5f, 24.5f);
        pos.y = Mathf.Clamp(pos.y, -18f, 18f);
        pos.z = -1;
        transform.position = pos;

        if (cdImages[0].fillAmount > 0)
        {
            cdImages[0].fillAmount = nextFireExplosion - Time.time;
        }

        if (cdImages[1].fillAmount > 0)
        {
            cdImages[1].fillAmount = (nextFireSlowField - Time.time) / 5f;
        }

        if (cdImages[2].fillAmount > 0)
        {
            cdImages[2].fillAmount = (nextFireMine - Time.time) / 2f;
        }

        // A Button
        if (Input.GetButtonDown("Fire1") && Time.time > nextFireExplosion || Input.GetKeyDown(KeyCode.Space) && Time.time > nextFireExplosion)
        {
            source.PlayOneShot(clip);

            cdImages[0].fillAmount = 1f;

            fireRate = 1f;
            nextFireExplosion = Time.time + fireRate;
            CameraShake.script.Shake(.5f, .2f);
            GameObject expl = Instantiate(Resources.Load("Prefabs/Abilities/PrimaryFireParticles"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            // Add explosion force
            Vector2 dir = player.transform.position - transform.position;
            dir = dir.normalized;
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance < 15f)
            {
				Debug.Log("HIT");
                player.GetComponent<Rigidbody2D>().AddForce((dir * (1 / (distance))) * 1000);
            }
            Destroy(expl, 0.7f);
        }

        // B Button
        if (Input.GetButtonDown("Fire2") && Time.time > nextFireSlowField)
        {
            cdImages[1].fillAmount = 1f;

            fireRate = 5f;
            //If the player fired, reset the NextFire time to a new point in the future
            nextFireSlowField = Time.time + fireRate;

            GameObject slow = Instantiate(Resources.Load("Prefabs/Abilities/SlowField"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            Destroy(slow, 3f);
        }

        // X Button
        if (Input.GetButtonDown("Fire3") && Time.time > nextFireMine)
        {
            cdImages[2].fillAmount = 1f;

            fireRate = 2f;
            //If the player fired, reset the NextFire time to a new point in the future
            nextFireMine = Time.time + fireRate;

            GameObject obj = Instantiate(Resources.Load("Prefabs/Abilities/Mine"), new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
            Destroy(obj, 10f);
        }

        // Y Button
        if (Input.GetButtonDown("Fire4"))
        {
            Debug.Log("Y button pressed");
        }
    }
}