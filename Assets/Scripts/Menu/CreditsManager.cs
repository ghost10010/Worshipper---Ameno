using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour {

    [SerializeField]
    private GameObject holoFramePrefab, holoConnectorPrefab, backgroundImagePrefab;

    [SerializeField]
    private float connectorTilingDistance = 440, backgroundTilingDistance = 475, scrollSpeed = 25, connectorYPos = -780, bgImgYPos = 780;

    [SerializeField]
    private Vector3[] frameRelativePositions;

    [SerializeField]
    private string[] names;

    private int namesIndex;

    private List<GameObject> holoConnectors;
    public List<GameObject> backgroundImages;

	// Use this for initialization
	void Start () {
        MusicScript.instance.PlayMenuMusic();

        holoConnectors = new List<GameObject>();
        backgroundImages = new List<GameObject>();

        //Instantiate the first holoconnectors and background images
        for (int i = -1; i < (names.Length / 2); i++)
        {
            GameObject holoConnector = Instantiate(holoConnectorPrefab);
            holoConnector.transform.SetParent(this.transform);
            holoConnector.transform.localPosition = new Vector3(0, connectorTilingDistance * i, 0);
            holoConnector.transform.localScale = new Vector3(1, 1, 1);
            holoConnectors.Add(holoConnector);

            GameObject backgroundImage = Instantiate(backgroundImagePrefab);
            backgroundImage.transform.SetParent(this.transform);
            backgroundImage.transform.localPosition = new Vector3(0, backgroundTilingDistance * i, 0);
            backgroundImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            backgroundImages.Add(backgroundImage);
        }

        //Instantiate first holoframes starting at random name in array
        namesIndex = Random.Range(0, names.Length);

        for (int i = 0; i < holoConnectors.Count*2; i++) //create 2 frames for each holoconnector
        {
            namesIndex++;
            if (namesIndex == names.Length)
            {
                namesIndex = 0;
            }

            if (names[namesIndex] != null && names[namesIndex].Trim() != "") //If name is not empty or null, create frame (leaves an open space otherwise)
            {
                GameObject newHoloFrame = Instantiate(holoFramePrefab);
                Text holoFrameText = newHoloFrame.GetComponentInChildren<Text>();

                string name = names[namesIndex].Replace("\\n", "\n");
                holoFrameText.text = name;

                newHoloFrame.transform.SetParent(holoConnectors[(int)i / 2].transform);
                if (i % 2 == 0) //if index is divisible by 2, create frame on the left side of holoconnector
                {
                    newHoloFrame.transform.localPosition = frameRelativePositions[0];
                }
                else //else, create on the right side of holoconnector
                {
                    newHoloFrame.transform.localPosition = frameRelativePositions[1];
                }
                newHoloFrame.transform.localScale = new Vector3(1, 1, 1);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        foreach (GameObject holoConnector in holoConnectors)
        {
            holoConnector.transform.position = Vector3.Lerp(holoConnector.transform.position, holoConnector.transform.position - holoConnector.transform.up, Time.deltaTime * scrollSpeed);
        }
        foreach (GameObject backGroundImage in backgroundImages)
        {
            backGroundImage.transform.position = Vector3.Lerp(backGroundImage.transform.position, backGroundImage.transform.position + backGroundImage.transform.up, Time.deltaTime * scrollSpeed);
        }

        if (Input.anyKeyDown)
        {
            Application.LoadLevel("Menu");
        }

        if (holoConnectors[0].transform.localPosition.y <= connectorYPos)
        {
            LoopHoloConnector();
        }

        if (backgroundImages[backgroundImages.Count-1].transform.localPosition.y >= bgImgYPos)
        {
            LoopBackgroundImage();
        }
	}

    void LoopHoloConnector()
    {
        //destroy old holoconnector object
        Destroy(holoConnectors[0]);
        holoConnectors.RemoveAt(0);

        //create new holoconnector
        GameObject holoConnector = Instantiate(holoConnectorPrefab);
        holoConnector.transform.SetParent(this.transform);
        holoConnector.transform.localPosition = new Vector3(0, holoConnectors[holoConnectors.Count-1].transform.localPosition.y + connectorTilingDistance, 0);
        holoConnector.transform.localScale = new Vector3(1, 1, 1);
        holoConnectors.Add(holoConnector);

        for (int i = 0; i < 2; i++) //create 2 frames for this holoconnector
        {
            namesIndex++;
            if (namesIndex == names.Length)
            {
                namesIndex = 0;
            }

            GameObject newHoloFrame = Instantiate(holoFramePrefab);
            Text holoFrameText = newHoloFrame.GetComponentInChildren<Text>();

            string name = names[namesIndex].Replace("\\n", "\n");
            holoFrameText.text = name;

            newHoloFrame.transform.SetParent(holoConnectors[holoConnectors.Count-1].transform);
            if (i % 2 == 0) //if index is divisible by 2, create frame on the left side of holoconnector
            {
                newHoloFrame.transform.localPosition = frameRelativePositions[0];
            }
            else //else, create on the right side of holoconnector
            {
                newHoloFrame.transform.localPosition = frameRelativePositions[1];
            }
            newHoloFrame.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void LoopBackgroundImage()
    {
        //destroy old holoconnector object
        Destroy(backgroundImages[backgroundImages.Count - 1]);
        backgroundImages.RemoveAt(backgroundImages.Count - 1);

        //create new holoconnector
        GameObject backgroundImage = Instantiate(backgroundImagePrefab);
        backgroundImage.transform.SetParent(this.transform);
        backgroundImage.transform.localPosition = new Vector3(0, backgroundImages[0].transform.localPosition.y - backgroundTilingDistance, 0);
        backgroundImage.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        backgroundImages.Insert(0, backgroundImage);
    }
}
