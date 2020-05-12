using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MOMmain
{
    public class ApplicationManager : MonoBehaviour
    {
        public ServerScript Server;
        public InputField InputID;

        public void CheckIDValidity()
        {
            StartCoroutine(Server.CheckData(InputID.text));
        }

        void ShowDBParameters()
        {

        }
    }
}
