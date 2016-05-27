using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crate : MonoBehaviour
{
    public ResourceType resourceType = ResourceType.None;
    public int amount = 5;

    [SerializeField] private List<GameObject> resourceParticles;

    public GameObject GetParticle()
    {
        return resourceParticles[(int)resourceType];
    }
}
