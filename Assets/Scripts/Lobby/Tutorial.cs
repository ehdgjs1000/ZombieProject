using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] CrawlZombieController tutorialCrawlZombieGO;
    [SerializeField] Text tutorialText;
    [SerializeField] Text tutorialSubText;
    [SerializeField] GameObject[] tutorialDoors;
    [SerializeField] GameObject tutorialBat;
    [SerializeField] Inventory[] handSlots;

    private bool isDoorOpend = false;
    bool isBandUsed = false;

    int tutorialID = 0;

    private void Awake()
    {
        tutorialCrawlZombieGO.GetComponent<CrawlZombieController>();
    }
    private void Update()
    {
        CheckTutorial();

        if (tutorialID == 0 && tutorialBat == null) tutorialID = 1;
        //#2
        if (tutorialID == 1)
        {
            for (int a = 0; a < 4; a++)
            {
                if (handSlots[a].item != null)
                {
                    if (handSlots[a].item.itemName == "Bat") tutorialID = 2;
                }

            }
        }
        //#3
        if (tutorialID == 2 &&tutorialCrawlZombieGO.GetComponent<CrawlZombieController>().isDead) tutorialID = 3;
        //#4
        if (tutorialID == 3 && tutorialCrawlZombieGO == null) tutorialID = 4;



        // Not Start Tutorial
        if (!isBandUsed && CharacterManager.instance.characterHp <= 80.0f) tutorialID = 10;


    }
    void CheckTutorial()
    {
        if (tutorialID == 0)
        {
            tutorialText.text = "몽둥이에 습득하시오";
            tutorialSubText.text = "E키를 누르시오";
        }
        else if (tutorialID == 1)
        {
            tutorialText.text = "몽둥이 슬롯에 넣어 장착하시오";
            tutorialSubText.text = "인벤토리에서 드래그하여 장착";
        }
        else if (tutorialID == 2)
        {
            tutorialText.text = "좌클릭으로 좀비를 죽이시오";
            tutorialSubText.text = null;
        }
        else if (tutorialID == 3)
        {
            tutorialText.text = "좀비를 수색하시오";
            tutorialSubText.text = "E키로 수색";
        }
        else if (tutorialID == 4)
        {
            if (!isDoorOpend) StartCoroutine(OpenDoor());
        }
        else if (tutorialID == 10)
        {
            tutorialText.text = "붕대를 제작하시오";
            tutorialSubText.text = "제작 탭에서 제작";
        }
        else if (tutorialID == 11)
        {
            tutorialText.text = "붕대를 사용하시오";
            tutorialSubText.text = "우클릭으로 사용";
            isBandUsed = true;
        }
    }
    IEnumerator OpenDoor()
    {
        isDoorOpend = true;
        yield return new WaitForSeconds(1.0f);
        tutorialText.text = "재료를 파밍하고 베이스 캠프를 만드시오";
        tutorialSubText.text = null;

        tutorialDoors[0].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[1].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[2].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[3].transform.rotation = Quaternion.Euler(0, 90, 0);
    }

}