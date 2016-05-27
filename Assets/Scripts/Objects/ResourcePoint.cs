using UnityEngine;
using System.Collections;

public class ResourcePoint : MonoBehaviour
{
    public int amount = 100;
    public ResourceType type;

    public string key;

    public Village village;

    public bool isInUse = false;

	void Start ()
    {
        InitResourcePoint();
	}

    public void InitResourcePoint()
    {
        if (this.transform.parent != null)
        {
            village = this.transform.parent.GetComponentInParent<Village>();

            if (village != null)
            {
                village.AddObject(this);
            }
        }    
    }

    public void GatherResources(int amount)
    {
        this.amount -= amount;

        if (amount <= 0)
        {
            village.RemoveObject(this);
            Destroy(this.gameObject);
        }

        isInUse = false;
    }

    void OnMouseOver()
    {
        Village parentVillage = transform.root.GetComponent<Village>();

        if (!GameManger.instance.isMenu && !GameManger.instance.movingObjects)
        {
            if (parentVillage == null)
            {
                UIManager.instance.SetResourcePoint(this);
            }
            else if (parentVillage.owner == GameManger.instance.GetLocalPlayer().playerID)
            {
                UIManager.instance.SetResourcePoint(this);
            }
        }
    }
    void OnMouseExit()
    {
        if (!GameManger.instance.isMenu && !GameManger.instance.movingObjects)
        {
            UIManager.instance.TogglePanelState("ResourcePoint", false);
        }
    }
}