using UnityEngine;
using SQLite;

public class Day
{
    [PrimaryKey, AutoIncrement]
    public int id {get; set;}
    public int OrderPerDay {get; set;}
    public int quota {get; set;}

}
