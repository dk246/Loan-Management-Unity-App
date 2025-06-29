using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// Assumes these classes exist: ClientInfoList, ClientLoanDataList, ClientLoanData, SaveDataList, PaymentList, PaymentEntry

public class MonthlyDuePanel : MonoBehaviour
{
    [Header("Client Scroll")]
    public Transform clientContent;      // Content of Clients ScrollView
    public GameObject entryPrefab;       // Prefab with NameText & AmountText

    [Header("Shareholder Scroll")]
    public Transform shareholderContent; // Content of Shareholders ScrollView
    public GameObject shareEntryPrefab;  // Prefab with NameText & AmountText

    string savePath;
    const string clientInfoFile = "clientinfo.json";
    const string clientLoanFile = "client_loans.json";
    const string paymentFile = "client_payments.json";
    const string shareholderFile = "shareholders.json";
    const string shareholderPayFile = "shareholder_payments.json";

    void Start()
    {
        savePath = Path.Combine(Application.dataPath, "SaveData");
        RefreshAll();
    }

    public void RefreshAll()
    {
        PopulateClients();
        PopulateShareholders();
    }

    void PopulateClients()
    {
        // clear
        for (int i = clientContent.childCount - 1; i >= 0; i--)
            Destroy(clientContent.GetChild(i).gameObject);
        var loanMetaPath = Path.Combine(savePath, clientLoanFile);
        var clientInfoPath = Path.Combine(savePath, clientInfoFile);
      
        if (!File.Exists(clientInfoPath) || !File.Exists(loanMetaPath)) return;

        var infoList = JsonUtility.FromJson<ClientInfoList>(File.ReadAllText(clientInfoPath));
        var loanMeta = JsonUtility.FromJson<ClientLoanDataList>(File.ReadAllText(loanMetaPath));

        foreach (var lm in loanMeta.loans)
        {
            if (lm.currentLoan <= 0f) continue;
            var info = infoList.dataList.Find(c => c.name.Equals(lm.name, StringComparison.OrdinalIgnoreCase));
            if (info == null) continue;
            if (!float.TryParse(info.interestRate, out float rate)) continue;

            // Directly use the field
            float monthlyInterest = lm.unpaidInterestThisMonth;

            if (monthlyInterest > 0.01f)
            {
                var go = Instantiate(entryPrefab, clientContent);
                go.transform.Find("NameText").GetComponent<Text>().text = lm.name;
                go.transform.Find("AmountText").GetComponent<Text>().text = $"Interest: {monthlyInterest:F2}";
            }
        }
    }

    void PopulateShareholders()
    {
        // clear
        for (int i = shareholderContent.childCount - 1; i >= 0; i--)
            Destroy(shareholderContent.GetChild(i).gameObject);

        var shInfoPath = Path.Combine(savePath, shareholderFile);
        var shPayPath = Path.Combine(savePath, shareholderPayFile);
        if (!File.Exists(shInfoPath) || !File.Exists(shPayPath)) return;

        // load shareholder info and payments
        var shInfoList = JsonUtility.FromJson<SaveDataList>(File.ReadAllText(shInfoPath));
        var payList = JsonUtility.FromJson<PaymentList>(File.ReadAllText(shPayPath)).payments;

        // group payments by shareholder
        var byName = payList
            .GroupBy(p => p.name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.OrderBy(p => DateTime.Parse(p.date)).ToList(), StringComparer.OrdinalIgnoreCase);

        foreach (var info in shInfoList.dataList)
        {
            if (!byName.TryGetValue(info.name, out var entries)) continue;

            // find first deposit date
            var firstDeposit = entries.FirstOrDefault(p => p.type == "deposit");
            DateTime firstDate = firstDeposit != null
                ? DateTime.Parse(firstDeposit.date)
                : DateTime.Now;

            // find last profit date
            var lastProfit = entries.Where(p => p.type == "profit")
                                    .OrderByDescending(p => DateTime.Parse(p.date))
                                    .FirstOrDefault();
            DateTime lastProfitDate = lastProfit != null
                ? DateTime.Parse(lastProfit.date)
                : firstDate;

            // if 30+ days since lastProfit, profit is due
            if ((DateTime.Now - lastProfitDate).TotalDays >= 30)
            {
                if (!float.TryParse(info.interestRate, out float rate)) continue;
                float profit = entries.Last().property * rate / 100f;
                if (profit <= 0f) continue;

                var go = Instantiate(shareEntryPrefab, shareholderContent);
                go.transform.Find("NameText").GetComponent<Text>().text = info.name;
                go.transform.Find("AmountText").GetComponent<Text>().text = $"Profit: {profit:F2}";
            }
        }
    }
}