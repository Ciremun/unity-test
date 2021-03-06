﻿using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public GameObject dialog;
    public Image icon;
    public InventoryItem item;

    public void AddItem(InventoryItem newItem)
    {
        item = newItem;
        icon.sprite = item.icon;
        icon.enabled = true;
    }

    public void ClearSlot()
    {
        item = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public void ShowDialog()
    {
        if (item == null) return;
        InventorySlotDialog dialogObject = dialog.GetComponent<InventorySlotDialog>(); 
        dialogObject.UpdateDialog(this);
        dialog.SetActive(true);
    }
}
