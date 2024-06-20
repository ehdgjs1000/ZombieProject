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
            tutorialText.text = "�����̿� �����Ͻÿ�";
            tutorialSubText.text = "EŰ�� �����ÿ�";
        }
        else if (tutorialID == 1)
        {
            tutorialText.text = "������ ���Կ� �־� �����Ͻÿ�";
            tutorialSubText.text = "�κ��丮���� �巡���Ͽ� ����";
        }
        else if (tutorialID == 2)
        {
            tutorialText.text = "��Ŭ������ ���� ���̽ÿ�";
            tutorialSubText.text = null;
        }
        else if (tutorialID == 3)
        {
            tutorialText.text = "���� �����Ͻÿ�";
            tutorialSubText.text = "EŰ�� ����";
        }
        else if (tutorialID == 4)
        {
            if (!isDoorOpend) StartCoroutine(OpenDoor());
        }
        else if (tutorialID == 10)
        {
            tutorialText.text = "�ش븦 �����Ͻÿ�";
            tutorialSubText.text = "���� �ǿ��� ����";
        }
        else if (tutorialID == 11)
        {
            tutorialText.text = "�ش븦 ����Ͻÿ�";
            tutorialSubText.text = "��Ŭ������ ���";
            isBandUsed = true;
        }
    }
    IEnumerator OpenDoor()
    {
        isDoorOpend = true;
        yield return new WaitForSeconds(1.0f);
        tutorialText.text = "��Ḧ �Ĺ��ϰ� ���̽� ķ���� ����ÿ�";
        tutorialSubText.text = null;

        tutorialDoors[0].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[1].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[2].transform.rotation = Quaternion.Euler(0, 90, 0);
        tutorialDoors[3].transform.rotation = Quaternion.Euler(0, 90, 0);
    }

}