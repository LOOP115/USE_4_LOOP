using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {
	public Transform [] metalImpactPrefabs;

    private void OnCollisionEnter (Collision collision) 
	{
		if (collision.transform.tag == "Metal") 
		{
			Instantiate (metalImpactPrefabs [Random.Range(0, metalImpactPrefabs.Length)], transform.position, 
				Quaternion.LookRotation (collision.contacts [0].normal));
			Destroy(gameObject);
		}

		if(collision.transform.tag == "Enemy")
        {
            if (collision.transform.GetComponent<EnemyAI>().enemyHealth>0)
            {
				int damage = PlayerManager.instance.player.transform.GetComponent<PlayerInfo>().GetDamage();

				collision.transform.GetComponent<EnemyAI>().TakeDamage(Random.Range(damage - 2,damage + 2) , transform.position);
				Destroy(gameObject);
            }
		}

		if(collision.transform.tag == "Boss1")
        {
            if (collision.transform.GetComponent<SkeletonAI>().enemyHealth > 0)
            {
                collision.transform.GetComponent<SkeletonAI>().TakeDamage(Random.Range(18, 22), transform.position);
                Destroy(gameObject);
            }
        }
			
		if (collision.transform.tag == "ExplosiveBarrel") 
		{
			collision.transform.gameObject.GetComponent<ExplosiveBarrel>().explode = true;
			Destroy(gameObject);
		}

		StartCoroutine( DestroyTimer());

	}

	private IEnumerator DestroyTimer ()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }

}