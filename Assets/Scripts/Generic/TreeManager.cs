using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    [SerializeField] float treeHp = 100.0f;

    [SerializeField] GameObject woodPrefab;
    Animator _animator;
    Vector3 itemDropPos;

    public static List<string> CreatedItems = new List<string>();
    public string itemID;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        CreateUniqueItemID();
    }
    private void Update()
    {
        ChekcPharmed();
    }
    public void Chopping()
    {
        treeHp -= 30.0f;
        StartCoroutine(GetHit());
        if (treeHp <= 0)
        {
            StartCoroutine(DestroyTree());
        }
    }
    IEnumerator DestroyTree()
    {
        GameIDManager.instance.isGenericPharmed.Add(itemID);
        yield return new WaitForSeconds(0.5f);
        _animator.SetTrigger("isDone");
        yield return new WaitForSeconds(1.5f);

        itemDropPos = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);

        //좀비 근처 랜덤한 위치에 아이템 뿌리기

        int woodNum = Random.Range(1, 4);
        for (int i = 0; i < woodNum; i++)
        {
            Instantiate(woodPrefab, new Vector3(itemDropPos.x + Random.Range(-1f, 1f),
                        itemDropPos.y + 0.5f, itemDropPos.z + Random.Range(-1f, 1f)), Quaternion.identity);
        }
        Destroy(gameObject);
    }
    IEnumerator GetHit()
    {
        yield return new WaitForSeconds(0.3f);
        _animator.SetTrigger("isHit");
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