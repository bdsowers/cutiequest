using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class BasicActionSet : PlayerActionSet
{
    private InputDevice mBoundDevice;
    private bool mBound;

    public InputDevice boundDevice {  get { return mBoundDevice; } }

    public PlayerAction MoveLeft;
    public PlayerAction MoveRight;
    public PlayerAction MoveUp;
    public PlayerAction MoveDown;
    public PlayerTwoAxisAction Move;

    public PlayerAction HoldPosition;

    public PlayerAction Activate;
    public PlayerAction Spell;

    public PlayerAction Pause;
    public PlayerAction ToggleMap;

    public PlayerAction Like;
    public PlayerAction Dislike;

    public PlayerAction CloseMenu;

    public BasicActionSet()
    {
        MoveLeft = CreatePlayerAction("Move Left");
        MoveRight = CreatePlayerAction("Move Right");
        MoveUp = CreatePlayerAction("Move Up");
        MoveDown = CreatePlayerAction("Move Down");
        Move = CreateTwoAxisPlayerAction(MoveLeft, MoveRight, MoveDown, MoveUp);

        HoldPosition = CreatePlayerAction("Hold Position");
        Activate = CreatePlayerAction("Activate");
        Spell = CreatePlayerAction("Cast Spell");

        Pause = CreatePlayerAction("Pause");

        ToggleMap = CreatePlayerAction("Activate Map");

        Like = CreatePlayerAction("Like");
        Dislike = CreatePlayerAction("Dislike");

        CloseMenu = CreatePlayerAction("Close Menu");
    }

    public void DetectController()
    {
        InputDevice device = (InputManager.ActiveDevice.Name == "None" ? null : InputManager.ActiveDevice);

        if (device != mBoundDevice || !mBound)
        {
            mBound = true;
            BindToDevice(device);
        }
    }

    public void BindToDevice(InputDevice device)
    {
        mBoundDevice = device;

        if (device == null)
        {
            MoveLeft.AddDefaultBinding(Key.LeftArrow);
            MoveRight.AddDefaultBinding(Key.RightArrow);
            MoveUp.AddDefaultBinding(Key.UpArrow);
            MoveDown.AddDefaultBinding(Key.DownArrow);

            HoldPosition.AddDefaultBinding(Key.Shift);
            Activate.AddDefaultBinding(Key.E);
            Spell.AddDefaultBinding(Key.Space);

            Pause.AddDefaultBinding(Key.Escape);

            ToggleMap.AddDefaultBinding(Key.M);

            Like.AddDefaultBinding(Key.RightArrow);
            Dislike.AddDefaultBinding(Key.LeftArrow);

            CloseMenu.AddDefaultBinding(Key.Escape);

            this.Device = null;
        }
        else
        {
            MoveLeft.AddDefaultBinding(InputControlType.RightStickLeft);
            MoveRight.AddDefaultBinding(InputControlType.RightStickRight);
            MoveUp.AddDefaultBinding(InputControlType.RightStickUp);
            MoveDown.AddDefaultBinding(InputControlType.RightStickDown);

            MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
            MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
            MoveUp.AddDefaultBinding(InputControlType.LeftStickUp);
            MoveDown.AddDefaultBinding(InputControlType.LeftStickDown);

            MoveLeft.AddDefaultBinding(InputControlType.DPadLeft);
            MoveRight.AddDefaultBinding(InputControlType.DPadRight);
            MoveUp.AddDefaultBinding(InputControlType.DPadUp);
            MoveDown.AddDefaultBinding(InputControlType.DPadDown);

            HoldPosition.AddDefaultBinding(InputControlType.RightTrigger);
            Activate.AddDefaultBinding(InputControlType.Action1);
            Spell.AddDefaultBinding(InputControlType.Action3);

            Pause.AddDefaultBinding(InputControlType.Command);

            ToggleMap.AddDefaultBinding(InputControlType.Action4);

            Like.AddDefaultBinding(InputControlType.RightTrigger);
            Like.AddDefaultBinding(InputControlType.RightBumper);

            Dislike.AddDefaultBinding(InputControlType.LeftTrigger);
            Dislike.AddDefaultBinding(InputControlType.LeftBumper);

            CloseMenu.AddDefaultBinding(InputControlType.Action2);

            this.Device = boundDevice;
        }
    }
}
