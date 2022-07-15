using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private PlayerInput playerInput;

    public Vector3 moveDirection;
    public Vector3 rotateDirection;
    public bool Throw;

    private void Awake()
    {
        playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        moveDirection = playerInput.PlayerMain.Move.ReadValue<Vector2>();
        rotateDirection = playerInput.PlayerMain.Look.ReadValue<Vector2>();

        if (playerInput.PlayerMain.Throw.triggered)
        {
            Throw = true;
        }
    }
}
