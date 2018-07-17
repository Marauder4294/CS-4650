//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    InputManager inputManager;
    readonly GameManager GM;

    bool isPaused;

    GameObject pauseMenu;
    Image restartUnderline;
    Image exitUnderline;

    int selectionTimer;

    void Awake ()
    {
        GameManager GM = GetComponent<GameManager>();
        inputManager = new InputManager();

        isPaused = false;

        Image[] images = FindObjectsOfType<Image>();

        pauseMenu = FindObjectsOfType<GameObject>().First(a => a.name == "PauseMenu");
        restartUnderline = images.First(a => a.name == "RestartUnderline");
        exitUnderline = images.First(a => a.name == "ExitUnderline");

        pauseMenu.SetActive(false);
        restartUnderline.enabled = false;
        exitUnderline.enabled = false;

        selectionTimer = 0;
    }

    void Update()
    {
        if (!isPaused)
        {
            if (!isPaused)
            {
                if (inputManager.GetButtonDown(InputManager.InputKey.Attack))
                {
                    EventManager.AttackInitiated(true);
                }
                else if (inputManager.GetButtonDown(InputManager.InputKey.Jump))
                {
                    EventManager.JumpInitiated(true);
                }

                EventManager.BlockInitiated(inputManager.GetButton(InputManager.InputKey.Block));
            }
        }
        else
        {
            PauseMenu(inputManager.GetAxis(InputManager.InputKey.LeftThumstickY), inputManager.GetButtonDown(InputManager.InputKey.Attack));
        }

        if (inputManager.GetButtonDown(InputManager.InputKey.Start))
        {
            PauseToggle();
        }
    }

    private void FixedUpdate()
    {
        if (!isPaused)
        {
            EventManager.MoveInitiated(inputManager.GetAxis(InputManager.InputKey.LeftThumstickY), inputManager.GetAxis(InputManager.InputKey.LeftThumstickX));
            //EventManager.AvoidInitiated(Input.GetAxis(inputManager.RightThumstickY), Input.GetAxis(inputManager.RightThumstickX));
        }
    }

    private void PauseToggle()
    {
        if (!isPaused)
        {
            isPaused = true;

            Time.timeScale = 0;

            pauseMenu.SetActive(true);
            restartUnderline.enabled = true;
            exitUnderline.enabled = false;
        }
        else
        {
            isPaused = false;

            Time.timeScale = 1;

            pauseMenu.SetActive(false);
            restartUnderline.enabled = false;
            exitUnderline.enabled = false;
        }
    }


    // TODO Refactor Pause Menu
    private void PauseMenu(float moveY, bool actionButton)
    {
        //if (((Mathf.Abs(moveY) >= 0.5f) || (inputManager.controllerType == InputManager.ControllerType.Keyboard && (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)))) && selectionTimer == 0)
        //{
        //    if (restartUnderline.enabled)
        //    {
        //        restartUnderline.enabled = false;
        //        exitUnderline.enabled = true;
        //    }
        //    else
        //    {
        //        restartUnderline.enabled = true;
        //        exitUnderline.enabled = false;
        //    }

        //    selectionTimer = 30;
        //}
        //else if (actionButton)
        //{
        //    if (restartUnderline.enabled)
        //    {
        //        Time.timeScale = 1;
        //        SceneManager.LoadScene("Level 1");
        //    }
        //    else
        //    {
        //        Application.Quit();
        //    }
        //}

        //if (selectionTimer > 0)
        //{
        //    --selectionTimer;
        //}
    }
}
