using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    void Update()
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
            Debug.Log("A button pressed");
        }

        // B Button
        bool Bbtn = Input.GetKeyDown("joystick button 1");
        if (Bbtn)
        {
            Debug.Log("B button pressed");
        }

        // X Button
        bool Xbtn = Input.GetKeyDown("joystick button 2");
        if (Xbtn)
        {
            Debug.Log("X button pressed");
        }

        // Y Button
        bool Ybtn = Input.GetKeyDown("joystick button 3");
        if (Ybtn)
        {
            Debug.Log("Y button pressed");
        }
    }
}
