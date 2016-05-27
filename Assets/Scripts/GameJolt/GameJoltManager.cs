using UnityEngine;
using System.Collections;

public class GameJoltManager : MonoBehaviour
{
    public static GameJoltManager instance;

    public bool loggedIn = false;
    public bool test = true;

    private string winsKey = "wins";
    private string manaKey = "usedMana";
    private string villagerKey = "";
    private string pickupKey = "pickupMovable";

    void Awake()
    {
        instance = this;
    }

    public void AddTrophy(int trophyId)
    {
        if (loggedIn)
        {
            GameJolt.API.Trophies.Get(trophyId, (GameJolt.API.Objects.Trophy trophy) => {
                if (!trophy.Unlocked)
                {
                    GameJolt.API.Trophies.Unlock(trophyId);
                }
                if (test)
                {
                    GameJolt.API.Trophies.Unlock(trophyId);
                }
            });
        }
    }
    public void AddData(string key, string value)
    {
        GameJolt.API.DataStore.Get(key, true, (string dataValue) =>
        {
            if (dataValue == null)
            {
                GameJolt.API.DataStore.Set(key, value, true, (bool success) =>
                {
                    
                });
            }
            else
            {
                GameJolt.API.DataStore.Update(key, value, GameJolt.API.DataStoreOperation.Add, true, (string updateValue) =>
                {
                    
                });
            }
        });
    }
    public void AddScore(int score, string extraData)
    {
        string scoreText = score + " score"; // A string representing the score to be shown on the website.
        int tableID = 123022; // Set it to 0 for main highscore table.
        
        if (loggedIn)
        {
            GameJolt.API.Scores.Add(score, scoreText, tableID, extraData, (bool success) => {
            });
        }
        else
        {
            string guestName = "Guest";
            GameJolt.API.Scores.Add(score, scoreText, guestName, tableID, extraData, (bool success) => {
            });
        }
    }

    public void CheckTrophyWins()
    {
        GameJolt.API.DataStore.Get(winsKey, true, (string dataValue) =>
        {
            if (dataValue != null)
            {
                int trophyId = -1;

                switch(dataValue)
                {
                    case "1": //Conqueror: Adysseus
                        trophyId = 48571;
                        break;
                    case "10": //Conqueror: Aeneas
                        trophyId = 48572;
                        break;
                    case "25": //Conqueror: Achilles
                        trophyId = 48573;
                        break;
                    case "50": //Conqueror: Heracles
                        trophyId = 48574;
                        break;
                }

                if (trophyId != -1)
                {
                    AddTrophy(trophyId);
                }
            }
        });
    }
    public void CheckTrophyPickups()
    {
        GameJolt.API.DataStore.Get(winsKey, true, (string dataValue) =>
        {
            if (dataValue != null)
            {
                int trophyId = -1;

                switch (dataValue)
                {
                    case "10":
                        trophyId = 48626;
                        break;
                    case "25":
                        trophyId = 48627;
                        break;
                    case "50":
                        trophyId = 48628;
                        break;
                    case "100":
                        trophyId = 48629;
                        break;
                }

                if (trophyId != -1)
                {
                    AddTrophy(trophyId);
                }
            }
        });
    }
    public void CheckTrophyMana()
    {
        GameJolt.API.DataStore.Get(winsKey, true, (string dataValue) =>
        {
            if (dataValue != null)
            {
                int trophyId = -1;

                switch (dataValue)
                {
                    case "100": //Demi God
                        trophyId = 48563;
                        break;
                    case "1000": //Lesser God
                        trophyId = 48564;
                        break;
                    case "10000": //God
                        trophyId = 48565;
                        break;
                    case "100000": //Titan
                        trophyId = 48566;
                        break;
                }

                if (trophyId != -1)
                {
                    AddTrophy(trophyId);
                }
            }
        });
    }
    public void CheckTrophyVillager()
    {
        GameJolt.API.DataStore.Get(winsKey, true, (string dataValue) =>
        {
            if (dataValue != null)
            {
                int trophyId = -1;

                switch (dataValue)
                {
                    case "1": //Cyclops
                        trophyId = 48567;
                        break;
                    case "5": //Cerberus
                        trophyId = 48568;
                        break;
                    case "15": //Tartarus
                        trophyId = 48569;
                        break;
                    case "50": //Hades
                        trophyId = 48570;
                        break;
                }

                if (trophyId != -1)
                {
                    AddTrophy(trophyId);
                }
            }
        });
    }
}