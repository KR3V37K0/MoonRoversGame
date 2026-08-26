using System;
using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour,IBootable
{
    [SerializeField] DayVisualizer visualizer;
    public static DaySystem i{get; private set;}
    public Day currendDay{get; private set;}
    List<Day> days;
    public void Boot()
    {
        i=this;
        days = DataBaseSystem.i.GetDays();
        SetDay(0);
    }
    void SetDay(int i)
    {   
        if(days.Count<i)return;
        currendDay=days[i];
        visualizer.Set_Day(currendDay);
    }
    public void NextDay()
    {
        SetDay(currendDay.id+1);
    }
}
