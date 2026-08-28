using UnityEngine;
using SQLite;

public class Rover
{
    [PrimaryKey, AutoIncrement]
    public int id { get; set; }
    public int energy { get; set; }
    public int max_weight { get; set; }
    public string ico { get; set; }
}