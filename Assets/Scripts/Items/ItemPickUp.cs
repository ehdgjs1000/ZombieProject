using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public Item item;
    public string itemID;

    private void Awake()
    {
        float ranY = Random.Range(0.0f, 180.0f);
        transform.rotation = Quaternion.Euler(0, ranY, 0);
    }

    private void Start()
    {
        CreateUniqueItemID();
        
    }
    private void Update()
    {
        CheckPicked();
    }

    void CheckPicked()
    {
        for (int a = 0;a< GameIDManager.instance.isIngridentPicked.Count; a++)
        {
            if(this.itemID == GameIDManager.instance.isIngridentPicked[a])
            {
                Destroy(this.gameObject);
            }
        }
    }
    /*private void Start()
    {
        for (int a = 0; a<GameIDManager.instance.isIngridentPicked.Count; a++)
        {
            if (GameIDManager.instance.isIngridentPicked[a] != null && 
                GameIDManager.instance.isIngridentPicked[a] == itemID)
            {
                Destroy(this.gameObject);
            }
        }
    }*/

    public void CreateUniqueItemID()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(itemID))
        {
            InternalCreateUniqueID();
        }
#endif
    }

    [Button("Force Create UniqueID")]
    private void InternalCreateUniqueID()
    {
        string guid = System.Guid.NewGuid().ToString();
        int chance = 100;
        while (ItemManager.CreatedItems.Exists(x => x.Equals(guid)) && chance > 0)
        {
            guid = System.Guid.NewGuid().ToString();
            chance--;
        }

        itemID = guid;
        ItemManager.CreatedItems.Add(itemID);
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
    }
}