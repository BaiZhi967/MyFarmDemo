using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WzFarm.Inventory
{
    public class TradeUI : MonoBehaviour
    {
        public Image itemIcon;
        public TextMeshProUGUI itemName;
        public InputField tradeAmount;
        public Button submitButton;
        public Button cancelButton;

        private ItemDetails item;
        private bool isSellTrade;

        private void Awake()
        {
            cancelButton.onClick.AddListener(CancelTrade);
            submitButton.onClick.AddListener(TradeItem);
        }

        /// <summary>
        /// 设置TradeUI显示详情
        /// </summary>
        /// <param name="item"></param>
        /// <param name="isSell"></param>
        public void SetupTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        private void TradeItem()
        {

            string amount = tradeAmount.text;
            int cnt = 0;
            Int32.TryParse(amount,out cnt);
            //Debug.Log(amount+":"+cnt);
            InventoryManager.Instance.TradeItem(item, cnt, isSellTrade);

            CancelTrade();
        }


        private void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }

}