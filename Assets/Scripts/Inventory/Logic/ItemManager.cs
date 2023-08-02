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

    private void OnEnable()
    {
        EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        EventHandler.DropItemEvent += OnDropItemEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    

    private void OnDisable()
    {
        EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        EventHandler.DropItemEvent -= OnDropItemEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnBeforeSceneUnloadEvent()
    {
        GetAllSceneItems();
    }

    private void OnAfterSceneLoadedEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
        RecreateAllItems();
    }
    

    private void OnInstantiateItemInScene(int itemID, Vector3 pos)
    {
        var item = Instantiate(itemPrefab, pos, Quaternion.identity,itemParent);
        item.itemID = itemID;

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
}
