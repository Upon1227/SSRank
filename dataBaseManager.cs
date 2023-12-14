using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using System;
using TMPro;


public class dataBaseManager : MonoBehaviour
{
    [SerializeField] TMP_InputField signupplayername, signuppassword,rankvalue,signinplayername,signinpassword;
    [SerializeField] TextMeshProUGUI statustext;
    [SerializeField] GameObject RankingPanel, SignUpPanel, SignInPanel;
    string accountDBName = "アカウントDBの名前";
    string rankingDBName = "ランキングDBの名前";
    string playername;

    public void  OnCreateAccount()
    {
        var service = OpenSheet();
        bool isError = ReadWrite(service, signupplayername.text, signuppassword.text, accountDBName);
        if (isError)
        {
            statustext.text = "Account already exists.";
        }
        else
        {
            statustext.text = "Done.";
            playername = signupplayername.text;
            RankingPanel.SetActive(true);
            SignInPanel.SetActive(false);
            SignUpPanel.SetActive(false);
        }
    }

    public void OnSignAccount()
    {
        var service = OpenSheet();
        Sign(service, signinplayername.text, signinpassword.text, accountDBName);
    }

    public void OnAddValue()
    {
        var service = OpenSheet();
        ReadValue(service, playername, rankvalue.text, rankingDBName);
    }

    static string ApplicationName = "Google Sheets API .NET Quickstart";
    static string spreadsheetId = "シートID";

    static SheetsService OpenSheet()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(Application.streamingAssetsPath + "/Jsonのファイル名", FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.Spreadsheets);
        }
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        return service;
    }


    void Sign(SheetsService service, string key, string keyvalue, string sheetName)
    {
        bool isError = false;
        ValueRange rVR;
        string wRange;
        int rowNumber = 1;
        wRange = string.Format("{0}!A{1}:B", sheetName, rowNumber);
        SpreadsheetsResource.ValuesResource.GetRequest getRequest
            = service.Spreadsheets.Values.Get(spreadsheetId, wRange);
        rVR = getRequest.Execute();
        var values = rVR.Values;
        bool isaccount = false;
        if (values.Count != 0)
        {
            foreach (var value in values)
            {
                for (int i = 0;i < value.Count - 1; i++)
                {
                    if(i % 2 == 0)
                    {
                        if (value[i].ToString() == key)
                        {
                            if(keyvalue == value[i + 1].ToString())
                            {
                                statustext.text = "success";
                                playername = key;
                                RankingPanel.SetActive(true);
                                SignInPanel.SetActive(false);
                                SignUpPanel.SetActive(false);

                            }
                            else
                            {
                                statustext.text = "Password is incorrect";
                            }
                            isaccount  = true;
                        }
                    }
                }
            }

            if(isaccount == false)
            {
                statustext.text = "Account does not exist";
            }
            
        }
    }
    static bool ReadWrite(SheetsService service,string key,string keyvalue,string sheetName)
    {
        bool isexistieren = false;
        List<string> playernamelist = new List<string>();
        ValueRange rVR;
        string wRange;
        int rowNumber = 1;
        wRange = string.Format("{0}!A{1}:B", sheetName, rowNumber);
        SpreadsheetsResource.ValuesResource.GetRequest getRequest
            = service.Spreadsheets.Values.Get(spreadsheetId, wRange);
        rVR = getRequest.Execute();
        var values = rVR.Values;
        if(values.Count != 0)
        {
            foreach (var value in values)
            {
                foreach (var valuee in value)
                {
                    playernamelist.Add(valuee.ToString());
                    break;
                }
            }
            foreach (string playername in playernamelist)
            {
                if (key == playername)
                {
                    isexistieren = true;
                    break;
                }
            }
        }

        if(isexistieren == false)
        {
            if (values != null && values.Count > 0) rowNumber = values.Count + 1;
            wRange = string.Format("{0}!A{1}:B{1}", sheetName, rowNumber); 
            ValueRange valueRange = new ValueRange();
            valueRange.Range = wRange;
            valueRange.MajorDimension = "ROWS";
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            string dts = dt.ToString("HH:mm:ss");
            var oblist = new List<object>() { string.Format("{0}", key), keyvalue };
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, wRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var uUVR = updateRequest.Execute();
            return false;
        }
        else
        {
            return true;
        }

    }

    void  ReadValue(SheetsService service, string key, string keyvalue, string sheetName)
    {
        bool isexistieren = false;
        List<string> playernamelist = new List<string>();
        int n = 0;
        ValueRange rVR;
        string wRange;
        int rowNumber = 1;
        wRange = string.Format("{0}!A{1}:B", sheetName, rowNumber);
        SpreadsheetsResource.ValuesResource.GetRequest getRequest
            = service.Spreadsheets.Values.Get(spreadsheetId, wRange);
        rVR = getRequest.Execute();
        var values = rVR.Values;
        if (values.Count != 0)
        {
            foreach (var value in values)
            {
                foreach (var valuee in value)
                {
                    playernamelist.Add(valuee.ToString());
                    break;
                }
            }
            foreach (string playername in playernamelist)
            {
                if (key == playername)
                {
                    isexistieren = true;
                    break;
                }
                n++;
            }
        }

        if (isexistieren == false)
        {
            if (values != null && values.Count > 0) rowNumber = values.Count + 1;
            wRange = string.Format("{0}!A{1}:B{1}", sheetName, rowNumber); 
            ValueRange valueRange = new ValueRange();
            valueRange.Range = wRange;
            valueRange.MajorDimension = "ROWS";
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            string dts = dt.ToString("HH:mm:ss");
            var oblist = new List<object>() { string.Format("{0}", key), keyvalue };
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, wRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var uUVR = updateRequest.Execute();
        }
        else
        {
            rowNumber = n + 1;
            Debug.Log(rowNumber);
            wRange = string.Format("{0}!A{1}:B{1}", sheetName, rowNumber); 
            ValueRange valueRange = new ValueRange();
            valueRange.Range = wRange;
            valueRange.MajorDimension = "ROWS";
            DateTime dt = new DateTime();
            dt = DateTime.Now;
            string dts = dt.ToString("HH:mm:ss");
            var oblist = new List<object>() { string.Format("{0}", key), keyvalue };
            valueRange.Values = new List<IList<object>> { oblist };
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, wRange);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            var uUVR = updateRequest.Execute();
        }

    }
}
