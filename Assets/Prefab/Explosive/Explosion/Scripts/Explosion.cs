using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	[Header("Customizations")]
	// Time before the explosion prefab is destroyed
	public float timeDestroy = 10.0f;
	// Time duraion of light flash
	public float lightDuration = 0.02f;
	
	public Light lightFlash;

	public AudioClip[] Sounds;
	public AudioSource audioSource;

    private void Start()
    {
        StartCoroutine(DestroyTimer());
        StartCoroutine(LightFlash());
        // Gemerate a random explosive sound
        audioSource.clip = Sounds[Random.Range(0, Sounds.Length)];
        audioSource.Play();
    }

    private IEnumerator LightFlash()
    {
        lightFlash.GetComponent<Light>().enabled = true;
        yield return new WaitForSeconds(lightDuration);
        lightFlash.GetComponent<Light>().enabled = false;
    }

    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(timeDestroy);
        Destroy(gameObject);
    }
}