using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ClientViewManage : MonoBehaviour
{
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;
    public InputField nicInput;
    public InputField addressInput;
    public InputField collateralTypeInput;
    public InputField collateralNameInput;
    public InputField collateralValueInput;
    public InputField interestRateInput;

    public Button editButton;
    public Button saveButton;

    private string savePath;
    private string fileName = "clientinfo.json";
    private SaveData currentClient;

    public Text clientNameTxt;

    private void Update()
    {
        clientNameTxt.text = BoxElement.ClientName;
        //LoadClientData(BoxElement.ClientName);
    }
    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");

        editButton.onClick.AddListener(EnableEditing);
        saveButton.onClick.AddListener(SaveChanges);
        saveButton.interactable = false;
        SetInputsInteractable(false);
    }

    // Call this function and pass the name string from elsewhere
    public void LoadClientData(string clientName)
    {
 
        string filePath = Path.Combine(savePath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Client data file not found.");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveDataList allData = JsonUtility.FromJson<SaveDataList>(json);

        currentClient = allData.dataList.Find(d => d.name == clientName);

        if (currentClient != null)
        {
            nameInput.text = currentClient.name;
            emailInput.text = currentClient.email;
            phoneInput.text = currentClient.phone;
            nicInput.text = currentClient.nic;
            addressInput.text = currentClient.address;
            collateralTypeInput.text = currentClient.collateralType;
            collateralNameInput.text = currentClient.collateralName;
            collateralValueInput.text = currentClient.collateralValue;
            interestRateInput.text = currentClient.interestRate;

            SetInputsInteractable(false);
        }
        else
        {
            Debug.LogWarning("Client not found.");
        }
    }

    private void EnableEditing()
    {
        SetInputsInteractable(true, excludeName: true);
        saveButton.interactable = true;
    }

    private void SaveChanges()
    {
        if (currentClient == null)
        {
            Debug.LogWarning("No client is loaded.");
            return;
        }

        string filePath = Path.Combine(savePath, fileName);
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Data file missing.");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveDataList allData = JsonUtility.FromJson<SaveDataList>(json);

        // Replace existing client data by name
        for (int i = 0; i < allData.dataList.Count; i++)
        {
            if (allData.dataList[i].name == currentClient.name)
            {
                allData.dataList[i].email = emailInput.text;
                allData.dataList[i].phone = phoneInput.text;
                allData.dataList[i].nic = nicInput.text;
                allData.dataList[i].address = addressInput.text;
                allData.dataList[i].collateralType = collateralTypeInput.text;
                allData.dataList[i].collateralName = collateralNameInput.text;
                allData.dataList[i].collateralValue = collateralValueInput.text;
                allData.dataList[i].interestRate = interestRateInput.text;
                break;
            }
        }

        // Save back to JSON
        string updatedJson = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, updatedJson);

        Debug.Log("Client data updated.");
        saveButton.interactable = false;
        SetInputsInteractable(false);
    }

    private void SetInputsInteractable(bool state, bool excludeName = false)
    {
        if (!excludeName) nameInput.interactable = state;
        emailInput.interactable = state;
        phoneInput.interactable = state;
        nicInput.interactable = state;
        addressInput.interactable = state;
        collateralTypeInput.interactable = state;
        collateralNameInput.interactable = state;
        collateralValueInput.interactable = state;
        interestRateInput.interactable = state;
    }
}
