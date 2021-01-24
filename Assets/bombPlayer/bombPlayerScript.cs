﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombPlayerScript : MonoBehaviourPunCallbacks
{
    public GameObject _player;

    public GameObject _bomb;

    public GameObject _BigBomb;

    private float speed = 300f;

    [SerializeField]
    private bool fireable = true;
    [SerializeField]
    private bool reloading = false;
    [SerializeField]
    private bool skillAvailable = true;

    public float AttackDelayTime = 0.5f;
    public float ReloadTime = 2f;
    public float SkillCoolTime = 1f;
    public int max_bomb = 5;

    [SerializeField]
    private int current_bomb;

    private float horizontal;
    private float vertical;

    private Vector3 velocity;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        _player = gameObject;
        current_bomb = max_bomb;

        
    }

    IEnumerator Reload()
    {
        Debug.Log("reloading");
        yield return new WaitForSeconds(ReloadTime);
        reloading = false;
        current_bomb++;
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(AttackDelayTime);
        fireable = true;
    }

    IEnumerator SkillCool()
    {
        Debug.Log("SkillCool");
        yield return new WaitForSeconds(SkillCoolTime);
        skillAvailable = true;
        
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        velocity.x = speed * horizontal * Time.deltaTime;
        velocity.y = speed * vertical * Time.deltaTime;

        transform.Translate(velocity * Time.deltaTime);

        if (Input.GetMouseButtonUp(0) && current_bomb > 0 && fireable)
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerposition = transform.position;
            photonView.RPC("ThrowBomb", PhotonTargets.All, new object[] { mouse, playerposition });
        }

        if (!reloading && current_bomb < max_bomb)
        {
            reloading = true;
            StartCoroutine("Reload");
        }

        if (Input.GetKeyUp(KeyCode.Space) && skillAvailable)
        {
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 playerposition = transform.position;
            photonView.RPC("ThrowBigBomb", PhotonTargets.All, new object[] { mouse, playerposition });
        }
    }
    
    [PunRPC]
    private void Attack()
    {
        animator.SetTrigger("Attack");
    }

    [PunRPC]
    public void ThrowBomb(Vector2 mouse, Vector2 playerposition)
    {
        photonView.RPC("Attack", PhotonTargets.All);
        fireable = false;
        current_bomb--;
        _bomb.GetComponent<bombObject>().Target = mouse;
        _bomb.GetComponent<bombObject>().startposition = playerposition;
        GameObject bomb = Instantiate(_bomb);
        
        StartCoroutine("AttackDelay");
    }

    [PunRPC]
    public void ThrowBigBomb(Vector2 mouse, Vector2 playerposition)
    {
        photonView.RPC("Attack", PhotonTargets.All);
        skillAvailable = false;
        _BigBomb.GetComponent<bigBombScript>().Target = mouse;
        _BigBomb.GetComponent<bigBombScript>().startposition = playerposition;
        
        GameObject bomb = Instantiate(_BigBomb);

        StartCoroutine("SkillCool");
    }
}
