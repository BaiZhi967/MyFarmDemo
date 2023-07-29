using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WzFarm.Inventory;

public class ItemManager : MonoBehaviour
{
    public Item itemPrefab;
    private Transform itemParent;

    private void OnEnable()
    {
        EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        itemParent = GameObject.FindWithTag("ItemParent").transform;
    }
    

    private void OnInstantiateItemInScene(int itemID, Vector3 pos)
    {
        var item = Instantiate(itemPrefab, pos, Quaternion.identity,itemParent);
        item.itemID = itemID;

    }
}
