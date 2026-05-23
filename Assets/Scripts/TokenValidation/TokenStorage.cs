using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TokenStorage
{
    private const string TOKEN_KEY = "MAPBOX_TOKEN";

    public static void SaveToken(string token)
    {
        PlayerPrefs.SetString(TOKEN_KEY, token);
        PlayerPrefs.Save();
    }

    public static string LoadToken()
    {
        return PlayerPrefs.GetString(TOKEN_KEY, "");
    }
}
