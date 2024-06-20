using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DeafZombieController : MonoBehaviour
{
    public float zombieHp = 100.0f;
    public float zombieDamage;
    public bool longRange;

    Animator _zombieAnimator;
    [SerializeField] GameObject zombieRef;
    private Vector3 startPos;
    public float respawnTime;

    public Transform targetTr;
    //시야 check 범위
    public float range = 10.0f;
    public float attackRange = 0.3f;

    bool isChecked = false;
    public bool isDead = false;
    bool isRolling = false;

    float inRangeTime;
    float outRangeTime;
    float exploseTime = 5.0f;
    float exploseDamage = 10.0f;

    [SerializeField] GameObject exploseVfx;

    CapsuleCollider zombieCo;
    [SerializeField] AudioClip[] zombieIdleClips;
    AudioSource audioSource;
    int playSfxCount = 0;

    //Nav Mesh
    protected NavMeshAgent nav;
    protected Vector3 destination;
    //UI

    // Start is called before the first frame update
    private void Awake()
    {
        //Prefab으로 할 경우 캐릭터 찾아오기 위함
        targetTr = GameObject.Find("Geometry").transform;
        startPos = transform.position;
    }

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        _zombieAnimator = GetComponent<Animator>();
        zombieCo = GetComponent<CapsuleCollider>();
        nav.speed = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        ZombieControl();

        if (zombieHp <= 0.0f && isDead == false)
        {
            _zombieAnimator.SetBool("isDead", true);
            ZombieDie();
        }

    }
    IEnumerator PlaySfx(int clipNum)
    {
        audioSource.PlayOneShot(zombieIdleClips[clipNum]);
        playSfxCount++;
        yield return null;
    }
    IEnumerator Respawn(float respawnTime)
    {
        yield return respawnTime;
        GameObject zombieClone = (GameObject)Instantiate(zombieRef);
        DeafZombieController zc = zombieClone.GetComponent<DeafZombieController>();
        zc.zombieHp = 100.0f;
        zombieClone.transform.position = startPos;
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
    bool playerChecked;
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
                if (playSfxCount == 0) StartCoroutine(PlaySfx(0));
            }
            else playerChecked = false;
        }
        IsPlayerInRange();
    }
    IEnumerator Roll()
    {
        isRolling = true;
        _zombieAnimator.SetTrigger("Roll");

        yield return new WaitForSeconds(0.9f);
        StartCoroutine(Explose());

        //StartCoroutine(Respawn(respawnTime));
    }
    IEnumerator Explose()
    {
        Instantiate(exploseVfx, transform.position, Quaternion.identity);

        Vector3 zombieRay = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);

        if (Physics.Raycast(zombieRay, targetRay - zombieRay, out zombieHitInfo, 2.0f))
        {
            if (zombieHitInfo.transform.name == "Player")
            {
                CharacterManager.instance.characterHp -= exploseDamage;
            }
        }


        yield return new WaitForSeconds(0.05f);
        StartCoroutine(PlaySfx(1));
        Destroy(this.gameObject);
        yield return null;
    }

    void ZombieControl()
    {
        float distance = Vector3.Distance(transform.position, targetTr.position);
        float soundDistance = Vector3.Distance(transform.position, ThirdPersonShooterController.soundPos);
        Vector3 zombieRay = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
        Vector3 targetRay = new Vector3(targetTr.transform.position.x,
            targetTr.transform.position.y + 1.0f, targetTr.transform.position.z);
        if (Physics.Raycast(zombieRay, targetRay - zombieRay, out zombieHitInfo, 5.0f))
        {
            if (zombieHitInfo.transform.name == "Player" && !isRolling)
            {
                StartCoroutine(Roll());
            }
        }
        if (isDead == false)
        {
            if (distance <= range)
            {
                CheckPlayer();
                if (playerChecked)
                {
                    isChecked = true;

                    nav.SetDestination(targetTr.position);
                    _zombieAnimator.SetBool("isRunning", true);
                }
            }
            if (isChecked == true)
            {
                CheckPlayer();
                exploseTime -= Time.deltaTime;
                nav.SetDestination(targetTr.position);
            }
            else exploseTime = 5.0f;
            if (exploseTime <= 0.0f) StartCoroutine(Explose());

        }
    }
    private void ZombieDie()
    {
        exploseTime = 5.0f;
        nav.enabled = false;
        zombieCo.radius = 0.001f;
        isDead = true;

        StartCoroutine(Explose());
        //StartCoroutine(Respawn(respawnTime));
    }
    void OnTriggerEnter(Collider co)
    {
        if (co.gameObject.tag == "Player")
        {
            StartCoroutine(Explose());
        }
    }
}
