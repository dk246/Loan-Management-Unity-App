using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxElement : MonoBehaviour
{
    public static string ClientName;
    public static string ShareHolderName;
    private PanelManager pm;
    private ClientViewManage cvm;
    private ShareHolderViewManage shvm;
    private ShareHolderPayment shp;
    private ClientPayment chp;

    private void Start()
    {
        pm = GameObject.Find("panelManage").GetComponent<PanelManager>();
        cvm = GameObject.Find("Canvas").GetComponent<ClientViewManage>();
        shvm = GameObject.Find("Canvas").GetComponent <ShareHolderViewManage>();
        shp = GameObject.Find("Canvas").GetComponent<ShareHolderPayment>();
        chp = GameObject.Find("Canvas").GetComponent<ClientPayment>();
    }
    public void ClientBtnClick()
    {
        string temp = gameObject.GetComponentInChildren<Text>().text;
        ClientName = temp;
        pm.ClientSelectScreen();
        cvm.LoadClientData(ClientName);
        chp.LoadClient(ClientName);
    }

    public void ShareHolderBtnClick()
    {
        string temp = gameObject.GetComponentInChildren<Text>().text;
        ShareHolderName = temp;
        pm.ShareHolderSelectScreen();
        shvm.LoadShareHolder(ShareHolderName);
        shp.LoadShareHolderPayments(ShareHolderName);
    }
}
