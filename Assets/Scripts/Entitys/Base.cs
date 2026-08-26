using UnityEngine;
using SQLite;
using System.Collections.Generic;

[Table("Base")]
public class Base
{
    [PrimaryKey, AutoIncrement]
    public int id {get; set;}
    public string name {get; set;}
    public string s_position {get; set;}
    public Vector2 position {get; set;}
    public string s_order_names {get; set;}
    public List<string> order_names {get; set;}

    public void Process()
    {
        ProcessNames();
        ProcessPosition();
    }
    void ProcessNames()
    {
        if (!string.IsNullOrEmpty(s_order_names))
        {
            string[] names = s_order_names.Split('|', System.StringSplitOptions.RemoveEmptyEntries);
            order_names = new List<string>(names);
        }
        else
        {
            order_names = new List<string>();
            Debug.Log("wrong orders names "+s_order_names);
        }
    }
    void ProcessPosition()
    {
        if (!string.IsNullOrEmpty(s_position))
        {
            string[] parts = s_position.Split('|');
            if (parts.Length == 2 && 
                float.TryParse(parts[0], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x) &&
                float.TryParse(parts[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y))
            {
                position = new Vector2(x, y);
                return;
            }
        }
        Debug.LogWarning($"wrong s_position: {s_position}");
    }
}
