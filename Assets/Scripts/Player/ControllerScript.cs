using System;
using System.Linq;
using UnityEngine;

public class ControllerScript : MonoBehaviour
{
    [SerializeField] private Transform arms;
    [SerializeField] private Vector3 armPosition;

    [SerializeField] private float walkingSpeed = 4f;
    [SerializeField] private float runningSpeed = 6f;
    [SerializeField] private float movementSmoothness = 0.125f;
    [SerializeField] private float jumpForce = 35f;

    [SerializeField] private float mouseSensitivity = 7f;
    [SerializeField] private float rotationSmoothness = 0.05f;
    [SerializeField] private float minVerticalAngle = -90f;
    [SerializeField] private float maxVerticalAngle = 90f;
    [SerializeField] private FpsInput input;

    [SerializeField] private AudioSource wakingAudio;

    private Rigidbody playerRigidBody;
    private CapsuleCollider playerCollider;
    private SmoothRotation rotation_X;
    private SmoothRotation rotation_Y;
    private SmoothVelocity velocity_X;
    private SmoothVelocity velocity_Z;
    private bool playerIsOnGround;
    private bool cameraCanRotate = true;
    private bool soundIsPlaying = false;

    private RaycastHit[] groundRaycasts = new RaycastHit[8];
    private RaycastHit[] wallRaycasts = new RaycastHit[8];

    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        playerCollider = GetComponent<CapsuleCollider>();
        arms.SetPositionAndRotation(transform.position, transform.rotation);
        rotation_X = new SmoothRotation(RotationXRaw);
        rotation_Y = new SmoothRotation(RotationYRaw);
        velocity_X = new SmoothVelocity();
        velocity_Z = new SmoothVelocity();
        Cursor.lockState = CursorLockMode.Locked;
        CheckRotationBlock();
    }

    private void CheckRotationBlock()
    {
        minVerticalAngle = ClampRotationBlock(minVerticalAngle, -90, 90);
        maxVerticalAngle = ClampRotationBlock(maxVerticalAngle, -90, 90);
        if (maxVerticalAngle >= minVerticalAngle) return;
        var min = minVerticalAngle;
        minVerticalAngle = maxVerticalAngle;
        maxVerticalAngle = min;
    }

    private static float ClampRotationBlock(float rotationRestriction, float min, float max)
    {
        if (rotationRestriction >= min && rotationRestriction <= max) return rotationRestriction;
        var message = string.Format("Rotation restrictions should be between {0} and {1} degrees.", min, max);
        return Mathf.Clamp(rotationRestriction, min, max);
    }

    /// Checks if the character is on the ground.
    private void OnCollisionStay()
    {
        var bounds = playerCollider.bounds;
        var extents = bounds.extents;
        var radius = extents.x - 0.01f;
        Physics.SphereCastNonAlloc(bounds.center, radius, Vector3.down,
            groundRaycasts, extents.y - radius * 0.5f, ~0, QueryTriggerInteraction.Ignore);
        if (!groundRaycasts.Any(hit => hit.collider != null && hit.collider != playerCollider)) return;
        for (var i = 0; i < groundRaycasts.Length; i++)
        {
            groundRaycasts[i] = new RaycastHit();
        }

        playerIsOnGround = true;
    }

    private void FixedUpdate()
    {
        if (cameraCanRotate && !InGameMenu.isPause)
        {
            CameraRotate();
            Moving();
            playerIsOnGround = false;
        }
    }

    private void Update()
    {
        if (!InGameMenu.isPause)
        {
            arms.position = transform.position + transform.TransformVector(armPosition);
            Jump();
            PlaySound();
        }
        else
        {
            wakingAudio.Stop();
            soundIsPlaying = false;
        }
    }

    public void SetCameraRoation(bool boolean)
    {
        cameraCanRotate = boolean;
    }

    private void CameraRotate()
    {
	    var rotationX = rotation_X.Update(RotationXRaw, rotationSmoothness);
	    var rotationY = rotation_Y.Update(RotationYRaw, rotationSmoothness);
        var clampedY = BlockVRotation(rotationY);
	    rotation_Y.Current = clampedY;
	    var worldUp = arms.InverseTransformDirection(Vector3.up);
	    var rotation = arms.rotation * Quaternion.AngleAxis(rotationX, worldUp) *
	                       Quaternion.AngleAxis(clampedY, Vector3.left);
	    transform.eulerAngles = new Vector3(0f, rotation.eulerAngles.y, 0f);
	    arms.rotation = rotation;
    }

    /// Returns the target rotatiWWon of the camera around the y axis with no smoothing.
    private float RotationXRaw
    {
        get { return input.RotateX * mouseSensitivity; }
    }

    /// Returns the target rotation of the camera around the x axis with no smoothing.
    private float RotationYRaw
    {
        get { return input.RotateY * mouseSensitivity; }
    }

    private float BlockVRotation(float mouseY)
    {
        float currentAngle = NormalizeAngle(arms.eulerAngles.x);
        float minY = minVerticalAngle + currentAngle;
        float maxY = maxVerticalAngle + currentAngle;
        return Mathf.Clamp(mouseY, minY + 0.01f, maxY - 0.01f);
    }

    private static float NormalizeAngle(float angleDegrees)
    {
        while (angleDegrees > 180f)
        {
            angleDegrees -= 360f;
        }

        while (angleDegrees <= -180f)
        {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }

    private void Moving()
    {
        Vector3 direction = new Vector3(input.Move, 0f, input.Strafe).normalized;
        Vector3 worldDirection = transform.TransformDirection(direction);
        Vector3 velocity = worldDirection * (input.Run ? runningSpeed : walkingSpeed);
        bool intersectsWall = CheckCollisionsWithWalls(velocity);
        if (intersectsWall)
        {
            velocity_X.Current = velocity_Z.Current = 0f;
            return;
        }
        float smoothZ = velocity_Z.Update(velocity.z, movementSmoothness);
        float smoothX = velocity_X.Update(velocity.x, movementSmoothness);
        Vector3 rigidbodyVelocity = playerRigidBody.velocity;
        Vector3 force = new Vector3(smoothX - rigidbodyVelocity.x, 0f, smoothZ - rigidbodyVelocity.z);
        playerRigidBody.AddForce(force, ForceMode.VelocityChange);
    }

    private bool CheckCollisionsWithWalls(Vector3 velocity)
    {
        if (playerIsOnGround) return false;
        Bounds bounds = playerCollider.bounds;
        float colliderRadius = playerCollider.radius;
        float offsetHight = playerCollider.height * 0.5f - colliderRadius * 1.0f;
        Vector3 point1 = bounds.center;
        Vector3 point2 = bounds.center;
        point1.y += offsetHight;
        point2.y -= offsetHight;
        Physics.CapsuleCastNonAlloc(point1, point2, colliderRadius, velocity.normalized, wallRaycasts,
            colliderRadius * 0.04f, ~0, QueryTriggerInteraction.Ignore);
        bool isCollide = wallRaycasts.Any(hit => hit.collider != null && hit.collider != playerCollider);
        if (!isCollide) return false;
        for (int i = 0; i < wallRaycasts.Length; i++)
        {
            wallRaycasts[i] = new RaycastHit();
        }

        return true;
    }

    private void PlaySound()
    {
        if (playerIsOnGround && transform.GetComponent<Rigidbody>().velocity.magnitude > 0.1f && !soundIsPlaying)
        {
            wakingAudio.Play();
            soundIsPlaying = true;
        }

        if(!playerIsOnGround || transform.GetComponent<Rigidbody>().velocity.magnitude <= 0.1f || transform.GetComponent<PlayerInfo>().playerHealth <= 0)
        {
            wakingAudio.Stop();
            soundIsPlaying = false;
        }
    }

    private void Jump()
    {
        if (playerIsOnGround && input.Jump)
        {
            playerIsOnGround = false;
            playerRigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void IncreaseSpeed(float value)
    {
        walkingSpeed += value;
        runningSpeed += value;
    }

    private class SmoothRotation
    {
        private float currentAngle;
        private float currentVelocity;

        public SmoothRotation(float startAngle)
        {
            currentAngle = startAngle;
        }

        public float Update(float target, float smoothingTime)
        {
            return currentAngle = Mathf.SmoothDampAngle(currentAngle, target, ref currentVelocity, smoothingTime);
        }

        public float Current
        {
            set { currentAngle = value; }
        }
    }

    private class SmoothVelocity
    {
        private float currentAngle;
        private float currentVelocity;

        public float Update(float target, float smoothTime)
        {
            return currentAngle = Mathf.SmoothDamp(currentAngle, target, ref currentVelocity, smoothTime);
        }

        public float Current
        {
            set { currentAngle = value; }
        }
    }

    [Serializable]
    private class FpsInput
    {
        [SerializeField] private string rotateX = "Mouse X";
        [SerializeField] private string rotateY = "Mouse Y";
        [SerializeField] private string move = "Horizontal";
        [SerializeField] private string strafe = "Vertical";
        [SerializeField] private string run = "Fire3";
        [SerializeField] private string jump = "Jump";

        public float Move
        {
            get { return Input.GetAxisRaw(move); }
        }

        public bool Jump
        {
            get { return Input.GetButtonDown(jump); }
        }

        public float RotateY
        {
            get { return Input.GetAxisRaw(rotateY); }
        }
        public float Strafe
        {
            get { return Input.GetAxisRaw(strafe); }
        }

        public bool Run
        {
            get { return Input.GetButton(run); }
        }

        public float RotateX
        {
            get { return Input.GetAxisRaw(rotateX); }
        }
    }
}