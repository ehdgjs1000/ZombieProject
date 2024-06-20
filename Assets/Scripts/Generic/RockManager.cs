using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RockManager : MonoBehaviour
{
    [SerializeField] float rockHp = 100.0f;

    [SerializeField] GameObject rockPrefab;
    Vector3 itemDropPos;
    int miningNum = 2;
    float createTime = 5.0f;

    public static List<string> CreatedItems = new List<string>();
    public string itemID;

    private void Start()
    {
        CreateUniqueItemID();
    }
    private void Update()
    {
        ChekcPharmed();
    }
    public void Mining()
    {
        rockHp -= 10.0f;
        --miningNum;
        if (miningNum == 0)
        {
            StartCoroutine(MiningRock());
            miningNum = 2;
        }

        if (rockHp <= 0)
            StartCoroutine(DestroyRock());
    }
    IEnumerator DestroyRock()
    {
        yield return new WaitForSeconds(0.5f);
        GameIDManager.instance.isGenericPharmed.Add(itemID);
        Destroy(gameObject);

        /*yield return new WaitForSeconds(createTime);
        StartCoroutine(CreateRock());*/
    }
    IEnumerator MiningRock()
    {
        yield return new WaitForSeconds(0.5f);

        transform.localScale -= new Vector3(0.06f, 0.06f, 0.06f);
        int ranNum = Random.Range(1, 10);
        if (ranNum >= 4)
        {
            itemDropPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
            Instantiate(rockPrefab, new Vector3(itemDropPos.x + Random.Range(-1f, 1f),
                        itemDropPos.y + 1f, itemDropPos.z + Random.Range(-1f, 1f)), Quaternion.identity);
        }
    }
    void ChekcPharmed()
    {
        for (int a = 0; a < GameIDManager.instance.isGenericPharmed.Count; a++)
        {
            if (this.itemID == GameIDManager.instance.isGenericPharmed[a])
            {
                Destroy(this.gameObject);
            }
        }
    }
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