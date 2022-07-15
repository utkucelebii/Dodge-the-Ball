using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Animator _animator;
    private InputManager inputManager;
    private LevelManager levelManager;

    [Header("Game Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform ball;

    private Vector3 moveDirection;
    private bool isAlive = true;
    private bool throwing, ballThrowed;
    private Vector3 positionToThrow;

    private void Awake()
    {
        inputManager = InputManager.Instance;
        levelManager = LevelManager.Instance;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        moveSpeed = levelManager.moveSpeed;
    }

    private void Update()
    {
        moveDirection = new Vector3(inputManager.moveDirection.x, 0, inputManager.moveDirection.y);
    }

    private void FixedUpdate()
    {
        if (!isAlive)
            return;

        if(moveDirection != Vector3.zero)
        {
            _animator.SetBool("HoldBall", false);
            _animator.SetBool("Run", true);
            transform.position += moveDirection * Time.deltaTime * moveSpeed;
            transform.LookAt(transform.position + moveDirection);
        }
        else
        {
            _animator.SetBool("HoldBall", true);
            _animator.SetBool("Run", false);
        }

        if (inputManager.Throw && !throwing)
        {
            inputManager.Throw = false;
            ThrowBall();
        }

        // Throwing ball
        if (throwing)
        {
            ball.transform.localPosition = Vector3.Lerp(ball.transform.localPosition, new Vector3(0, 0f, 0.75f), Time.deltaTime * 10);
            
            if(ballThrowed)
                ball.transform.position = Vector3.Lerp(ball.transform.position, positionToThrow, Time.deltaTime);
        }
    }

    public void ThrowBall()
    {
        throwing = true;
        StartCoroutine(Throw());
    }

    IEnumerator Throw()
    {
        _animator.SetBool("Throw", true);
        _animator.SetBool("HoldBall", false);
        _animator.SetBool("Run", false);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("HoldBall", true);
        _animator.SetBool("Throw", false);
        _animator.SetBool("Run", false);
        ballThrowed = true;
        positionToThrow = ball.transform.position + transform.forward * 150;
        positionToThrow.y -= 0.6f;
        yield return new WaitForSeconds(1f);
        throwing = false;
        ballThrowed = false;
        ball.transform.localPosition = new Vector3(0, -1.008f, 0.75f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Ball" && isAlive)
        {
            isAlive = false;
            _animator.SetBool("Death", true);
            _animator.SetBool("HoldBall", false);
            _animator.SetBool("Throw", false);
            _animator.SetBool("Run", false);
            ball.gameObject.SetActive(false);
            levelManager.gamers.Remove(this.gameObject);
            StartCoroutine(GoDie());
        }
    }

    IEnumerator GoDie()
    {
        yield return new WaitForSeconds(1f);
        gameObject.SetActive(false);
        levelManager.Respawn();
    }
}
