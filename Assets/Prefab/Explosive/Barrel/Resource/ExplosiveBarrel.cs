using UnityEngine;
using System.Collections;

public class ExplosiveBarrel : MonoBehaviour {

	private float prepareTime;
	private bool routineStarted = false;
	public bool explode = false;

	[SerializeField] private Transform explosionPrefab;
	[SerializeField] private Transform barrelCrackPrefab;

	// Time before the barrel explodes
	public float minTime = 0.05f;
	public float maxTime = 0.25f;

	public float explosionRadius = 12.5f;
	public float explosionForce = 4000.0f;
	
	private void Update () {
		prepareTime = Random.Range (minTime, maxTime);

		if (explode == true) 
		{
			if (routineStarted == false) 
			{
				StartCoroutine(Explode());
				routineStarted = true;
			} 
		}
	}
	
	private IEnumerator Explode () {
		yield return new WaitForSeconds(prepareTime);

		Instantiate (barrelCrackPrefab, transform.position, transform.rotation); 

		// Explosion force
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
		foreach (Collider hit in colliders) {
			Rigidbody body = hit.GetComponent<Rigidbody> ();
			
			if (body != null)
				body.AddExplosionForce (explosionForce * 10, explosionPos, explosionRadius);

			if (hit.transform.tag == "ExplosiveBarrel") 
			{
				hit.transform.gameObject.GetComponent<ExplosiveBarrel>().explode = true;
			}

			if (hit.GetComponent<Collider>().tag == "Player")
			{
				hit.gameObject.GetComponent<PlayerInfo>().TakeDamage(50);
			}


			if (hit.GetComponent<Collider>().tag == "Enemy")
			{
				hit.gameObject.GetComponent<EnemyAI>().TakeDamage(105, hit.gameObject.transform.localPosition);
			}

            if (hit.GetComponent<Collider>().tag == "Boss1")
            {
                hit.gameObject.GetComponent<SkeletonAI>().TakeDamage(105, hit.gameObject.transform.localPosition);
            }
        }

		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
			Instantiate (explosionPrefab, checkGround.point, Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		Destroy (gameObject);
	}
}