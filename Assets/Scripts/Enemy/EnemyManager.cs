using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    [SerializeField] private GameObject smallZombiePrefebs;
    [SerializeField] private GameObject skeletonPrefebs;
    //[SerializeField] private float spawnTime = 10f;

    [SerializeField] private AudioSource audioSource;
    private bool isPlaying = false;
    public static int totalNumberZombieAlive = 0;
    public static LayerMask groundLayer;
    public static LayerMask treeLayer;

    private int skeletonToSpawn = 1;
    private int skeletonSpawned = 0;
    private DayNightManager dayNightManager;
    private Transform playerTrans;
    private float time = 0;
    private int numberZombieSpawned = 0;
    private int numberOfZombiesToSpawn;
    private int day = 0;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        treeLayer= LayerMask.GetMask("Tree");
    }

    void Start()
    {
        audioSource.Play();
        isPlaying = true;
        dayNightManager = GameObject.Find("SceneManagement").GetComponent<DayNightManager>();
        if (dayNightManager == null)
        {
            Debug.LogError("Cannot find dayNightManager");
        }
        numberOfZombiesToSpawn = 20;
        playerTrans = PlayerManager.instance.player.transform;
    }

    private void Update()
    {
        //Spawn around player
        //transform.localPosition = playerTrans.localPosition;
        time -= Time.deltaTime;
        if (totalNumberZombieAlive > 0 && !isPlaying)
        {
            isPlaying = true;
            audioSource.Play();
        }
        if(totalNumberZombieAlive <= 0 && isPlaying)
        {
            audioSource.Stop();
            isPlaying = false;
        }
        float currentTime = dayNightManager.GetCurrentTime();

        if (dayNightManager.GetDay() != day && currentTime >= 18)
        {
            if(day < 1)
            {
                if (numberZombieSpawned != numberOfZombiesToSpawn)
                {
                    SpawnOneEnemy(0);
                }
            }else
            {
                if (numberZombieSpawned != numberOfZombiesToSpawn)
                {
                    SpawnOneEnemy(0);
                }
                if (skeletonSpawned != skeletonToSpawn)
                {
                    SpawnOneEnemy(1);
                }
            }
        }

        if (numberZombieSpawned >= numberOfZombiesToSpawn)
        {
            numberZombieSpawned = 0;
            skeletonSpawned = 0;
            if (day < 2)
            {
                numberOfZombiesToSpawn += 50;
            }
            else if (day < 5)
            {
                numberOfZombiesToSpawn += 20;
                skeletonToSpawn += 1;
            }else
            {
                skeletonToSpawn += 4;
                numberOfZombiesToSpawn = 100;
            }
            day++;
        }
    }

    private void SpawnOneEnemy(int type)
    {
        int randomNumber = Mathf.RoundToInt(Random.Range(0f, spawnPoints.Length - 1));
        Vector3 spawnPosition = GetGroundPostion(spawnPoints[randomNumber].transform.position);
        if (spawnPosition != new Vector3(-1, -1, -1))
        {
            if(type == 0)
            {
                Instantiate(smallZombiePrefebs, spawnPosition + new Vector3(0, 0.1f, 0), Quaternion.identity);
                numberZombieSpawned++;
                totalNumberZombieAlive++;
            }
            else if (type == 1)
            {
                Instantiate(skeletonPrefebs, spawnPosition + new Vector3(0, 0.1f, 0), Quaternion.identity);
                skeletonSpawned ++;
            }
        }    
    }
    public static Vector3 GetGroundPostion(Vector3 position)
    {
        position.y = 100;
        RaycastHit hit = new RaycastHit();
        Ray ray = new Ray(position, Vector3.down);
        if (Physics.Raycast(ray, out hit, 10, treeLayer))
        {
            return new Vector3(-1, -1, -1);
        }
        if (Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            return hit.point;
        }
        return new Vector3(-1,-1,-1);
    }

}
