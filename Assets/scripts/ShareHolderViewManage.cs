using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ShareHolderViewManage : MonoBehaviour
{
    public InputField nameInput;
    public InputField emailInput;
    public InputField phoneInput;
    public InputField nicInput;
    public InputField addressInput;
    public InputField depositInput;

    public Button editButton;
    public Button saveButton;

    private string savePath;
    private string fileName = "shareholders.json";
    private SaveData currentShareholder;

    public Text shareholderNameTxt;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");

        if (editButton != null)
            editButton.onClick.AddListener(EnableEditing);

        if (saveButton != null)
            saveButton.onClick.AddListener(SaveEditedShareHolder);
        saveButton.interactable = false;
        SetInputsInteractable(false);
    }

    public void LoadShareHolder(string name)
    {
        string filePath = Path.Combine(savePath, fileName);

        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Shareholder data file not found.");
            return;
        }

        string json = File.ReadAllText(filePath);
        SaveDataList allData = JsonUtility.FromJson<SaveDataList>(json);

        currentShareholder = allData.dataList.Find(d => d.name.Trim().ToLower() == name.Trim().ToLower());

        if (currentShareholder != null)
        {
            nameInput.text = currentShareholder.name;
            emailInput.text = currentShareholder.email;
            phoneInput.text = currentShareholder.phone;
            nicInput.text = currentShareholder.nic;
            addressInput.text = currentShareholder.address;
            depositInput.text = currentShareholder.interestRate;

            if (shareholderNameTxt != null)
                shareholderNameTxt.text = currentShareholder.name;

            SetInputsInteractable(false);
        }
        else
        {
            Debug.LogWarning("Shareholder not found.");
        }
    }

    private void EnableEditing()
    {
        SetInputsInteractable(true, excludeName: true);
        saveButton.interactable = true;
    }

    private void SaveEditedShareHolder()
    {
        if (currentShareholder == null)
        {
            Debug.LogWarning("No shareholder selected to save.");
            return;
        }

        string filePath = Path.Combine(savePath, fileName);
        string json = File.ReadAllText(filePath);
        SaveDataList allData = JsonUtility.FromJson<SaveDataList>(json);

        for (int i = 0; i < allData.dataList.Count; i++)
        {
            if (allData.dataList[i].name.Trim().ToLower() == currentShareholder.name.Trim().ToLower())
            {
                allData.dataList[i].email = emailInput.text;
                allData.dataList[i].phone = phoneInput.text;
                allData.dataList[i].nic = nicInput.text;
                allData.dataList[i].address = addressInput.text;
                allData.dataList[i].interestRate = depositInput.text;
                break;
            }
        }

        string updatedJson = JsonUtility.ToJson(allData, true);
        File.WriteAllText(filePath, updatedJson);

        Debug.Log("Shareholder data updated.");
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
        depositInput.interactable = state;
    }
}
