using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControl : MonoBehaviour
{
    //Movement variables
    private float speed = 0.3f;
    private float rotSpeed = 200f;
    private bool isSprinting = false;
    private float gravity = -19.6f; // Gravity value
    private float acceleration = 2f; // Acceleration value
    private float desceleration = 3f; // Desceleration value
    private float playerVelocity = 0;
    private float velocityValue = 1;
    private Vector3 velocity; // Velocity vector
    private float jumpForce; // Jump force value
    private float jumpHeight = 1.5f;
    private bool isGrounded = false;
    private bool isJumping = false;

    [Range (0.1f, 0.5f)] public float distanceToGround;
    public LayerMask layerMask;

    //Game Components
    private UnityEngine.CharacterController controller;
    [SerializeField]private Animator anim;

    //Game transform
    [SerializeField]private Transform cameraTransform;

    //Input system
    private PlayerInput playerInput;
    private InputAction moveAction;

    // Start is called before the first frame update
    void Start()
    {
        jumpForce = Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpHeight);
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

        // Initialize move action
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        playerInput.actions["Sprint"].started += context => OnSprint();
        playerInput.actions["Sprint"].canceled += context => OnSprintCanceled();
        playerInput.actions["Jump"].canceled += context => OnJumpCanceled();
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
         // Define the ground layer
        LayerMask groundLayer = LayerMask.GetMask("Ground");
        // Define the distance to check for ground
        float groundCheckDistance = 0.6f;
        // Define the starting position of the SphereCast to be slightly above the ground
        Vector3 sphereCastStart = transform.position + Vector3.up * (controller.height / 2);
        // Perform the SphereCast
        isGrounded = Physics.SphereCast(sphereCastStart, controller.radius, Vector3.down, out RaycastHit hit, controller.height / 2f + groundCheckDistance, groundLayer);

        Debug.DrawRay(sphereCastStart, Vector3.down * (controller.height / 2f + groundCheckDistance), Color.red);

        //Get the input value from unity input system
        Vector2 position = moveAction.ReadValue<Vector2>();

        Debug.Log(isGrounded);

        if(isJumping){
            if(isGrounded){
                speed = 0.3f;
                isJumping = false;
                anim.SetBool("IsJumping", false);
            }
        }
        // Calculate the move direction relative to the camera's rotation
        Vector3 camF = Camera.main.transform.forward;
        Vector3 camR = Camera.main.transform.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        Vector3 move = (camF * position.y + camR * position.x) * speed * Time.deltaTime;

        //Character movement handle with CharacterController
        velocity.y += gravity * Time.deltaTime;

        Vector3 currentPos = transform.position;
        // Move the controller with gravity
        controller.Move(move + velocity * Time.deltaTime);
        
        Vector3 moveDirection = transform.position - currentPos;
        
        // If there's some input
        if (move != Vector3.zero) {
            if(playerVelocity < velocityValue){playerVelocity += Time.deltaTime * acceleration;}
            // Rotate the character to face the direction of movement
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotSpeed * Time.deltaTime);
            //fix y rotation
            transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
        }
        else {
            if(playerVelocity > 0){playerVelocity -= Time.deltaTime * acceleration;}
        }
        if(isSprinting == false) {
            if(playerVelocity > 1){playerVelocity -= Time.deltaTime * desceleration;}
        }
        anim.SetFloat("Acceleration", playerVelocity);
    }

    void OnSprint() {   
        if(isGrounded){
            velocityValue = 2;
            isSprinting = true;
        }
    }

    void OnSprintCanceled() {
        velocityValue = 1;
        isSprinting = false;
    }

    public void OnJump() {
        Debug.Log("Jump");
        if (isGrounded) {
            speed = 2f;
            anim.SetLayerWeight(0, 1);
            anim.SetLayerWeight(1, 1);
            anim.SetBool("IsJumping", true);
        }
    }

    public void PerformJump() {
        Debug.Log("Jump");
        if(isSprinting == true) {
            velocity.y = jumpForce * 1.5f;
        }
        else{
            velocity.y = jumpForce;
        }
    }

    public void JumpLoop(){
        isJumping = true;
    }

    public void EndJumpLoop(){
        anim.SetLayerWeight(0, 1);
        anim.SetLayerWeight(1, 0);
    }

    public void OnJumpCanceled() {
        
    }

    private void OnAnimatorIK(int layerIndex) {
        if(anim && !isJumping) {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight"));
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, anim.GetFloat("IKLeftFootWeight")); 
            Vector3 originalRayOrigin = anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up;
            Vector3 offsetRayOrigin = originalRayOrigin + transform.TransformDirection(new Vector3(0, 0, 0.12f));

            // anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
            // anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);

            RaycastHit hit;

            Ray ray = new Ray(originalRayOrigin, Vector3.down);
            Ray offsetRay = new Ray(offsetRayOrigin, transform.TransformDirection(Vector3.down));
            Debug.DrawRay(ray.origin, ray.direction * (distanceToGround + 1f), Color.red);
            Debug.DrawRay(offsetRay.origin, offsetRay.direction * (distanceToGround + 1f), Color.blue);
            if (Physics.Raycast(ray, out hit, distanceToGround + 1f, layerMask)) {
                Vector3 footPosition = hit.point;
                footPosition.y += distanceToGround;
                anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            }

            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, anim.GetFloat("IKRightFootWeight"));
            // anim.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
            // anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1); 

            RaycastHit rightHit;
            Ray rightRay = new Ray(anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            if (Physics.Raycast(rightRay, out rightHit, distanceToGround + 1f, layerMask)) {
                Vector3 footPosition = rightHit.point;
                footPosition.y += distanceToGround;
                anim.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                //anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hit.normal));
            //anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            }
        }
    }
}
