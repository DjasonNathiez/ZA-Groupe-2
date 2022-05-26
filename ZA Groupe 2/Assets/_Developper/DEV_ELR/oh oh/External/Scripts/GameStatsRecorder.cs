using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class GameStatsRecorder : MonoBehaviour
{
    private const string SEPARATOR_META_DATA_TOOLTIP = "Used to tell Excel that we're seperating fields by our separator. Be careful, Tableau cannot read the CSV with this meta data. It should be used to view your data in Excel. To get rid of it you can always open the CSV with any notepad application and delete the first line.";
    
    private static GameStatsRecorder _instance;
    public static GameStatsRecorder Instance => _instance;
    
    [SerializeField] private string _separator = ";";
    [SerializeField] [Tooltip(SEPARATOR_META_DATA_TOOLTIP)] private bool _addSeparatorMetaData = false;
    [SerializeField] private bool _printSavePathToConsole = true;
    [SerializeField] private bool _runRecorderInEditor = true;

    private List<string> _recordedCsvLines = new List<string>();
    private DateTime _recordTimestamp;
    private string _timestampString => _recordTimestamp.ToUniversalTime().ToString("yyyy-MM-dd-HH-mm-ss");
    private bool _isRecording = false;
    private float _recordStartTime = 0f;
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this);
        }
    }
    
    void Start()
    {
#if UNITY_EDITOR
        if (_runRecorderInEditor)
        {
            StartRecord();
        }
#else
        StartRecord();
#endif
    }

    void OnApplicationQuit()
    {
        EndRecord();
    }

    public void StartRecord()
    {
        if (_isRecording) return;

        _recordTimestamp = DateTime.UtcNow;
        _recordStartTime = Time.unscaledTime;
        _isRecording = true;
    }

    public void EndRecord(bool saveFile = true)
    {
        if (!_isRecording) return;

        if (saveFile)
        {
            SaveGameStatsToFile();
        }
    }

    private void SaveGameStatsToFile()
    {
        List<string> CsvArray = new List<string>();
        
        if (_addSeparatorMetaData)
        {
            CsvArray.Add($"sep={_separator}");
        }
        
        CsvArray.Add(GenerateCsvFirstLine());
        CsvArray.AddRange(_recordedCsvLines);
        
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < CsvArray.Count; i++)
        {
            string line = CsvArray[i];
            stringBuilder.AppendLine(line);
        }

        string csvToSave = stringBuilder.ToString();
        
        string savedPath = CsvUtility.SaveToCsv(csvToSave, Path.Combine(Application.persistentDataPath, "GameStats"), _timestampString);

        if (_printSavePathToConsole)
        {
            Debug.Log($"Saved Stats to : {savedPath}");
        }
    }

    private string GenerateCsvFirstLine()
    {
        List<string> contents = new List<string> {"Timestamp", "EventTime"};
        
        foreach (var field in typeof(GameStatsLineTemplate).GetFields())
        {
            contents.Add(field.Name);
        }

        return CsvUtility.MakeCsvLine(contents);
    }

    private string GenerateGameStatCsvLine(GameStatsLineTemplate statContent)
    {
        StringBuilder stringBuilder = new StringBuilder();

        string timestamp = _timestampString;
        string timeSinceRecordStarted = Mathf.RoundToInt(GetTimeSinceRecordStarted()).ToString();

        stringBuilder.AppendJoin(_separator, new List<string>
        {
            timestamp,
            timeSinceRecordStarted,
            statContent.ToString()
        });
        
        return stringBuilder.ToString();
    }
    
    private float GetTimeSinceRecordStarted()
    {
        return _isRecording ? Time.unscaledTime - _recordStartTime : 0f;
    }

    public void RegisterEvent(GameStatsLineTemplate statContent)
    {
        _recordedCsvLines.Add(GenerateGameStatCsvLine(statContent));
    }
}
