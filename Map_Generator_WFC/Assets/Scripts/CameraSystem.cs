using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private WorldBorder border;

    //настройки управления камерой вкл/вкл
    [SerializeField] private bool useEdgeScrolling = true;
    [SerializeField] private bool useDragPan = true;
    [SerializeField] private bool useWASD = true;
    [SerializeField] private bool useZoomField = false;
    [SerializeField] private bool useZoomMove = false;
    [SerializeField] private bool useZoomY = true;

    //настройки зума
    [SerializeField] private float fieldOfViewMax = 50;
    [SerializeField] private float fieldOfViewMin = 10;
    [SerializeField] private float followOffsetMax = 50f;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMaxY = 50f;
    [SerializeField] private float followOffsetMinY = 10f;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private float dragPanSpeed = -0.08f;
    [SerializeField] private float zoomSpeed = 10f;

    [SerializeField] private float borderSize = 20;


    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private float targetFieldOfView = 50;
    private Vector3 followOffset;


    private void Awake()
    {
        followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }



    private void Update()
    {

        if (useWASD) HandleCameraMovement();
        if (useEdgeScrolling) HandleEdgeScrolling();
        if (useDragPan) HandleDragPan();

        HandleCameraRotation();

        if (useZoomField) HandleCameraZoom_FieldOfView();
        if (useZoomMove) HandleCameraZoom_MoveForward();
        if (useZoomY) HandleCameraZoom_LowerY();

        HandleCameraBorder();

    }

    private void HandleCameraBorder() //ограничение движения камеры на основе границ игрового мира
    {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, border.mapLeft + borderSize, border.mapRight - borderSize),
            transform.position.y,
            Mathf.Clamp(transform.position.z, border.mapBottom + borderSize, border.mapTop - borderSize));

    }

    private void HandleCameraMovement() //перемещение на кнопки
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleEdgeScrolling()//прокрутка мышью    
    {
        Vector3 inputDir = Vector3.zero;

        int edgeScrollSize = 20;
        if (Input.mousePosition.x < edgeScrollSize) inputDir.x = -1f;
        if (Input.mousePosition.y < edgeScrollSize) inputDir.z = -1f;
        if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x = +1f;
        if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z = +1f;

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleDragPan() //перетаскивание мышью
    {
        Vector3 inputDir = Vector3.zero;


        if (Input.GetMouseButtonDown(1)) //пкм нажата
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition;

        }

        if (Input.GetMouseButtonUp(1))//пкм отпущена
        {
            dragPanMoveActive = false;

        }

        if (dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * dragPanSpeed;

            lastMousePosition = Input.mousePosition;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraRotation() //поворот камеры
    {
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);

    }

    private void HandleCameraZoom_FieldOfView() //зум на основе поля зрения
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 5;
        }
        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

        cinemachineVirtualCamera.m_Lens.FieldOfView =
            Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        //cinemachineVirtualCamera.m_Lens.FieldOfView = targetFieldOfView;
    }
    private void HandleCameraZoom_MoveForward() //зум на основе приближения
    {
        Vector3 zoomDir = followOffset.normalized;
        float zoomAmount = 3f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset -= zoomDir * zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset += zoomDir * zoomAmount;
        }

        if (followOffset.magnitude < followOffsetMin)
        {
            followOffset = zoomDir * followOffsetMin;
        }
        if (followOffset.magnitude > followOffsetMax)
        {
            followOffset = zoomDir * followOffsetMax;
        }

        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);

    }

    private void HandleCameraZoom_LowerY() //зум уменьшение Y
    {
        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }

        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);


        cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);

    }
}
