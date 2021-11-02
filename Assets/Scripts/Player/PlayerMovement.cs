using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private bool isGround;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float gravity;
    [SerializeField] private LayerMask groundMask;

    private Vector3 moveDirection;
    private Vector3 velocity;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        isGround = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if(isGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        moveDirection = new Vector3(moveX, 0, moveZ);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = runSpeed;
        }else
        {
            moveSpeed = walkSpeed;
        }

        moveDirection *= moveSpeed;

        controller.Move(moveDirection * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity);
    }
}
