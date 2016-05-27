using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Networking;

public class Grow : NetworkBehaviour
{
    public int age = 1;

    public float growTimer = 5.0f;
    public float growTime = 5.0f;

    public float growAmount = 0.1f;

    public bool isPickedup = false;
    public bool isGrowing = false;
    public bool isPlacedByEditor = false;
    public bool isLoaded = false;

    void Start()
    {
        if (!isPlacedByEditor)
        {
            GrowTree();
        }
    }
    
    void Update()
    {
        if (isGrowing && !isPickedup)
        {
            growTimer -= Time.deltaTime;

            float scaleIncrease = (growAmount / growTime) * Time.deltaTime;

            SetScaleIncrease(scaleIncrease);

            if (growTimer < 0)
            {
                growTimer = growTime;

                age++;

                if (age == 10)
                {
                    isGrowing = false;
                }
            }
        }
    }

    public void SetAge(int age)
    {
        this.age = age;

        if (age >= 9)
        {
            isGrowing = false;
        }
        else
        {
            isGrowing = true;
        }

        float scale = 0.1f * age;
        this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
    }

    private void SetScaleIncrease(float increase)
    {
        Vector3 scale = this.gameObject.transform.localScale;
        scale.x += increase;
        scale.y += increase;
        scale.z += increase;

        this.gameObject.transform.localScale = scale;
    }

    public void GrowTree()
    {
        if (age < 10)
        {
            isGrowing = true;

            this.gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }
    }

    public void PickupObject()
    {
        isPickedup = true;
    }
    public void DropObject()
    {
        isPickedup = false;
    }
}