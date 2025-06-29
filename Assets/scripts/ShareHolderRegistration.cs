using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShareholderData
{
    public string name;
    public string email;
    public string phone;
    public string nic;
    public string address;
    public string interestRate;
}

[System.Serializable]
public class ShareholderDataList
{
    public List<ShareholderData> dataList = new List<ShareholderData>();
}

public class ShareHolderRegistration : MonoBehaviour
{
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;
    public InputField nicInput;
    public InputField addressInput;
    public InputField depositInput;

    public Text outputText;

    private string savePath;
    private string fileName = "shareholders.json";

    public Button registerButton;
    public GameObject WarningPrefab;
    private Transform contentPanel;

    // Reference to the UI script (set in Inspector!)
    public ShareHolderListUI shareHolderListUI;

    void Start()
    {
        contentPanel = GameObject.Find("Canvas").transform;
        savePath = Path.Combine(Application.dataPath, "SaveData");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        nameInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        emailInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        phoneInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        nicInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        addressInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        depositInput.onValueChanged.AddListener(delegate { CheckInputFields(); });

        CheckInputFields();
    }

    void CheckInputFields()
    {
        bool allFilled =
            !string.IsNullOrWhiteSpace(nameInput.text) &&
            !string.IsNullOrWhiteSpace(emailInput.text) &&
            !string.IsNullOrWhiteSpace(phoneInput.text) &&
            !string.IsNullOrWhiteSpace(nicInput.text) &&
            !string.IsNullOrWhiteSpace(addressInput.text) &&
            !string.IsNullOrWhiteSpace(depositInput.text);

        registerButton.interactable = allFilled;
    }

    public void SaveToFile()
    {
        string filePath = Path.Combine(savePath, fileName);

        ShareholderData newData = new ShareholderData
        {
            name = nameInput.text.Trim(),
            email = emailInput.text.Trim(),
            phone = phoneInput.text.Trim(),
            nic = nicInput.text.Trim(),
            address = addressInput.text.Trim(),
            interestRate = depositInput.text.Trim()
        };

        ShareholderDataList allData = LoadAllData();

        bool exists = allData.dataList.Exists(d =>
            d.name == newData.name &&
            d.address == newData.address &&
            d.phone == newData.phone
        );

        if (exists)
        {
            GameObject shareHolderData = Instantiate(WarningPrefab, contentPanel);
            Text[] shareHolderDataTxt = shareHolderData.GetComponentsInChildren<Text>();
            if (shareHolderDataTxt != null)
                shareHolderDataTxt[1].text = "Duplicate entry! Not saved";

            if (outputText != null) outputText.text = "Duplicate entry! Not saved.";
            CheckInputFields();
            return;
        }

        allData.dataList.Add(newData);
        string json = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, json);

        GameObject clientGO = Instantiate(WarningPrefab, contentPanel);
        Text[] nameText = clientGO.GetComponentsInChildren<Text>();
        if (nameText != null)
            nameText[1].text = "Shareholder data saved!";

        nameInput.text = "";
        emailInput.text = "";
        phoneInput.text = "";
        nicInput.text = "";
        addressInput.text = "";
        depositInput.text = "";
        CheckInputFields();

        if (outputText != null) outputText.text = "Data saved successfully.";

        // REFRESH UI IMMEDIATELY
        if (shareHolderListUI != null)
        {
            shareHolderListUI.LoadShareHolderList();
        }
    }

    private ShareholderDataList LoadAllData()
    {
        string filePath = Path.Combine(savePath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ShareholderDataList>(json);
        }
        return new ShareholderDataList();
    }
}