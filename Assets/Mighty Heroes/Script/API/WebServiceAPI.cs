using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebServiceAPI
{
    public static IEnumerator PostRequest(string url, WWWForm data, Action<string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, data))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                UIManager.AddDebugMessage(www.error, LogVerbose.Error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }
}
