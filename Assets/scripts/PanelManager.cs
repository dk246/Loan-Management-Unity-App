using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject[] panels;

    void Start()
    {
        LoginDisplay();
    }

    // Update is called once per frame

    public void LoginDisplay()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[0].SetActive(true);
    }

    public void StartScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[1].SetActive(true);
    }

    public void ClientRegScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[2].SetActive(true);
    }
    public void ShareHoldertRegScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[3].SetActive(true);
    }

    public void ClientsScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[4].SetActive(true);
    }

    public void ShareHoldersScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[5].SetActive(true);
    }

    public void ClientSelectScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[6].SetActive(true);
    }

    public void ShareHolderSelectScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[7].SetActive(true);
    }

    public void ClientViewScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[8].SetActive(true);
    }

    public void ShareHolderviewScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[9].SetActive(true);
    }

    public void ClientPaymentScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[10].SetActive(true);
    }

    public void ShareHolderpaymentScreen()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[11].SetActive(true);
    }

    public void UpdatePanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {

            panels[i].SetActive(false);
        }

        panels[12].SetActive(true);
    }

    public void Quit()
    {
       Application.Quit();
    }
}
