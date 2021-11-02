using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public int damage = 30;
    private float time = 0;
    private void Update()
    {
        time -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (time<=0&& other.tag == "Player" && transform.GetComponentInParent<SkeletonAI>().IsAttacking())
        {
            other.transform.GetComponent<PlayerInfo>().TakeDamage(damage);
            time = 0.7f;
        }
    }
}
