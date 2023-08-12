using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WzFarm.Inventory;

public class ItemManager : MonoBehaviour
{
    public Item itemPrefab;
    public Item bouncePrefab;
    private Transform itemParent;
    private Transform playerTrans =>FindObjectOfType<Player>().transform;

    public Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();
    //记录场景家具
    private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();


    private void OnEnable()
    {
        EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        EventHandler.DropItemEvent += OnDropItemEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
        //建造
        EventHandler.BuildFurnitureEvent += OnBuildFurnitureEvent;
    }

    

    private void OnDisable()
    {
        EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        EventHandler.DropItemEvent -= OnDropItemEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
        EventHandler.BuildFurnitureEvent -= OnBuildFurnitureEvent;
    }

    private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
    {
        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
        var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);
        if (buildItem.GetComponent<Box>())
        {
            buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
            buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        GetAllSceneItems();
        GetAllSceneFurniture();
    }

    private void OnAfterSceneLoadedEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
        RecreateAllItems();
        RebuildFurniture();
    }
    

    private void OnInstantiateItemInScene(int itemID, Vector3 pos)
    {
        var item = Instantiate(bouncePrefab, pos, Quaternion.identity,itemParent);
        item.itemID = itemID;
        item.GetComponent<ItemBounce>().InitBounceItem(pos,Vector3.up);
        

    }
    
    private void OnDropItemEvent(int itemID, Vector3 pos,ItemType itemType)
    {
        if (itemType == ItemType.Seed)
        {
            return;
        }
        var item = Instantiate(bouncePrefab, playerTrans.position, Quaternion.identity,itemParent);
        item.itemID = itemID;
        var dir = (pos - playerTrans.position).normalized;
        item.GetComponent<ItemBounce>().InitBounceItem(pos,dir);
    }

    /// <summary>
    /// 获得当前场景所有Item
    /// </summary>
    private void GetAllSceneItems()
    {
        List<SceneItem> currentSceneItems = new List<SceneItem>();

        foreach (var item in FindObjectsOfType<Item>())
        {
            SceneItem sceneItem = new SceneItem
            {
                itemID = item.itemID,
                position = new SerializableVector3(item.transform.position)
            };
            currentSceneItems.Add(sceneItem);
        }

        if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
        {
            sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
        }
        else
        {
            sceneItemDict.Add(SceneManager.GetActiveScene().name,currentSceneItems);
        }
    }

    /// <summary>
    /// 刷新重建当前场景物品
    /// </summary>
    private void RecreateAllItems()
    {
        List<SceneItem> currentSceneItems = new List<SceneItem>();

        if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
        {
            if (currentSceneItems != null)
            {
                //清场
                foreach (var item in FindObjectsOfType<Item>())
                {
                    Destroy(item.gameObject);
                }

                foreach (var item in currentSceneItems)
                {
                    Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                    newItem.Init(item.itemID);
                }
            }
        }
    }
    
    /// <summary>
    /// 获得场景所有家具
    /// </summary>
    private void GetAllSceneFurniture()
    {
        List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

        foreach (var item in FindObjectsOfType<Furniture>())
        {
            SceneFurniture sceneFurniture = new SceneFurniture
            {
                itemID = item.itemID,
                position = new SerializableVector3(item.transform.position)
            };
            if (item.GetComponent<Box>())
                sceneFurniture.boxIndex = item.GetComponent<Box>().index;

            currentSceneFurniture.Add(sceneFurniture);
        }

        if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
        {
            //找到数据就更新item数据列表
            sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
        }
        else    //如果是新场景
        {
            sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
        }
    }
    
    /// <summary>
    /// 重建当前场景家具
    /// </summary>
    private void RebuildFurniture()
    {
        List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

        if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
        {
            if (currentSceneFurniture != null)
            {
                foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                {
                    BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(sceneFurniture.itemID);
                    var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);
                    if (buildItem.GetComponent<Box>())
                    {
                        buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                    }
                }
            }
        }
    }
}
