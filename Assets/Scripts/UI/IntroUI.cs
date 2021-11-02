using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroUI : MonoBehaviour
{
    private DayNightManager dayNightManager;
    [SerializeField] private Text IntroText;
    [SerializeField] private Text UsingItemText;
    [SerializeField] private GameObject tutorialZombie;
    [SerializeField] private GameObject spawnPoint;

    private int enemyHealth;
    private float time = 0;
    private Transform playerTrans;
    private bool isSpawn = false;
    private GameObject zombie;
    // Start is called before the first frame update
    void Start()
    {
        dayNightManager = GameObject.Find("SceneManagement").GetComponent<DayNightManager>();
        UsingItemText.text = "";
        playerTrans = PlayerManager.instance.player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (zombie != null && time > 30)
        {
            enemyHealth = zombie.GetComponent<EnemyAI>().enemyHealth;
            IntroText.text = "Prepare to fight !  Stay alive !";
            playerTrans.GetComponent<PlayerInfo>().IncreaseHealth(0);
            Destroy(gameObject, 6f);
        }

        dayNightManager.SetTime(14);
        time += Time.deltaTime;

        if(time > 2 && time < 6)
        {
            IntroText.text = "Look around with your mouse";

        }
        else if (time >6 && time < 11)
        {
            IntroText.text = "Move around with W S A D";
        }else if (time > 11 && time < 17)
        {
            IntroText.text = "";
            UsingItemText.text = "Press F to place a barrel. Shoot it!";
        }
        else if (time > 17 && time < 21)
        {
            IntroText.text = "";
            UsingItemText.text = "Press G to throw a Grenade";
        }
        else if (time > 21 && time < 26)
        {
            IntroText.text = "";
            UsingItemText.text = "Press TAB to open shop menu";
        }
        else if(time > 26 && !isSpawn)
        {
            IntroText.text = "Kill the enemy in front of you";
            UsingItemText.text = "You can always press ESC to check Control settings";
            zombie = Instantiate(tutorialZombie, spawnPoint.transform.position, Quaternion.identity);
            isSpawn = true;
        }
    }
}
