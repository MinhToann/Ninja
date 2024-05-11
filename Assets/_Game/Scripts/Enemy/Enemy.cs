using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] Rigidbody2D _rigidbody2D;
    [SerializeField] float attackRange;
    [SerializeField] float speedMove;
    [SerializeField] GameObject parentEnemy;
    [SerializeField] GameObject attackPrefab;
    public bool isEnemyDead = false;
    private Character target;
    public Character Target => target;

    bool isAttack;
    public bool IsAttack => isAttack;
    public static Enemy instance;
    // animation IDs
    string animIDIdle = "Idle";
    string animIDRun = "Run";
    string animIDJump = "StartJump";
    string animIDInAir = "InAir";
    string animIDFly = "Fly";
    string animIDAttack = "Attack";
    string animIDShoot = "Shoot";
    string animIDDeath = "Death";


    private IState currentState;
    private void Awake()
    {
        instance = this;
    }
    public override void Start()
    {
        base.Start();
        //AssignAnimationIDs();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            OnDead();
            _rigidbody2D.velocity = Vector2.zero;
            return;
        }

        if (currentState != null)
        {
            currentState.OnExecute(this);
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        attackPrefab.SetActive(false);
        ChangeState(new IdleState());
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        Destroy(parentEnemy);
    }

    protected override void OnDead()
    {
        ChangeAnim(animIDDeath);
        ChangeState(null);
        base.OnDead();
        isEnemyDead = true;
    }

    public void ChangeState(IState newState)
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

    public void Move()
    {
        ChangeAnim(animIDRun);
        _rigidbody2D.velocity = transform.right * speedMove;   
    }

    public void StopMove()
    {
        ChangeAnim(animIDIdle);
        _rigidbody2D.velocity = Vector2.zero;
    }

    public void Attack()
    {
        isAttack = true;
        ChangeAnim(animIDAttack);
        attackPrefab.SetActive(true);
        Invoke(nameof(ResetAttack), _anim.GetCurrentAnimatorStateInfo(0).length);
    }

    void ResetAttack()
    {
        isAttack = false;
        attackPrefab.SetActive(false);
        ChangeAnim(animIDIdle);

    }

    public bool IsTargetInRange()
    {
        if (Target != null)
        {
            return Vector2.Distance(Target.transform.position, transform.position) <= attackRange;
        }
        else return false;
    }

    public void SetTarget(Character character)
    {
        target = character;
        if (Target != null) ChangeState(new PatrolState());
    }

    private void AssignAnimationIDs()
    {
        currentAnimID = animIDIdle;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "EnemyWall")
        {
            transform.right = -(_rigidbody2D.velocity).normalized;
        }
    }
}
