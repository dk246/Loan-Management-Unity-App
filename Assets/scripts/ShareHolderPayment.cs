using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PaymentEntry
{
    public string name;
    public string type; // deposit, withdraw, profit
    public string date;
    public float amount;
    public float property;
}

[Serializable]
public class PaymentList
{
    public List<PaymentEntry> payments = new List<PaymentEntry>();
}

public class ShareHolderPayment : MonoBehaviour
{
    public InputField depositInput;
    public InputField withdrawInput;
    public Text propertyText;
    public Text profitText;
    public Button depositButton;
    public Button withdrawButton;
    public Button payProfitButton;

    private string savePath;
    private string paymentFile = "shareholder_payments.json";
    private string shareholderFile = "shareholders.json";

    private string currentName;
    private float currentProperty = 0f;
    private float interestRate = 0f;
    private DateTime? lastProfitDate = null;

    public Text nameTxt;
    public GameObject paymentPrefab; // Assign in inspector
    public Transform historyContainer; // The parent content holder (e.g., inside a ScrollView)


    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");

        depositButton.onClick.AddListener(HandleDeposit);
        withdrawButton.onClick.AddListener(HandleWithdraw);
        payProfitButton.onClick.AddListener(HandleProfitPayment);

        UpdateUI();
    }

    public void LoadShareHolderPayments(string name)
    {

        currentName = name;
        nameTxt.text = currentName;
        currentProperty = 0f;
        interestRate = LoadInterestRate(name);
        lastProfitDate = null;

        List<PaymentEntry> entries = LoadPaymentData();

        foreach (var entry in entries)
        {
            if (entry.name == name)
            {
                currentProperty = entry.property;
                if (entry.type == "profit")
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(entry.date, out parsedDate))
                    {
                        if (lastProfitDate == null || parsedDate > lastProfitDate)
                            lastProfitDate = parsedDate;
                    }
                }
            }
        }

        UpdateUI();
        DisplayPaymentHistory();
    }

    private float LoadInterestRate(string name)
    {
        string filePath = Path.Combine(savePath, shareholderFile);
        if (!File.Exists(filePath)) return 0f;

        string json = File.ReadAllText(filePath);
        SaveDataList data = JsonUtility.FromJson<SaveDataList>(json);
        var match = data.dataList.Find(d => d.name == name);
        if (match != null && float.TryParse(match.interestRate, out float rate))
            return rate;

        return 0f;
    }

    private List<PaymentEntry> LoadPaymentData()
    {
        string filePath = Path.Combine(savePath, paymentFile);
        if (!File.Exists(filePath)) return new List<PaymentEntry>();

        string json = File.ReadAllText(filePath);
        PaymentList data = JsonUtility.FromJson<PaymentList>(json);
        return data.payments;
    }

    private void SavePaymentEntry(string type, float amount)
    {
        string filePath = Path.Combine(savePath, paymentFile);
        PaymentList data = new PaymentList();

        if (File.Exists(filePath))
            data = JsonUtility.FromJson<PaymentList>(File.ReadAllText(filePath));

        PaymentEntry entry = new PaymentEntry
        {
            name = currentName,
            type = type,
            date = DateTime.Now.ToString("yyyy-MM-dd"),
            amount = amount,
            property = currentProperty
        };

        data.payments.Add(entry);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    private void HandleDeposit()
    {
        if (float.TryParse(depositInput.text, out float amount) && amount > 0)
        {
            currentProperty += amount;
            SavePaymentEntry("deposit", amount);

            depositInput.text = ""; // Clear input
            UpdateUI();
            DisplayPaymentHistory();
        }
    }


    private void HandleWithdraw()
    {
        if (float.TryParse(withdrawInput.text, out float amount) && amount > 0 && amount <= currentProperty)
        {
            currentProperty -= amount;
            SavePaymentEntry("withdraw", amount);

            withdrawInput.text = ""; // Clear input
            UpdateUI();
            DisplayPaymentHistory();
        }
    }

    private void HandleProfitPayment()
    {
        float interestAmount = currentProperty * interestRate / 100f;
        SavePaymentEntry("profit", interestAmount);
        lastProfitDate = DateTime.Now;
        UpdateUI();
        DisplayPaymentHistory();
    }

    private void UpdateUI()
    {
        propertyText.text = $"Property: {currentProperty:F2}";

        bool canPayProfit = false;

        if (lastProfitDate == null)
        {
            // Check if any deposit exists
            var allPayments = LoadPaymentData();
            foreach (var p in allPayments)
            {
                if (p.name == currentName && p.type == "deposit")
                {
                    if (DateTime.TryParse(p.date, out DateTime firstDepositDate))
                    {
                        if ((DateTime.Now - firstDepositDate).TotalDays >= 30)
                        {
                            canPayProfit = true;
                            break;
                        }
                    }
                }
            }
        }
        else if ((DateTime.Now - lastProfitDate.Value).TotalDays >= 30)
        {
            canPayProfit = true;
        }

        float profitAmount = currentProperty * interestRate / 100f;
        profitText.text = canPayProfit ? $"Rs: {profitAmount:F2}" : "Rs: 0.00";
        payProfitButton.interactable = canPayProfit;
    }

    private void DisplayPaymentHistory()
    {
        // Clear old items
        foreach (Transform child in historyContainer)
        {
            Destroy(child.gameObject);
        }

        List<PaymentEntry> entries = LoadPaymentData();

        entries.Reverse();

        foreach (var entry in entries)
        {
            if (entry.name == currentName)
            {
                GameObject item = Instantiate(paymentPrefab, historyContainer);

                Text[] texts = item.GetComponentsInChildren<Text>();
                foreach (Text t in texts)
                {
                    if (t.name.ToLower().Contains("date"))
                        t.text = entry.date;
                    else if (t.name.ToLower().Contains("amount"))
                        t.text = $"Rs: {entry.amount:F2}";
                    else if (t.name.ToLower().Contains("type"))
                        t.text = $"Type: {entry.type}";
                }
            }
        }
    }

}
