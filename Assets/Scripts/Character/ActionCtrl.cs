using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionCtrl : MonoBehaviour
{
    public static ActionCtrl instance;
    [SerializeField] private GameObject go_inventoryPanel;
    public static bool inventoryActivated = false;
    private CharacterController characterController;

    public float radius = 0f;
    public float range = 0f;
    public float checkRange = 10.0f;

    //Audio
    AudioSource audioSource;
    [SerializeField] AudioClip[] eatFoodClips;

    //Zombie ????
    ZombieItem zombieItem;
    public LayerMask zombieLayer;
    public Collider[] colliders;
    public Collider near_enemy;

    //Item ????
    bool pickupActivated = false;
    RaycastHit hitInfo;
    public Image[] zombieImgSet;
    public Image[] inventoryItemImgs;


    //?????? ????? ????
    [SerializeField] LayerMask itemLayer;
    [SerializeField] LayerMask waterLayer;
    [SerializeField] Text actionTxt;
    [SerializeField] Image actionImage;
    [SerializeField] Sprite[] actionSprites;

    [SerializeField] InventoryCtrl inventoryCtrl;

    [SerializeField] Inventory[] handSlots;
    [SerializeField] Image[] handSlotBorder;

    public static bool isEquipedAxe = false;
    public static bool isEquipedUsed = false;
    public static bool isEquipedShortWeapon = false;
    public static bool isEquipedPistol = false;
    //?? ????
    Animator _animator;
    [SerializeField] private Gun currentGun;
    public static bool isEquipedWeapon = false;

    public GameObject aimingGunPos;
    public GameObject aimingNonePos;
    public RuntimeAnimatorController rifleAnim;
    public RuntimeAnimatorController noWeaponAnim;
    public RuntimeAnimatorController axeAnim;
    public RuntimeAnimatorController pistolAnim;
    // Start is called before the first frame update

    public bool isDead = false;
    [SerializeField] GameObject respawnPanel;
    private float tempHp;
    private bool isPlayingHitAnim = false;

    private float drinkWaterTerm = 3.0f;

    private void Awake()
    {
        instance = this;
        isDead = false;
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        _animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        WeaponManager.isChangeWeapon = true;
        tempHp = CharacterManager.instance.characterHp;
    }

    // Update is called once per frame
    void Update()
    {
        TryActtion();
        SearchZombie();
        TryOpenInventory();
        EquipSlotItem();
        CheckCharacterHp();
        drinkWaterTerm -= Time.deltaTime;
    }
    private void CheckCharacterHp()
    {
        if (tempHp != CharacterManager.instance.characterHp &&
            tempHp >= CharacterManager.instance.characterHp && !isPlayingHitAnim)
        {
            StartCoroutine(GetHitAnim());
        }
        tempHp = CharacterManager.instance.characterHp;

        if (CharacterManager.instance.characterHp <= 0 && !isDead)
        {
            isDead = true;
            CharacterManager.instance.isDie = true;
            _animator.SetTrigger("Die");

            Invoke("CharacterDie", 5.0f);
            StopAllCoroutines();
            characterController.enabled = false;
        }
    }
    public IEnumerator PlaySfx(int soundNum)
    {
        audioSource.PlayOneShot(eatFoodClips[soundNum]);
        yield return null;
    }
    IEnumerator GetHitAnim()
    {
        isPlayingHitAnim = true;
        _animator.SetTrigger("Hit");
        yield return new WaitForSeconds(1.0f);
        isPlayingHitAnim = false;
    }
    private void ItemDrop()
    {
        for (int i = 0; i < InventoryCtrl.instance.slots.Length; i++)
        {
            if (InventoryCtrl.instance.slots[i].item != null)
            {
                //Drop Item
                float ranX = Random.Range(-1.0f, 1.0f);
                float ranZ = Random.Range(-1.0f, 1.0f);
                float ranRot = Random.Range(-180.0f, 180.0f);
                Instantiate(InventoryCtrl.instance.slots[i].item.itemPrefab,
                    new Vector3(transform.position.x + ranX, transform.position.y + 0.01f, transform.position.z + ranZ),
                    Quaternion.Euler(0, ranRot, 0));

                InventoryCtrl.instance.slots[i].ClearSlot();
            }

        }
    }
    private void CharacterDie()
    {
        StopAllCoroutines();
        CharacterManager.instance.isDie = true;
        respawnPanel.SetActive(true);
        ItemDrop();
    }
    public void RespawnBtnOnClick()
    {
        //SpawnCharacter();
        respawnPanel.SetActive(false);
    }
    public void BaseBtnOnClick(int baseNum)
    {
        SpawnCharacter(baseNum);
        CharacterManager.instance.isDie = false;
        respawnPanel.SetActive(false);
    }

    public void SpawnCharacter(int baseNum)
    {
        isDead = false;
        transform.position = CharacterManager.basePos[baseNum];
        _animator.Play("Idle Walk Run Blend", -1, 0f);
        characterController.enabled = true;
        CharacterManager.instance.characterHp = 20.0f;
        CharacterManager.instance.characterWater = 20.0f;
        CharacterManager.instance.characterHungry = 20.0f;
    }
    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            inventoryActivated = !inventoryActivated;
            if (inventoryActivated)
                OpenMapInventory();
            else
                CloseInventory();

        }
    }
    private void OpenMapInventory()
    {
        SettingCtrl.isSettingPanelActive = true;
        SettingCtrl.instance.MapBtnOnClick();
        go_inventoryPanel.SetActive(true);
    }
    private void OpenInventory()
    {
        SettingCtrl.isSettingPanelActive = true;
        go_inventoryPanel.SetActive(true);
    }
    private void CloseInventory()
    {
        SettingCtrl.isSettingPanelActive = false;
        go_inventoryPanel.SetActive(false);
    }

    //?? ???? ????
    public void ChangeAnimator()
    {
        if (isEquipedWeapon)
        {
            _animator.runtimeAnimatorController = rifleAnim;
            weaponGO.SetActive(true);
            aimingNonePos.SetActive(false);
            aimingGunPos.SetActive(true);
        }
        else if (isEquipedPistol)
        {
            _animator.runtimeAnimatorController = pistolAnim;
            weaponGO.SetActive(true);
            aimingGunPos.SetActive(true);
            aimingNonePos.SetActive(false);
        }
        else if (isEquipedAxe || isEquipedShortWeapon)
        {
            _animator.runtimeAnimatorController = axeAnim;
            weaponGO.SetActive(true);
            aimingGunPos.SetActive(false);
            aimingNonePos.SetActive(true);
        }
        else if (isEquipedUsed)
        {
            _animator.runtimeAnimatorController = noWeaponAnim;
            weaponGO.SetActive(true);
            aimingGunPos.SetActive(false);
            aimingNonePos.SetActive(true);
        }
        else
        {
            _animator.runtimeAnimatorController = noWeaponAnim;
            weaponGO.SetActive(false);
            aimingGunPos.SetActive(false);
            aimingNonePos.SetActive(true);
        }
    }
    public static bool canShoot;
    IEnumerator ChangeWeapon(int weaponType)
    {
        canShoot = false;
        isEquipedAxe = false;
        isEquipedShortWeapon = false;
        _animator.SetTrigger("ChangeWeapon");
        if (weaponType == 0)
        {
            isEquipedWeapon = true;
            isEquipedPistol = false;
        }
        else
        {
            isEquipedWeapon = false;
            isEquipedPistol = true;
        }
        yield return new WaitForSeconds(1.5f);
        canShoot = true;
        weaponGO.SetActive(true);
    }
    IEnumerator ChangeShortWeapon()
    {
        _animator.SetTrigger("ChangeWeapon");
        yield return null;
    }

    void changeBgColor(int num)
    {
        for (int i = 0; i < 4; i++)
        {
            if (num == i)
            {
                handSlotBorder[i].gameObject.SetActive(true);
            }
            else
            {
                handSlotBorder[i].gameObject.SetActive(false);
            }
        }
    }

    private int bottleHandNum;
    void weaponChange(int num)
    {
        InventoryCtrl.instance.ReloadAmmo(currentGun.bulletLevel, -currentGun.currentBulletCount);


        //if slot is type.weapon
        if (handSlots[num].item.itemType == Item.ItemType.Weapon)
        {

            if (handSlots[num].item.itemName == "PickAxe")
            {
                currentGun.GunSetting(handSlots[num].item.name, handSlots[num].item.fireRate, 0, 0, handSlots[num].item.gunSound, handSlots[num].item.damage);

                isEquipedWeapon = false;
                isEquipedPistol = false;
                isEquipedAxe = true;
                isEquipedUsed = false;
                isEquipedShortWeapon = false;
                ChangeAnimator();
                StartCoroutine(ChangeShortWeapon());
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
            else if (handSlots[num].item.itemName == "Axe")
            {
                currentGun.GunSetting(handSlots[num].item.name, handSlots[num].item.fireRate, 0, 0, handSlots[num].item.gunSound,
                    handSlots[num].item.damage);

                isEquipedWeapon = false;
                isEquipedPistol = false;
                isEquipedAxe = true;
                isEquipedUsed = false;
                isEquipedShortWeapon = false;
                ChangeAnimator();
                StartCoroutine(ChangeShortWeapon());
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
            else if (handSlots[num].item.itemName == "Machete" || handSlots[num].item.itemName == "Bat" ||
                handSlots[num].item.itemName == "MeleeLv1" || handSlots[num].item.itemName == "MeleeLv2" ||
                handSlots[num].item.itemName == "MeleeLv3" || handSlots[num].item.itemName == "MeleeLv4")
            {
                currentGun.GunSetting(handSlots[num].item.name, handSlots[num].item.fireRate, 0, 0, handSlots[num].item.gunSound,
                    handSlots[num].item.damage);

                CharacterManager.weaponDmg = currentGun.damage;
                isEquipedWeapon = false;
                isEquipedPistol = false;
                isEquipedAxe = false;
                isEquipedUsed = false;
                isEquipedShortWeapon = true;
                ChangeAnimator();
                StartCoroutine(ChangeShortWeapon());
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
            else if (handSlots[num].item.itemName == "PistolLv1" || handSlots[num].item.itemName == "PistolLv2")
            {
                StartCoroutine(ChangeWeapon(1));

                //?¥ê??????? ??¢¥? ??? ???? ????????
                int carryBulletCount = 0;
                for (int i = 0; i < InventoryCtrl.instance.slots.Length; i++)
                {
                    if (InventoryCtrl.instance.slots[i].item != null &&
                        InventoryCtrl.instance.slots[i].item.ammoLevel == handSlots[num].item.bulletLevel)
                    {
                        carryBulletCount = InventoryCtrl.instance.slots[i].itemCount;
                    }

                }
                currentGun.GunSetting(handSlots[num].item.name, handSlots[num].item.fireRate, handSlots[num].item.maxBulletCount,
                    handSlots[num].item.retroForce, handSlots[num].item.gunSound, handSlots[num].item.damage,
                    handSlots[num].item.bulletLevel, carryBulletCount);
                CharacterManager.weaponDmg = currentGun.damage;
                ChangeAnimator();
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
            else
            {
                StartCoroutine(ChangeWeapon(0));

                //?¥ê??????? ??¢¥? ??? ???? ????????
                int carryBulletCount = 0;
                for (int i = 0; i < InventoryCtrl.instance.slots.Length; i++)
                {
                    if (InventoryCtrl.instance.slots[i].item != null &&
                        InventoryCtrl.instance.slots[i].item.ammoLevel == handSlots[num].item.bulletLevel)
                    {
                        carryBulletCount = InventoryCtrl.instance.slots[i].itemCount;
                    }

                }
                currentGun.GunSetting(handSlots[num].item.name, handSlots[num].item.fireRate, handSlots[num].item.maxBulletCount,
                    handSlots[num].item.retroForce, handSlots[num].item.gunSound, handSlots[num].item.damage,
                    handSlots[num].item.bulletLevel, carryBulletCount);
                CharacterManager.weaponDmg = currentGun.damage;
                ChangeAnimator();
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
        }
        else if (handSlots[num].item.itemType == Item.ItemType.Used)
        {
            if (handSlots[num].item.itemName == "????")
            {
                bottleHandNum = num;

                currentGun.GunSetting(handSlots[num].item.name, 0, 0, 0, null, 0);
                CharacterManager.weaponDmg = currentGun.damage;
                isEquipedWeapon = false;
                isEquipedAxe = false;
                isEquipedUsed = true;
                isEquipedShortWeapon = false;
                ChangeAnimator();
                changeBgColor(num);
                weaponGO.GetComponentInChildren<MeshFilter>().mesh = handSlots[num].item.weaponMesh;
            }
        }
    }

    [SerializeField] private GameObject weaponGO;
    void EquipSlotItem()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (handSlots[0].item != null)
            {
                weaponChange(0);
            }
            else
            {
                isEquipedWeapon = false;
                ChangeAnimator();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (handSlots[1].item != null)
            {
                weaponChange(1);
            }
            else
            {
                isEquipedWeapon = false;
                ChangeAnimator();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (handSlots[2].item != null)
            {
                weaponChange(2);
            }
            else
            {
                isEquipedWeapon = false;
                ChangeAnimator();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (handSlots[3].item != null)
            {
                //???? ???? or ?????? ???
                weaponChange(3);
            }
            else //?????? ??? ???? ??????
            {
                isEquipedWeapon = false;
                ChangeAnimator();
            }
        }
    }
    void TryActtion()
    {
        CheckItem();
        if (Input.GetKeyDown(KeyCode.E))
        {
            CanPickUp();
            PharmingZombie();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {

        }
    }

    void PlayAudio(AudioClip clip)
    {
        
    }

    void DrinkWater()
    {
        if (drinkWaterTerm <= 0.0f)
        {
            _animator.SetTrigger("Drink");
            CharacterManager.instance.characterWater += 30.0f;

            drinkWaterTerm = 3.0f;

            int debuffProb = Random.Range(0, 10);
            if (debuffProb <= 1)
            {
                //?????
                CharacterManager.instance.debuffType[4] = true;
            }
        }
    }
    void CanPickUp()
    {
        if (pickupActivated)
        {
            if (hitInfo.transform != null)
            {
                if (hitInfo.transform.GetComponent<ItemPickUp>().item.itemType == Item.ItemType.Ammo)
                {
                    inventoryCtrl.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item, 5);
                    GameIDManager.instance.isIngridentPicked.Add(hitInfo.transform.GetComponent<ItemPickUp>().itemID);
                }
                else
                {
                    inventoryCtrl.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                    GameIDManager.instance.isIngridentPicked.Add(hitInfo.transform.GetComponent<ItemPickUp>().itemID);
                    /*inventoryCtrl.AcquireItem(hitInfo.transform.GetComponent<ItemPickUp>().item);
                    if (hitInfo.transform.GetComponent<ItemPickUp>().item.itemType == Item.ItemType.Ingrediant)
                    {
                        GameIDManager.instance.isIngridentPicked.Add(hitInfo.transform.GetComponent<ItemPickUp>().itemID);
                    }*/
                }
                Destroy(hitInfo.transform.gameObject);
                InfoDIssapear();
            }
        }
    }
    void CheckItem()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit1, checkRange, itemLayer))
        {
            hitInfo = raycastHit1;
            if (hitInfo.transform.tag == "Item")
            {
                ItemInfoAppear();
            }
            else if (hitInfo.transform.tag == "Water")
            {
                ItemInfoAppearWater();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    DrinkWater();
                }
            }
            else InfoDIssapear();
        }
        else InfoDIssapear();


    }
    void InfoDIssapear()
    {
        pickupActivated = false;
        actionTxt.gameObject.SetActive(false);
        actionImage.gameObject.SetActive(false);
    }

    void ItemInfoAppearWater()
    {
        actionImage.sprite = actionSprites[0];
        actionImage.gameObject.SetActive(true);
        actionTxt.gameObject.SetActive(true);
        actionTxt.text = "Press" + "<color=red>" + "'E'" + "</color>";
    }
    void ItemInfoAppear()
    {
        pickupActivated = true;
        actionImage.sprite = actionSprites[2];
        actionImage.gameObject.SetActive(true);
        actionTxt.gameObject.SetActive(true);
        actionTxt.text = hitInfo.transform.GetComponent<ItemPickUp>().item.itemName;
        //+"???" + "<color=red>" + "'E'" + "</color>";
    }


    //???? ????
    void SearchZombie()
    {
        {
            colliders = Physics.OverlapSphere(transform.position, radius, zombieLayer);

            if (colliders.Length > 0)
            {
                float short_distance = Vector3.Distance(transform.position, colliders[0].transform.position);
                foreach (Collider col in colliders)
                {
                    float short_distance2 = Vector3.Distance(transform.position, col.transform.position);
                    if (short_distance >= short_distance2)
                    {
                        short_distance = short_distance2;
                        near_enemy = col;

                    }
                }
            }
            else
            {
                near_enemy = null;
            }

        }
    }
    void PharmingZombie()
    {
        if (near_enemy != null)
        {
            if ((near_enemy.gameObject.tag == "DeafZombie" && near_enemy.GetComponent<DeafZombieController>().isDead == true) ||
                (near_enemy.gameObject.tag == "CrawlZombie" && near_enemy.GetComponent<CrawlZombieController>().isDead == true) ||
                (near_enemy.gameObject.tag == "ShortZombie" && near_enemy.GetComponent<ShortZombieController>().isDead == true) ||
                (near_enemy.gameObject.tag == "LongZombie" && near_enemy.GetComponent<LongZombieController>().isDead == true))
            {
                int itemCount = 0;
                ZombieItem zombieItem = near_enemy.GetComponent<ZombieItem>();

                zombieItem.GetComponent<ZombieItem>();
                zombieItem.DropItem();

                Destroy(zombieItem.gameObject);
            }
            else if (near_enemy.gameObject.tag == "BossZombie" && near_enemy.GetComponent<BossZombieController>().isDead == true)
            {
                int itemCount = 0;
                ZombieItem zombieItem = near_enemy.GetComponent<ZombieItem>();

                zombieItem.GetComponent<ZombieItem>();
                zombieItem.DropItem();
                Destroy(zombieItem.gameObject);

            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Outside")
        {
            CharacterDie();
        }
    }
}
