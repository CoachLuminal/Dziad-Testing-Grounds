using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public enum PlayerState
{
       Idle,
    Walking,
    Running,
    Jumping,
    Sliding,
    Falling
}

public class PlayerController : MonoBehaviour
{
    private float gravityValue = -9.81f;

    [Header("Player Starting Stats")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int maxStamina;
    [SerializeField] private int maxJumpHeight = 5;
    [SerializeField] private float groundCheckRadius = 0.52f;
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 3f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float airAcceleration = 1f;
    [SerializeField] private float mouseSensivity = 1f;
    [SerializeField] private float maxDeathCameraDistance = 10f;
    [SerializeField] private float minDeathCameraDistance = 1f;
    [SerializeField] private float deathCameraRadius = 0.3f;
    [SerializeField] private float deathCameraHeightOffset = 2f;


    [Header("Player Current Stats")]
    [SerializeField] public int currentHealth = 100;
    [SerializeField] public int currentStamina = 100;

    [Header("Player Movement Stats")]
    [SerializeField] public float movementHorizontal;
    [SerializeField] public float movementVertical;
    [SerializeField] public float cameraViewMouseX;
    [SerializeField] public float cameraViewMouseY;
    [SerializeField] private string currentWalkState;
    [SerializeField] private string currentPlayerState;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentAcceleration;

    [Header("Camera Settings")]
    [SerializeField] private float cameraSmoothVelocity;
    [SerializeField] private float cameraTargetY;

    [Header("Boole skurwole")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private bool isWalking = false;
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private bool isSliding = false;
    [SerializeField] private bool isFalling = false;
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private bool isInAir = false;
    [SerializeField] private bool wasGroundTouched = false;


    private CharacterController playerController;
    private SphereCollider playerGroundCheck;
    private Camera playerCamera;
    private Camera deathCamera;

    private Vector3 moveVector;
    private Vector2 cameraRotation;
    private Rigidbody playerRagdoll;
    private GameObject deathCameraPivot;
    private PlayerInventory playerInventory;




    private void Awake()
    {
        QualitySettings.vSyncCount = 1;
    }

    void Start()
    {
        maxHealth = currentHealth;
        maxStamina = currentStamina;
        


        playerController = GetComponent<CharacterController>();
        playerGroundCheck = GetComponentInChildren<SphereCollider>();
        playerCamera = GetComponentInChildren<Camera>();
        deathCameraPivot = transform.Find("DeathCameraPivot").gameObject;
        deathCamera = deathCameraPivot.transform.Find("Death Camera").GetComponent<Camera>();

    }

    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;

        CurrentPlayerState();

        HandleCameraLook();

        if (!isDead)
        {
            ApplyGravity();
            HandleJumping();
            HandleGrounded();
            HandleMovement();
            HandleCrouching();

        }

            DebugShit();
    }

    void CurrentPlayerState()
    {
        if (currentHealth <= 0)
        {
            isDead = true;
            isWalking = false;
            isCrouching = false;
            isRunning = false;
            isGrounded = false;
            isInAir = false;
            isJumping = false;  
            isFalling = false; 


            currentPlayerState = "Dead";

            deathCamera.gameObject.SetActive(true);
            playerCamera.gameObject.SetActive(false);

            if (playerRagdoll == null)
            {
                Destroy(playerController);
                deathCameraPivot.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                playerRagdoll = gameObject.AddComponent<Rigidbody>();
                playerRagdoll.mass = 15f;
            }

            Debug.Log("Player is dead");
 
        } else
        {
            currentPlayerState = "Alive";

            currentAcceleration = acceleration;

            currentWalkState = "Idle";

            if (isWalking)
            {
                targetSpeed = moveSpeed;
                currentWalkState = "Walking";

            }
            else
            {
                targetSpeed = 0f;
                currentWalkState = "Idle";
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                targetSpeed = sprintSpeed;
                currentWalkState = "Sprint";
                isRunning = true;
            }
            else
            {
                isWalking = true;
                isRunning = false;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                targetSpeed = crouchSpeed;
                currentWalkState = "Crouch";
                isCrouching = true;
                
            }
            else
            {
                isWalking = true;
                isCrouching = false;
            }

            if (isInAir)
            {
                targetSpeed = moveSpeed;
                currentWalkState = "In Air";
                currentAcceleration = airAcceleration;
            }

            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, currentAcceleration * Time.deltaTime);
        }

    }

    void HandleGrounded()
    {

        isGrounded = Physics.CheckSphere(playerGroundCheck.transform.position, groundCheckRadius, LayerMask.GetMask("Terrain"));

        if (isGrounded)
        {
            if (!wasGroundTouched)
            {
                moveVector.y = 0f;
            }
            isInAir = false;
            wasGroundTouched = true;
            Debug.Log("Grounded: True");

        } else
        {
            isInAir = true;
            wasGroundTouched = false;
        }
    }

    void HandleJumping ()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isJumping)
        {
            isJumping = true;

            if (isJumping)
            {
                moveVector.y += maxJumpHeight;
                Debug.Log("Player Jumped");
            }
        }

        if (moveVector.y < 0)
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }

        if (wasGroundTouched)
        {
            isJumping = false;
            isFalling = false;
            Debug.Log("Player landed");
        }
        
    }

    void HandleMovement()
    {
        movementHorizontal = Input.GetAxis("Horizontal");
        movementVertical = Input.GetAxis("Vertical");

        Vector3 moveCharacterVector = new Vector3(movementHorizontal, 0, movementVertical);

        moveCharacterVector = Vector3.ClampMagnitude(moveCharacterVector, 1f);

        Vector3 forward = new Vector3(playerCamera.transform.forward.x, 0f, playerCamera.transform.forward.z).normalized;
        Vector3 right = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        Vector3 moveDirectionVector = forward * moveCharacterVector.z + right * moveCharacterVector.x;


        moveVector.x = moveDirectionVector.x * currentSpeed;
        moveVector.z = moveDirectionVector.z * currentSpeed;
    }

    void HandleCameraLook()
    {
        if(!isDead)
        {

            cameraViewMouseX = Input.GetAxis("Mouse X");
            cameraViewMouseY = Input.GetAxis("Mouse Y");

            cameraRotation.x += cameraViewMouseX * mouseSensivity;
            cameraRotation.y += cameraViewMouseY * mouseSensivity;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -89f, 89f);

            playerCamera.transform.localRotation = Quaternion.Euler(-cameraRotation.y, 0f, 0f);

            transform.rotation = Quaternion.Euler(0f, cameraRotation.x, 0f);

        }
        else
        {
            cameraViewMouseX = Input.GetAxis("Mouse X");
            cameraViewMouseY = Input.GetAxis("Mouse Y");

            cameraRotation.x += cameraViewMouseX * mouseSensivity;
            cameraRotation.y += cameraViewMouseY * mouseSensivity;
            cameraRotation.y = Mathf.Clamp(cameraRotation.y, -60f, 60f); // ograniczenie k¹ta

            Vector3 pivotPos = deathCameraPivot.transform.position + new Vector3(0, deathCameraHeightOffset, 0);

            Quaternion rotation = Quaternion.Euler(-cameraRotation.y, cameraRotation.x, 0f);
            Vector3 direction = rotation * Vector3.back;  // wektor w ty³ w lokalnych wspó³rzêdnych
            Vector3 desiredPos = pivotPos + direction * maxDeathCameraDistance;

            RaycastHit hit;
            if (Physics.SphereCast(pivotPos, deathCameraRadius, direction, out hit, maxDeathCameraDistance))
            {
                float hitDistance = hit.distance;
                float adjustedDistance = Mathf.Clamp(hitDistance - 0.1f, minDeathCameraDistance, maxDeathCameraDistance);

                deathCamera.transform.position = pivotPos + direction * adjustedDistance;
            }
            else
            {
                deathCamera.transform.position = desiredPos;
            }

            deathCamera.transform.LookAt(pivotPos);
        }
    }

    void ApplyGravity()
    {
        if(!isGrounded)
        {
            wasGroundTouched = false;
            moveVector.y += gravityValue * Time.deltaTime;
        }

        playerController.Move(moveVector * Time.deltaTime);
    }

    void HandleCrouching()
    {
        float targetHeight = isCrouching ? 1f : 1.7f;
        float targetCenterY = isCrouching ? 0.5f : 0f;
        cameraTargetY = isCrouching ? 0.274f : 0.758f;
        playerController.height = Mathf.Lerp(playerController.height, targetHeight, 10f * Time.deltaTime);
        
        Vector3 camPos = playerCamera.transform.localPosition;
        camPos.y = Mathf.SmoothDamp(camPos.y, cameraTargetY, ref cameraSmoothVelocity, 0.08f);
        playerCamera.transform.localPosition = camPos;

    }

    void DebugShit()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            currentHealth = 0;
            Debug.Log("Player health set to 0 for testing purposes.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(playerGroundCheck.transform.position, groundCheckRadius);
        Gizmos.DrawRay(playerGroundCheck.transform.position, Vector3.down * 1f);
    }

}


