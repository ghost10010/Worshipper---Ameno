using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;

    private float scrollEdge = 0.01f;

    private float terrainHeight = 0.0f;

    [SerializeField] private float scrollSpeed = 25.0f;
    [SerializeField] private float dragSpeed = 25.0f;
    [SerializeField] private float zoomSpeed = 10.0f;
    [SerializeField] private float panSpeed = 25.0f;

    [SerializeField] private Vector2 levelRange = new Vector2(-25.0f, 25.0f);
    [SerializeField] private Vector2 zoomRange = new Vector2(10.0f, 40.0f);
    [SerializeField] private Vector2 panRange = new Vector2(30.0f, 45.0f);

    private Vector3 initPos;
    private Vector3 initRot;

    public bool mouseBorderInput = true;

    private bool moveToHome = false;

    public TerrainObject terrainObject;
    public bool blockZoom = false;
    public bool blockPan = false;
    public bool blockMoveRestriction = false;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        initPos = this.transform.position;
        initRot = this.transform.eulerAngles;
    }

    void Update()
    {
        Vector3 translation = Vector3.zero;
        Vector3 rotation = Vector3.zero;

        if (Input.GetMouseButton(2)) //Mouse drag //Middle mouse button
        {
            translation -= new Vector3(Input.GetAxis("Mouse X") * dragSpeed * Time.deltaTime, 0,
                                       Input.GetAxis("Mouse Y") * dragSpeed * Time.deltaTime);
        }
        else //Mouse edge of screen
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) || (Input.mousePosition.y >= Screen.height * (1 - scrollEdge) && mouseBorderInput)) //Up
            {
                translation += mainCamera.transform.forward * scrollSpeed * Time.deltaTime;
                translation.y = 0; //ignore camera x rotation
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow) || (Input.mousePosition.y <= Screen.height * scrollEdge && mouseBorderInput)) //Down
            {
                translation += -mainCamera.transform.forward * scrollSpeed * Time.deltaTime;
                translation.y = 0; //ignore camera x rotation
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || (Input.mousePosition.x <= Screen.width * scrollEdge && mouseBorderInput)) //Left
            {
                translation += -mainCamera.transform.right * scrollSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || (Input.mousePosition.x >= Screen.width * (1 - scrollEdge) && mouseBorderInput)) //Right
            {
                translation += mainCamera.transform.right * scrollSpeed * Time.deltaTime;
            }
        }

        //Zoom
        if (!blockZoom)
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

            if (zoomDelta != 0)
            {
                translation -= Vector3.up * zoomDelta * zoomSpeed;
            }
            //Pan with camera zoom
            float pan = mainCamera.transform.eulerAngles.x - zoomDelta * zoomSpeed;
            pan = Mathf.Clamp(pan, panRange.x, panRange.y);
            if (zoomDelta < 0 || mainCamera.transform.position.y < (zoomRange.y / 2))
            {
                mainCamera.transform.eulerAngles = new Vector3(pan, 0, 0);
            }
        }

        //Rotate //add speed
        if (Input.GetKey(KeyCode.Q))
        {
            rotation.y -= 1;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotation.y += 1;
        }
        if (Input.GetKey(KeyCode.Home))
        {
            moveToHome = true;
            //rotation = initRot;
        }

        MoveToPoint(LimitMovement(translation), rotation);

        if (!blockMoveRestriction)
        {
            SetTerrainOffset();
        }
    }

    private Vector3 LimitMovement(Vector3 newVector)
    {
        Vector3 translation = newVector;
        Vector3 desiredPosition = GetComponent<Camera>().transform.position + translation;
        if (desiredPosition.x < levelRange.x || levelRange.y < desiredPosition.x)
        {
            translation.x = 0;
        }
        if (desiredPosition.y < zoomRange.x || zoomRange.y < desiredPosition.y - terrainHeight)
        {
            translation.y = 0;
        }
        if (desiredPosition.z < levelRange.x || levelRange.y < desiredPosition.z)
        {
            translation.z = 0;
        }

        return translation;
    }

    private void SetTerrainOffset()
    {
        if (terrainObject != null)
        {
            Vector3 terrainOffset = Vector3.zero;

            float terrainHeightOld = terrainHeight;

            terrainHeight = terrainObject.GetTerrainHeight(new Vector3(mainCamera.transform.position.x, 40.0f, mainCamera.transform.position.y));

            terrainOffset.y += (terrainHeight - terrainHeightOld);
            mainCamera.transform.position += terrainOffset;
        }
    }
    private void MoveToPoint(Vector3 point, Vector3 rotation)
    {
        Vector3 newPosition = point;
        newPosition.y -= terrainHeight;
        mainCamera.transform.position += point;
        mainCamera.transform.eulerAngles += rotation;
    }
    private void MoveToHome()
    {
        if (Vector3.Distance(mainCamera.transform.position, initPos) > 1)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, initPos, Time.deltaTime);
        }

        if (Quaternion.Angle(mainCamera.transform.rotation, Quaternion.Euler(initRot)) > 1)
        {
            mainCamera.transform.rotation = Quaternion.Lerp(mainCamera.transform.rotation, Quaternion.Euler(initRot), Time.deltaTime);
        }

        Debug.Log(Quaternion.Angle(mainCamera.transform.rotation, Quaternion.Euler(initRot)));

        if (Vector3.Distance(mainCamera.transform.position, initPos) < 1 && Quaternion.Angle(mainCamera.transform.rotation, Quaternion.Euler(initRot)) < 1)
        {
            moveToHome = false;
            mainCamera.transform.position = initPos;
            mainCamera.transform.eulerAngles = initRot;
        }
    }

    public void SetLevelWidth(int width)
    {
        levelRange.x = -(width / 2);
        levelRange.y = width / 2;
    }
    public void SetTerrainObject(TerrainObject terrainObject)
    {
        this.terrainObject = terrainObject;
    }
}