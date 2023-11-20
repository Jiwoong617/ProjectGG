﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : BaseController
{
    public PlayerStat stat = new();
    public Vector3 moveDir;
    public bool isAlive = true;

    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip AttackClip;

    const string ANIM_IDLE = "Idle";
    const string ANIM_MOVE = "Move";
    const string ANIM_SPRINT = "Sprint";
    const string ANIM_ATTACK = "Attack";
    const string ANIM_DEATH = "Death";


    private void Awake()
    {
        Global.Player = gameObject;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        //임시
        stat.SetStat(new StatInfo(100, 100, 100, 100, 10));
    }


    protected override void OnIdle()
    {
        if(moveDir != Vector3.zero)
        {
            state = State.Moving;
            return;
        }

        animator.Play(ANIM_IDLE);
    }

    protected override void OnMoving()
    {
        if(moveDir == Vector3.zero)
        {
            state = State.Idle;
            return;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveDir), 20 * Time.deltaTime);
        transform.position += moveDir * Time.deltaTime * stat.MoveSpeed;

        if(stat.MoveSpeed > 15) animator.Play(ANIM_SPRINT);
        else animator.Play(ANIM_MOVE);
    }

    protected override void OnDie()
    {
        animator.Play(ANIM_DEATH);
        animator.Play(ANIM_DEATH, 1);
        isAlive = false;
    }

    public override void OnAttacked(Stat s)
    {
        stat.TakeDamage(s);
        if (stat.Hp <= 0)
            state = State.Die;
    }

    public void SetDir(Vector3 dir) => moveDir = dir;


    public void Attack()
    {
        Collider[] col = Physics.OverlapBox(transform.position + transform.forward*2 + Vector3.up, new Vector3(1, 0.5f, 1), transform.rotation, 1<< LayerMask.NameToLayer("Monster"));
        //if (col.Length > 0) Debug.Log(col.Length);
        //Debug.Log("attack");
        foreach(Collider c in col)
        {
            c.GetComponent<BaseController>().OnAttacked(stat);
        }
    }
    //IEnumerator AttackLoop()
    //{
    //    yield return new WaitForSeconds(AttackClip.length/2);

    //    while(state != State.Die)
    //    {
    //        Collider[] col = Physics.OverlapBox(transform.position + transform.forward*2 + Vector3.up, new Vector3(1, 0.5f, 1), transform.rotation, 1<< LayerMask.NameToLayer("Monster"));
    //        //if (col.Length > 0) Debug.Log(col.Length);
    //        Debug.Log("attack");
    //        foreach(Collider c in col)
    //        {
    //            c.GetComponent<BaseController>().OnAttacked(stat);
    //        }

    //        yield return new WaitForSeconds(AttackClip.length);
    //    }
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(new Vector3(0, 1, 2), new Vector3(1, 0.5f, 1)*2);
    //}
}