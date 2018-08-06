//using System.Collections;
//using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager
{
    //TODO Add Left and Right Trigger Buttons for XBOX

    #region ControllerType & Button Declarations

    public enum ControllerType { Keyboard, PS4, XBOX };
    enum InputType
    {
        Button, AxisX, AxisXInverted, AxisY, AxisYInverted, Axis3, Axis3Inverted, Axis4, Axis4Inverted, Axis5, Axis5Inverted, Axis6, Axis6Inverted, Axis7, Axis7Inverted,
        Axis8, Axis8Inverted, Axis9, Axis9Inverted, Axis10, Axis10Inverted
    }

    public enum InputKey
    {
        Attack, Block, Jump, LeftThumstickX, LeftThumstickY, RightThumstickX, RightThumstickY, Start, Inventory, MagicOne, MagicTwo, MagicThree, KeyboardMoveUp, KeyboardMoveDown,
        KeyboardMoveLeft, KeyboardMoveRight, KeyboardAvoidUp, KeyboardAvoidDown, KeyboardAvoidLeft, KeyboardAvoidRight
    }

    public ControllerType controllerType;

    #region Buttons

    struct ControlInput
    {
        public string inputName;
        public ControllerType controllerType;
        public InputType inputType;
        public KeyCode inputKeyCode;

        public ControlInput(string iName, ControllerType ct, InputType it, KeyCode ik)
        {
            inputName = iName;
            controllerType = ct;
            inputType = it;
            inputKeyCode = ik;
        }
    }

    struct PlayInput
    {
        public InputKey inputKey;
        public ControlInput controlInput;

        public PlayInput(InputKey ik, ControlInput ci)
        {
            inputKey = ik;
            controlInput = new ControlInput(ci.inputName, ci.controllerType, ci.inputType, ci.inputKeyCode);
        }
    }

    ControlInput[] PS4controlInputs = {

        #region Buttons

        new ControlInput("X", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton1),
        new ControlInput("Square", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton0),
        new ControlInput("Circle", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton2),
        new ControlInput("Triangle", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton3),
        new ControlInput("L1", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton4),
        new ControlInput("R1", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton5),
        new ControlInput("L2", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton6),
        new ControlInput("R2", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton7),
        new ControlInput("Share", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton8),
        new ControlInput("Options", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton9),
        new ControlInput("L3", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton10),
        new ControlInput("R3", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton11),
        new ControlInput("PS", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton12),
        new ControlInput("Pad", ControllerType.PS4, InputType.Button, KeyCode.JoystickButton13),

        #endregion

        #region Axes

        new ControlInput("Left Stick X", ControllerType.PS4, InputType.AxisXInverted, KeyCode.None),
        new ControlInput("Left Stick Y", ControllerType.PS4, InputType.AxisY, KeyCode.None),
        new ControlInput("Right Stick X", ControllerType.PS4, InputType.Axis3Inverted, KeyCode.None),
        new ControlInput("Right Stick Y", ControllerType.PS4, InputType.Axis4, KeyCode.None),
        new ControlInput("D-Pad X", ControllerType.PS4, InputType.Axis7, KeyCode.None),
        new ControlInput("D-Pad Y", ControllerType.PS4, InputType.Axis8, KeyCode.None)

        #endregion
    };

    ControlInput[] XBOXcontrolInputs = {

        #region Buttons

        new ControlInput("A", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton0),
        new ControlInput("X", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton2),
        new ControlInput("B", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton1),
        new ControlInput("Y", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton3),
        new ControlInput("Left Trigger", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton4),
        new ControlInput("Right Trigger", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton5),
        new ControlInput("Back", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton6),
        new ControlInput("Start", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton7),
        new ControlInput("Left Stick Click", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton8),
        new ControlInput("Right Stick Click", ControllerType.XBOX, InputType.Button, KeyCode.JoystickButton9),

        #endregion

        #region Axes

        new ControlInput("Left Stick X", ControllerType.XBOX, InputType.AxisXInverted, KeyCode.None),
        new ControlInput("Left Stick Y", ControllerType.XBOX, InputType.AxisY, KeyCode.None),
        new ControlInput("Right Stick X", ControllerType.XBOX, InputType.Axis4Inverted, KeyCode.None),
        new ControlInput("Right Stick Y", ControllerType.XBOX, InputType.Axis5, KeyCode.None),
        new ControlInput("Left Trigger", ControllerType.XBOX, InputType.Axis9, KeyCode.None),
        new ControlInput("Right Trigger", ControllerType.XBOX, InputType.Axis10, KeyCode.None),
        new ControlInput("D-Pad X", ControllerType.XBOX, InputType.Axis6, KeyCode.None),
        new ControlInput("D-Pad Y", ControllerType.XBOX, InputType.Axis7, KeyCode.None)

        #endregion
    };

    ControlInput[] KeyboardcontrolInputs = {

        #region Buttons

        new ControlInput("Backspace", ControllerType.Keyboard, InputType.Button, KeyCode.Backspace),
        new ControlInput("Delete", ControllerType.Keyboard, InputType.Button, KeyCode.Delete),
        new ControlInput("Tab", ControllerType.Keyboard, InputType.Button, KeyCode.Tab),
        new ControlInput("Clear", ControllerType.Keyboard, InputType.Button, KeyCode.Clear),
        new ControlInput("Return/Enter", ControllerType.Keyboard, InputType.Button, KeyCode.Return),
        new ControlInput("Pause/Break", ControllerType.Keyboard, InputType.Button, KeyCode.Pause),
        new ControlInput("Escape", ControllerType.Keyboard, InputType.Button, KeyCode.Escape),
        new ControlInput("Space", ControllerType.Keyboard, InputType.Button, KeyCode.Space),
        new ControlInput("Keypad 0", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad0),
        new ControlInput("Keypad 1", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad1),
        new ControlInput("Keypad 2", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad2),
        new ControlInput("Keypad 3", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad3),
        new ControlInput("Keypad 4", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad4),
        new ControlInput("Keypad 5", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad5),
        new ControlInput("Keypad 6", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad6),
        new ControlInput("Keypad 7", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad7),
        new ControlInput("Keypad 8", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad8),
        new ControlInput("Keypad 9", ControllerType.Keyboard, InputType.Button, KeyCode.Keypad9),
        new ControlInput("Keypad Period", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadPeriod),
        new ControlInput("Keypad Divide", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadDivide),
        new ControlInput("Keypad Multiply", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadMultiply),
        new ControlInput("Keypad Minus", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadMinus),
        new ControlInput("Keypad Plus", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadPlus),
        new ControlInput("Keypad Enter", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadEnter),
        new ControlInput("Keypad Equals", ControllerType.Keyboard, InputType.Button, KeyCode.KeypadEquals),
        new ControlInput("Up Arrow", ControllerType.Keyboard, InputType.Button, KeyCode.UpArrow),
        new ControlInput("Down Arrow", ControllerType.Keyboard, InputType.Button, KeyCode.DownArrow),
        new ControlInput("Right Arrow", ControllerType.Keyboard, InputType.Button, KeyCode.RightArrow),
        new ControlInput("Left Arrow", ControllerType.Keyboard, InputType.Button, KeyCode.LeftArrow),
        new ControlInput("Insert", ControllerType.Keyboard, InputType.Button, KeyCode.Insert),
        new ControlInput("Home", ControllerType.Keyboard, InputType.Button, KeyCode.Home),
        new ControlInput("End", ControllerType.Keyboard, InputType.Button, KeyCode.End),
        new ControlInput("Page Up", ControllerType.Keyboard, InputType.Button, KeyCode.PageUp),
        new ControlInput("PageDown", ControllerType.Keyboard, InputType.Button, KeyCode.PageDown),
        new ControlInput("F1", ControllerType.Keyboard, InputType.Button, KeyCode.F1),
        new ControlInput("F2", ControllerType.Keyboard, InputType.Button, KeyCode.F2),
        new ControlInput("F3", ControllerType.Keyboard, InputType.Button, KeyCode.F3),
        new ControlInput("F4", ControllerType.Keyboard, InputType.Button, KeyCode.F4),
        new ControlInput("F5", ControllerType.Keyboard, InputType.Button, KeyCode.F5),
        new ControlInput("F6", ControllerType.Keyboard, InputType.Button, KeyCode.F6),
        new ControlInput("F7", ControllerType.Keyboard, InputType.Button, KeyCode.F7),
        new ControlInput("F8", ControllerType.Keyboard, InputType.Button, KeyCode.F8),
        new ControlInput("F9", ControllerType.Keyboard, InputType.Button, KeyCode.F9),
        new ControlInput("F10", ControllerType.Keyboard, InputType.Button, KeyCode.F10),
        new ControlInput("F11", ControllerType.Keyboard, InputType.Button, KeyCode.F11),
        new ControlInput("F12", ControllerType.Keyboard, InputType.Button, KeyCode.F12),
        new ControlInput("AlphaNumeric 0/Right Parenthesis", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha0),
        new ControlInput("AlphaNumeric 1/Exclamation Point", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha1),
        new ControlInput("AlphaNumeric 2/@ Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha2),
        new ControlInput("AlphaNumeric 3/# Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha3),
        new ControlInput("AlphaNumeric 4/$ Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha4),
        new ControlInput("AlphaNumeric 5/% Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha5),
        new ControlInput("AlphaNumeric 6/^ Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha6),
        new ControlInput("AlphaNumeric 7/& Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha7),
        new ControlInput("AlphaNumeric 8/* Symbol", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha8),
        new ControlInput("AlphaNumeric 9/Left Parenthesis", ControllerType.Keyboard, InputType.Button, KeyCode.Alpha9),
        new ControlInput("Quote/Double Quote", ControllerType.Keyboard, InputType.Button, KeyCode.Quote),
        new ControlInput("Comma/Less Than", ControllerType.Keyboard, InputType.Button, KeyCode.Comma),
        new ControlInput("Minus/Underscore", ControllerType.Keyboard, InputType.Button, KeyCode.Minus),
        new ControlInput("Period/Greater Than", ControllerType.Keyboard, InputType.Button, KeyCode.Period),
        new ControlInput("Slash/Question Mark", ControllerType.Keyboard, InputType.Button, KeyCode.Slash),
        new ControlInput("Semicolon/Colon", ControllerType.Keyboard, InputType.Button, KeyCode.Semicolon),
        new ControlInput("Equals/Plus", ControllerType.Keyboard, InputType.Button, KeyCode.Equals),
        new ControlInput("Left Square Bracket/ Left Curly Bracket", ControllerType.Keyboard, InputType.Button, KeyCode.LeftBracket),
        new ControlInput("Backslash", ControllerType.Keyboard, InputType.Button, KeyCode.Backslash),
        new ControlInput("Right Square Bracket/ Right Curly Bracket", ControllerType.Keyboard, InputType.Button, KeyCode.RightBracket),
        new ControlInput("Backquote/Tilde", ControllerType.Keyboard, InputType.Button, KeyCode.BackQuote),
        new ControlInput("A", ControllerType.Keyboard, InputType.Button, KeyCode.A),
        new ControlInput("B", ControllerType.Keyboard, InputType.Button, KeyCode.B),
        new ControlInput("C", ControllerType.Keyboard, InputType.Button, KeyCode.C),
        new ControlInput("D", ControllerType.Keyboard, InputType.Button, KeyCode.D),
        new ControlInput("E", ControllerType.Keyboard, InputType.Button, KeyCode.E),
        new ControlInput("F", ControllerType.Keyboard, InputType.Button, KeyCode.F),
        new ControlInput("G", ControllerType.Keyboard, InputType.Button, KeyCode.G),
        new ControlInput("H", ControllerType.Keyboard, InputType.Button, KeyCode.H),
        new ControlInput("I", ControllerType.Keyboard, InputType.Button, KeyCode.I),
        new ControlInput("J", ControllerType.Keyboard, InputType.Button, KeyCode.J),
        new ControlInput("K", ControllerType.Keyboard, InputType.Button, KeyCode.K),
        new ControlInput("L", ControllerType.Keyboard, InputType.Button, KeyCode.L),
        new ControlInput("M", ControllerType.Keyboard, InputType.Button, KeyCode.M),
        new ControlInput("N", ControllerType.Keyboard, InputType.Button, KeyCode.N),
        new ControlInput("O", ControllerType.Keyboard, InputType.Button, KeyCode.O),
        new ControlInput("P", ControllerType.Keyboard, InputType.Button, KeyCode.P),
        new ControlInput("Q", ControllerType.Keyboard, InputType.Button, KeyCode.Q),
        new ControlInput("R", ControllerType.Keyboard, InputType.Button, KeyCode.R),
        new ControlInput("S", ControllerType.Keyboard, InputType.Button, KeyCode.S),
        new ControlInput("T", ControllerType.Keyboard, InputType.Button, KeyCode.T),
        new ControlInput("U", ControllerType.Keyboard, InputType.Button, KeyCode.U),
        new ControlInput("V", ControllerType.Keyboard, InputType.Button, KeyCode.V),
        new ControlInput("W", ControllerType.Keyboard, InputType.Button, KeyCode.W),
        new ControlInput("X", ControllerType.Keyboard, InputType.Button, KeyCode.X),
        new ControlInput("Y", ControllerType.Keyboard, InputType.Button, KeyCode.Y),
        new ControlInput("Z", ControllerType.Keyboard, InputType.Button, KeyCode.Z),
        new ControlInput("Numlock", ControllerType.Keyboard, InputType.Button, KeyCode.Numlock),
        new ControlInput("CapsLock", ControllerType.Keyboard, InputType.Button, KeyCode.CapsLock),
        new ControlInput("Right Shift", ControllerType.Keyboard, InputType.Button, KeyCode.RightShift),
        new ControlInput("Left Shift", ControllerType.Keyboard, InputType.Button, KeyCode.LeftShift),
        new ControlInput("Right Control", ControllerType.Keyboard, InputType.Button, KeyCode.RightControl),
        new ControlInput("Left Control", ControllerType.Keyboard, InputType.Button, KeyCode.LeftControl),
        new ControlInput("Right Alt", ControllerType.Keyboard, InputType.Button, KeyCode.RightAlt),
        new ControlInput("Left Alt", ControllerType.Keyboard, InputType.Button, KeyCode.LeftAlt)

        #endregion

    };


    readonly PlayInput[] playInputs = new PlayInput[20];

    #endregion

    #endregion

    public InputManager()
    {
        SetController();
    }

    public void SetController()
    {
        string[] controllerTypes = Input.GetJoystickNames();

        if (controllerTypes.Any(a => a.Length == 33))
        {
            SetXBOXControllerAndButtons();
        }
        else if (controllerTypes.Any(a => a.Length == 19))
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

        playInputs[(int)InputKey.Attack] = new PlayInput(InputKey.Attack, PS4controlInputs.First(a => a.inputName == "X"));
        playInputs[(int)InputKey.Block] = new PlayInput(InputKey.Block, PS4controlInputs.First(a => a.inputName == "L2"));
        playInputs[(int)InputKey.Jump] = new PlayInput(InputKey.Jump, PS4controlInputs.First(a => a.inputName == "Triangle"));
        playInputs[(int)InputKey.Start] = new PlayInput(InputKey.Start, PS4controlInputs.First(a => a.inputName == "Options"));
        playInputs[(int)InputKey.MagicOne] = new PlayInput(InputKey.MagicOne, PS4controlInputs.First(a => a.inputName == "Square"));
        playInputs[(int)InputKey.MagicTwo] = new PlayInput(InputKey.MagicTwo, PS4controlInputs.First(a => a.inputName == "Circle"));
        playInputs[(int)InputKey.MagicThree] = new PlayInput(InputKey.MagicThree, PS4controlInputs.First(a => a.inputName == "R2"));
        playInputs[(int)InputKey.LeftThumstickX] = new PlayInput(InputKey.LeftThumstickX, PS4controlInputs.First(a => a.inputName == "Left Stick X"));
        playInputs[(int)InputKey.LeftThumstickY] = new PlayInput(InputKey.LeftThumstickY, PS4controlInputs.First(a => a.inputName == "Left Stick Y"));
        playInputs[(int)InputKey.RightThumstickX] = new PlayInput(InputKey.RightThumstickX, PS4controlInputs.First(a => a.inputName == "Right Stick X"));
        playInputs[(int)InputKey.RightThumstickY] = new PlayInput(InputKey.RightThumstickY, PS4controlInputs.First(a => a.inputName == "Right Stick X"));
    }

    void SetXBOXControllerAndButtons()
    {
        controllerType = ControllerType.XBOX;

        playInputs[(int)InputKey.Attack] = new PlayInput(InputKey.Attack, XBOXcontrolInputs.First(a => a.inputName == "A"));
        playInputs[(int)InputKey.Block] = new PlayInput(InputKey.Block, XBOXcontrolInputs.First(a => a.inputName == "Left Trigger"));
        playInputs[(int)InputKey.Jump] = new PlayInput(InputKey.Jump, XBOXcontrolInputs.First(a => a.inputName == "Y"));
        playInputs[(int)InputKey.Start] = new PlayInput(InputKey.Start, XBOXcontrolInputs.First(a => a.inputName == "Start"));
        playInputs[(int)InputKey.MagicOne] = new PlayInput(InputKey.MagicOne, XBOXcontrolInputs.First(a => a.inputName == "X"));
        playInputs[(int)InputKey.MagicTwo] = new PlayInput(InputKey.MagicTwo, XBOXcontrolInputs.First(a => a.inputName == "B"));
        playInputs[(int)InputKey.MagicThree] = new PlayInput(InputKey.MagicThree, XBOXcontrolInputs.First(a => a.inputName == "Right Trigger"));
        playInputs[(int)InputKey.LeftThumstickX] = new PlayInput(InputKey.LeftThumstickX, XBOXcontrolInputs.First(a => a.inputName == "Left Stick X"));
        playInputs[(int)InputKey.LeftThumstickY] = new PlayInput(InputKey.LeftThumstickY, XBOXcontrolInputs.First(a => a.inputName == "Left Stick Y"));
        playInputs[(int)InputKey.RightThumstickX] = new PlayInput(InputKey.RightThumstickX, XBOXcontrolInputs.First(a => a.inputName == "Right Stick X"));
        playInputs[(int)InputKey.RightThumstickY] = new PlayInput(InputKey.RightThumstickY, XBOXcontrolInputs.First(a => a.inputName == "Right Stick Y"));
    }

    void SetKeyboardAndButtons()
    {
        controllerType = ControllerType.Keyboard;

        playInputs[(int)InputKey.Attack] = new PlayInput(InputKey.Attack, KeyboardcontrolInputs.First(a => a.inputName == "Right Control"));
        playInputs[(int)InputKey.Block] = new PlayInput(InputKey.Block, KeyboardcontrolInputs.First(a => a.inputName == "Right Shift"));
        playInputs[(int)InputKey.Jump] = new PlayInput(InputKey.Jump, KeyboardcontrolInputs.First(a => a.inputName == "Space"));
        playInputs[(int)InputKey.MagicOne] = new PlayInput(InputKey.MagicOne, KeyboardcontrolInputs.First(a => a.inputName == "Keypad 1"));
        playInputs[(int)InputKey.Start] = new PlayInput(InputKey.Start, KeyboardcontrolInputs.First(a => a.inputName == "Return/Enter"));

        #region Custom Keyboard Input Control

        playInputs[(int)InputKey.KeyboardMoveUp] = new PlayInput(InputKey.KeyboardMoveUp, KeyboardcontrolInputs.First(a => a.inputName == "W"));
        playInputs[(int)InputKey.KeyboardMoveDown] = new PlayInput(InputKey.KeyboardMoveDown, KeyboardcontrolInputs.First(a => a.inputName == "S"));
        playInputs[(int)InputKey.KeyboardMoveLeft] = new PlayInput(InputKey.KeyboardMoveLeft, KeyboardcontrolInputs.First(a => a.inputName == "A"));
        playInputs[(int)InputKey.KeyboardMoveRight] = new PlayInput(InputKey.KeyboardMoveRight, KeyboardcontrolInputs.First(a => a.inputName == "D"));
        playInputs[(int)InputKey.KeyboardAvoidUp] = new PlayInput(InputKey.KeyboardAvoidUp, KeyboardcontrolInputs.First(a => a.inputName == "Up Arrow"));
        playInputs[(int)InputKey.KeyboardAvoidDown] = new PlayInput(InputKey.KeyboardAvoidDown, KeyboardcontrolInputs.First(a => a.inputName == "Down Arrow"));
        playInputs[(int)InputKey.KeyboardAvoidLeft] = new PlayInput(InputKey.KeyboardAvoidLeft, KeyboardcontrolInputs.First(a => a.inputName == "Left Arrow"));
        playInputs[(int)InputKey.KeyboardAvoidRight] = new PlayInput(InputKey.KeyboardAvoidRight, KeyboardcontrolInputs.First(a => a.inputName == "Right Arrow"));

        #endregion
    }

    public void SetUIButtons(UnityEngine.UI.RawImage[] rawImages, Texture[] textures)
    {
        if (controllerType == ControllerType.PS4)
        {
            rawImages.First(a => a.name == "AttackButton").texture = textures.First(a => a.name == "PS4 X Button");
            rawImages.First(a => a.name == "BlockButton").texture = textures.First(a => a.name == "PS4 L2 Button");
            rawImages.First(a => a.name == "JumpButton").texture = textures.First(a => a.name == "PS4 Triangle Button");
            rawImages.First(a => a.name == "LightningButton").texture = textures.First(a => a.name == "PS4 Square Button");
            rawImages.First(a => a.name == "IceBlastButton").texture = textures.First(a => a.name == "PS4 Circle Button");
            rawImages.First(a => a.name == "FireBoltButton").texture = textures.First(a => a.name == "PS4 R2 Button");

            rawImages.First(a => a.name == "RestartHighlight").texture = textures.First(a => a.name == "PS4 X Button");
            rawImages.First(a => a.name == "ExitHighlight").texture = textures.First(a => a.name == "PS4 X Button");
        }
        else if (controllerType == ControllerType.XBOX)
        {
            rawImages.First(a => a.name == "AttackButton").texture = textures.First(a => a.name == "XBOX A Button");
            rawImages.First(a => a.name == "BlockButton").texture = textures.First(a => a.name == "XBOX Left Trigger Button");
            rawImages.First(a => a.name == "JumpButton").texture = textures.First(a => a.name == "XBOX Y Button");
            rawImages.First(a => a.name == "LightningButton").texture = textures.First(a => a.name == "XBOX X Button");
            rawImages.First(a => a.name == "IceBlastButton").texture = textures.First(a => a.name == "XBOX B Button");
            rawImages.First(a => a.name == "FireBoltButton").texture = textures.First(a => a.name == "XBOX Right Trigger Button");

            rawImages.First(a => a.name == "RestartHighlight").texture = textures.First(a => a.name == "XBOX A Button");
            rawImages.First(a => a.name == "ExitHighlight").texture = textures.First(a => a.name == "XBOX A Button");
        }
        else
        {
            rawImages.First(a => a.name == "AttackButton").texture = textures.First(a => a.name == "Keyboard Right Control Button");
            rawImages.First(a => a.name == "BlockButton").texture = textures.First(a => a.name == "Keyboard Right Shift Button");
            rawImages.First(a => a.name == "JumpButton").texture = textures.First(a => a.name == "Keyboard Space Button");
            rawImages.First(a => a.name == "LightningButton").texture = textures.First(a => a.name == "Keyboard Keypad 1 Button");

            rawImages.First(a => a.name == "RestartHighlight").texture = textures.First(a => a.name == "Keyboard Right Control Button");
            rawImages.First(a => a.name == "ExitHighlight").texture = textures.First(a => a.name == "Keyboard Right Control Button");
        }
    }

    public bool GetButtonDown(InputKey button)
    {
        if (playInputs[(int)button].controlInput.inputType == InputType.Button)
        {
            return Input.GetKeyDown(playInputs[(int)button].controlInput.inputKeyCode);
        }
        else if (playInputs[(int)button].controlInput.inputType != InputType.Button && Mathf.Abs(Input.GetAxis(playInputs[(int)button].controlInput.inputType.ToString())) >= 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetButton(InputKey button)
    {
        if (playInputs[(int)button].controlInput.inputType == InputType.Button)
        {
            return Input.GetKey(playInputs[(int)button].controlInput.inputKeyCode);
        }
        else if (playInputs[(int)button].controlInput.inputType != InputType.Button && Mathf.Abs(Input.GetAxis(playInputs[(int)button].controlInput.inputType.ToString())) >= 0.5f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetAxis(InputKey axis)
    {
        if (playInputs[(int)axis].controlInput.inputType != InputType.Button)
        {
            return Input.GetAxis(playInputs[(int)axis].controlInput.inputType.ToString());
        }
        else if (playInputs[(int)axis].controlInput.inputType == InputType.Button && Input.GetKey(playInputs[(int)axis].controlInput.inputKeyCode))
        {
            return 0.9f;
        }
        else
        {
            return 0;
        }
    }

    public float GetKeyboardAxis(InputKey axisPlus, InputKey axisMinus)
    {
        if (Input.GetKey(playInputs[(int)axisPlus].controlInput.inputKeyCode))
        {
            return 0.9f;
        }
        else if (Input.GetKey(playInputs[(int)axisMinus].controlInput.inputKeyCode))
        {
            return -0.9f;
        }
        else
        {
            return 0;
        }
    }
}