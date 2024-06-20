using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CharacterManager : MonoBehaviour, IPointerEnterHandler
{
    public static CharacterManager instance;

    [Header("Character Stats")]
    public float characterHp = 100.0f;
    public float characterHpMax = 100.0f;
    public float characterWater = 100.0f;
    public float characterWaterMax = 100.0f;
    public float characterHungry = 100.0f;
    public float characterHungryMax = 100.0f;

    public Image characterHpImg;
    public Image characterWaterImg;
    public Image characterHungryImg;
    float waterDecreaseTime = 30.0f;
    float hungryDecreaseTime = 60.0f;
    float hungryDecrease = 60.0f;

    public bool isDie = false;
    //캐릭터 디버프 추가하기

    [Header("Character Slots")]
    public Image slot1_Img;
    public Mesh[] bagMeshes;
    public MeshFilter[] meshFilters;
    public Mesh[] helmetMeshes;

    public static float weaponDmg;

    public static Vector3[] basePos = new Vector3[3];
    public static int baseCount = 1;

    private float decreaseTime = 60.0f;
    private float nearBaseTime = 10.0f;
    float hungryIncreaseHpTime = 30.0f;

    [SerializeField] GameObject playerArmature;

    public bool[] debuffType = new bool[5];
    float bleedingTime = 10.0f;
    float thirstTime = 30.0f;
    public GameObject dizzyPanel;
    [SerializeField] GameObject scratchImg;
    [SerializeField] GameObject dizzyImg;
    [SerializeField] GameObject poisionImg;
    [SerializeField] GameObject powerUpImg;
    [SerializeField] Text healthTxt;
    [SerializeField] Text waterTxt;
    [SerializeField] Text hungryTxt;

    public float poisionRemoveTime = 300.0f;
    public float dizzyRemoveTime = 300.0f;


    //Singleton
    private void Awake()
    {
        instance = this;
        basePos[0] = new Vector3(470, 10f, -146);
    }
    // Start is called before the first frame update
    void Start()
    {
        meshFilters = GetComponentsInChildren<MeshFilter>();
    }

    int a = 0;
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(nearBaseTime);

        CharacterStatsUpdate();
        ChangeBag();
        ChangeHelmet();
        StatsDecrease();
        NearBaseCamp();
        CheckDebuff();
        CheckHungry();

        waterDecreaseTime -= Time.deltaTime;
        hungryDecreaseTime -= Time.deltaTime;
        hungryDecrease -= Time.deltaTime;
        hungryIncreaseHpTime -= Time.deltaTime;


        if (characterHp <= 0) characterHp = 0;
        if (characterWater <= 0) characterWater = 0;
        if (characterHp >= 100.0f) characterHp = 100.0f;
        if (characterWater >= 100.0f) characterWater = 100.0f;
        if (characterHungry <= 0) characterHungry = 0;
        if (characterHungry >= 100.0f) characterHungry = 100.0f;

        if (characterWater <= 0.0f && waterDecreaseTime <= 0.0f)
        {
            characterHp -= 3.0f;
            waterDecreaseTime = 30.0f;
        }
        if (characterHungry <= 0.0f && hungryDecreaseTime < 0.0f)
        {
            characterHp -= 1.0f;
            hungryDecreaseTime = 20.0f;
        }
        if (hungryDecrease <= 0.0f)
        {
            characterHungry -= 1.5f;
            hungryDecrease = 60.0f;
        }
        if(characterHungry >=90 && hungryIncreaseHpTime <= 0.0f)
        {
            characterHp += 1.0f;
            hungryIncreaseHpTime = 30.0f;
        }
        healthTxt.text = Mathf.Round(characterHp).ToString();
        waterTxt.text = Mathf.Round(characterWater).ToString();
        hungryTxt.text = Mathf.Round(characterHungry).ToString();
    }

    void NearBaseCamp()
    {
        int nearBaseCount = 0;
        for (int i = 0; i < basePos.Length; i++)
        {
            if (Vector3.Distance(playerArmature.transform.position, basePos[i]) <= 10.0f) nearBaseCount++;
        }
        if (nearBaseCount > 0)
        {
            nearBaseTime -= Time.deltaTime;
            if (nearBaseTime <= 0.0f)
            {
                characterHp += 1.2f;
                nearBaseTime = 5.0f;
            }
        }
        else if (nearBaseCount <= 0) nearBaseTime = 5.0f;
    }

    void CheckHungry()
    {
        if(characterHungry >= 90.0f) powerUpImg.SetActive(true);
        else powerUpImg.SetActive(false);
    }

    void StatsDecrease()
    {
        decreaseTime -= Time.deltaTime;

        if (decreaseTime <= 0)
        {
            characterWater -= 1f;
            decreaseTime = 60.0f;
        }
    }


    void ChangeBag()
    {
        if (EquipSlotManager.bagLv != 0)
        {
            meshFilters[1].sharedMesh = bagMeshes[EquipSlotManager.bagLv - 1];
        }
    }
    void ChangeHelmet()
    {
        if (EquipSlotManager.helmetLv != 0)
        {
            meshFilters[0].sharedMesh = helmetMeshes[EquipSlotManager.helmetLv - 1];
        }
    }

    void CharacterStatsUpdate()
    {
        characterHpImg.fillAmount = characterHp / characterHpMax;
        characterWaterImg.fillAmount = characterWater / characterWaterMax;
        characterHungryImg.fillAmount = characterHungry / characterHungryMax;
    }

    //Hp/Water 증가 함수
    public void IncreaseHp(float hpVal)
    {
        characterHp += hpVal;
        if (characterHp >= characterHpMax) characterHp = characterHpMax;
    }
    public void IncreasetWater(float waterVal)
    {
        characterWater += waterVal;
        if (characterWater >= characterWaterMax) characterWater = characterWaterMax;
    }
    public void IncreaseHungry(float hungryVal)
    {
        characterHungry += hungryVal;
        if (characterHungry >= characterHungryMax) characterHungry = characterHungryMax;
    }
    void CheckDebuff()
    {
        if (debuffType[1])
        {
            scratchImg.SetActive(true);
            bleedingTime -= Time.deltaTime;
            if (bleedingTime <= 0)
            {
                //bleeding
                characterHp -= 1;
                bleedingTime = 10.0f;
            }
        }
        else scratchImg.SetActive(false);
        if (debuffType[2])
        {
            //어지러움
            dizzyImg.SetActive(true);
            dizzyPanel.SetActive(true);
            dizzyRemoveTime -= Time.deltaTime;
            if (dizzyRemoveTime <= 0.0f) debuffType[2] = false;
        }
        else
        {
            dizzyRemoveTime = 300.0f;
            dizzyImg.SetActive(false);
            dizzyPanel.SetActive(false);
        }

        if (debuffType[3])
        {
            //환각
        }

        //식중독
        if (debuffType[4])
        {
            poisionImg.SetActive(true);
            thirstTime -= Time.deltaTime;
            poisionRemoveTime -= Time.deltaTime;
            if (thirstTime <= 0.0f)
            {
                characterWater -= 1.0f;
                thirstTime = 30.0f;
            }
            if (poisionRemoveTime <= 0.0f) debuffType[4] = false;
        }
        else
        {
            poisionImg.SetActive(false);
            thirstTime = 30.0f;
            poisionRemoveTime = 300.0f;
        }

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        string a = null;
        if (debuffType[1])
        {
            a += "출혈" + "\n";
        }
        if (debuffType[2])
        {
            a += "어지러움" + "\n";
        }
        if (debuffType[4])
        {
            a += "식중독" + "\n";
        }
    }

    float damageTerm = 0.1f;
    private void OnTriggerEnter(Collider co)
    {
        if (co.tag == "Outside")
        {
            damageTerm -= Time.deltaTime;
            if (damageTerm <= 0.0f)
            {
                CharacterManager.instance.characterHp -= 1.0f;
                damageTerm = 0.2f;
            }
        }
    }


}