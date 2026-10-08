using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Animations;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
//using static UnityEditor.Experimental.GraphView.GraphView;

public class CharacterLocomotion : MonoBehaviour
{
    private InputActionAsset inputAsset;
    public InputActionMap player;
    private InputAction move;
    
    public GameObject mainCamera;

    //movement fields
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private Transform wallCheck;
    private bool nearWall;
    private bool jumpedNearWall;
    private float countJumpTime;

    public int userID;
    private PlayerManager playerManager;

    public LevelMangaer levelManager;

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

    public PlayerManager Users { get => playerManager; set => playerManager = value; }

    [System.Obsolete]
    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

        Users = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
        userID = Users.players.Count - 1;

        //Random Mesh
        Random.seed = (int)System.DateTime.Now.Ticks;
        gameObject.transform.GetChild(1).GetChild(Random.Range(0, 2)).gameObject.SetActive(true);

        foreach (Transform head in gameObject.GetComponentsInChildren<Transform>())
        {
            if (head.CompareTag("head")) head.gameObject.transform.GetChild(Random.Range(0, 3)).gameObject.SetActive(true);
        }
        //Random Mesh

        levelManager = GameObject.Find("LevelManager").GetComponent<LevelMangaer>();

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void OnEnable()
    {
        player.FindAction("Pause").started += Pause;
        player.FindAction("Jump").started += Jump;
        move = player.FindAction("Move");
        player.Enable();
    }

    private void OnDisable()
    {
        player.FindAction("Jump").started -= Jump;
        player.FindAction("Pause").started -= Pause;
        player.Disable();
    }

    void Start()
    { 
        animator = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        input = move.ReadValue<Vector2>();

        smoothMovementInput = Vector2.SmoothDamp(
            smoothMovementInput,
            input,
            ref movementInputSmoothVelocity,
            0.1f);

        animator.SetFloat("InputX", smoothMovementInput.x);
        animator.SetFloat("InputY", smoothMovementInput.y);

        if (jumpedNearWall)
        {
            countJumpTime += Time.deltaTime;
        }
    }

    void OnAnimatorMove()
    {
        rootMotion += animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        if (NearWall())
        {
            nearWall = true;
        }
        else
        {
            nearWall = false;
        }

        if (jumpedNearWall && countJumpTime >= 1.0f)
        {
            jumpedNearWall = false;
            countJumpTime = 0;  
        }

        foreach (var groundCheck in groundChecks)
        {

            if (Physics.OverlapSphere(groundCheck.position, 0.2f, ~playerMask).Length == 0)
            {
                isGrounded = false;
                animator.SetBool("isJumping", false);
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

    private bool NearWall()
    {
        if(Physics.OverlapSphere(wallCheck.position, 0.5f, ~playerMask).Length == 0)
        return false;
        else
            return true;
    }

    private void Pause(InputAction.CallbackContext obj)
    {
        if(mainCamera.activeSelf == false)
        levelManager.PauseGame();
    }

    private void Jump(InputAction.CallbackContext obj)
    {
        if (isGrounded)
        {
            if ((jumpedNearWall == false && nearWall == true) || nearWall == false)
            {
                float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
                SetInAir(jumpVelocity);

                if(nearWall == true)
                jumpedNearWall = true;

            }
        }
    }

    private void SetInAir(float jumpVelocity)
    {
        isJumping = true;
        velocity = animator.velocity * jumpDamp * groundSpeed;
        velocity.y = jumpVelocity;
    }

    private void UpdateInAir()
    {
        velocity.y -= gravity * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;
        displacement += CalculateAirControl();

        cc.Move(displacement);
        isJumping = !cc.isGrounded;

        rootMotion = Vector3.zero;
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
    }

    Vector3 CalculateAirControl()
    {
        return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic)
            return;

        if (hit.moveDirection.y < -0.3f)
            return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.velocity = pushDir * pushPower;
    }

}
