using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public struct GameStatsLineTemplate
{
    // To add parameters/collumns to your CSV, add a public string field and add it to the constructor below
    
    public string EventName;
    public string EventPositionX;
    public string EventPositionY;
    public string EventPositionZ;
    
    public GameStatsLineTemplate(Vector3 position)
    {
        EventName = "attack";
        EventName = "throw";
        EventName = "Roll";
        EventPositionX = position.x.ToString(CultureInfo.CurrentCulture);
        EventPositionY = position.y.ToString(CultureInfo.CurrentCulture);
        EventPositionZ = position.z.ToString(CultureInfo.CurrentCulture);
    }
    
    

    public override string ToString()
    {
        List<string> fieldsValue = new List<string>();
        
        foreach (var field in typeof(GameStatsLineTemplate).GetFields())
        {
            fieldsValue.Add(field.GetValue(this).ToString());
        }
        
        return CsvUtility.MakeCsvLine(fieldsValue);
    }
}