using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private LevelManager levelManager;
    private Animator _animator;

    [Header("Game Settings")]
    private bool hardness;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Transform ball;
    [SerializeField] private float throwableTimeLimit = 3f;
    [SerializeField] private float changeDirectionTimeLimit = 2f;

    // Throwing
    private bool isRotating;
    private float timeToChangeDirection, throwableTime;
    private bool isAlive = true;
    private bool throwing, ballThrowed;
    private Vector3 positionToThrow;

    // Other players
    private List<GameObject> otherPlayers = new List<GameObject>();
    private GameObject throwToPlayer;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        moveSpeed = levelManager.moveSpeed;
        hardness = levelManager.hardness;
        otherPlayers = levelManager.gamers.Where(i => i.transform != this.transform).ToList();
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            gameObject.SetActive(false);
            return;
        }

        if (transform.position.y < 0)
        {
            isAlive = false;
            gameObject.SetActive(false);
        }

        throwableTime += Time.deltaTime;
        timeToChangeDirection += Time.deltaTime;

        // Throwing ball
        if (throwing)
        {
            ball.transform.localPosition = Vector3.Lerp(ball.transform.localPosition, new Vector3(0, 0f, 0.75f), Time.deltaTime * 10);
            if (ballThrowed)
            {
                ball.transform.position = Vector3.Lerp(ball.transform.position, positionToThrow, Time.deltaTime);
            }
        }
        else // Movement
        {
            _animator.SetBool("Run", true);
            _animator.SetBool("HoldBall", false);
            _animator.SetBool("Throw", false);
            transform.position += transform.forward * Time.deltaTime * moveSpeed;
            // Rotating
            if (!isRotating)
            {
                if(Mathf.Abs(Mathf.Abs(transform.position.x) - levelManager.sizeX / 2) <= 5 || Mathf.Abs(Mathf.Abs(transform.position.z) - levelManager.sizeY / 2) <= 5)
                {
                    StartCoroutine(ChangeDirectionCoroutine(true));
                }
                else if (timeToChangeDirection > changeDirectionTimeLimit)
                {
                    StartCoroutine(ChangeDirectionCoroutine(false));
                }

            }

            foreach (var player in otherPlayers)
            {
                float distanceWithPlayer = Vector3.Distance(transform.position, player.transform.position);

                if (distanceWithPlayer < 3f)
                {
                    int hardnessLevel = 0;
                    if (hardness)
                        hardnessLevel = Random.Range(0, 7);
                    else
                        hardnessLevel = Random.Range(0, 3);

                    if (Random.Range(0, 10) < hardnessLevel && throwableTime > throwableTimeLimit)
                    {
                        throwToPlayer = player;
                        StartCoroutine(Throw());
                        throwableTime = 0;
                    }
                    else
                    {
                        if (!isRotating)
                        {
                            isRotating = true;
                            StartCoroutine(ChangeDirectionCoroutine(false));
                        }
                    }

                    break;
                }
            }
        }
    }

    IEnumerator ChangeDirectionCoroutine(bool status)
    {
        isRotating = true;
        if (status == true)
        {
            transform.LookAt(new Vector3(0, 0, 0));
        }
        else
        {
            float angle = Random.Range(0f, 360f);
            Vector3 rot = new Vector3(0, angle, 0);
            transform.rotation = Quaternion.Euler(rot);
        }
        timeToChangeDirection = 0;
        yield return new WaitForSeconds(changeDirectionTimeLimit / 2f);
        isRotating = false;
    }
    IEnumerator Throw()
    {
        throwing = true;
        _animator.SetBool("Throw", true);
        _animator.SetBool("HoldBall", false);
        _animator.SetBool("Run", false);
        yield return new WaitForSeconds(0.2f);
        _animator.SetBool("HoldBall", true);
        _animator.SetBool("Throw", false);
        _animator.SetBool("Run", false);
        ballThrowed = true;
        transform.LookAt(throwToPlayer.transform);
        positionToThrow = ball.transform.position + transform.forward * 150;
        positionToThrow.y -= 0.6f;
        yield return new WaitForSeconds(1f);
        ballThrowed = false;
        throwing = false;
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
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
