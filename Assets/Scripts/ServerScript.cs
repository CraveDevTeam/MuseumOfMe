using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Networking;

public class ServerScript : MonoBehaviour
{
    public static ServerScript Instance;
    public static bool initialized;
    public string ServerAddress;
    public bool IDValid = false;

    void Awake()
    {
        try
        {
            ServerAddress = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/ServerAddress.txt");
            Debug.Log("Connected");
        }
        catch
        {
            System.IO.File.WriteAllText(Application.streamingAssetsPath + "/ServerAddress.txt", "[Server Address]");
            ServerAddress = "ERROR";
            Debug.Log("ERROR");
        }
        Instance = this;
    }

    public IEnumerator SetAppData(AppData AppData, Action<string> Callback = null)
    {
        string Response = "";
        yield return StartCoroutine(SetAppData(InnerCallback =>
        {
            Response = InnerCallback;
        }, AppData.GUID, AppData.AppName, AppData.AppValues, AppData.Image, AppData.GUID));
        if (Callback != null && Response != "")
            Callback(Response);
    }

    public IEnumerator SetAppData(Action<string> Callback = null, string GUID = "", string AppName = "", string AppValues = "", Texture2D File = null,
        string FileName = "", string DIRID = "", string QRID = "", Texture2D Selfie = null, Texture2D[] Photo = null, string NickName = "", string CharValues = "", string TimeUpdate = "", string DateUpdate = "")
    {
        string Response = "";
        string URL = "http://" + ServerAddress + "/" + "index.php";
        WWWForm Form = new WWWForm();
        Form.AddField("Function", "SetAppData");
        Form.AddField("GUID", GUID);
        Form.AddField("AppName", AppName);
        Form.AddField("AppValues", AppValues);

        //neo
        Form.AddField("DIRID", DIRID);
        Form.AddField("QRID", QRID);
        Form.AddField("NickName", NickName);
        Form.AddField("CharValues", CharValues);
        Form.AddField("DateUpdate", DateUpdate);

        if (File != null)
        {
            //Form.AddField("FileName", FileName == "" ? GUID : "");
            //Form.AddBinaryData("FileBytes", File.EncodeToJPG(), FileName);
            Form.AddField("FileName", FileName == "" ? DIRID : "");
            Form.AddBinaryData("FileBytes", File.EncodeToJPG(), FileName);
        }
        using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
        {
            Request.timeout = 10;
            Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return Request.SendWebRequest();
            Debug.Log(Request.downloadHandler.text);
            if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
            {
                Response = Request.downloadHandler.text;
            }
        }

        if (Callback != null && Response != "")
            Callback(Response);

    }

    public IEnumerator UploadImage(Action<bool> Callback, string AppName, string FileName, Texture2D File)
    {
        if (File != null)
        {
            string Response = "";
            string URL = "http://" + ServerAddress + "/" + "index.php";
            WWWForm Form = new WWWForm();
            Form.AddField("Function", "UploadImage");
            Form.AddField("AppName", AppName);
            Form.AddField("FileName", FileName);
            Form.AddBinaryData("FileBytes", File.EncodeToJPG(), FileName);


            using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
            {
                Request.timeout = 10;
                Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
                yield return Request.SendWebRequest();

                if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
                {
                    Response = Request.downloadHandler.text;
                }
            }
            Debug.Log(Response);
            if (!Response.ToLower().Contains("error"))
                Callback(true);
            else
                Callback(false);
        }
        else
            Callback(false);
    }

    public IEnumerator GetAppData(Action<AppData[]> Callback, string GUID = "", string AppName = "", string DateCreated = "", int Limit = 20, string DIRID = "", string QRID = "", Texture2D Selfie = null, string NickName = "", string CharValues = "", string DateUpdate = "")
    {
        string Response = "";
        AppData[] AppData = null;
        string URL = "http://" + ServerAddress + "/" + "Index.php";
        WWWForm Form = new WWWForm();
        Form.AddField("Function", "GetAppData");
        Form.AddField("GUID", GUID);
        Form.AddField("AppName", AppName);
        Form.AddField("DateCreated", DateCreated);
        Form.AddField("Limit", Limit);

        //neo
        Form.AddField("DIRID", DIRID);
        Form.AddField("QRID", QRID);
        Form.AddField("NickName", NickName);
        Form.AddField("CharValues", CharValues);
        Form.AddField("DateUpdate", DateUpdate);

        using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
        {
            Request.timeout = 10;
            Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return Request.SendWebRequest();
            Debug.Log(Request.downloadHandler.text);
            if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
            {
                Response = Request.downloadHandler.text;
            }
        }

        if (Response != "")
        {
            try
            {
                AppData = Json.FromJson<AppData>(Response);
            }
            catch
            {
                Debug.Log("No Data Received\n" + Response);
            }

        }
        Callback(AppData);

    }

    public IEnumerator Ping(Action<bool> Callback)
    {
        string Response = "";
        string URL = "http://" + ServerAddress;
        WWWForm Form = new WWWForm();

        using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
        {
            Request.timeout = 10;
            Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
            yield return Request.SendWebRequest();

            if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
            {
                Response = Request.downloadHandler.text;
            }
        }
        Callback(Response.Contains("Connected"));
    }

    [Obsolete]
    public IEnumerator GrabImage(string FileName, string Path, Action<Texture2D> Callback)
    {

        string URL = "http://" + ServerAddress + "/Images/" + Path + "/" + FileName;
        WWW www = new WWW(URL);
        Debug.Log(URL);
        yield return www;
        if (www.error == null)
        {
            Callback(www.texture);
        }
        else
        {
            Callback(null);
        }
    }

    public IEnumerator CheckData(string DirID)
    {
        string Response = "";
        string URL = "http://" + ServerAddress + "/mom/" + "CheckData.php";
        WWWForm form = new WWWForm();
        form.AddField("DirID", DirID);

        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                IDValid = true;
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                IDValid = false;
            }
        }
    }

    [System.Serializable]
    public class AppData
    {
        public string GUID = "";
        public string AppName = "";
        public string AppValues = "";
        public string DateCreated = "";
        [System.NonSerialized] public Texture2D Image;
        [System.NonSerialized] public System.Object Values;

        //neo
        public string DIRID = "";
        public string QRID = "";
        [System.NonSerialized] public Texture2D Selfie;
        [System.NonSerialized] public Texture2D[] Photo;
        public string NickName = "";
        public string CharValues = "";
        public string DateUpdate = "";
    }

    public static class Json
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToJson<T>(T[] array)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToJson<T>(T[] array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T>();
            wrapper.Items = array;
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }

    class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }
}

