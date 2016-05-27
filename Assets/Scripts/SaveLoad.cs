using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;

    private XmlDocument xmlSave;
    private XmlElement xmlSaveBaseNode;

    private bool loaded = false;

    void Awake()
    {
        instance = this;
    }

    public void Save()
    {
        Debug.Log("Save");
        string xmlString = "";
        xmlString += "<Ameno>\n";

        GameObject[] villages = GameObject.FindGameObjectsWithTag("Village");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        ResourcePoint[] resourceObjects = GameObject.FindObjectsOfType<ResourcePoint>();
        
        for (int villageIndex = 0; villageIndex < villages.Length; villageIndex++)
        {
            Village saveVillage = villages[villageIndex].GetComponent<Village>();
            xmlString += "<Village>\n";
            xmlString += "<Owner>" + saveVillage.owner + "</Owner>\n";
            xmlString += "<Food>" + saveVillage.food + "</Food>\n";
            xmlString += "<Wood>" + saveVillage.wood + "</Wood>\n";
            xmlString += "<Iron>" + saveVillage.iron + "</Iron>\n";
            xmlString += "<Villagers>" + saveVillage.villagers + "</Villagers>\n";
            xmlString += "<MaxVillagers>" + saveVillage.maxVillagers + "</MaxVillagers>\n";
            xmlString += "<Soldiers>" + saveVillage.soldiers + "</Soldiers>\n";
            xmlString += "<MaxSoldiers>" + saveVillage.maxSoldiers + "</MaxSoldiers>\n";
            xmlString += "<Buildings>\n";

            for (int buildingIndex = 0; buildingIndex < saveVillage.buildings.Count; buildingIndex++)
            {
                xmlString += "<Building>\n";
                xmlString += "<Type>" + (int)saveVillage.buildings[buildingIndex].GetComponent<Building>().buildingType + "</Type>\n";
                xmlString += "<Position>" + saveVillage.buildings[buildingIndex].transform.position + "</Position>\n";
                xmlString += "<Rotation>" + saveVillage.buildings[buildingIndex].transform.eulerAngles + "</Rotation>\n";
                xmlString += "</Building>\n";
            }

            xmlString += "</Buildings>\n";
            xmlString += "</Village>\n";
        }
        for (int index = 0; index < players.Length; index++)
        {
            Player player = players[index].GetComponent<Player>();

            xmlString += "<Player>\n";
            xmlString += "<Id>" + player.playerID + "</Id>\n";
            xmlString += "<Mana>" + player.mana + "</Mana>\n";
            xmlString += "<Score>" + player.score + "</Score>\n";
            xmlString += "</Player>\n";
        }

        xmlString += "<TerrainObjects>";

        for (int index = 0; index < resourceObjects.Length; index++)
        {
            xmlString += "<Object>\n";

            xmlString += "<Type>" + (int)resourceObjects[index].type + "</Type>\n";
            xmlString += "<Amount>" + resourceObjects[index].amount + "</Amount>\n";

            if (resourceObjects[index].transform.root.name.Contains("Village"))
            {
                xmlString += "<Parent>" + resourceObjects[index].transform.root.GetComponent<Village>().owner + "</Parent>\n";
            }
            else
            {
                xmlString += "<Parent>-1</Parent>\n";
            }

            if (resourceObjects[index].type == ResourceType.Food || resourceObjects[index].type == ResourceType.Wood)
            {
                Grow growObject = resourceObjects[index].GetComponent<Grow>();

                if (growObject != null)
                {
                    xmlString += "<Age>" + growObject.age + "</Age>\n";
                }
                else
                {
                    xmlString += "<Age>10</Age>\n";
                }
            }
            else
            {
                xmlString += "<Age>10</Age>\n";
            }

            string name = resourceObjects[index].name;
            if (name.Contains("(Clone)"))
            {
                name = name.Remove(name.IndexOf('('));
            }
            string id = name.Substring(name.Length - 1);
            Debug.Log("ID: " + id);
            xmlString += "<Id>" + id + "</Id>\n";
            xmlString += "<Position>" + resourceObjects[index].transform.position + "</Position>\n";

            xmlString += "</Object>\n";
        }

        xmlString += "</TerrainObjects>";
        xmlString += "</Ameno>";

        WriteXml(xmlString);
    }
    public void Load()
    {
        if (!loaded)
        {
            ReadXml();
        }
    }

    public Hashtable GetPlayerData(int playerId)
    {
        Load();

        Hashtable playerData = new Hashtable();
        
        for (int index = 0; index < xmlSaveBaseNode.ChildNodes.Count; index++)
        {
            if (xmlSaveBaseNode.ChildNodes[index].Name == "Player")
            {
                if (xmlSaveBaseNode.ChildNodes[index].ChildNodes[0].InnerText == playerId.ToString())
                {   
                    playerData.Add("Mana", xmlSaveBaseNode.ChildNodes[index].ChildNodes[1].InnerText);
                    playerData.Add("Score", xmlSaveBaseNode.ChildNodes[index].ChildNodes[2].InnerText);

                    break;
                }
            }
        }

        return playerData;
    }
    public Hashtable GetVillageData(int playerId)
    {
        Load();

        Hashtable villageData = new Hashtable();

        for (int index = 0; index < xmlSaveBaseNode.ChildNodes.Count; index++)
        {
            if (xmlSaveBaseNode.ChildNodes[index].Name == "Village")
            {
                if (xmlSaveBaseNode.ChildNodes[index].ChildNodes[0].InnerText == playerId.ToString())
                {
                    Debug.Log("XmlPlayer: " + xmlSaveBaseNode.ChildNodes[index].ChildNodes[0].InnerText);
                    Debug.Log("Mana" + xmlSaveBaseNode.ChildNodes[index].ChildNodes[1].InnerText);
                    Debug.Log("Score" + xmlSaveBaseNode.ChildNodes[index].ChildNodes[2].InnerText);
                    villageData.Add("Food", xmlSaveBaseNode.ChildNodes[index].ChildNodes[1].InnerText);
                    villageData.Add("Wood", xmlSaveBaseNode.ChildNodes[index].ChildNodes[2].InnerText);
                    villageData.Add("Iron", xmlSaveBaseNode.ChildNodes[index].ChildNodes[3].InnerText);
                    villageData.Add("Villagers", xmlSaveBaseNode.ChildNodes[index].ChildNodes[4].InnerText);
                    villageData.Add("MaxVillagers", xmlSaveBaseNode.ChildNodes[index].ChildNodes[5].InnerText);
                    villageData.Add("Soldiers", xmlSaveBaseNode.ChildNodes[index].ChildNodes[6].InnerText);
                    villageData.Add("MaxSoldiers", xmlSaveBaseNode.ChildNodes[index].ChildNodes[7].InnerText);

                    break;
                }
            }
        }

        return villageData;
    }
    public Hashtable GetVillageBuildings(int playerId)
    {
        Load();
        Debug.Log("GetVilalgeBuildings");
        for (int index = 0; index < xmlSaveBaseNode.ChildNodes.Count; index++)
        {
            if (xmlSaveBaseNode.ChildNodes[index].Name == "Village")
            {
                if (xmlSaveBaseNode.ChildNodes[index].ChildNodes[0].InnerText == playerId.ToString())
                {
                    Debug.Log("Got Village: " + playerId);
                    //8 == buildings
                    
                    XmlElement xmlBuildings = (XmlElement)xmlSaveBaseNode.ChildNodes[index].ChildNodes[8];

                    return GetBuildingData(xmlBuildings);
                }
            }
        }

        return null;
    }
    private Hashtable GetBuildingData(XmlElement buildingXml)
    {
        Hashtable buildingsData = new Hashtable();

        for (int index = 0; index < buildingXml.ChildNodes.Count; index++)
        {
            buildingsData.Add(("Building" + index + "Type"), buildingXml.ChildNodes[index].ChildNodes[0].InnerText);
            buildingsData.Add(("Building" + index + "Position"), buildingXml.ChildNodes[index].ChildNodes[1].InnerText);
            buildingsData.Add(("Building" + index + "Rotation"), buildingXml.ChildNodes[index].ChildNodes[2].InnerText);
        }

        return buildingsData;
    }
    public Hashtable GetTerrainObjects()
    {
        Load();
        Debug.Log("GetTerrainObjects");
        Hashtable terrainObjects = new Hashtable();

        for (int index = 0; index < xmlSaveBaseNode.ChildNodes.Count; index++)
        {
            if (xmlSaveBaseNode.ChildNodes[index].Name == "TerrainObjects")
            {
                XmlElement xmlObjects = (XmlElement)xmlSaveBaseNode.ChildNodes[index];

                return GetObjectData(xmlObjects);
            }
        }
        Debug.Log("Return NUll");
        return null;
    }
    private Hashtable GetObjectData(XmlElement objectXml)
    {
        Hashtable objectsData = new Hashtable();
        Debug.Log("GetObjectData: " + objectXml.InnerXml);
        Debug.Log("OBjectCount: " + objectXml.ChildNodes.Count);
        for (int index = 0; index < objectXml.ChildNodes.Count; index++)
        {
            Debug.Log("AddTo: " + index);
            objectsData.Add(("Object" + index + "Type"), objectXml.ChildNodes[index].ChildNodes[0].InnerText);
            objectsData.Add(("Object" + index + "Amount"), objectXml.ChildNodes[index].ChildNodes[1].InnerText);
            objectsData.Add(("Object" + index + "Parent"), objectXml.ChildNodes[index].ChildNodes[2].InnerText);
            objectsData.Add(("Object" + index + "Age"), objectXml.ChildNodes[index].ChildNodes[3].InnerText);
            objectsData.Add(("Object" + index + "Id"), objectXml.ChildNodes[index].ChildNodes[4].InnerText);
            objectsData.Add(("Object" + index + "Position"), objectXml.ChildNodes[index].ChildNodes[5].InnerText);
        }

        return objectsData;
    }

    private void WriteXml(string xmlString)
    {
        string filePath = Application.dataPath + "/Resources/Save.xml";

        XmlDocument playerXml = new XmlDocument();
        playerXml.PreserveWhitespace = true;
        playerXml.LoadXml(xmlString);
        playerXml.Save(filePath);
    }
    private void ReadXml()
    {
        Debug.Log("ReadXml");
        //string filePath = Application.dataPath + "/../Save.xml";
        string filePath ="Save";
        Debug.Log(filePath);
        TextAsset xmlFile = (TextAsset)Resources.Load(filePath);
        Debug.Log("Got XmlFile");
        xmlSave = new XmlDocument();
        xmlSave.LoadXml(xmlFile.text);
        Debug.Log("LoadXml");
        xmlSaveBaseNode = xmlSave.DocumentElement;
        Debug.Log("Loaded");
        loaded = true;
    }
}
