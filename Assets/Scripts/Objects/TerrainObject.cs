using UnityEngine;
using System.Collections;

[System.Serializable]
public class TerrainObject : MonoBehaviour
{
    public int size = 2048;

    public Vector3 startPosition1;
    public Vector3 startPosition2;

    void OnMouseDown()
    {
        if (GameManger.instance != null)
        {
            if (!GameManger.instance.isLevelEditor && !GameManger.instance.isMenu)
            {
                if (!UIManager.instance.IsMouseOverUI())
                {
                    if (!GameManger.instance.isBuilding)
                    {
                        UIManager.instance.UnselectVillager("Click");
                        UIManager.instance.UnselectBuilding();// TogglePanelState("BuildingInfo", false);
                        GameManger.instance.DeselectVillage();
                    }
                }
            }
        }
    }

    public float GetTerrainHeight(Vector3 position)
    {
        Vector3 fwd = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        
        if (Physics.Raycast(position, fwd, out hit, 100.0f, 01111111))
        {
            if (hit.collider.name == "Terrain")
            {
                return hit.point.y;
            }
        }

        return 0.0f;
    }
}