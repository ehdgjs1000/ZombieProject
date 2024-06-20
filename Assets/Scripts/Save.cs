using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using Defective.JSON;

[System.Serializable]
public class SaveItem
{
    public SaveItem(string _itemName, int _itemCount)
    {
        itemName = _itemName;
        itemCount = _itemCount;
    }

    public string itemName;
    public int itemCount;
}
[System.Serializable]
public class SavePicked
{
    public SavePicked(string _pickedItemID)
    {
        pickedItemID = _pickedItemID;
    }

    public string pickedItemID;
}
[System.Serializable]
public class SaveZombie
{
    public SaveZombie(string _zombieID)
    {
        zombieID = _zombieID;
    }

    public string zombieID;
}
[System.Serializable]
public class SaveGeneric
{
    public SaveGeneric(string _genericItemID)
    {
        genericItemID = _genericItemID;
    }

    public string genericItemID;
}
[System.Serializable]
public class SaveBase
{
    public SaveBase(int _baseNum, float _basePosX, float _basePosY, float _basePosZ)
    {
        baseNum = _baseNum;
        basePosX = _basePosX;
        basePosY = _basePosY;
        basePosZ = _basePosZ;
    }
    public int baseNum;
    public float basePosX;
    public float basePosY;
    public float basePosZ;
}

public class Save : MonoBehaviour
{
    //# Item
    [SerializeField] public List<SaveItem> saveItemList;
    [SerializeField] public List<SaveItem> loadItemList;
    [SerializeField] private InventoryCtrl inventoryCtrl;

    //# BaseCamp
    [SerializeField] public List<SaveBase> saveBaseList;
    [SerializeField] public List<SaveBase> loadBaseList;
    [SerializeField] GameObject baseCampPrefab;

    //# pickedData
    [SerializeField] public List<SavePicked> savePickedList;
    [SerializeField] public List<SavePicked> loadPickedList;

    //# genericData
    [SerializeField] public List<SaveGeneric> saveGenericList;
    [SerializeField] public List<SaveGeneric> loadGenericList;

    //# zombieData
    [SerializeField] public List<SaveZombie> saveZombieList;
    [SerializeField] public List<SaveZombie> loadZombieList;
    public void SaveGame()
    {
        //Save Items
        string itemFilePath = Application.persistentDataPath + "/InventoryData.json";
        //string filePath = Application.dataPath + "/Resources/InventoryData.json";
        string inventoryData = null;
        Debug.Log(itemFilePath);
        File.WriteAllText(itemFilePath, null);

        for (int i = 0; i < InventoryCtrl.instance.slots.Length; i++)
        {
            //if inventory is not null
            if (InventoryCtrl.instance.slots[i].item != null)
            {
                saveItemList.Add(new SaveItem(InventoryCtrl.instance.slots[i].item.itemName,
                    InventoryCtrl.instance.slots[i].itemCount));
            }
        }
        inventoryData = JsonConvert.SerializeObject(saveItemList, Formatting.Indented);
        File.WriteAllText(itemFilePath, inventoryData);

        //Save BaseCamp
        string baseCameFilePath = Application.persistentDataPath + "/BaseCampData.json";
        string baseData = null;
        File.WriteAllText(baseCameFilePath, null);
        for(int a = 0; a < CharacterManager.baseCount; a++)
        {
            if(CharacterManager.basePos[a] != null)
            {
                saveBaseList.Add(new SaveBase(a,CharacterManager.basePos[a].x, CharacterManager.basePos[a].y,
                    CharacterManager.basePos[a].z));
            }
        }
        baseData = JsonConvert.SerializeObject(saveBaseList, Formatting.Indented);
        File.WriteAllText(baseCameFilePath, baseData);

        SavePickedData();
        SaveGenericData();
        SaveZombieData();
    }
    public void SavePickedData()
    {
        //Save Items
        string pickedFilePath = Application.persistentDataPath + "/PickedData.json";
        string pickedData = null;
        File.WriteAllText(pickedFilePath, null);

        for (int i = 0; i < GameIDManager.instance.isIngridentPicked.Count; i++)
        {
            //if inventory is not null
            if (GameIDManager.instance.isIngridentPicked[i]!= null)
            {
                savePickedList.Add(new SavePicked(GameIDManager.instance.isIngridentPicked[i]));
            }
        }
        pickedData = JsonConvert.SerializeObject(savePickedList, Formatting.Indented);
        File.WriteAllText(pickedFilePath, pickedData);
    }
    public void SaveGenericData()
    {
        //Save Items
        string genericFilePath = Application.persistentDataPath + "/GenericData.json";
        string genericData = null;
        Debug.Log(genericFilePath);
        File.WriteAllText(genericFilePath, null);

        for (int i = 0; i < GameIDManager.instance.isGenericPharmed.Count; i++)
        {
            //if inventory is not null
            if (GameIDManager.instance.isGenericPharmed[i] != null)
            {
                saveGenericList.Add(new SaveGeneric(GameIDManager.instance.isGenericPharmed[i]));
            }
        }
        genericData = JsonConvert.SerializeObject(saveGenericList, Formatting.Indented);
        File.WriteAllText(genericFilePath, genericData);
    }
    public void SaveZombieData()
    {
        //Save Items
        string zombieFilePath = Application.persistentDataPath + "/ZombieData.json";
        string zombieData = null;
        Debug.Log(zombieFilePath);
        File.WriteAllText(zombieFilePath, null);

        for (int i = 0; i < GameIDManager.instance.isZombieDie.Count; i++)
        {
            //if inventory is not null
            if (GameIDManager.instance.isZombieDie[i] != null)
            {
                saveZombieList.Add(new SaveZombie(GameIDManager.instance.isZombieDie[i]));
            }
        }
        zombieData = JsonConvert.SerializeObject(saveZombieList, Formatting.Indented);
        File.WriteAllText(zombieFilePath, zombieData);
    }
    public void LoadGame()
    {
        // Load Items
        string itemFilePath = Application.persistentDataPath + "/InventoryData.json";
        string itemJsonString = File.ReadAllText(itemFilePath);

        loadItemList = JsonConvert.DeserializeObject<List<SaveItem>>(itemJsonString);
        for (int itemNum = 0; itemNum < loadItemList.Count; itemNum++)
        {
            if (loadItemList[itemNum] != null)
            {
                //인벤토리에 아이템 넣기
                Item loadItem = Resources.Load<Item>("Items/"+loadItemList[itemNum].itemName);

                //Debug.Log(loadItem.itemType);
                inventoryCtrl.AcquireItem(loadItem, loadItemList[itemNum].itemCount);

            }
        }
        // Load BaseCamp
        string baseCameFilePath = Application.persistentDataPath + "/BaseCampData.json";
        string baseJsonString = File.ReadAllText(baseCameFilePath);

        loadBaseList = JsonConvert.DeserializeObject<List<SaveBase>>(baseJsonString);
        for (int baseNum = 0; baseNum < loadBaseList.Count; baseNum++)
        {
            if (loadBaseList[baseNum] != null)
            {
                Vector3 basePos = new Vector3(loadBaseList[baseNum].basePosX, loadBaseList[baseNum].basePosY,
                    loadBaseList[baseNum].basePosZ);
                CharacterManager.basePos[baseNum] = basePos;
                Instantiate(baseCampPrefab, basePos, Quaternion.identity);
            }
        }
        LoadPickedData();
        LoadGenericData();
        LoadZombieData();
    }
    public void LoadPickedData()
    {
        string pickedFilePath = Application.persistentDataPath + "/PickedData.json";
        string itemJsonString = File.ReadAllText(pickedFilePath);

        loadPickedList = JsonConvert.DeserializeObject<List<SavePicked>>(itemJsonString);
        for (int itemNum = 0; itemNum < loadPickedList.Count; itemNum++)
        {
            if (loadPickedList[itemNum] != null)
            {
                GameIDManager.instance.isIngridentPicked.Add(loadPickedList[itemNum].pickedItemID);
            }
        }
    }
    public void LoadGenericData()
    {
        string genericFilePath = Application.persistentDataPath + "/GenericData.json";
        string itemJsonString = File.ReadAllText(genericFilePath);

        loadGenericList = JsonConvert.DeserializeObject<List<SaveGeneric>>(itemJsonString);
        for (int itemNum = 0; itemNum < loadGenericList.Count; itemNum++)
        {
            if (loadGenericList[itemNum] != null)
            {
                GameIDManager.instance.isGenericPharmed.Add(loadGenericList[itemNum].genericItemID);
            }
        }
    }
    public void LoadZombieData()
    {
        string zombieFilePath = Application.persistentDataPath + "/ZombieData.json";
        string itemJsonString = File.ReadAllText(zombieFilePath);

        loadZombieList = JsonConvert.DeserializeObject<List<SaveZombie>>(itemJsonString);
        for (int itemNum = 0; itemNum < loadZombieList.Count; itemNum++)
        {
            if (loadZombieList[itemNum] != null)
            {
                GameIDManager.instance.isZombieDie.Add(loadZombieList[itemNum].zombieID);
            }
        }
    }
    public void SaveBtnOnClick()
    {
        SaveGame();
    }
    public void LoadBtnOnClick()
    {
        Debug.Log("LoadData");
        LoadGame();
    }
}