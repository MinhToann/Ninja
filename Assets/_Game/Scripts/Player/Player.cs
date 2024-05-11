using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    CapsuleCollider2D _capsuleCollider2D;
    PlayerInputManager _input;
    Rigidbody2D _rigidbody2D;
    public static Player instance;
    [Header("StatusChecking")]
    [SerializeField] bool isGround = true;
    public bool isAttack = false;
    public bool isPlayerDead = false;
    [Header("StatesChecking")]
    [SerializeField] bool inOnGroundState;

    [Header("GeneralSetting")]
    [SerializeField] GameObject _mainCamera;
    [SerializeField] LayerMask groundLayer;
    public float speed;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject kunaiPrefab;
    [SerializeField] Transform kunaiSpawn;
    [SerializeField] GameObject attackPrefab;
    Vector3 savePoint;
    [HideInInspector] public int coin = 0;


    private float horizontal;
    public bool isJump = false;
    IStatePlayer currentState;

    // animation IDs
    string animIDIdle = "Idle";
    string animIDRun = "Run";
    string animIDJump = "StartJump";
    string animIDInAir = "InAir";
    string animIDFly = "Fly";
    string animIDAttack = "Attack";
    string animIDShoot = "Shoot";
    string animIDDeath = "Death";

    private void Awake()
    {
        instance = this;
    }
    public override void Start()
    {
        base.Start();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _input = GetComponent<PlayerInputManager>();
        _mainCamera = FindObjectOfType<CameraFollow>().gameObject;
        //AssignAnimationIDs();
        SavePoint();
        coin = 0;
        UIManager.instance.SetCoin(coin);
    }
    public override void OnInit()
    {
        base.OnInit();
        ResetAttack();
        transform.position = savePoint;
        ChangeAnim(animIDIdle);
        //ChangeState(new OnGroundStatePlayer());
        attackPrefab.SetActive(false);
    }

    public override void OnDespawn()
    {
        OnInit();
        base.OnDespawn();
    }

    protected override void OnDead()
    {
        base.OnDead();
        ChangeAnim(animIDDeath);
        isPlayerDead = true;
    }
    void Update()
    {
        //UpdateOnGroundState();
        
        isGround = GroundCheck();
        Debug.Log(GroundCheck());
        if (isGround)
        {
            if (isJump)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
            if (Mathf.Abs(horizontal) > 0.1f)
            {
                ChangeAnim(animIDRun);
            }
            if (isAttack)
            {
                _rigidbody2D.velocity = Vector2.zero;
                return;
            }
            if (Input.GetKeyDown(KeyCode.U))
            {
                Shoot();
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                Attack();
            }
            if (isDead)
            {
                OnDead();
                _rigidbody2D.velocity = Vector2.zero;
                return;
            }

        }
        if (!isGround && _rigidbody2D.velocity.y < 0)
        {
            ChangeAnim(animIDInAir);
            isJump = false;
        }
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            ChangeAnim(animIDRun);
            _rigidbody2D.velocity = new Vector2(horizontal * speed * Time.deltaTime, _rigidbody2D.velocity.y);
            transform.rotation = Quaternion.Euler(0, horizontal > 0 ? 0 : 180, 0);
        }
        else if (isGround)
        {
            ChangeAnim(animIDIdle);
            _rigidbody2D.velocity = Vector2.zero;
        }

        if (currentState != null)
        {
            currentState.OnExecute(this);
        }
    }

    public void StartOnGroundState()
    {
        inOnGroundState = true;
    }
    public void UpdateOnGroundState()
    {
        isGround = GroundCheck();
    }
    public void ExitOnGroundState()
    {
        inOnGroundState = false;
    }

    public void ChangeState(IStatePlayer newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }


    void Attack()
    {
        isAttack = true;
        ChangeAnim(animIDAttack);
        attackPrefab.SetActive(true);
        Invoke(nameof(ResetAttack), 0.5f);
    }

    void ResetAttack()
    {
        isAttack = false;
        attackPrefab.SetActive(false);       
        ChangeAnim(animIDIdle);
        //Invoke(nameof(Attack), 0.6f);
    }
    private void Jump()
    {
        isJump = true;
        ChangeAnim(animIDJump);
        _rigidbody2D.AddForce(jumpForce * Vector2.up);
    }
    void Shoot()
    {
        isAttack = true;
        ChangeAnim(animIDShoot);
        Invoke(nameof(ResetAttack), 0.5f);
        Instantiate(kunaiPrefab, kunaiSpawn.position, transform.rotation);
    }
    void SavePoint()
    {
        savePoint = transform.position;
    }
    bool GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        return hit.collider != null;
    }
    void AssignAnimationIDs()
    {
        currentAnimID = animIDIdle;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            coin++;
            UIManager.instance.SetCoin(coin);
            Destroy(collision.gameObject);
        }
        if (collision.tag == "DeathZone")
        {
            hp = 0;
            ChangeAnim(animIDDeath);
            isPlayerDead = true;
            Invoke(nameof(OnInit), 1f);
        }
        if (collision.tag == "SavePoint")
        {
            savePoint = collision.transform.position;
        }
    }


}

