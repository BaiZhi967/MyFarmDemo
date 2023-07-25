using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    public float inputX;
    public float inputY;
    public float speed;
    public Vector2 movementInput;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void PlayerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        if (inputX != 0 && inputY != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }
        movementInput = new Vector2(inputX, inputY);
    }

    private void Movement()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + movementInput * speed * Time.deltaTime);
    }
}
