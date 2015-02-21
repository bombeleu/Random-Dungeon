using UnityEngine;
using System.Collections;
using System;

public class InputController : MonoBehaviour
{
    public event Action<IntVector2> OnKeydown;

    

    void Update()
    {
        Keyboard();
        
    }

    public void Keyboard()
    {
       if (KeyboardUp())
        {
            OnKeydown(Direction.NORTH);
        }
        // EAST
        else if (KeyboardRight())
        {
            OnKeydown(Direction.EAST);
        }
        // WEST
        else if (KeyboardLeft())
        {
            OnKeydown(Direction.WEST);
        }
        // SOUTH
        else if (KeyboardDown())
        {
            OnKeydown(Direction.SOUTH);
        }
        // MIDDLE
        else {
            // idle
        }
       
    }

    #region player inputs
    public bool KeyboardUp()
    {
        return Input.GetKey("up") || Input.GetKey(KeyCode.W);
    }
    public bool KeyboardLeft()
    {
        return Input.GetKey("left") || Input.GetKey(KeyCode.A);
    }
    public bool KeyboardDown()
    {
        return Input.GetKey("down") || Input.GetKey(KeyCode.S);
    }
    public bool KeyboardRight()
    {
        return Input.GetKey("right") || Input.GetKey(KeyCode.D);
    }
    #endregion

    
}