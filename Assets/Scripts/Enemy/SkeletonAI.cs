using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonAI : MonoBehaviour
{
    public HealthBar healthBar;
    public int maxHealth;
    public int enemyHealth;

    private Transform playerTransform;
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    bool isAttack = false;
    bool isDead = false;

    private float attackCoolDown = 0;
    [SerializeField] private GameObject bar;
    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private GameObject criticleDamageTextPrefab;
    [SerializeField] private GameObject bloodPrefab;
    [SerializeField] private GameObject explosionBloodPrefab;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject deadExplosionPrefab;
    [SerializeField] private float armor;
    [SerializeField] private float speed;
    [SerializeField] private float attckRange;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float attackAnimSpeed;

    [System.Serializable]
    public class SoundClips
    {
        public AudioClip attackSound;
        public AudioClip takeDamageSound1;
        public AudioClip takeDamageSound2;
        public AudioClip dieSound;
        public AudioClip chasingSound1;
        public AudioClip chasingSound2;
    }
    [SerializeField] private SoundClips soundClips;
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    [SerializeField] private AudioSource chasingSource;

    private float canPlayChasingSound = 0;
    void Start()
    {

        enemyHealth = maxHealth;
        healthBar.SetMaxHealth(enemyHealth);
        healthBar.SetHealth(enemyHealth);
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = 999;
        playerTransform = PlayerManager.instance.player.transform;

        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (canPlayChasingSound <= 0)
        {
            float random = Random.Range(0,2);
            Debug.Log(random);
            if (random >= 1f)
            {
                chasingSource.clip = soundClips.chasingSound1;
            }
            else
            {
                chasingSource.clip = soundClips.chasingSound2;
            }

            if (enemyHealth < 100)
            {
                chasingSource.clip = soundClips.dieSound;
            }
            chasingSource.Play();
            canPlayChasingSound = 8f;
        }
        canPlayChasingSound -= Time.deltaTime;

        if (enemyHealth <= 0 && !isDead)
        {
            isDead = true;
            StopMoving();
            StartCoroutine(Dead());
        }

        if (enemyHealth > 0)
        {
            attackCoolDown -= Time.deltaTime;
            if (distance <= attckRange && attackCoolDown <= 0 && isAttack == false)
            {
                StopMoving();
                StartCoroutine(Attack());
                attackCoolDown = 3f / attackSpeed;
            }

            if (!isAttack)
            {
                Walk();
            }
        }
    }

    private void StopMoving()
    {
        navMeshAgent.speed = 0f;
        navMeshAgent.acceleration = 0f;
        navMeshAgent.velocity = new Vector3(0, 0, 0);
    }

    private void StartMoving()
    {
        navMeshAgent.speed = speed;
        navMeshAgent.acceleration = 999f;
    }

    public void TakeDamage(int damage, Vector3 position)
    {
        float randomNumber = Random.Range(0f, 2f);
        if (randomNumber >= 1)
        {
            audioSource1.clip = soundClips.takeDamageSound1;
        }
        else
        {
            audioSource1.clip = soundClips.takeDamageSound2;
        }
        audioSource1.Play();
        bar.SetActive(true);
        damage = Mathf.RoundToInt((damage + Random.Range(-5, 5)) * (1f - armor));
        enemyHealth -= damage;
        if (damage > 50)
        {
            Instantiate(criticleDamageTextPrefab, transform.position, Quaternion.identity).GetComponent<CriticleText>().Initialise(damage);
            if (enemyHealth <= 0)
                Instantiate(explosionBloodPrefab, position - new Vector3(0, 0.5f, 0), Quaternion.identity);
            else
                Instantiate(bloodPrefab, position, Quaternion.identity);
        }
        else
        {
            Instantiate(damageTextPrefab, transform.position, Quaternion.identity).GetComponent<DamageText>().Initialise(damage);
            Instantiate(bloodPrefab, position, Quaternion.identity);
        }
        healthBar.SetHealth(enemyHealth);

    }

    private void Walk()
    {
        StartMoving();
        navMeshAgent.SetDestination(playerTransform.position);
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }

    private IEnumerator Attack()
    {
        audioSource1.clip = soundClips.attackSound;
        audioSource1.Play();
        isAttack = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackAnimSpeed);
        isAttack = false;
    }

    private IEnumerator Dead()
    {
        audioSource2.Play();
        playerTransform.GetComponent<PlayerInfo>().AddScore(100);
        EnemyManager.totalNumberZombieAlive--;
        transform.GetComponent<BoxCollider>().enabled = false;
        transform.GetComponent<Rigidbody>().isKinematic = true;
        for (int i=0; i<=15; i++)
        {
            float randomNumber = Random.Range(-2f, 2f);
            float randomNumber2 = Random.Range(-2f, 2f);
            Vector3 location = EnemyManager.GetGroundPostion(transform.localPosition + new Vector3(randomNumber, 10f, randomNumber2));
            Instantiate(coinPrefab, location+new Vector3(0, 0.3f, 0), Quaternion.identity);
        }
        Instantiate(deadExplosionPrefab, transform.localPosition + new Vector3(0, 0.3f, 0), Quaternion.identity);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    public bool IsAttacking()
    {
        return isAttack;
    }
}
