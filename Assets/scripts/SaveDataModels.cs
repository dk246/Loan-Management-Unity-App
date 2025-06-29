using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public string name;
    public string email;
    public string phone;
    public string nic;
    public string address;
    public string collateralType;
    public string collateralName;
    public string collateralValue;
    public string interestRate;
}

[Serializable]
public class SaveDataList
{
    public List<SaveData> dataList = new List<SaveData>();
}
