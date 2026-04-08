using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float walkSpeed = 4f;

    [SerializeField] private Animator animator;

    [SerializeField] private float runSpeed = 8f;
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

    private bool isMoving = false;

    private Vector3 targetPosition;
    [SerializeField] private float followSpeed = 5f; // 摄像机跟随速度
    [SerializeField] private bool lookInMoveDirection = true; // 摄像机是否看向移动方向

    void Start()
    {
        // Get components
        characterController = GetComponent<CharacterController>();
        playerTransform = transform;
        mainCamera = Camera.main.transform;

        // If no camera target assigned, use player as target
        if (cameraTarget == null)
            cameraTarget = playerTransform;

        // Lock cursor
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        // Initialize distance
        currentDistance = defaultDistance;
        targetDistance = defaultDistance;

        targetPosition = mainCamera.position;
        // Position camera initially
        //UpdateCameraPosition(0f);
    }

    void Update()
    {
        HandleMouseLook();
        HandleZoom();
        //HandleJump();
    }

    void FixedUpdate()
    {
        HandleMovement();
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


    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 检测是否奔跑
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        targetSpeed = isRunning ? runSpeed : walkSpeed;

        // 获取摄像机的前方和右方方向（忽略俯仰角）
        Vector3 cameraForward = mainCamera.forward;
        Vector3 cameraRight = mainCamera.right;

        // 忽略Y轴（防止摄像机抬头/低头时，角色沿Y轴上下移动）
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // 归一化：确保摄像机方向向量长度为1，移动速度一致
        cameraForward.Normalize();
        cameraRight.Normalize();

        // 4. 仅当输入有效（过滤微小值）时处理移动
        if (Mathf.Abs(vertical) > 0.1f || Mathf.Abs(horizontal) > 0.1f)
        {
            isMoving = true;
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
        else
        {
            isMoving = false;
        }

        float currentspeed = isMoving ? isRunning ? runSpeed: walkSpeed : 0;
        animator.SetFloat("targetSpeed", currentspeed);
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
}