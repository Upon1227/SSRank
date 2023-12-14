using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.IO;
using System;
using TMPro;
using System.Linq;

public class rankingManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI ranktext;
    [SerializeField]
    bool reverse;
    SortedDictionary<int, string> rankdic = new SortedDictionary<int, string>();
    Dictionary<int,string> rankdicreverse = new Dictionary<int, string>();

    void Start()
    {
        writeSheet();
    }
    static string ApplicationName = "Google Sheets API .NET Quickstart";
    static string spreadsheetId = "スプレッドシートID";
    static string sheetName = "ランキングのDB名";

    void writeSheet()
    {
        var service = OpenSheet();
        ReadWrite(service);
    }
    static SheetsService OpenSheet()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(Application.streamingAssetsPath + "/jsonファイル名", FileMode.Open, FileAccess.Read))
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

    void ReadWrite(SheetsService service)
    {
        int ranknum = 1;
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
            for(int i = 1;i < values.Count; i++)
            {
                var value = values[i];
                rankdic.Add(int.Parse(value[1].ToString()), value[0].ToString());
            }
        }
        rankdicreverse = rankdic.Reverse().ToDictionary(c => c.Key,c => c.Value);
        if (reverse)
        {
            foreach (var pair in rankdicreverse)
            {
                ranktext.text += ranknum.ToString() + "." + pair.Value + ":" + pair.Key + "\n";
                ranknum++;
            }
        }
        else
        {
            foreach (var pair in rankdic)
            {
                ranktext.text += ranknum.ToString() + "." + pair.Value + ":" + pair.Key + "\n";
                ranknum++;
            }
        }

    }
}
