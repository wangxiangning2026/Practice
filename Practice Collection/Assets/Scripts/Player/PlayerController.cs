using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    public float walkSpeed = 4f;

    [SerializeField] private Animator animator;

    [SerializeField] public float runSpeed = 8f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float targetSpeed = 0f;

    [Header("Camera Settings")] [SerializeField]
    private Transform cameraTarget; // 摄像机看向的目标点

    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float maxLookAngle = 80f;
    [SerializeField] private float minLookAngle = -30f; // 稍微限制向下角度

    [Header("Camera Distance")] [SerializeField]
    private float defaultDistance = 5f;

    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float smoothFollowSpeed = 10f;

    [Header("Camera Collision")] [SerializeField]
    private LayerMask collisionLayers = -1;

    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.2f;

    // Private variables
    private CharacterController characterController;
    private Transform mainCamera;
    private Transform playerTransform;

    private float currentDistance;
    private float targetDistance;
    private float xRotation = 0f;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private Vector3 cameraVelocity;

    private bool isGrounded;
    private bool isRunning;

    public bool isMoving = false;

    private Vector3 targetPosition;
    [SerializeField] private float followSpeed = 5f; // 摄像机跟随速度
    [SerializeField] private bool lookInMoveDirection = true; // 摄像机是否看向移动方向

    private AnimationManager animationManager;
    
    private FSMController fsm;

    void Start()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        playerTransform = transform;
        mainCamera = Camera.main.transform;
        
        if (cameraTarget == null)
            cameraTarget = playerTransform;
        
        currentDistance = defaultDistance;
        targetDistance = defaultDistance;

        targetPosition = mainCamera.position;

        RegisterStates();
    }

    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;
    
    private void RegisterStates()
    {
        // 注册所有状态
        fsm.RegisterStates(
            new IdleState(fsm, this),
            new WalkState(fsm, this),
            new RunState(fsm, this)
        );
            
        // 设置默认状态
        fsm.SetDefaultState<IdleState>();
        fsm.Initialize();
    }
    
    void Update()
    {
        // 获取输入
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            
        // 状态切换条件放在Update中检查（更好的做法是放在各个状态内部）
        
        HandleMouseLook();
        HandleZoom();
        //HandleJump();
    }

    void FixedUpdate()
    {
        //HandleMovement();
        //ApplyGravity();
    }

    void LateUpdate()
    {
        UpdateCameraPosition();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        mainCamera.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);
    }

    public bool IsMoving()
    {
        return Mathf.Abs(moveInput.y) > 0.1f || Mathf.Abs(moveInput.x) > 0.1f;
    }

    public void HandleMovement(float speed)
    {
        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        // 获取摄像机的前方和右方方向（忽略俯仰角）
        Vector3 cameraForward = mainCamera.forward;
        Vector3 cameraRight = mainCamera.right;

        // 忽略Y轴（防止摄像机抬头/低头时，角色沿Y轴上下移动）
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // 归一化：确保摄像机方向向量长度为1，移动速度一致
        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = Vector3.zero; // 最终移动方向
        Vector3 facingDirection = Vector3.zero; // 角色朝向方向

        // 处理W/S键：前后移动并转向
        if (Mathf.Abs(vertical) > 0.1f)
        {
            if (vertical > 0) // W键：向前
            {
                facingDirection = cameraForward;
                moveDirection = cameraForward;
            }
            else // S键：向后
            {
                facingDirection = -cameraForward;
                moveDirection = -cameraForward;
            }
        }

        // 处理A/D键：左右移动并转向
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            if (horizontal > 0) // D键：向右
            {
                facingDirection = cameraRight;
                moveDirection = cameraRight;
            }
            else // A键：向左
            {
                facingDirection = -cameraRight;
                moveDirection = -cameraRight;
            }
        }

        // 如果同时有垂直和水平输入，使用合成方向
        if (Mathf.Abs(vertical) > 0.1f && Mathf.Abs(horizontal) > 0.1f)
        {
            // 合成移动方向（斜向移动）
            moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

            // 面向合成方向
            facingDirection = moveDirection;
        }

        // 应用旋转 - 面向移动方向
        if (facingDirection != Vector3.zero)
        {
            playerTransform.rotation = Quaternion.LookRotation(facingDirection);
        }

        // 应用移动
        characterController.Move(moveDirection * targetSpeed * Time.fixedDeltaTime);
    }

    void HandleWalk(float speed)
    {
    }


    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.fixedDeltaTime;
        characterController.Move(velocity * Time.fixedDeltaTime);
    }

    void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            Debug.Log(scrollInput);

            // 获取当前偏移值
            float height = cameraOffset.y; // Y轴：控制摄像机高度
            float distance = cameraOffset.z; // Z轴：控制摄像机前后距离

            // 修改距离（Z轴），而不是X轴
            distance -= scrollInput * zoomSpeed;

            // 限制范围
            distance = Mathf.Clamp(distance, -8f, -2f); // 前后距离限制
            height = Mathf.Clamp(height, 1f, 3f); // 高度限制（可选）

            // 更新cameraOffset
            cameraOffset = new Vector3(0f, height, distance);

            Debug.Log($"New camera offset: height={height}, distance={distance}");
        }
    }

    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 2f, -5f); // 摄像机相对角色的固定偏移

    void UpdateCameraPosition()
    {
        // 计算摄像机应该在的位置（基于角色当前朝向）
        Vector3 desiredPosition = transform.position + cameraOffset;

        // 平滑移动到目标位置（跟随人物移动方向）
        targetPosition = Vector3.Lerp(targetPosition, desiredPosition, Time.deltaTime * followSpeed);
        mainCamera.position = targetPosition;
    }

    public void PlayAnimation(string animName, float crossFade = 0.1f)
    {
        animationManager?.Play(animName, crossFade);
    }

    public void SetAnimatorFloat(string param, float value)
    {
        animationManager?.SetFloat(param, value);
    }

    private bool isCrouching;
    public bool IsCrouching => isCrouching;

    public void SetCrouch(bool crouch)
    {
        isCrouching = crouch;

        // 修改碰撞体大小
        // if (collider2D != null)
        // {
        //     var size = collider2D.bounds.size;
        //     size.y = crouch ? crouchHeight : standHeight;
        //     // 调整碰撞体...
        // }

        // 设置动画参数
        animationManager?.SetBool("IsCrouching", crouch);
    }
}