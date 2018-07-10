//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager
{

    #region ControllerType & Button Declarations

    public enum ControllerType { PS4 , XBOX, Keyboard  };
    public ControllerType controllerType;

    #region Buttons

    #region PS4 Controller Buttons

    /*
    
    JoystickButton0 = Square
    JoystickButton1 = X
    JoystickButton2 = Circle
    JoystickButton3 = Triangle
    JoystickButton4 = L1
    JoystickButton5 = R1
    JoystickButton6 = L2
    JoystickButton7 = R2
    JoystickButton8 = Share Button
    JoystickButton9 = Options Button
    JoystickButton10 = L3
    JoystickButton11 = R3
    JoystickButton12 = PS Button
    JoystickButton13 = Pad Button

    LeftStickX      = X-Axis
    LeftStickY      = Y-Axis (Inverted?)
    RightStickX     = 3rd Axis
    RightStickY     = 4th Axis (Inverted?)
    L2              = 5th Axis (-1.0f to 1.0f range, unpressed is -1.0f)
    R2              = 6th Axis (-1.0f to 1.0f range, unpressed is -1.0f)
    DPadX           = 7th Axis
    DPadY           = 8th Axis (Inverted?)

    */

    #endregion

    #region XBOX Controller Buttons

    /*
    
    JoystickButton0 = A
    JoystickButton1 = B
    JoystickButton2 = X
    JoystickButton3 = Y
    JoystickButton4 = Left Bumper
    JoystickButton5 = Right Bumper
    JoystickButton6 = Back Button
    JoystickButton7 = Start Button
    JoystickButton8 = Left Stick Click
    JoystickButton9 = Right Stick Click

    */

    #endregion

    public string AttackButton { get; set; }
    public string BlockButton { get; set; }
    public string JumpButton { get; set; }
    public string LeftThumstickY { get; set; }
    public string LeftThumstickX { get; set; }
    public string RightThumstickY { get; set; }
    public string RightThumstickX { get; set; }
    public string StartButton { get; set; }
    public string MagicOneButton { get; set; }
    public string MagicTwoButton { get; set; }
    public string MagicThreeButton { get; set; }
    public string InventoryButton { get; set; }

    #endregion

    #endregion

    public InputManager()
    {
        string[] controllerTypes = Input.GetJoystickNames();
        
        if (controllerTypes.Contains("Controller (XBOX 360 For Windows)"))
        {
            SetXBOXControllerAndButtons();
        }
        else if (controllerTypes.Contains("Wireless Controller"))
        {
            SetPS4ControllerAndButtons();
        }
        else
        {
            SetKeyboardAndButtons();
        }
    }

    void SetPS4ControllerAndButtons()
    {
        controllerType = ControllerType.PS4;

        AttackButton = "PS4 X Button";
        BlockButton = "PS4 L2 Button";
        JumpButton = "PS4 Square Button";
        StartButton = "PS4 Options Button";
        LeftThumstickY = "Controller Left Stick Y";
        LeftThumstickX = "Controller Left Stick X";
        RightThumstickX = "PS4 Right Stick X";
        RightThumstickY = "PS4 Right Stick Y";
    }

    void SetXBOXControllerAndButtons()
    {
        controllerType = ControllerType.XBOX;

        AttackButton = "XBOX A Button";
        BlockButton = "XBOX Y Button";
        JumpButton = "XBOX X Button";
        StartButton = "XBOX Start Button";
        LeftThumstickY = "Controller Left Stick Y";
        LeftThumstickX = "Controller Left Stick X";
        RightThumstickX = "XBOX Right Stick X";
        RightThumstickY = "XBOX Right Stick Y";
    }

    void SetKeyboardAndButtons()
    {
        controllerType = ControllerType.Keyboard;

        AttackButton = "Keyboard Right Ctrl";
        BlockButton = "Keyboard Right Shift";
        JumpButton = "Keyboard Space";
        StartButton = "Keyboard Return";
        LeftThumstickY = "Keyboard Left Stick Y";
        LeftThumstickX = "Keyboard Left Stick X";
        RightThumstickX = "Keyboard Right Stick X";
        RightThumstickY = "Keyboard Right Stick Y";
    }
}
