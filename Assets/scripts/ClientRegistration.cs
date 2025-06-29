using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for UI interaction


public class ClientRegistration : MonoBehaviour
{
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;
    public InputField nicInput;
    public InputField addressInput;
    public InputField collateralTypeInput;
    public InputField collateralValueInput;
    public InputField collateralNameInput;
    public InputField interestRateInput;

    private string savePath;
    private string fileName = "clientinfo.json";

    public GameObject WarningPrefab;
    private Transform contentPanel;
    public Button registerButton;


    void Start()
    {
        contentPanel = GameObject.Find("Canvas").transform;
        savePath = Path.Combine(Application.dataPath, "SaveData");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        // Add listeners
        nameInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        emailInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        phoneInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        nicInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        addressInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        collateralTypeInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        collateralValueInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        collateralNameInput.onValueChanged.AddListener(delegate { CheckInputFields(); });
        interestRateInput.onValueChanged.AddListener(delegate { CheckInputFields(); });

        // Initial check
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
            !string.IsNullOrWhiteSpace(collateralTypeInput.text) &&
            !string.IsNullOrWhiteSpace(collateralValueInput.text) &&
            !string.IsNullOrWhiteSpace(collateralNameInput.text) &&
            !string.IsNullOrWhiteSpace(interestRateInput.text);

        registerButton.interactable = allFilled;
    }

    public void SaveToFile()
    {
        SaveData newData = new SaveData
        {
            name = nameInput.text.Trim(),
            email = emailInput.text.Trim(),
            phone = phoneInput.text.Trim(),
            nic = nicInput.text.Trim(),
            address = addressInput.text.Trim(),
            collateralType = collateralTypeInput.text.Trim(),
            collateralName = collateralNameInput.text.Trim(),
            collateralValue = collateralValueInput.text.Trim(),
            interestRate = interestRateInput.text.Trim()
        };

        string filePath = Path.Combine(savePath, fileName);
        SaveDataList dataList = new SaveDataList();

        // Load existing data
        if (File.Exists(filePath))
        {
            string existingJson = File.ReadAllText(filePath);
            dataList = JsonUtility.FromJson<SaveDataList>(existingJson);
        }

        // Check uniqueness by name, phone, and address
        bool isDuplicate = dataList.dataList.Exists(d =>
            d.name == newData.name &&
            d.phone == newData.phone &&
            d.address == newData.address
        );

        if (isDuplicate)
        {

            GameObject warning = Instantiate(WarningPrefab, contentPanel);
            Text[] warningText = warning.GetComponentsInChildren<Text>();
            if (warningText != null)
                warningText[1].text = "A client already exists.";

         
            //Debug.LogWarning("A client with the same name, phone, and address already exists.");
            CheckInputFields();
            return;
        }

        // Add new entry
        dataList.dataList.Add(newData);

        // Save updated list
        string updatedJson = JsonUtility.ToJson(dataList, true);
        File.WriteAllText(filePath, updatedJson);

        GameObject clientGO = Instantiate(WarningPrefab, contentPanel);
        Text[] nameText = clientGO.GetComponentsInChildren<Text>();
        if (nameText != null)
            nameText[1].text = "Client saved successfully.";

        nameInput.text = "";
        emailInput.text = "";
        phoneInput.text = "";
        nicInput.text = "";
        addressInput.text = "";
        collateralTypeInput.text = "";
        collateralValueInput.text = "";
        collateralNameInput.text = "";
        interestRateInput.text = "";
        CheckInputFields();
    }


}
