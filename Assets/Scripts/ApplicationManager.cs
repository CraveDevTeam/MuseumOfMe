using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ApplicationManager : MonoBehaviour
{
    public ServerScript Server;
    public InputField InputDIRID;
    public InputField InputQRID;
    public InputField InputNickname;
    public int CharValues;
    public string DateNow;
    public Text output;

    public void CheckData()
    {
        StartCoroutine(Server.CheckData(InputDIRID.text));
    }

    public void SubmitData()
    {
        StartCoroutine(Server.SubmitData(InputDIRID.text, InputQRID.text, InputNickname.text, CharValues, DateNow));
    }

    public void GetData()
    {
        StartCoroutine(Server.GetData(InputDIRID.text));
    }

    //public void ShowAllValues()
    //{
    //    ServerScript.AppData AppData = new ServerScript.AppData();
    //    AppData.DateUpdate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    //    Debug.Log("DIRID: " + AppData.DIRID);
    //    Debug.Log("NickName: " + AppData.NickName);
    //    Debug.Log("DateUpdate: " + AppData.DateUpdate);
    //}

    public void GenerateGUIDButton()
    {
        StartCoroutine(GenerateGUID());
    }


    IEnumerator GenerateGUID()
    {
        //Generate a GUID
        string GUID = System.Guid.NewGuid().ToString();

        ServerScript.AppData AppData = new ServerScript.AppData();
        Debug.Log("GUID : " + GUID);
        AppData.GUID = GUID;

        yield return StartCoroutine(ServerScript.Instance.SetAppData(AppData));

        yield return new WaitForSeconds(5);
    }

    private void Update()
    {
        DateTime DateUpdate = DateTime.Now;
        DateNow = DateUpdate.ToString("yyyy-MM-dd");
        //Submit();
    }
}
