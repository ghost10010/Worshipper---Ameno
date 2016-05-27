using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResourcePointInfoController : MonoBehaviour
{
    [SerializeField] private GameObject pnlResourcePoint;
    [SerializeField] private Text txtResorceName;
    [SerializeField] private Text txtResourceAmount;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetState(bool state)
    {
        pnlResourcePoint.SetActive(state);
    }

    public void ShowResourcePointInformation(ResourcePoint point)
    {
        txtResorceName.text = Language.instance.GetTextWithKey(point.key);
        txtResourceAmount.text = point.amount.ToString();
    }
}
