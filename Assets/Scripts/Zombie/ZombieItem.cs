using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieItem : MonoBehaviour
{
    [SerializeField] public int leatherProbability;
    [SerializeField] public int boneProbability;
    [SerializeField] public int metalProbability;
    [SerializeField] public int glassProbability;
    [SerializeField] public int gunPowderProbability;

    public bool itemLeather;
    public bool itemBone;
    public bool itemMetal;
    public bool itemGlass;
    public bool itemGunPowder;
    public bool[] items;

    public int itemCount;

    public Sprite[] dropImgs;
    public GameObject[] dropItems;

    private Vector3 itemDropPos;


    private void Awake()
    {
        ItemChanceCheck();
    }

    public void ItemChanceCheck()
    {
        //itemCount 보스일 경우 바꿔주기
        itemCount = 5;
        for (int a = 0; a < itemCount; a++)
        {
            int change = Random.Range(0, 100);
            if (a == 0 && change <= leatherProbability) {
                itemLeather = true;
                items[a] = true;
            }
            if (a == 1 && change <= boneProbability) {
                itemBone = true;
                items[a] = true;
            }
            if (a == 2 && change <= metalProbability) {
                itemMetal = true;
                items[a] = true;
            }
            if (a == 3 && change <= glassProbability) {
                itemGlass = true;
                items[a] = true;
            }
            if (a == 4 && change <= gunPowderProbability) {
                itemGunPowder = true;
                items[a] = true;
            }

        }
    }

    //아이템 뿌리기
    public void DropItem()
    {
        itemDropPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        //좀비 근처 랜덤한 위치에 아이템 뿌리기
        if (itemLeather)
            Instantiate(dropItems[0], new Vector3(itemDropPos.x + Random.Range(-0.5f, 0.5f),
                itemDropPos.y+1f, itemDropPos.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
        if(itemBone)
            Instantiate(dropItems[1], new Vector3(itemDropPos.x + Random.Range(-0.5f, 0.5f),
                itemDropPos.y+1f, itemDropPos.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
        if (itemMetal)
            Instantiate(dropItems[2], new Vector3(itemDropPos.x + Random.Range(-0.5f, 0.5f),
                itemDropPos.y+1f , itemDropPos.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
        if (itemGlass)
            Instantiate(dropItems[3], new Vector3(itemDropPos.x + Random.Range(-0.5f, 0.5f),
                itemDropPos.y + 1f, itemDropPos.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
        if (itemGunPowder)
        {
            int ranNum = Random.Range(1,3);
            for(int a = 0; a < ranNum; a++)
            {
                Instantiate(dropItems[4], new Vector3(itemDropPos.x + Random.Range(-0.5f, 0.5f),
                itemDropPos.y + 1f, itemDropPos.z + Random.Range(-0.5f, 0.5f)), Quaternion.identity);
            }
        }
            

    }



}
