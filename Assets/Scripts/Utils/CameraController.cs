using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraController : MonoBehaviour
{
    private InputManager inputManager;

    public Transform player;
    [SerializeField] private float lookSpeed = 1;
    private CinemachineFreeLook cinemachine;

    private void Awake()
    {
        cinemachine = GetComponent<CinemachineFreeLook>();
        inputManager = InputManager.Instance;
    }

    private void Start()
    {

        cinemachine.LookAt = player;
        cinemachine.Follow = player;
    }

    private void Update()
    {
        if(!cinemachine.LookAt || !cinemachine.Follow)
        {
            cinemachine.LookAt = player;
            cinemachine.Follow = player;
        }

        Vector2 rotateDirection = inputManager.rotateDirection;
        cinemachine.m_XAxis.Value += rotateDirection.x * 200 * lookSpeed * Time.deltaTime;
        cinemachine.m_XAxis.Value += rotateDirection.y * lookSpeed * Time.deltaTime;
    }
}
