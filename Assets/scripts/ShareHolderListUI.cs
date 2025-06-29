using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ShareHolderListUI : MonoBehaviour
{
    public GameObject shareHolderBoxPrefab; // Assign your prefab
    public Transform contentPanel;          // Parent object with vertical layout group
    private string fileName = "shareholders.json";
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");
        //LoadShareHolderList();
    }

    public void LoadShareHolderList()
    {
        string filePath = Path.Combine(savePath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Shareholder file not found: " + filePath);
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
            GameObject clientGO = Instantiate(shareHolderBoxPrefab, contentPanel);
            Text nameText = clientGO.GetComponentInChildren<Text>();
            if (nameText != null)
                nameText.text = client.name;
        }
    }

  
}