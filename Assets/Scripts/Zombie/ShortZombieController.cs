using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ShortZombieController : MonoBehaviour
{
    //GUID
    public string itemID;

    public float zombieHp = 100.0f;
    public float zombieDamage = 10.0f;
    public float speed = 1.0f;

    Animator _zombieAnimator;
    [SerializeField] GameObject zombieRef;
    private Vector3 startPos;
    public float respawnTime;

    public Transform targetTr;
    //�þ� check ����
    public float range = 20.0f;
    public float attackRange = 1.0f;
    float checkRange;

    public bool playerChecked = false;
    public bool isChecked = false;
    public bool isDead = false;
    bool isAttacking = false;
    float canAttackTerm = 1.0f;

    float inRangeTime;
    float outRangeTime;

    private bool canRandomMove = true;
    private float randomMoveTerm = 5.0f;
    public float updateInterval = 5.0f;
    private float timeSinceLastUpdate;

    //���Ÿ� ����
    [SerializeField] GameObject zombieThrowingPrefab;
    [SerializeField] private Transform spawnThrowingPos;

    CapsuleCollider zombieCo;
    [SerializeField] AudioClip[] zombieIdleClips;
    AudioSource audioSource;
    int screamCount = 0;

    //Nav Mesh
    protected NavMeshAgent nav;
    protected Vector3 destination;
    //UI

    // Start is called before the first frame update
    private void Awake()
    {
        startPos = transform.position;
        //Prefab���� �� ��� ĳ���� ã�ƿ��� ����
        targetTr = GameObject.Find("Geometry").transform;
    }

    void Start()
    {
        CreateUniqueItemID();
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        _zombieAnimator = GetComponent<Animator>();
        zombieCo = GetComponent<CapsuleCollider>();
        nav.speed = speed;
        nav.enabled = true;
        isDead = false;
        timeSinceLastUpdate = updateInterval;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPicked();
        ZombieControl();
        CheckRandomMove();

        if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.SetBool("isDead", true);
            //StartCoroutine(Respawn(respawnTime));
            ZombieDie();
        }
        canAttackTerm -= Time.deltaTime;

    }
    void CheckRandomMove()
    {
        timeSinceLastUpdate += Time.deltaTime; // �ð� ���� �����մϴ�.

        if (timeSinceLastUpdate >= updateInterval) // ������ �ð� ������ �������� Ȯ���մϴ�.
        {
            Vector3 randomPosition = GetRandomPositionOnNavMesh(); // NavMesh ���� ������ ��ġ�� �����ɴϴ�.
            nav.SetDestination(randomPosition); // NavMeshAgent�� ��ǥ ��ġ�� ���� ��ġ�� �����մϴ�.
            timeSinceLastUpdate = 0f; // �ð� ���� �ʱ�ȭ�մϴ�.
        }
    }
    Vector3 GetRandomPositionOnNavMesh()
    {
        Vector3 randomDirection = Random.insideUnitSphere * 20f; // ���ϴ� ���� ���� ������ ���� ���͸� �����մϴ�.
        randomDirection += transform.position; // ���� ���� ���͸� ���� ��ġ�� ���մϴ�.

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 20f, NavMesh.AllAreas)) // ���� ��ġ�� NavMesh ���� �ִ��� Ȯ���մϴ�.
        {
            return hit.position; // NavMesh ���� ���� ��ġ�� ��ȯ�մϴ�.
        }
        else
        {
            return transform.position; // NavMesh ���� ���� ��ġ�� ã�� ���� ��� ���� ��ġ�� ��ȯ�մϴ�.
        }
    }
    void CheckPicked()
    {
        for (int a = 0; a < GameIDManager.instance.isIngridentPicked.Count; a++)
        {
            if (this.itemID == GameIDManager.instance.isIngridentPicked[a])
            {
                Destroy(this.gameObject);
            }
        }
    }
    IEnumerator Respawn(float respawnTime)
    {
        Debug.Log("respawn Called");
        GameObject zombieClone = Instantiate(zombieRef);
        ShortZombieController zc = zombieClone.GetComponent<ShortZombieController>();
        zc.zombieHp = 100.0f;
        zc.isDead = false;
        zc.playerChecked = false;
        zc.isChecked = false;
        zombieClone.transform.position = startPos;
        zombieClone.SetActive(false);

        yield return new WaitForSeconds(5.0f);
        zombieClone.SetActive(true);

    }
    public void BossOrderAttack()
    {
        if (zombieHp > 0)
        {
            nav.SetDestination(targetTr.position);
            _zombieAnimator.SetBool("isRunning", true);
        }

    }
    void IsPlayerInRange()
    {
        if (playerChecked)
        {
            inRangeTime += Time.deltaTime;
            outRangeTime = 0.0f;
        }
        else
        {
            inRangeTime = 0.0f;
            outRangeTime += Time.deltaTime;
        }
        if (outRangeTime >= 3)
        {
            isChecked = false;
        }

    }

    RaycastHit zombieHitInfo;

    void CheckPlayer()
    {
        Vector3 zombieRay = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);

        if (Physics.Raycast(zombieRay, targetRay - zombieRay, out zombieHitInfo, range + 5.0f))
        {
            if (zombieHitInfo.transform.name == "Player")
            {
                playerChecked = true;
            }
            else playerChecked = false;
        }
        IsPlayerInRange();
    }


    void ZombieControl()
    {
        float distance = Vector3.Distance(transform.position, targetTr.position);
        float soundDistance = Vector3.Distance(transform.position, ThirdPersonShooterController.soundPos);
        if (isDead == false)
        {
            //�ѼҸ��� ���� ���� �νİŸ� ����
            if (ThirdPersonShooterController.isShooting)
                checkRange = range * 2;
            else
                checkRange = range;

            //�ѼҸ� ���� ������ �̵�
            if (soundDistance <= checkRange && ThirdPersonShooterController.isShooting)
            {
                nav.SetDestination(ThirdPersonShooterController.soundPos);
                _zombieAnimator.SetBool("isRunning", true);
                _zombieAnimator.speed = speed;
            }

            if (distance <= checkRange && !isAttacking)
            {
                CheckPlayer();
                if (playerChecked)
                {
                    isChecked = true;

                    if (screamCount == 0) StartCoroutine(ZombieScream(0));

                    /*destination = new Vector3(transform.position.x - targetTr.position.x,
                        0f, transform.position.z - targetTr.position.z).normalized;*/
                    nav.SetDestination(targetTr.position);

                    _zombieAnimator.SetBool("isRunning", true);
                    _zombieAnimator.speed = speed;
                }
            }
            if (isChecked == true && !isAttacking)
            {
                CheckPlayer();
                nav.SetDestination(targetTr.position);
            }

            if (!isChecked && !playerChecked)
            {
                randomMoveTerm -= Time.deltaTime;
            }

            // ����
            if (distance <= attackRange)
            {
                if (!isAttacking && canAttackTerm < 0.0f)
                {
                    StartCoroutine(Attack());
                }
            }

        }
    }
    IEnumerator Attack()
    {
        nav.enabled = false;
        isAttacking = true;
        _zombieAnimator.SetBool("isAttack", true);
        yield return new WaitForSeconds(0.2f);
        float distance = Vector3.Distance(transform.position, targetTr.position);
        if (distance <= 1.2f)
        {
            CharacterManager.instance.characterHp -= zombieDamage *
                (1 - (EquipSlotManager.helmetLv * 5f / 100) - (EquipSlotManager.armorLv * 5f / 100));

            //�����
            CharacterDebuff();
        }

        yield return new WaitForSeconds(2.0f);
        nav.enabled = true;
        canAttackTerm = 0.01f;
        isAttacking = false;
    }
    IEnumerator ZombieScream(int audioNum)
    {
        screamCount++;
        audioSource.PlayOneShot(zombieIdleClips[audioNum]);
        yield return null;
    }

    private void ZombieDie()
    {
        GameIDManager.instance.isZombieDie.Add(itemID);

        isDead = true;
        nav.enabled = false;
        zombieCo.radius = 0.001f;

        Destroy(gameObject, 180.0f);
    }
    void CharacterDebuff()
    {
        int duffProbabillity = Random.Range(0, 100);
        if (CharacterManager.instance.debuffType[0] == false &&
           CharacterManager.instance.debuffType[1] == false)
        {
            if (duffProbabillity <= 10) // ����� �߰� 
            {
                int debuffType = Random.Range(0, 2);
                switch (debuffType)
                {
                    case 0:
                        //����
                        CharacterManager.instance.debuffType[0] = true;
                        break;
                    case 1:
                        //����
                        CharacterManager.instance.debuffType[1] = true;
                        break;
                }
            }
        }
        else if (CharacterManager.instance.debuffType[0] == true ||
           CharacterManager.instance.debuffType[1] == true)
        {
            if (duffProbabillity <= 10) // ����� �߰� 
            {
                int debuffType = Random.Range(0, 2);
                switch (debuffType)
                {
                    case 0:
                        //��������
                        CharacterManager.instance.debuffType[2] = true;
                        break;
                    case 1:
                        //ȯ��
                        CharacterManager.instance.debuffType[3] = true;
                        break;
                }
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