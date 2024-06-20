using Cinemachine;
using StarterAssets;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ThirdPersonShooterController : MonoBehaviour
{
    public static bool isShooting;

    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform spawnBulletPosRifle;
    [SerializeField] private Transform spawnBulletPosPistol;
    [SerializeField] private MeshRenderer muzzleFlashRifle;
    [SerializeField] private MeshRenderer muzzleFlashPistol;
    [SerializeField] private AudioClip fireSfx;
    [SerializeField] private AudioClip reloadSfx;
    [SerializeField] private Text maxBulletCountTxt;
    [SerializeField] private Text currentBulletCountTxt;
    [SerializeField] BoxCollider shortWeaponArea;

    [SerializeField] private ParticleSystem bloodVfx;

    AudioSource source = null;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;

    private bool isReload = false;
    private Animator _animator;

    [SerializeField] private Gun currentGun;

    [SerializeField] private AudioClip rockMiningClip;
    [SerializeField] private AudioClip treeChoppinClip;

    float fireTerm = 0.15f;
    float axeTerm = 1.5f;
    float grenadeTerm = 3.0f;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }
    private void Start()
    {
        source = GetComponent<AudioSource>();
        muzzleFlashRifle.enabled = false;
        muzzleFlashPistol.enabled = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReload)
        {
            if (currentGun.currentBulletCount != currentGun.maxBulletCount)
            {
                StartCoroutine(Reload());
            }

        }
        currentBulletCountTxt.text = currentGun.currentBulletCount.ToString();
        maxBulletCountTxt.text = currentGun.maxBulletCount.ToString();

        if (!SettingCtrl.isSettingPanelActive)
        {
            Vector3 mouseWorldPosition = Vector3.zero;

            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
            {
                debugTransform.position = raycastHit.point;
                mouseWorldPosition = raycastHit.point;
            }
            if (starterAssetsInputs.aim)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                thirdPersonController.SetSensitivity(aimSensitivity);
                thirdPersonController.SetRotateOnMove(false);

                Vector3 worldAimTarget = mouseWorldPosition;
                worldAimTarget.y = transform.position.y;
                Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
            }
            else
            {
                aimVirtualCamera.gameObject.SetActive(false);
                thirdPersonController.SetSensitivity(normalSensitivity);
                thirdPersonController.SetRotateOnMove(true);
            }
            fireTerm -= Time.deltaTime;
            axeTerm -= Time.deltaTime;
            grenadeTerm -= Time.deltaTime;
            if (fireTerm <= 0)
            {
                fireTerm = 0.0f;
                if (starterAssetsInputs.aim && starterAssetsInputs.shoot && !isReload)
                {
                    if (ActionCtrl.isEquipedWeapon) Fire(mouseWorldPosition, 1);
                    else if (ActionCtrl.isEquipedPistol) Fire(mouseWorldPosition, 0);

                    fireTerm = currentGun.fireRate;
                }
            }
            if (axeTerm <= 0)
            {
                if (starterAssetsInputs.shoot)
                {
                    UseShortWeapon();
                }
            }
        }
    }
    IEnumerator PlaySFX(int sfxnum)
    {
        if (sfxnum == 0)
        {
            yield return new WaitForSeconds(0.5f);
            source.PlayOneShot(treeChoppinClip);
        }
        else if (sfxnum == 1)
        {
            yield return new WaitForSeconds(0.5f);
            source.PlayOneShot(rockMiningClip);
        }
        else if (sfxnum == 2)
        {
            yield return new WaitForSeconds(0.3f);
            source.PlayOneShot(treeChoppinClip);
        }
    }

    void UseShortWeapon()
    {
        axeTerm = currentGun.fireRate;
        if (ActionCtrl.isEquipedAxe)
        {
            Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hit, 6f, aimColliderLayerMask))
            {
                if (hit.transform.tag == "Tree")
                {
                    StartCoroutine(PlaySFX(0));
                    hit.collider.gameObject.GetComponent<TreeManager>().Chopping();
                }
                else if (hit.transform.tag == "Rock")
                {
                    StartCoroutine(PlaySFX(1));
                    hit.collider.gameObject.GetComponent<RockManager>().Mining();
                }
                else StartCoroutine(AttackDamage(hit));
            }
            _animator.SetTrigger("AxeAttack");
            CharacterManager.instance.characterWater -= 0.5f;
        }
        else if (ActionCtrl.isEquipedShortWeapon)
        {
            /*Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
            if (Physics.Raycast(ray, out RaycastHit hit, 8f, aimColliderLayerMask))
            {
                StartCoroutine(AttackDamage(hit));
                
            }*/
            StartCoroutine(Swing());
            CharacterManager.instance.characterWater -= 0.1f;
            int a = Random.Range(0, 2);
            if (a == 0) _animator.SetTrigger("AxeAttack");
            else _animator.SetTrigger("hitCrawl");
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        shortWeaponArea.enabled = true;
        shortWeaponArea.GetComponent<Melee>().damage = currentGun.damage;
        
        yield return new WaitForSeconds(0.5f);
        shortWeaponArea.enabled = false;
    }
    IEnumerator AttackDamage(RaycastHit hit)
    {
        yield return new WaitForSeconds(0.5f);
        if (hit.transform.tag == "DeafZombie")
        {
            StartCoroutine(PlaySFX(2));
            hit.transform.gameObject.GetComponent<DeafZombieController>().zombieHp -= currentGun.damage;
            StartCoroutine(MakeBloodEffect(0.2f, hit));
        }
        else if (hit.transform.tag == "CrawlZombie")
        {
            StartCoroutine(PlaySFX(2));
            hit.transform.gameObject.GetComponent<CrawlZombieController>().zombieHp -= currentGun.damage;
            StartCoroutine(MakeBloodEffect(0.2f, hit));
        }
        else if (hit.transform.tag == "ShortZombie")
        {
            StartCoroutine(PlaySFX(2));
            hit.transform.gameObject.GetComponent<ShortZombieController>().zombieHp -= currentGun.damage;
            StartCoroutine(MakeBloodEffect(0.2f, hit));
        }
        else if (hit.transform.tag == "LongZombie")
        {
            StartCoroutine(PlaySFX(2));
            hit.transform.gameObject.GetComponent<LongZombieController>().zombieHp -= currentGun.damage;
            StartCoroutine(MakeBloodEffect(0.2f, hit));
        }
    }
    public void PublicPlaySFX(int a)
    {
        StartCoroutine(PlaySFX(a));
    }
    IEnumerator MakeBloodEffect(float makeTime, RaycastHit hit)
    {
        yield return new WaitForSeconds(makeTime);
        Vector3 bloodPos = new Vector3(hit.transform.position.x, hit.transform.position.y + 0.5f,
            hit.transform.position.z);
        Instantiate(bloodVfx, bloodPos, Quaternion.identity);
    }
    void Fire(Vector3 _mousePos, int weaponType)
    {
        if (!isReload && ActionCtrl.isEquipedWeapon && ActionCtrl.canShoot ||
            !isReload && ActionCtrl.isEquipedPistol && ActionCtrl.canShoot)
        {
            if (currentGun.currentBulletCount > 0)
                Shoot(_mousePos, weaponType);
            else
                StartCoroutine(Reload());
        }
    }

    public static Vector3 soundPos;
    void Shoot(Vector3 _mousePos, int weaponType)
    {
        //반동 
        isShooting = true;
        soundPos = transform.position;

        float retroForce = 0;
        int retroChance = Random.Range(0,10);
        if(retroChance >= 7)
        {
            retroForce = currentGun.retroActionForce * 0.1f;
        }
        else
        {
            retroForce = currentGun.retroActionForce * 0.02f;
        }
        float randomX = Random.Range(-retroForce, retroForce);
        float randomY = Random.Range(-retroForce, retroForce);

        CreateBullet(new Vector3(_mousePos.x + randomX, _mousePos.y + randomY, _mousePos.z), weaponType);
        currentGun.currentBulletCount--;

        source.PlayOneShot(currentGun.gunSound, 0.9f);

        if (weaponType == 1)
        {
            StartCoroutine(this.ShowMuzzleFlashRifle());
        }
        else StartCoroutine(this.ShowMuzzleFlashPistol());
    }
    IEnumerator Reload()
    {
        if (currentGun.carryBulletCount > 0)
        {
            isReload = true;
            //재장전 Animation 및 Sound
            _animator.SetTrigger("Reload");
            yield return new WaitForSeconds(1f);
            source.PlayOneShot(reloadSfx);

            //남은 탄 넣기
            currentGun.carryBulletCount += currentGun.currentBulletCount;
            InventoryCtrl.instance.ReloadAmmo(currentGun.bulletLevel, -currentGun.currentBulletCount);
            currentGun.currentBulletCount = 0;

            yield return new WaitForSeconds(currentGun.reloadTime);


            //가진 총알 수 만큼 재장전
            //InventoryCtrl에서 Bullet 갯수 가져오기
            if (currentGun.carryBulletCount >= currentGun.reloadBulletConut)
            {
                currentGun.currentBulletCount = currentGun.reloadBulletConut;
                currentGun.carryBulletCount -= currentGun.reloadBulletConut;
            }
            else
            {
                currentGun.currentBulletCount = currentGun.carryBulletCount;
                currentGun.carryBulletCount = 0;
            }
            InventoryCtrl.instance.ReloadAmmo(currentGun.bulletLevel, currentGun.reloadBulletConut);

            isReload = false;
        }
    }


    void CreateBullet(Vector3 _mousePos, int weaponType)
    {
        if (weaponType == 1)
        {
            Vector3 aimDir = (_mousePos - spawnBulletPosRifle.position).normalized;
            Instantiate(bulletPrefab, spawnBulletPosRifle.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }
        else
        {
            Vector3 aimDir = (_mousePos - spawnBulletPosPistol.position).normalized;
            Instantiate(bulletPrefab, spawnBulletPosPistol.position, Quaternion.LookRotation(aimDir, Vector3.up));
        }

    }

    IEnumerator ShowMuzzleFlashRifle()
    {
        float scale = Random.Range(0.5f, 0.8f);
        muzzleFlashRifle.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlashRifle.transform.localRotation = rot;

        muzzleFlashRifle.enabled = true;

        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        isShooting = false;
        muzzleFlashRifle.enabled = false;
    }
    IEnumerator ShowMuzzleFlashPistol()
    {
        float scale = Random.Range(0.5f, 0.8f);
        muzzleFlashPistol.transform.localScale = Vector3.one * scale;

        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlashPistol.transform.localRotation = rot;

        muzzleFlashPistol.enabled = true;

        yield return new WaitForSeconds(Random.Range(0.01f, 0.03f));

        isShooting = false;
        muzzleFlashPistol.enabled = false;
    }




}