using UnityEngine;
using SQLite;
using System.Collections.Generic;
using System.IO;

public class DataBaseSystem : MonoBehaviour,IBootable
{
    SQLiteConnection db;
    public static DataBaseSystem i;
    public void Boot()
    {
        if(i!=null)DestroyImmediate(gameObject);
        i=this;
        db = new SQLiteConnection(Path.Combine(Application.streamingAssetsPath, "MoonRovers.db"));
    }

    public List<Rover> GetRovers()
    {
        return db.Table<Rover>().ToList();
    }
    public List<Base> GetBases()
    {
        var bases = db.Table<Base>().ToList();
    
        foreach (var baseItem in bases)
        {
            baseItem.Process();
        }
        
        return bases;
    }
    public List<Day> GetDays()
    {
        return db.Table<Day>().ToList();
    }
}