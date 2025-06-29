using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ClientInfo
{
    public string name;
    public string interestRate;
    public string collateralValue;
}

[Serializable]
public class ClientInfoList
{
    public List<ClientInfo> dataList;
}

[Serializable]
public class ClientPaymentEntry
{
    public string name;
    public string type;          // "take loan" or "monthly payment"
    public string date;
    public float amount;
    public float remainingLoan;
}

[Serializable]
public class ClientPaymentList
{
    public List<ClientPaymentEntry> payments = new List<ClientPaymentEntry>();
}

[Serializable]
public class ClientLoanData
{
    public string name;
    public float currentLoan;
    public string firstLoanDate;
    public float unpaidInterestThisMonth;
    public string lastInterestDate; // Now stores the date of the last interest period
}

[Serializable]
public class ClientLoanDataList
{
    public List<ClientLoanData> loans = new List<ClientLoanData>();
}

[Serializable]
public class ShareholderPaymentEntry
{
    public string name;
    public string type;
    public string date;
    public float amount;
    public float property;
}

[Serializable]
public class ShareholderPaymentList
{
    public List<ShareholderPaymentEntry> payments = new List<ShareholderPaymentEntry>();
}

public class ClientPayment : MonoBehaviour
{
    [Header("Inputs & Buttons")]
    public InputField loanInput;
    public InputField paymentInput;
    public Button addLoanButton;
    public Button monthlyPaymentButton;

    [Header("UI Texts")]
    public Text nameTxt;
    public Text remainingLoanText;
    public Text currentLoanText;
    public Text clientEquityText;
    public Text monthlyInterestText;
    public Text companyEquityText;

    [Header("History UI")]
    public GameObject paymentHistoryPrefab;
    public Transform paymentHistoryPanel;

    private string savePath;
    private string clientFile = "clientinfo.json";
    private string paymentFile = "client_payments.json";
    private string loanFile = "client_loans.json";
    private string shareholderPaymentFile = "shareholder_payments.json";

    private string currentName;
    private float remainingLoan = 0f;
    private float currentLoan = 0f;
    private float interestRate = 0f;
    private float collateralValue = 0f;
    private DateTime? firstLoanDate = null;

    // Rolling 30-day period interest tracking
    private float unpaidInterestThisMonth = 0f;
    private DateTime lastInterestDate = DateTime.MinValue;

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");
        addLoanButton.onClick.AddListener(HandleAddLoan);
        monthlyPaymentButton.onClick.AddListener(HandleMonthlyPayment);
    }

    public void LoadClient(string name)
    {
        currentName = name;
        remainingLoan = 0f;
        currentLoan = 0f;
        firstLoanDate = null;
        unpaidInterestThisMonth = 0f;
        lastInterestDate = DateTime.MinValue;

        nameTxt.text = name;
        LoadClientData();
        LoadLoanData();
        LoadPaymentHistory();
        InitOrAdvanceInterestPeriod();
        UpdateUI();
    }

    private void LoadClientData()
    {
        string path = Path.Combine(savePath, clientFile);
        if (!File.Exists(path)) return;

        var json = File.ReadAllText(path);
        var infoList = JsonUtility.FromJson<ClientInfoList>(json);
        var info = infoList.dataList.Find(c => c.name.Equals(currentName, StringComparison.OrdinalIgnoreCase));
        if (info != null)
        {
            float.TryParse(info.interestRate, out interestRate);
            float.TryParse(info.collateralValue, out collateralValue);
        }
    }

    private void LoadLoanData()
    {
        string path = Path.Combine(savePath, loanFile);
        ClientLoanDataList meta = new ClientLoanDataList();
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            meta = JsonUtility.FromJson<ClientLoanDataList>(json);
        }

        var entry = meta.loans.Find(l => l.name == currentName);
        if (entry != null)
        {
            currentLoan = entry.currentLoan;
            unpaidInterestThisMonth = entry.unpaidInterestThisMonth;
            if (DateTime.TryParse(entry.firstLoanDate, out DateTime dt))
                firstLoanDate = dt;
            if (DateTime.TryParse(entry.lastInterestDate, out DateTime dt2))
                lastInterestDate = dt2;
        }
        else
        {
            currentLoan = 0f;
            remainingLoan = 0f;
            firstLoanDate = null;
            unpaidInterestThisMonth = 0f;
            lastInterestDate = DateTime.MinValue;
            return;
        }

        var pays = LoadPaymentData();
        foreach (var p in pays)
        {
            if (p.name != currentName) continue;
            remainingLoan = p.remainingLoan;
            if (p.type == "take loan")
            {
                if (DateTime.TryParse(p.date, out DateTime d))
                {
                    if (firstLoanDate == null || d < firstLoanDate)
                        firstLoanDate = d;
                }
            }
        }
    }

    private void SaveLoanData()
    {
        string path = Path.Combine(savePath, loanFile);
        ClientLoanDataList meta = new ClientLoanDataList();
        if (File.Exists(path))
        {
            meta = JsonUtility.FromJson<ClientLoanDataList>(File.ReadAllText(path));
        }

        var entry = meta.loans.Find(l => l.name == currentName);
        if (entry != null)
        {
            entry.currentLoan = currentLoan;
            entry.firstLoanDate = firstLoanDate?.ToString("yyyy-MM-dd") ?? "";
            entry.unpaidInterestThisMonth = unpaidInterestThisMonth;
            entry.lastInterestDate = lastInterestDate.ToString("yyyy-MM-dd");
        }
        else
        {
            meta.loans.Add(new ClientLoanData
            {
                name = currentName,
                currentLoan = currentLoan,
                firstLoanDate = firstLoanDate?.ToString("yyyy-MM-dd") ?? "",
                unpaidInterestThisMonth = unpaidInterestThisMonth,
                lastInterestDate = lastInterestDate.ToString("yyyy-MM-dd")
            });
        }

        File.WriteAllText(path, JsonUtility.ToJson(meta, true));
    }

    private List<ClientPaymentEntry> LoadPaymentData()
    {
        string path = Path.Combine(savePath, paymentFile);
        if (!File.Exists(path)) return new List<ClientPaymentEntry>();
        var json = File.ReadAllText(path);
        return JsonUtility.FromJson<ClientPaymentList>(json).payments;
    }

    private void SavePaymentEntry(string type, float amount)
    {
        string path = Path.Combine(savePath, paymentFile);
        ClientPaymentList list = new ClientPaymentList();
        if (File.Exists(path))
            list = JsonUtility.FromJson<ClientPaymentList>(File.ReadAllText(path));

        list.payments.Add(new ClientPaymentEntry
        {
            name = currentName,
            type = type,
            date = DateTime.Now.ToString("yyyy-MM-dd"),
            amount = amount,
            remainingLoan = remainingLoan
        });

        File.WriteAllText(path, JsonUtility.ToJson(list, true));
    }

    // --- Rolling 30-day period interest logic ---
    private void InitOrAdvanceInterestPeriod()
    {
        if (currentLoan <= 0f || interestRate <= 0f)
        {
            unpaidInterestThisMonth = 0f;
            lastInterestDate = DateTime.Now;
            return;
        }

        if (lastInterestDate == DateTime.MinValue)
        {
            // If no last interest date, set to first loan date or today
            lastInterestDate = firstLoanDate ?? DateTime.Now;
        }

        DateTime now = DateTime.Now;

        // Roll over for every 30-day period missed
        while ((now - lastInterestDate).TotalDays >= 30)
        {
            // Capitalize unpaid interest
            currentLoan += unpaidInterestThisMonth;
            remainingLoan += unpaidInterestThisMonth;

            // Advance to next period
            lastInterestDate = lastInterestDate.AddDays(30);
            unpaidInterestThisMonth = currentLoan * interestRate / 100f;
        }

        // If period just started (first loan), and unpaidInterestThisMonth not set:
        if (unpaidInterestThisMonth == 0f)
            unpaidInterestThisMonth = currentLoan * interestRate / 100f;
    }

    private float GetUnpaidInterestForCurrentMonth()
    {
        if (currentLoan <= 0f || interestRate <= 0f) return 0f;
        return unpaidInterestThisMonth;
    }

    private void HandleAddLoan()
    {
        InitOrAdvanceInterestPeriod();

        if (!float.TryParse(loanInput.text, out float amount) || amount <= 0) return;

        float clientEquity = collateralValue - currentLoan;
        if (amount <= clientEquity)
        {
            remainingLoan += amount;
            currentLoan += amount;

            if (firstLoanDate == null)
            {
                firstLoanDate = DateTime.Now;
                lastInterestDate = firstLoanDate.Value;
            }

            // Update interest for this period after loan
            unpaidInterestThisMonth = currentLoan * interestRate / 100f;

            SavePaymentEntry("take loan", amount);
            SaveLoanData();
            UpdateUI();
        }

        loanInput.text = "";
    }

    private void HandleMonthlyPayment()
    {
        InitOrAdvanceInterestPeriod();

        if (!float.TryParse(paymentInput.text, out float payment) || payment <= 0)
            return;

        float interestDue = unpaidInterestThisMonth;

        float principalPaid = 0f;
        float interestPaid = 0f;

        if (payment >= interestDue)
        {
            // Pay all this period's interest, rest goes to principal
            interestPaid = interestDue;
            principalPaid = payment - interestDue;
            unpaidInterestThisMonth = 0f;
        }
        else
        {
            // Partial interest paid, nothing to principal
            interestPaid = payment;
            unpaidInterestThisMonth -= payment;
            if (unpaidInterestThisMonth < 0f) unpaidInterestThisMonth = 0f;
            principalPaid = 0f;
        }

        remainingLoan -= principalPaid;
        if (remainingLoan < 0f) remainingLoan = 0f;

        if (Mathf.Approximately(remainingLoan, 0f))
            currentLoan = 0f;

        SavePaymentEntry("monthly payment", payment);
        SaveLoanData();
        paymentInput.text = "";
        UpdateUI();
    }

    private void LoadPaymentHistory()
    {
        foreach (Transform c in paymentHistoryPanel)
            Destroy(c.gameObject);

        var pays = LoadPaymentData();

        // Reverse order: latest first
        for (int i = pays.Count - 1; i >= 0; i--)
        {
            var p = pays[i];
            if (p.name != currentName) continue;

            var go = Instantiate(paymentHistoryPrefab, paymentHistoryPanel);
            var txts = go.GetComponentsInChildren<Text>();
            txts[0].text = p.date;
            txts[1].text = p.amount.ToString("F2");
            txts[2].text = p.type;
        }
    }

    private void UpdateUI()
    {
        InitOrAdvanceInterestPeriod();

        bool canLoan = (collateralValue - currentLoan) > 0;
        loanInput.interactable = canLoan;
        addLoanButton.interactable = canLoan;

        remainingLoanText.text = $"Remaining Loan: {remainingLoan:F2}";
        currentLoanText.text = $"Current Loan:   {currentLoan:F2}";
        clientEquityText.text = $"Client Equity:  {(collateralValue - currentLoan):F2}";
        monthlyInterestText.text = $"Monthly Interest: {GetUnpaidInterestForCurrentMonth():F2}";
        companyEquityText.text = $"Company Equity: {GetCompanyEquity():F2}";
        monthlyPaymentButton.interactable = true;

        LoadPaymentHistory();
    }

    private float GetCompanyEquity()
    {
        float totalShareholderDeposits = 0f;
        float totalClientLoans = 0f;

        // Sum all deposits from all shareholders
        string shareholderPaymentFilePath = Path.Combine(savePath, "shareholder_payments.json");
        if (File.Exists(shareholderPaymentFilePath))
        {
            string json = File.ReadAllText(shareholderPaymentFilePath);
            ShareholderPaymentList paymentList = JsonUtility.FromJson<ShareholderPaymentList>(json);

            foreach (var payment in paymentList.payments)
            {
                if (payment.type == "deposit")
                    totalShareholderDeposits += payment.amount;
            }
        }

        // Sum all currentLoan values from all clients
        string clientLoanFilePath = Path.Combine(savePath, "client_loans.json");
        if (File.Exists(clientLoanFilePath))
        {
            string json = File.ReadAllText(clientLoanFilePath);
            ClientLoanDataList loanData = JsonUtility.FromJson<ClientLoanDataList>(json);

            foreach (var loan in loanData.loans)
            {
                totalClientLoans += loan.currentLoan;
            }
        }

        return totalShareholderDeposits - totalClientLoans;
    }
}