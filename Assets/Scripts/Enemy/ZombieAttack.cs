using UnityEngine;

public class ZombieAttack : MonoBehaviour
{
    public int damage = 10;
    private float time = 0;
    private void Update()
    {
        time -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (time<0 &&other.tag == "Player" && transform.GetComponentInParent<EnemyAI>().IsAttacking())
        {
            other.transform.GetComponent<PlayerInfo>().TakeDamage(damage);
            time = 0.7f;
        }
    }
}
