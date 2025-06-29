using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ClientListUI : MonoBehaviour
{
    public GameObject clientImagePrefab; // Prefab with Image + Text
    public Transform contentPanel;       // Parent panel to hold all instances

    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");
        //DisplayAllClients();
    }

    public void DisplayAllClients()
    {
        string filePath = Path.Combine(savePath, "clientinfo.json");

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("No client data file found.");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveDataList allData = JsonUtility.FromJson<SaveDataList>(json);

        // Clear previous children
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (var client in allData.dataList)
        {
            GameObject clientGO = Instantiate(clientImagePrefab, contentPanel);
            Text nameText = clientGO.GetComponentInChildren<Text>();
            if (nameText != null)
                nameText.text = client.name;
        }
    }
}
