using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public int maxMagazine = 30;
    public int nowMagazine;
    public float fireTerm = 0.3f;
    public float bulletSpeed = 40f;

    public static bool isChangeWeapon = false;

    [SerializeField] private string currentWeaponType;
    public static Transform currentWeapon;

    [SerializeField] private float changeWeaponDelayTime;
    [SerializeField] private float changeWeaponEndDelayTime;

    //무기 종류들 관리
    [SerializeField] private Gun[] guns;
    [SerializeField] private Hand[] hands;
    

    //총 넣고 꺼내는것
    //gunDictionary.Add("AK47", guns[0]);
    //gunDictionary["AK47"]
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChangeWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

            }
            else if(Input.GetKeyDown(KeyCode.Alpha2))
            {

            }
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        //총 넣고 뺴는 애니메이션 추가

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CanclePreWeaponAction();

    }
    private void CanclePreWeaponAction()
    {
        switch (currentWeaponType)
        {
            case "GUN":
                
                break;
            case "HAND":
                break;
        }
    }

}
