﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public Text[] isConnectedText = new Text[2];
    private bool bothConnected = false;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        CheckControllers();
        Time.timeScale = 1f;
    }

    // Start game if both controllers are connected and one of the players presses start
    public void Update()
    {
        CheckControllers();

        if (bothConnected && SceneManager.GetActiveScene().name == "menu")
        {
            if (Input.GetButtonDown("Start"))
            {
                MenuManager.Instance.sceneToStart = 1;
                MenuManager.Instance.StartPressed();
            }
        }
    }

    // TODO: Do this at specific times to check if controllers are still connected (activate pause menu during gameplay or smth)
    public void CheckControllers()
    {
        //Get Joystick Names
        string[] temp = Input.GetJoystickNames();

        //Iterate over every element
        for (int i = 0; i < temp.Length; ++i)
        {
            //Check if the string is empty or not
            if (!string.IsNullOrEmpty(temp[i]))
            {
                //Not empty, controller temp[i] is connected
                isConnectedText[i].text = "Controller " + (i+1) + " is connected";
                isConnectedText[i].color = Color.green;

                if (i == 1)
                {
                    bothConnected = true;
                }
            }
            else
            {
                //If it is empty, controller i is disconnected where i indicates the controller number
                isConnectedText[i].text = "Controller " + (i+1) + " is NOT connected!";
                isConnectedText[i].color = Color.red;
                bothConnected = false;
            }
        }
    }
}