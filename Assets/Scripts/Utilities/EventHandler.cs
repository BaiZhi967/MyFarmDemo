using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static event Action<int, Vector3> InstantiateItemInScene;

    /// <summary>
    /// 呼叫UI事件中心
    /// </summary>
    /// <param name="location">物品位置</param>
    /// <param name="list">物品列表</param>
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location,list);
    }

    public static void CallInstantiateItemInScene(int itemID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(itemID,pos);
    }
}
