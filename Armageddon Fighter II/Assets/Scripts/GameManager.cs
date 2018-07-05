//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    InputManager inputManager;
    GameManager GM;

    void Awake ()
    {
        GameManager GM = GetComponent<GameManager>();
        inputManager = new InputManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown(inputManager.AttackButton))
        {
            EventManager.AttackInitiated(true);
        }
        else if (Input.GetButtonDown(inputManager.JumpButton))
        {
            EventManager.JumpInitiated(true);
        }

        EventManager.BlockInitiated(Input.GetButton(inputManager.BlockButton));
    }

    private void FixedUpdate()
    {
        EventManager.MoveInitiated(Input.GetAxis(inputManager.LeftThumstickY), Input.GetAxis(inputManager.LeftThumstickX));
        //EventManager.AvoidInitiated(Input.GetAxis(inputManager.RightThumstickY), Input.GetAxis(inputManager.RightThumstickX));
    }
}
