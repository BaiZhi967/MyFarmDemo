using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverride : MonoBehaviour
{
    private Animator[] _animators;
    public SpriteRenderer holdItem;

    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();
    private void Awake()
    {
        _animators = GetComponentsInChildren<Animator>();
        foreach (var animator in _animators)
        {
            animatorNameDict.Add(animator.name,animator);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        //WORKFLOW:不同工具返回不同动画 补全
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed => PartType.Carry,
            ItemType.Commodity => PartType.Carry,
            _ => PartType.None
        };
        holdItem.enabled = false;
        if (isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if (currentType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite == null ? itemDetails.itemIcon : itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
        }
        SwitchAnimator(currentType);
    }

    private void SwitchAnimator(PartType partType)
    {
        foreach (var item in animatorTypes)
        {
            if (item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
