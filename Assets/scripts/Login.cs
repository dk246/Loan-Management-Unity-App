using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private string username;
    private string password;

    public InputField nameInp;
    public InputField passwordInp;

    public static bool isLogged;

    public GameObject WarningPrefab;
    private Transform contentPanel;

    private PanelManager pm;
    void Start()
    {
        username = "Admin";
        password = "1234";
        isLogged = false;
        pm = GameObject.Find("panelManage").GetComponent<PanelManager>();
        contentPanel = GameObject.Find("Canvas").transform;
    }

    void Update()
    {
       
    }

    public void LoginBtn()
    {

        if (nameInp.text == username && passwordInp.text == password)
        {
            isLogged = true;
            pm.StartScreen();
        }
        else
        {
            nameInp.text = "";
            passwordInp.text = "";
            isLogged = false;
    

            GameObject clientGO = Instantiate(WarningPrefab, contentPanel);
            Text[] nameText = clientGO.GetComponentsInChildren<Text>();
            if (nameText != null)
                nameText[1].text = "Please check your Username !";
            else
                Debug.LogWarning("Text component not found!");




        }
    }
}
