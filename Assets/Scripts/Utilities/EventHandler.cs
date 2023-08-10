using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WzFarm.Dialogue;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
   
    
    

    /// <summary>
    /// 呼叫UI事件中心
    /// </summary>
    /// <param name="location">物品位置</param>
    /// <param name="list">物品列表</param>
    public static void CallUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location,list);
    }
    public static event Action<int, Vector3> InstantiateItemInScene;
    public static void CallInstantiateItemInScene(int itemID, Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(itemID,pos);
    }
    
    public static event Action<int, Vector3,ItemType> DropItemEvent;
    public static void CallDropItemEvent(int itemID, Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(itemID,pos,itemType);
    }

    public static event Action<ItemDetails, bool> ItemSelectedEvent;

    public static void CallItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails,isSelected);
    }

    public static event Action<int,int,int,Season> GameMinuteEvent;

    public static void CallGameMinuteEvent(int minute, int hour,int day,Season season)
    {
        GameMinuteEvent?.Invoke(minute,hour,day,season);
    }

    public static event Action<int, Season> GameDayEvent;

    public static void CallGameDayEvent(int day, Season season)
    {
        GameDayEvent?.Invoke(day,season);
    }
    public static event Action<int,int,int,int,Season> GameDateEvent;

    public static void CallGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        GameDateEvent?.Invoke(hour,day,month,year,season);
    }

    public static event Action<string, Vector3> TransitionEvent;

    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName,pos);
    }
    
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }

    public static event Action AfterSceneLoadedEvent;
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    public static event Action<Vector3, ItemDetails> MouseClickedEvent;

    public static void CallMouseClickedEvent(Vector3 pos, ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos,itemDetails);
    }
    
    
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimation;

    public static void CallExecuteActionAfterAnimation(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimation?.Invoke(pos,itemDetails);
    }

    public static event Action<int, TileDetails> PlantSeedEvent;

    public static void CallPlantSeedEvent(int ID, TileDetails tile)
    {
        PlantSeedEvent?.Invoke(ID,tile);
    }

    public static event Action<int> HarvestAtPlayerPosition;

    public static void CallHarvestAtPlayerPosition(int itemID)
    {
        HarvestAtPlayerPosition?.Invoke(itemID);
    }

    public static event Action RefreshCurrentMap;

    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }
    
    public static event Action<ParticleEffectType,Vector3> ParticleEffectEvent;

    public static void CallParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType,pos);
    }

    public static event Action GenerateCropEvent;
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;

    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;

    public static void CallBaseBagOpenEvent(SlotType slotType, InventoryBag_SO inventoryBagSo)
    {
        BaseBagOpenEvent?.Invoke(slotType,inventoryBagSo);
    }
    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;

    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO inventoryBagSo)
    {
        BaseBagCloseEvent?.Invoke(slotType,inventoryBagSo);
    }
    
    public static event Action<GameState> UpdateGameStateEvent;
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }
    
    public static event Action<ItemDetails, bool> ShowTradeUI;
    public static void CallShowTradeUI(ItemDetails item, bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
}
