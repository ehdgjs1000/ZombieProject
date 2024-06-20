using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePos;

    public AudioClip fireSfx;
    AudioSource source = null;
    public MeshRenderer muzzleFlash;

    float fireTimer = 0.15f;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        muzzleFlash.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            fireTimer = 0.0f;
            if (Input.GetMouseButton(0))
            {
                Fire();
                fireTimer = 0.15f;
            }
        }
    }

    void Fire()
    {
        //동적으로 총알 생성
        CreateBullet();

        source.PlayOneShot(fireSfx, 0.9f);
        StartCoroutine(this.ShowMuzzleFlash());
    }
    void CreateBullet()
    {
        Instantiate(bullet, firePos.position, firePos.rotation);
    }

    IEnumerator ShowMuzzleFlash()
    {
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.enabled = true;

        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        muzzleFlash.enabled = false;
    }
}
