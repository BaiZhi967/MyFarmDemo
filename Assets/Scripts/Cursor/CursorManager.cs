using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WzFarm.CropPlant;
using WzFarm.Map;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed,item;
    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;
    //鼠标检测
    private Camera mainCamera;
    private Grid currentGrid;
    private Vector3 mouseWorldPos;
    private Vector3Int mouseGirdPos;
    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
        //cursorEnable = true;
    }


    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("Cursor").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(normal);
        
        mainCamera = Camera.main;
    }


    private void Update()
    {
        if (cursorCanvas == null)
        {
            return;
        }

        cursorImage.transform.position = Input.mousePosition;
        if (!InteractWiteUI()&&cursorEnable )
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }
    }


    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos,currentItem);
        }
    }
    
    
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite"></param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }
    
    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);
        //Debug.Log("Now Red");
    }
    
    
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {

        
        
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {
            
            currentItem = itemDetails;
            
            //WORKFLOW:添加所有对应的图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                ItemType.CollectTool =>tool,
                _ => normal,
            };
            cursorEnable = true;
        }
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGirdPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        
        //判断在使用范围内

        if (Mathf.Abs(mouseGirdPos.x - playerGridPos.x) >currentItem.itemUseRadius || Mathf.Abs(mouseGirdPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGirdPos);
    
        //Debug.Log(mouseGirdPos);
        //Debug.Log(currentTile);
        if (currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            //WORKFLOW:补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                    
                case ItemType.Seed:
                    if (currentTile.daysSinceDug > -1 && currentTile.seedItemID == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDug > -1 && currentTile.daysSinceWatered == -1)
                    {
                        SetCursorValid();
                    }
                    else
                    {
                        SetCursorInValid();
                    }
                    break;
                case ItemType.ChopTool:
                case ItemType.CollectTool:
                    if (currentCrop != null)
                    {
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                        {
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays)
                            {
                                SetCursorValid();
                            }
                            else
                            {
                                SetCursorInValid();
                            }
                        }
                        
                        
                    }else
                    {
                        SetCursorInValid();
                    }
                    break;
                
            }
        }
        else
        {
            //Debug.Log("Red Now null");
            SetCursorInValid();
        }
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns></returns>
    private bool InteractWiteUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }

        return false;
    }
    
    
}
