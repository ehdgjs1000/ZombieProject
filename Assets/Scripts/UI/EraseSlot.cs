using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EraseSlot : MonoBehaviour, IDropHandler
{
    static public EraseSlot instance;

    public Inventory eraseSlot;

    private void Start()
    {
        instance = this;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragSlot.instance.dragSlot != null)
        {
            DragSlot.instance.dragSlot.ClearSlot();
        }
    }
}
