using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "New Item", menuName ="New Item/item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType itemType;
    public Sprite itemImg;
    public GameObject itemPrefab;
    public Mesh weaponMesh;

    public string equipType;
    public int equipLv;

    public float healAmount;
    public float thristAmonut;
    public float hungryAmount;
    public int drinkCount;
    public float purifyTime;

    public float fireRate;
    public int maxBulletCount;
    public float retroForce;
    public float damage;

    public int bulletLevel;
    public int ammoLevel;
    public int maxItemCount;

    public AudioClip reloadSfx;
    public AudioClip gunSound;

    public enum ItemType
    {
        Equipment,
        Used,
        Ingrediant,
        Weapon,
        Ammo,
        ETC
    }

}
