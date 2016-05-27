using UnityEngine;
using System.Collections;

public class Mouse : MonoBehaviour
{
    public static Mouse instance;

    public Vector3 delta = Vector3.zero;
    public Vector3 lastPos = Vector3.zero;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            delta = Input.mousePosition - lastPos;

            lastPos = Input.mousePosition;
        }
    }

	void Awake()
    {
        instance = this;
    }

    public Vector3 GetMouseWorldPosition()
    {
        RaycastHit vHit = new RaycastHit();
        Ray vRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (vHit.collider != null)
        {
            Debug.Log(vHit.collider.name);
        }
        if (Physics.Raycast(vRay, out vHit, 100.0f, 01111111))
        {
            return vHit.point;
        }

        return Vector3.zero;
    }
    public float GetMouseSpeed()
    {
        return delta.magnitude;
    }
}