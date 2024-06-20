using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Gun : MonoBehaviour
{
    public string gunName;
    public float range;
    public float accuracy;
    public float fireRate;
    public float reloadTime;

    public float damage;

    public int reloadBulletConut;
    public int currentBulletCount;
    public int maxBulletCount;
    public int carryBulletCount;

    public float retroActionForce; //반동 세기
    public float retroActionFineSightForce; //정조준 반동 세기

    public int bulletLevel;
    public AudioClip gunSound;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GunSetting(string _gunName,float _fireRate, int _maxBulletCount, float _retroActionForce,
        AudioClip _gunSound, float _damage=0,int _bulletLevel = 1 ,int _carryBulletCount = 0)
    {
        gunName = _gunName;
        fireRate = _fireRate;
        maxBulletCount = _maxBulletCount;
        reloadBulletConut = maxBulletCount;
        currentBulletCount = 0;
        retroActionForce = _retroActionForce;
        gunSound = _gunSound;
        bulletLevel = _bulletLevel;
        carryBulletCount = _carryBulletCount;

        damage = _damage;
    }
}
