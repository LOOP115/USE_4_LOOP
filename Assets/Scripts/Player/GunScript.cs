using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GunScript : MonoBehaviour {

	Animator anim;
	public Camera mainCamera;

	[Header("Camera Options")]
	public float fovSpeed = 15.0f;
	public float defaultFov = 40.0f;
	public float aimFov = 25.0f;

	private Vector3 initialSwayPosition;
	private float lastFired;

	// Weapon settings
	private float fireRate = 11;
	private bool autoReload = true;
	public float autoReloadDelay;
	private bool isReloading;
	private bool isRunning;
	private bool isAiming;
	private bool isInspecting;
	private int currentAmmo;
	private int ammo = 30;
	private bool noAmmo;
    private int numGrenade;
    private int numBarrel;
    private LayerMask groundLayer;
    private bool weaponSway = true;
    private float swayAmount = 0.02f;
    private float maxSwayAmount = 0.06f;
    private float swaySmoothValue = 4.0f;

    // Item Settings 
    private float bulletForce = 400.0f;
	private float showBulletInMagDelay = 0.6f;
	public SkinnedMeshRenderer bulletInMagRenderer;
	private float grenadeSpawnDelay = 0.35f;

    //Lights settings
	private bool randomflash = true;
	private int minRandomValue = 1;

    [Range(2, 25)]
    private int maxRandomValue = 5;
	private int randomflashValue;

	[SerializeField] private ParticleSystem muzzleParticles;
	[SerializeField] ParticleSystem sparkParticles;

    private int minSparkEmmit = 1;
    private int maxSparkEmmit = 7;

	public Light flashLight;
	private float lightDuration = 0.02f;

	[Header("Audio Source")]
	public AudioSource mainAudioSource;
	public AudioSource shootAudioSource;

	[Header("UI Components")]
	public Text currentAmmoText;
	public Text totalAmmoText;
    public Text grenadeText;
    public Text barrelText;
    public GameObject crossHair;

	[Header("Prefabs")]
	[SerializeField] private Transform bulletPrefab;
    [SerializeField] private Transform grenadePrefab;
    [SerializeField] private Transform barrelPrefab;

	[System.Serializable]
	public class spawnpoints
	{
		public Transform bulletSpawnPoint;
		public Transform grenadeSpawnPoint;
	}
	public spawnpoints Spawnpoints;

	[System.Serializable]
	public class soundClips
	{
		public AudioClip shootSound;
		public AudioClip reloadSound1;
		public AudioClip reloadSound2;
		public AudioClip aimSound;
	}
	public soundClips SoundClips;

	private bool soundHasPlayed = false;

	private void Awake() 
    {
		anim = GetComponent<Animator>();
		currentAmmo = ammo;
		flashLight.enabled = false;
	}

	private void Start() {
        numGrenade = 5;
        numBarrel = 5;
        InGameMenu.isPause = false;
		totalAmmoText.text = ammo.ToString();
		initialSwayPosition = transform.localPosition;
		shootAudioSource.clip = SoundClips.shootSound;

        groundLayer = LayerMask.GetMask("Ground");
    }

	private void LateUpdate() {
        if (!InGameMenu.isPause)
        {
            //Weapon sway
            if (weaponSway == true)
            {
                float movementX = -Input.GetAxis("Mouse X") * swayAmount;
                float movementY = -Input.GetAxis("Mouse Y") * swayAmount;
                //Clamp movement to min and max values
                movementX = Mathf.Clamp(movementX, -maxSwayAmount, maxSwayAmount);
                movementY = Mathf.Clamp(movementY, -maxSwayAmount, maxSwayAmount);
                Vector3 finalSwayPosition = new Vector3(movementX, movementY, 0);
                transform.localPosition = Vector3.Lerp(transform.localPosition, finalSwayPosition +
                        initialSwayPosition, Time.deltaTime * swaySmoothValue);
            }
        }
	}
	
	private void Update () {
        if (!InGameMenu.isPause)
        {
            grenadeText.text = "x " + numGrenade;
            barrelText.text = "x " + numBarrel;

            if (Input.GetButton("Fire2") && !isReloading && !isRunning && !isInspecting)
            {

                isAiming = true;
                crossHair.SetActive(false);
                //Start aiming
                anim.SetBool("Aim", true);

                //When right click is released
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView,
                    aimFov, fovSpeed * Time.deltaTime);

                if (!soundHasPlayed)
                {
                    mainAudioSource.clip = SoundClips.aimSound;
                    mainAudioSource.Play();

                    soundHasPlayed = true;
                }
            }
            else
            {
                crossHair.SetActive(true);
                //When right click is released
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView,
                    defaultFov, fovSpeed * Time.deltaTime);

                isAiming = false;
                //Stop aiming
                anim.SetBool("Aim", false);

                soundHasPlayed = false;
            }

            if (randomflash == true)
            {
                randomflashValue = Random.Range(minRandomValue, maxRandomValue);
            }

            currentAmmoText.text = currentAmmo.ToString();
            AnimationCheck();

            //out of ammo
            if (currentAmmo == 0)
            {
                noAmmo = true;
                if (autoReload == true && !isReloading)
                {
                    StartCoroutine(AutoReloadMag());
                }
            }
            else
            {
                noAmmo = false;
            }

            if (Input.GetKeyDown(KeyCode.F) && !isInspecting)
            {
                PlaceBarrel();
            }

            if (Input.GetKeyDown(KeyCode.G) && !isInspecting)
            {
                if (numGrenade > 0)
                {
                    numGrenade--;
                    StartCoroutine(ThrowGrenade());
                    //Play grenade throw animation
                    anim.Play("GrenadeThrow", 0, 0.0f);
                }
                grenadeText.text = "x " + numGrenade;
            }

            if (Input.GetMouseButton(0) && !noAmmo && !isReloading && !isInspecting && !isRunning)
            {
                if (Time.time - lastFired > 1 / fireRate)
                {
                    lastFired = Time.time;
                    currentAmmo -= 1;
                    shootAudioSource.clip = SoundClips.shootSound;
                    shootAudioSource.Play();

                    if (!isAiming)
                    {
                        anim.Play("Fire", 0, 0f);
                        if (!randomflash)
                        {
                            StartCoroutine(MuzzleFlashLight());
                            muzzleParticles.Emit(1);
                        }
                        else if (randomflash == true)
                        {
                            //Only emit if random value is 1
                            if (randomflashValue == 1)
                            {
                                sparkParticles.Emit(Random.Range(minSparkEmmit, maxSparkEmmit));
                                StartCoroutine(MuzzleFlashLight());
                                muzzleParticles.Emit(1);
                            }
                        }
                    }
                    else
                    {
                        anim.Play("Aim Fire", 0, 0f);
                        if (!randomflash)
                            muzzleParticles.Emit(1);
                        else if (randomflash == true)
                        {
                            if (randomflashValue == 1)
                            {
                                sparkParticles.Emit(Random.Range(minSparkEmmit, maxSparkEmmit));
                                muzzleParticles.Emit(1);
                                StartCoroutine(MuzzleFlashLight());
                            }
                        }
                    }

                    Transform bSpawnPointTrans = Spawnpoints.bulletSpawnPoint.transform;
                    Transform bullet = (Transform)Instantiate(bulletPrefab,bSpawnPointTrans.transform.position,bSpawnPointTrans.rotation);
                    bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * bulletForce;
                }
            }

            //Inspect weapon when T key is pressed
            if (Input.GetKeyDown(KeyCode.T))
            {
                anim.SetTrigger("Inspect");
            }

            //Reload 
            if (Input.GetKeyDown(KeyCode.R) && !isReloading && !isInspecting)
            {
                //Reload
                ReloadMag();
            }

            //Walking when pressing down WASD keys
            if (Input.GetKey(KeyCode.W) && !isRunning ||
                Input.GetKey(KeyCode.A) && !isRunning ||
                Input.GetKey(KeyCode.S) && !isRunning ||
                Input.GetKey(KeyCode.D) && !isRunning)
            {
                anim.SetBool("Walk", true);
            }
            else
            {
                anim.SetBool("Walk", false);
            }

            //Running when pressing down W and Left Shift key
            if ((Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift)))
            {
                isRunning = true;
            }
            else
            {
                isRunning = false;
            }

            //Run anim toggle
            if (isRunning == true)
            {
                anim.SetBool("Run", true);
            }
            else
            {
                anim.SetBool("Run", false);
            }
        }
		
	}

    private void PlaceBarrel()
    {
        if(numBarrel > 0)
        {
            Vector3 spawnPoint = Spawnpoints.bulletSpawnPoint.transform.position;
            RaycastHit hit = new RaycastHit();
            groundLayer = LayerMask.GetMask("Ground");
            Ray ray = new Ray(spawnPoint, transform.forward);
            if (Physics.Raycast(ray, out hit, 10, groundLayer))
            {
                Instantiate(barrelPrefab, hit.point + new Vector3(0, 0.5f, 0), Quaternion.identity);
                numBarrel--;
            }
        }

    }

    private IEnumerator ThrowGrenade () {
		yield return new WaitForSeconds (grenadeSpawnDelay);
		Instantiate(grenadePrefab, 
			Spawnpoints.grenadeSpawnPoint.transform.position, 
			Spawnpoints.grenadeSpawnPoint.transform.rotation);
	}

	private IEnumerator AutoReloadMag () {
		yield return new WaitForSeconds (autoReloadDelay);
		if (noAmmo == true) 
		{
			anim.Play ("Reload Out Of Ammo", 0, 0f);
			mainAudioSource.clip = SoundClips.reloadSound1;
			mainAudioSource.Play ();
			if (bulletInMagRenderer != null) 
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer> ().enabled = false;
				StartCoroutine (ShowBulletInMag ());
			}
		} 
		currentAmmo = ammo;
		noAmmo = false;
	}

	private void ReloadMag () {
		
		if (noAmmo == true) 
		{
			anim.Play ("Reload Out Of Ammo", 0, 0f);

			mainAudioSource.clip = SoundClips.reloadSound1;
			mainAudioSource.Play ();

			if (bulletInMagRenderer != null) 
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer> ().enabled = false;
				StartCoroutine (ShowBulletInMag ());
			}
		} 
		else 
		{
			anim.Play ("Reload Ammo Left", 0, 0f);

			mainAudioSource.clip = SoundClips.reloadSound2;
			mainAudioSource.Play ();

			if (bulletInMagRenderer != null) 
			{
				bulletInMagRenderer.GetComponent
				<SkinnedMeshRenderer> ().enabled = true;
			}
		}
		currentAmmo = ammo;
		noAmmo = false;
	}

	private IEnumerator ShowBulletInMag () {
		
		//Wait set amount of time before showing bullet in mag
		yield return new WaitForSeconds (showBulletInMagDelay);
		bulletInMagRenderer.GetComponent<SkinnedMeshRenderer> ().enabled = true;
	}

	private IEnumerator MuzzleFlashLight () {
		
		flashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		flashLight.enabled = false;
	}

	private void AnimationCheck () {
		
		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Out Of Ammo") || 
			anim.GetCurrentAnimatorStateInfo (0).IsName ("Reload Ammo Left")) 
		{
			isReloading = true;
		} 
		else 
		{
			isReloading = false;
		}

		if (anim.GetCurrentAnimatorStateInfo (0).IsName ("Inspect")) 
		{
			isInspecting = true;
		} 
		else 
		{
			isInspecting = false;
		}
	}

    public void IncreaseAmmo(int value)
    {
        ammo += value;
	    totalAmmoText.text = ammo.ToString();

    }
    public void IncreaseFireRate(float value)
    {
        fireRate += value;
    }
    public void IncreaseGrenade(int value)
    {
        numGrenade += value;
        grenadeText.text = "x " + numGrenade;
    }
    public void IncreaseBarrel(int value)
    {
        numBarrel += value;
        barrelText.text = "x " + numBarrel;
    }

}