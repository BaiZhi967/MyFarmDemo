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

    //使用工具
    private float mouseX;
    private float mouseY;
    private bool useTool;
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
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        EventHandler.MoveToPosition += OnMoveToPosition;
        EventHandler.MouseClickedEvent -= OnMouseClickedEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.Gameplay:
                inputDisable = false;
                break;
            case GameState.Pause:
                inputDisable = true;
                break;
        }
    }

    private void OnMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        if (useTool)
        {
            return;
        }
        
        //todo:执行动画

        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Commodity &&
            itemDetails.itemType != ItemType.Furniture)
        {
            mouseX = pos.x - transform.position.x;
            mouseY = pos.y - (0.85f  + transform.position.y);
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }

            StartCoroutine(UseToolRoutine(pos, itemDetails));
        }
        else
        {
            EventHandler.CallExecuteActionAfterAnimation(pos, itemDetails);
        }

    }

    private IEnumerator UseToolRoutine(Vector3 mousePosition, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true;
        yield return null;
        foreach (var anim in _animators)
        {
            anim.SetTrigger("useTool");
            anim.SetFloat("InputX",mouseX);
            anim.SetFloat("InputY",mouseY);
        }

        yield return new WaitForSeconds(0.45f);
        EventHandler.CallExecuteActionAfterAnimation(mousePosition,itemDetails);
        yield return new WaitForSeconds(0.25f);
        useTool = false;
        inputDisable = false;
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
            animator.SetFloat("mouseX",mouseX);
            animator.SetFloat("mouseY",mouseY);
            if (isMoving)
            {
                animator.SetFloat("InputX",inputX);
                animator.SetFloat("InputY",inputY);
            }
        }
    }
}
