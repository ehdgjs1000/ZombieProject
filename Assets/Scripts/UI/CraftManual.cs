using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*[System.Serializable]
public class Craft
{
    public string craftName;
    public GameObject go_Prefab;
    public GameObject go_PreviewPrefab;
}*/

public class CraftManual : MonoBehaviour
{
    public static bool isActivated = false;
    bool isPreviewActivate = false;

    [SerializeField] GameObject go_BaseUi;
    [SerializeField] Craft[] craft_fire;
    [SerializeField] Transform tf_Player;

    private GameObject go_Preview;

    //Preview 동적 생성
    RaycastHit hitInfo;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float range;

    public void SlotClick(int _slotNum)
    {
        go_Preview = Instantiate(craft_fire[_slotNum].go_PreviewPrefab,tf_Player.position + tf_Player.forward,
            Quaternion.identity);
        isPreviewActivate = true;
        go_BaseUi.SetActive(false);
        SettingCtrl.isSettingPanelActive = false;
    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) && !isPreviewActivate)
        {
            Window();
        }
        if (isPreviewActivate) PreviewPositionUpdate();

        if (Input.GetKey(KeyCode.Escape))
        {
            //Cancel();
        }
    }
    void PreviewPositionUpdate()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                Vector3 _location = raycastHit.point;
                Debug.Log(_location);
                go_Preview.transform.position = _location;
            }
        }
        /*if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo,
            range, layerMask))
        {
            if(hitInfo.transform != null)
            {
                Vector3 _location = hitInfo.point;
                go_Preview.transform.position = _location;
            }
        }*/
    }

    private void Cancel()
    {
        if (isPreviewActivate)
        {
            Destroy(go_Preview);
        }
        isActivated = false;
        isPreviewActivate=false;
        go_Preview =null;
        go_BaseUi.SetActive(false);
        
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
        SettingCtrl.isSettingPanelActive = true;
        //go_BaseUi.SetActive(true);
    }
    private void CloseWindow()
    {
        isActivated = false;
        SettingCtrl.isSettingPanelActive = false;
        //go_BaseUi.SetActive(false);
    }

}
