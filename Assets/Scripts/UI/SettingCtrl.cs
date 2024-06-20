using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab;
    public GameObject go_PreviewPrefab;
    public Item craftingItem;
    public bool isBuilable;

    public Item[] needItem;
    public int[] needItemConut;
}

public class SettingCtrl : MonoBehaviour
{
    public static bool isSettingPanelActive = false;

    public GameObject settingPanel;
    public GameObject[] panels;
    public static SettingCtrl instance;

    float rotSpeed = 20.0f;

    [SerializeField] GameObject[] slotSets;
    [SerializeField] Text canNotBuildTxt;
    [SerializeField] SlotToolTip theSlotToolTip;

    private void Awake()
    {
        StartCoroutine(StartSettingPanel());
        instance = this;
    }

    //시작시 panel 껏다켜기 ->GO 활성화 위해
    IEnumerator StartSettingPanel()
    {
        settingPanel.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        settingPanel.SetActive(false);
    }
    /*void OpenPanel()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ActionCtrl.inventoryActivated = true;
            isSettingPanelActive = true;
            settingPanel.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cancel();
            ActionCtrl.inventoryActivated = false;
            isSettingPanelActive = false;
            settingPanel.SetActive(false);
        }
    }*/

    public void InventoryBtnOnClick()
    {
        panels[0].SetActive(true);
        panels[1].SetActive(false);
        panels[2].SetActive(false);
        panels[3].SetActive(false);
        panels[4].SetActive(false);
    }
    public void ManufactureBtnOnClick()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(true);
        panels[2].SetActive(false);
        panels[3].SetActive(false);
        panels[4].SetActive(false);
    }
    public void MapBtnOnClick()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(false);
        panels[2].SetActive(true);
        panels[3].SetActive(false);
        panels[4].SetActive(false);
    }
    public void HintBtnOnClick()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(false);
        panels[2].SetActive(false);
        panels[3].SetActive(true);
        panels[4].SetActive(false);
    }
    public void OptionBtnOnClick()
    {
        panels[0].SetActive(false);
        panels[1].SetActive(false);
        panels[2].SetActive(false);
        panels[3].SetActive(false);
        panels[4].SetActive(true);
    }

    public static bool isActivated = false;
    bool isPreviewActivate = false;

    [SerializeField] Craft[] craft_Item;
    [SerializeField] Craft[] craft_Etc_Item;
    [SerializeField] Transform tf_Player;

    private GameObject go_Preview;
    private GameObject go_prefab;

    float rotAmount = 0.0f;

    //Preview 동적 생성
    RaycastHit hitInfo;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float range;

    public void CraftObject(int _slotNum)
    {
        CharacterManager.instance.characterWater -= 2f;

        go_Preview = Instantiate(craft_Item[_slotNum].go_PreviewPrefab, tf_Player.position + tf_Player.forward,
            Quaternion.identity);
        isPreviewActivate = true;
        go_prefab = craft_Item[_slotNum].go_Prefab;
        settingPanel.SetActive(false);
        ActionCtrl.inventoryActivated = false;
        isSettingPanelActive = false;

        for (int i = 0; i < craft_Item[_slotNum].needItem.Length; i++)
        {
            //인벤토리에 있어야 생성 가능
            for (int j = 0; j < InventoryCtrl.instance.slots.Length; j++)
            {
                //인벤토리에 아이템이 있을경우
                if (InventoryCtrl.instance.slots[j].item == craft_Item[_slotNum].needItem[i])
                {
                    InventoryCtrl.instance.slots[j].itemCount -= craft_Item[_slotNum].needItemConut[i];
                }
            }
        }
    }
    void CraftEquipment(int _slotNum)
    {
        for (int i = 0; i < craft_Item[_slotNum].needItem.Length; i++)
        {
            //인벤토리에 있어야 생성 가능
            for (int j = 0; j < InventoryCtrl.instance.slots.Length; j++)
            {
                //인벤토리에 아이템이 있을경우
                if (InventoryCtrl.instance.slots[j].item == craft_Item[_slotNum].needItem[i])
                {
                    InventoryCtrl.instance.slots[j].itemCount -= craft_Item[_slotNum].needItemConut[i];
                }
            }
        }
        //인벤토리에 아이템 추가
        for (int a = 0; a < InventoryCtrl.instance.slots.Length; a++)
        {
            //인벤토리에 만드는 아이템이 있을경우 갯수 추가
            if (InventoryCtrl.instance.slots[a].item == craft_Item[_slotNum].craftingItem)
            {
                InventoryCtrl.instance.slots[a].itemCount++;
                StartCoroutine(CanNotBuild(craft_Item[_slotNum].craftName + " 제작"));
                return;
            }
            else if (InventoryCtrl.instance.slots[a].item == null)
            {
                InventoryCtrl.instance.AcquireItem(craft_Item[_slotNum].craftingItem);
                //InventoryCtrl.instance.slots[a].itemCount++;
                StartCoroutine(CanNotBuild(craft_Item[_slotNum].craftName + " 제작"));
                return;
            }
        }

    }
    void CraftEtc(int _slotNum)
    {

        for (int i = 0; i < craft_Etc_Item[_slotNum].needItem.Length; i++)
        {
            //인벤토리에 있어야 생성 가능
            for (int j = 0; j < InventoryCtrl.instance.slots.Length; j++)
            {
                //인벤토리에 아이템이 있을경우
                if (InventoryCtrl.instance.slots[j].item == craft_Etc_Item[_slotNum].needItem[i])
                {
                    InventoryCtrl.instance.slots[j].itemCount -= craft_Etc_Item[_slotNum].needItemConut[i];
                }
            }
        }

        //인벤토리에 아이템 추가
        for (int a = 0; a < InventoryCtrl.instance.slots.Length; a++)
        {
            //인벤토리에 만드는 아이템이 있을경우 갯수 추가
            if (InventoryCtrl.instance.slots[a].item == craft_Etc_Item[_slotNum].craftingItem)
            {
                InventoryCtrl.instance.slots[a].itemCount++;
                StartCoroutine(CanNotBuild(craft_Item[_slotNum].craftName + " 제작"));
                return;
            }
            else if (InventoryCtrl.instance.slots[a].item == null)
            {
                InventoryCtrl.instance.AcquireItem(craft_Etc_Item[_slotNum].craftingItem);
                StartCoroutine(CanNotBuild(craft_Item[_slotNum].craftName + " 제작"));
                //InventoryCtrl.instance.slots[a].itemCount++;
                return;
            }
        }
    }
    public void EtcClick(int _slotNum)
    {
        int needItemCount = 0;
        string a = null;
        //아이템 생성
        {
            for (int i = 0; i < craft_Etc_Item[_slotNum].needItem.Length; i++)
            {
                //인벤토리에 있어야 생성 가능
                for (int j = 0; j < InventoryCtrl.instance.slots.Length; j++)
                {
                    //인벤토리에 아이템이 있을경우
                    if (InventoryCtrl.instance.slots[j].item == craft_Etc_Item[_slotNum].needItem[i])
                    {
                        if (craft_Etc_Item[_slotNum].needItemConut[i] <=
                            InventoryCtrl.instance.slots[j].itemCount)
                        {
                            needItemCount++;
                        }
                        else
                        {
                            a = craft_Etc_Item[_slotNum].needItem[i].itemName + " "
                                + ((InventoryCtrl.instance.slots[j].itemCount - craft_Etc_Item[_slotNum].needItemConut[i]) * -1).ToString()
                                + "개 부족";
                        }
                    }
                }
            }

            if (needItemCount == craft_Etc_Item[_slotNum].needItem.Length && !craft_Etc_Item[_slotNum].isBuilable)
            {
                CraftEtc(_slotNum);
                StartCoroutine(CanNotBuild(craft_Item[_slotNum].craftName + " 생성"));
            }
            else
            {
                StartCoroutine(CanNotBuild(a));
            }

        }
    }

    public void SlotClick(int _slotNum)
    {
        int needItemCount = 0;
        string a = null;
        //아이템 생성
        {
            for (int i = 0; i < craft_Item[_slotNum].needItem.Length; i++)
            {
                //인벤토리에 있어야 생성 가능
                for (int j = 0; j < InventoryCtrl.instance.slots.Length; j++)
                {
                    //인벤토리에 아이템이 있을경우
                    if (InventoryCtrl.instance.slots[j].item == craft_Item[_slotNum].needItem[i])
                    {
                        if (craft_Item[_slotNum].needItemConut[i] <=
                            InventoryCtrl.instance.slots[j].itemCount)
                        {
                            needItemCount++;
                        }
                        else
                        {
                            a += "\n" + craft_Item[_slotNum].needItem[i].itemName + " "
                                + ((InventoryCtrl.instance.slots[j].itemCount - craft_Item[_slotNum].needItemConut[i]) * -1).ToString()
                                + "개 부족";
                        }
                    }
                }
            }

            //설치 아이템일경우
            if (needItemCount == craft_Item[_slotNum].needItem.Length && craft_Item[_slotNum].isBuilable)
            {
                CraftObject(_slotNum);
            }
            //설치 아이템이 아닐경우
            else if (needItemCount == craft_Item[_slotNum].needItem.Length && !craft_Item[_slotNum].isBuilable)
            {
                CraftEquipment(_slotNum);
            }
            else
            {
                StartCoroutine(CanNotBuild(a));
            }

        }
    }
    IEnumerator CanNotBuild(string _text)
    {
        Debug.Log("called");
        canNotBuildTxt.gameObject.SetActive(true);
        canNotBuildTxt.text = _text;
        yield return new WaitForSeconds(1.0f);
        canNotBuildTxt.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && !isPreviewActivate)
        {
            //Window();
        }
        if (isPreviewActivate) PreviewPositionUpdate();
        if (Input.GetButtonDown("Fire1"))
        {
            Build();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnItems();
        }
        if (!isSettingPanelActive) theSlotToolTip.HideToolTop();
    }

    void ReturnItems()
    {
        if (go_Preview != null)
        {
            //return Items
            if (go_Preview.name.Contains("BaseCamp_Preview"))
            {

            }
        }
    }
    void Build()
    {
        if (go_Preview != null)
        {
            if (go_Preview.name.Contains("BaseCamp_Preview"))
            {
                if (isPreviewActivate && go_Preview.GetComponent<PreviewObject>().isBuildable() && CharacterManager.baseCount < 3)
                {
                    Instantiate(go_prefab, hitInfo.point, Quaternion.Euler(0, rotAmount, 0));

                    CharacterManager.basePos[CharacterManager.baseCount] = go_Preview.transform.position;
                    CharacterManager.baseCount++;

                    Destroy(go_Preview);
                    isActivated = false;
                    isPreviewActivate = false;
                    go_prefab = null;
                    go_Preview = null;
                }
            }
            else
            {
                if (isPreviewActivate && go_Preview.GetComponent<PreviewObject>().isBuildable() &&
                Vector3.Distance(hitInfo.point, CharacterManager.basePos[0]) <= 10 ||
                Vector3.Distance(hitInfo.point, CharacterManager.basePos[1]) <= 10 ||
                Vector3.Distance(hitInfo.point, CharacterManager.basePos[2]) <= 10)
                {
                    Instantiate(go_prefab, hitInfo.point, Quaternion.Euler(0, rotAmount, 0));

                    Destroy(go_Preview);
                    isActivated = false;
                    isPreviewActivate = false;
                    go_prefab = null;
                    go_Preview = null;
                }
            }
        }



    }
    void PreviewPositionUpdate()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
            }
        }


        rotAmount += Input.GetAxis("Mouse ScrollWheel") * rotSpeed;
        go_Preview.transform.rotation = Quaternion.Euler(0, rotAmount, 0);

    }

    private void Cancel()
    {
        if (isPreviewActivate)
        {
            Destroy(go_Preview);
        }
        isActivated = false;
        isPreviewActivate = false;
        go_Preview = null;
        go_prefab = null;
        settingPanel.SetActive(false);

    }
    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }
    private void OpenWindow()
    {
        isActivated = true;
        ActionCtrl.inventoryActivated = true;
        isSettingPanelActive = true;
        settingPanel.SetActive(true);
        Cursor.visible = true;
    }
    private void CloseWindow()
    {
        isActivated = false;
        ActionCtrl.inventoryActivated = false;
        isSettingPanelActive = false;
        settingPanel.SetActive(false);
    }

}