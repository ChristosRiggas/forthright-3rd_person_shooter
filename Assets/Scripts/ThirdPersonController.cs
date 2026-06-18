using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController : MonoBehaviour
{
    //input fields
    private ThirdPersonActionsAsset playerActionAsset;
    private InputAction move;

    //movement fields
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform[] groundChecks;

    public float jumpHeight = 3;
    public float gravity = 20;
    public float stepDown = 0.3f;
    public float airControl;
    public float jumpDamp;
    public float groundSpeed;
    public float pushPower;

    [SerializeField]
    private float forceMagnitude;

    Animator animator;
    CharacterController cc;
    Vector2 input = Vector2.zero;

    Vector3 rootMotion;
    Vector3 velocity;
    public bool isJumping;
    public bool isGrounded;

    private Vector2 smoothMovementInput;
    private Vector2 movementInputSmoothVelocity;

    void Start()
    {
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    private void Awake()
    {
        playerActionAsset = new ThirdPersonActionsAsset();
    }

    private void OnEnable()
    {
        playerActionAsset.Player.Jump.started += Jump;
        move = playerActionAsset.Player.Move;
        playerActionAsset.Player.Enable();
    }

    private void OnDisable ()
    {
        playerActionAsset.Player.Jump.started -= Jump;
        playerActionAsset.Player.Disable();
    }

    void Update()
    {
        //Debug.Log("Movement Values " + move.ReadValue<Vector2>());
        input = move.ReadValue<Vector2>();

        smoothMovementInput = Vector2.SmoothDamp(
            smoothMovementInput,
            input,
            ref movementInputSmoothVelocity,
            0.1f);

        animator.SetFloat("InputX", smoothMovementInput.x);
        animator.SetFloat("InputY", smoothMovementInput.y);
    }

    void OnAnimatorMove()
    {
        rootMotion += animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        isGrounded = false;
        foreach (var groundCheck in groundChecks)
        {
            if (Physics.OverlapSphere(groundCheck.position, 0.2f, playerMask).Length == 0)
            {
                isGrounded = false;
                animator.SetBool("isJumping", false);
                //return;
            }
            else
            {
                isGrounded = true;
                animator.SetBool("isJumping", true);
                break;
            }
        }

        if (isJumping)
        { //InAir
            UpdateInAir();
        }
        else
        {  //InGround
            UpdateOnGround();
        }

    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
            SetInAir(jumpVelocity);
            //animator.SetBool("isJumping", true);
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
        //animator.SetBool("isJumping", true);
    }

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();

        cc.Move(displacement);
        isJumping = !cc.isGrounded;



        rootMotion = Vector3.zero;
        //animator.SetBool("isJumping", isJumping);
    }

    private void UpdateOnGround()
    {
        Vector3 stepForwardAmount = rootMotion * groundSpeed;
        Vector3 stepDownAmount = Vector3.down * stepDown;

        //Debug.Log(cc.Move(stepForwardAmount + stepDownAmount));
        cc.Move(stepForwardAmount + stepDownAmount);
        rootMotion = Vector3.zero;


        if (!cc.isGrounded)
        {
            SetInAir(0);
        }
        ///animator.SetBool("isJumping", false);

    }

    Vector3 CalculateAirControl()
    {
         return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // no rigidbody
        if (body == null || body.isKinematic)
            return;

        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f)
            return;

        // Calculate push direction from move direction,
        // we only push objects to the sides never up and down
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // If you know how fast your character is trying to move,
        // then you can also multiply the push velocity by that.

        // Apply the push
        body.velocity = pushDir * pushPower;
    }

}
