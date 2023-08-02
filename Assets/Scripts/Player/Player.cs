using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private float inputX;
    private float inputY;
    public float speed;
    private Vector2 movementInput;
    private Animator[] _animators;
    private bool isMoving = false;
    private bool inputDisable;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _animators = GetComponentsInChildren<Animator>();
    }

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        //todo:执行动画
        EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails);
    }
    
    private void OnMoveToPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void OnAfterSceneLoadedEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;
    }

    private void Update()
    {
        if (!inputDisable)
        {
            PlayerInput();
        }
        else
        {
            isMoving = false;
        }
        SwitchAnimation();
    }

    private void FixedUpdate()
    {
        if (!inputDisable)
        {
            Movement();
        }
        
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

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }
        movementInput = new Vector2(inputX, inputY);
        isMoving = movementInput != Vector2.zero;
    }

    private void Movement()
    {
        _rigidbody2D.MovePosition(_rigidbody2D.position + movementInput * speed * Time.deltaTime);
    }

    private void SwitchAnimation()
    {
        foreach (var animator in _animators)
        {
            animator.SetBool("isMoving",isMoving);
            if (isMoving)
            {
                animator.SetFloat("InputX",inputX);
                animator.SetFloat("InputY",inputY);
            }
        }
    }
}
