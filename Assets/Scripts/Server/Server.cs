using System.Collections;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.Networking;

namespace MOMmain
{
    public class Server : MonoBehaviour
    {
        public static Server Instance;
        public static bool initialized;

        public string[] ServerAddress;
        private string Password, Token;

        string LastServerAddress = "";

        void Awake()
        {
            SetInstance();

            //Check for a file in streaming assest "ServerAddress.txt"
            LoadServerAddressesFromFile();

            //Check connection of the files
            StartCoroutine(GetConnectedServerAddress((ServerAddressCallback) =>
            {

                if (ServerAddressCallback == null)
                {
                    StartCoroutine(DetectServerAddress());
                }
            }));

            //RFID.StartRFID();
        }

        public void SetInstance()
        {
            Debug.Log("HI");
            Instance = this;

        }
        /*   
           //Set password
           public void SetPassword(string Password)
           {
               Token = Guid.NewGuid().ToString();
               this.Password = MD5.Convert(Password + Token);
           }
       */
        //Set server access data
        public void SetServerAccess(string ServerAddress, string Password = null)
        {
            SetServerAccess(new string[] { ServerAddress }, Password);
        }

        public void SetServerAccess(string[] ServerAddress, string Password = null)
        {
            this.ServerAddress = ServerAddress;
            //       SetPassword(Password == null? this.Password: Password);
        }

        //Get a specific Server Address
        public string GetServerAddress(int Index = 0)
        {
            return ServerAddress[Index];
        }

        //Get all Server Addresses
        public string[] GetAllServerAddresses()
        {
            return ServerAddress;
        }

        //Load Server Addresss from Text File
        public bool LoadServerAddressesFromFile()
        {
            try
            {
                string[] ServerAddresses = File.ReadAllText(Application.streamingAssetsPath + "/ServerAddresses.txt").Split(',');
                ServerAddress = ServerAddresses;

                for (int i = 0; i < ServerAddress.Length; i++)
                {
                    ServerAddress[i] = ServerAddress[i].Trim();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Write a single server address to Text File
        public bool SetServerAddressesToFile(string ServerAddress)
        {
            return SetServerAddressesToFile(new string[] { ServerAddress });
        }

        //Write several Server Addresses to Text File
        public bool SetServerAddressesToFile(string[] ServerAddresses)
        {
            try
            {
                string Text = "";
                foreach (string Element in ServerAddresses)
                {
                    Text = Text + Element + ",";
                }
                Text = Text.Trim(',');

                File.WriteAllText(Application.streamingAssetsPath + "/ServerAddresses.txt", Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Grab File
        public IEnumerator GrabFile(string FileName, string Path, Action<WWW> Callback)
        {

            string ServerAddress = GetServerAddress();
            //yield return StartCoroutine(GetConnectedServerAddress((ServerAddressCallback) => {
            //    ServerAddress = ServerAddressCallback;
            //}));
            string URL = "http://" + ServerAddress + "/" + Path + "/" + FileName;
            Message(URL);
            WWW www = new WWW(URL);
            yield return www;
            Callback(www);
        }

        //Upload File
        public IEnumerator UploadFile(string FileName, string Path, byte[] File, Action<bool> Callback, bool Global = true)
        {
            string Text = "";
            string ServerAddress = GetServerAddress();
            //yield return StartCoroutine(GetConnectedServerAddress((ServerAddressCallback) => {
            //    ServerAddress = ServerAddressCallback;
            //}));

            string URL = "http://" + ServerAddress + "/" + "UploadFile";

            //Upload File to the server
            WWWForm Form = new WWWForm();
            Form.AddBinaryData("FileBytes", File, FileName);
            Form.AddField("Path", Path);
            Form.AddField("Password", Password);
            Form.AddField("Token", Token);

            using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
            {
                Request.timeout = 10;
                Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
                yield return Request.SendWebRequest();

                if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
                {
                    Text = Request.downloadHandler.text;
                }
            }


            //WWW www = new WWW(URL, Form);
            // yield return www;
            // Message("Local Upload Request: "+www.text);

            if (!Text.Contains("Error"))
                Callback(true);
            else
                Callback(false);
            //if (Global)
            //{
            //   Form.AddField("Global", Global ? 1 : 0);
            //   Form.AddField("FileName", FileName);
            //   www = new WWW(URL, Form);
            //  yield return www;
            //   Message("Global Upload Request: " + www.text);
            //}
        }

        //Send SQL request
        public IEnumerator SendRequest(string SQL, Action<string> Callback = null, bool Global = true)
        {
            string Text = "";
            string ServerAddress = GetServerAddress();
            //yield return StartCoroutine(GetConnectedServerAddress((ServerAddressCallback) => {
            //    ServerAddress = ServerAddressCallback;
            // }));


            // if (ServerAddress != "" && ServerAddress != null) LastServerAddress = ServerAddress;
            // else ServerAddress = LastServerAddress;
            string URL = "http://" + ServerAddress + "/" + "SendRequest";
            Message(URL);
            Message(SQL);
            WWWForm Form = new WWWForm();
            Form.AddField("SQL", SQL);
            Form.AddField("Password", Password);
            Form.AddField("Token", Token);

            using (UnityWebRequest Request = UnityWebRequest.Post(URL, Form))
            {
                Request.timeout = 10;
                Request.certificateHandler = new AcceptAllCertificatesSignedWithASpecificKeyPublicKey();
                yield return Request.SendWebRequest();

                if (!Request.isNetworkError && !Request.isHttpError && Request.downloadHandler.isDone)
                {
                    Text = Request.downloadHandler.text;
                }
            }


            //WWW www = new WWW(URL, Form);
            // yield return www;
            //Message("Local URL Request: " + www.text);

            if (Callback != null && Text != "")
                Callback(Text);

            //if (Global)
            //{
            //    Form.AddField("Global", Global ? 1 : 0);
            //    www = new WWW(URL, Form);
            //    yield return www;
            //    Message("Global URL Request: " + www.text);
            //}

        }

        //Get the first connected network
        public IEnumerator GetConnectedServerAddress(Action<string> Callback)
        {
            string ServerAddress = GetServerAddress();
            bool IsConnected = false;

            foreach (string Element in this.ServerAddress)
            {
                yield return StartCoroutine(Ping(Element, (PingCallback) =>
                {
                    IsConnected = PingCallback;
                }));


                if (IsConnected)
                {
                    ServerAddress = Element;
                    break;
                }
            }

            if (ServerAddress == null || ServerAddress == "") ServerAddress = GetServerAddress();
            Callback(ServerAddress);
        }

        //Ping and check if is network is connected
        public IEnumerator Ping(string ServerAddress, Action<bool> Callback)
        {
            string URL = "http://" + ServerAddress + "/";

            WWW www = new WWW(URL);

            for (int i = 0; i < 10; i++)
            {
                yield return new WaitForSeconds(0.2f);
                if (www.isDone) break;
            }

            if (www.isDone && www.text == "Kiddo Server")
                Callback(true);
            else
                Callback(false);

        }

        //Ping and check if is network is connected
        public IEnumerator DetectServerAddress()
        {
            List<string> DetectedServerAddresses = new List<string>();

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 255; j++)
                {
                    string TempServerAddress = "192.168." + i + "." + j + "";
                    StartCoroutine(Ping(TempServerAddress, (PingCallback) =>
                    {

                        Message(TempServerAddress + ": " + PingCallback);
                        if (PingCallback)
                        {
                            DetectedServerAddresses.Add(TempServerAddress);
                        }
                    }));
                }
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.5f);
            if (DetectedServerAddresses.Count > 0)
            {
                SetServerAddressesToFile(DetectedServerAddresses.ToArray());
                LoadServerAddressesFromFile();
            }


        }

        //========================= Editor Functions

        void Message(string Content)
        {
#if UNITY_EDITOR
            //Debug.Log(Content);
#endif
        }

    }




    class AcceptAllCertificatesSignedWithASpecificKeyPublicKey : CertificateHandler
    {

        // Encoded RSAPublicKey
        private static string PUB_KEY = "mypublickey";
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            X509Certificate2 certificate = new X509Certificate2(certificateData);
            string pk = certificate.GetPublicKeyString();
            if (pk.ToLower().Equals(PUB_KEY.ToLower()))
                return true;
            //return false;
            return true;
        }
    }
}