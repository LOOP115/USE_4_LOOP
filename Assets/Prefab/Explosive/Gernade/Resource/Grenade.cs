using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {

	private float grenadeTimer = 2.0f;
	private float radius = 5f;
	private float powerRange = 320.0F;
	private float minForce = 5000.0f;
	private float maxForce = 6000.0f;
	private float throwForce;
	[SerializeField] private Transform explosionPrefab;
	[SerializeField] private AudioSource impactSound;

	private void Awake () 
	{
		throwForce = Random.Range(minForce, maxForce);
		float x = Random.Range(500, 2000);
		float y = 0;
		float z = Random.Range(0, 0) * Time.deltaTime * 5000;
        GetComponent<Rigidbody>().AddRelativeTorque(x,y,z);
	}

	private void Start () 
	{
		GetComponent<Rigidbody>().AddForce(gameObject.transform.forward * throwForce);
		StartCoroutine (ExplosionDelay ());
	}

	private void OnCollisionEnter (Collision collision) 
	{
		impactSound.Play ();
	}

	private IEnumerator ExplosionDelay () 
	{
		yield return new WaitForSeconds(grenadeTimer);

		RaycastHit checkGround;
		if (Physics.Raycast(transform.position, Vector3.down, out checkGround, 50))
		{
            Instantiate(explosionPrefab, checkGround.point,Quaternion.FromToRotation (Vector3.forward, checkGround.normal)); 
		}

		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		foreach (Collider hit in colliders) {
			Rigidbody body = hit.GetComponent<Rigidbody> ();

			// Force on colliders
			if (body != null)
				body.AddExplosionForce (powerRange * 5, explosionPos, radius, 3.0F);
			
			if (hit.GetComponent<Collider>().tag == "ExplosiveBarrel") 
			{
				hit.gameObject.GetComponent<ExplosiveBarrel> ().explode = true;
			}

			if (hit.GetComponent<Collider>().tag == "Player") 
			{
				hit.gameObject.GetComponent<PlayerInfo>().TakeDamage(50);
			}

			if (hit.GetComponent<Collider>().tag == "Enemy")
			{
				hit.gameObject.GetComponent<EnemyAI>().TakeDamage(105, hit.gameObject.transform.localPosition );
			}

            if (hit.GetComponent<Collider>().tag == "Boss1")
            {
                hit.gameObject.GetComponent<SkeletonAI>().TakeDamage(105, hit.gameObject.transform.localPosition);
            }

        }

		Destroy (gameObject);
	}
}