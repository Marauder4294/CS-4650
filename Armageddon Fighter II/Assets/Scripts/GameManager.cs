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
    Text pauseMenuText;
    RawImage restartHighlight;
    RawImage exitHighlight;

    bool gameOver;

    int selectionTimer;

    void Awake ()
    {
        GameManager GM = GetComponent<GameManager>();
        inputManager = new InputManager();

        isPaused = false;

        Image[] images = FindObjectsOfType<Image>();
        RawImage[] rawImages = FindObjectsOfType<RawImage>();
        Texture[] textures = Resources.LoadAll("Buttons", typeof(Texture)).Cast<Texture>().ToArray();
        Text[] text = FindObjectsOfType<Text>();

        inputManager.SetUIButtons(rawImages, textures);

        pauseMenu = FindObjectsOfType<GameObject>().First(a => a.name == "PauseMenu");
        pauseMenuText = text.First(a => a.name == "PauseMenuText");

        restartHighlight = rawImages.First(a => a.name == "RestartHighlight");
        exitHighlight = rawImages.First(a => a.name == "ExitHighlight");

        pauseMenuText.text = "Paused";

        pauseMenu.SetActive(false);
        restartHighlight.enabled = false;
        exitHighlight.enabled = false;

        selectionTimer = 0;
    }

    void Update()
    {
        if (!isPaused)
        {
            if (!isPaused)
            {
                if (inputManager.controllerType.ToString() != "Keyboard")
                {
                    EventManager.MoveInitiated(inputManager.GetAxis(InputManager.InputKey.LeftThumstickX), inputManager.GetAxis(InputManager.InputKey.LeftThumstickY));
                    EventManager.AvoidInitiated(inputManager.GetAxis(InputManager.InputKey.RightThumstickX), inputManager.GetAxis(InputManager.InputKey.RightThumstickY));
                }
                else
                {
                    EventManager.MoveInitiated(inputManager.GetKeyboardAxis(InputManager.InputKey.KeyboardMoveUp, InputManager.InputKey.KeyboardMoveDown),
                        inputManager.GetKeyboardAxis(InputManager.InputKey.KeyboardMoveRight, InputManager.InputKey.KeyboardMoveLeft));
                    EventManager.AvoidInitiated(inputManager.GetKeyboardAxis(InputManager.InputKey.KeyboardAvoidUp, InputManager.InputKey.KeyboardAvoidDown),
                        inputManager.GetKeyboardAxis(InputManager.InputKey.KeyboardAvoidLeft, InputManager.InputKey.KeyboardAvoidRight));
                }

                if (inputManager.GetButtonDown(InputManager.InputKey.Attack))
                {
                    EventManager.AttackInitiated();
                }
                else if (inputManager.GetButtonDown(InputManager.InputKey.Jump))
                {
                    EventManager.JumpInitiated();
                }
                else if (inputManager.GetButtonDown(InputManager.InputKey.MagicOne))
                {
                    EventManager.LightningInitiated();
                }

                EventManager.BlockInitiated(inputManager.GetButton(InputManager.InputKey.Block));
            }
        }
        else
        {
            if (inputManager.controllerType.ToString() != "Keyboard")
            {
                PauseMenu(inputManager.GetAxis(InputManager.InputKey.LeftThumstickY), inputManager.GetButtonDown(InputManager.InputKey.Attack));
            }
            else
            {
                PauseMenu(inputManager.GetKeyboardAxis(InputManager.InputKey.KeyboardMoveUp, InputManager.InputKey.KeyboardMoveDown), inputManager.GetButtonDown(InputManager.InputKey.Attack));
            }
        }

        if (inputManager.GetButtonDown(InputManager.InputKey.Start))
        {
            PauseToggle();
        }
    }

    private void PauseToggle()
    {
        if (!isPaused)
        {
            isPaused = true;

            Time.timeScale = 0;

            pauseMenu.SetActive(true);
            restartHighlight.enabled = true;
            exitHighlight.enabled = false;
        }
        else if (!gameOver)
        {
            isPaused = false;

            Time.timeScale = 1;

            pauseMenu.SetActive(false);
            restartHighlight.enabled = false;
            exitHighlight.enabled = false;
        }
    }
    
    private void PauseMenu(float moveY, bool actionButton)
    {
        if ((Mathf.Abs(moveY) >= 0.5f) && selectionTimer == 0)
        {
            if (restartHighlight.enabled)
            {
                restartHighlight.enabled = false;
                exitHighlight.enabled = true;
            }
            else
            {
                restartHighlight.enabled = true;
                exitHighlight.enabled = false;
            }

            selectionTimer = 15;
        }
        else if (actionButton)
        {
            if (restartHighlight.enabled)
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("Level 1");
            }
            else
            {
                Application.Quit();
            }
        }

        if (selectionTimer > 0)
        {
            --selectionTimer;
        }
    }

    public void GameOverToggle(bool win)
    {
        gameOver = true;

        if (win)
        {
            // play win music and display 'winner'
            pauseMenuText.text = "Winner";
        }
        else
        {
            // play game over music and display 'game over'
            pauseMenuText.text = "Game Over";
        }

        PauseToggle();
    }
}