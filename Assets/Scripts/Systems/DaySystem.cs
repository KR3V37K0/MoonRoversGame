using System.Collections.Generic;
using UnityEngine;

public class DaySystem : MonoBehaviour, IBootable
{
    [SerializeField] private DayVisualizer visualizer;
    [SerializeField] private RoverManager roverManager;
    [SerializeField] private OrdersGenerator ordersGenerator;
    [SerializeField] private int turnsPerDay = 24;

    public static DaySystem i { get; private set; }
    public Day currentDay { get; private set; }
    public static System.Action<int> OnNextDay;

    private List<Day> days;
    private int currentTurn = 0;
    private Base startBase;

    public void Boot()
    {
        i = this;
        days = DataBaseSystem.i.GetDays();
        
        var bases = DataBaseSystem.i.GetBases();
        startBase = bases.Count > 0 ? bases[0] : null;
        
        SetDay(0);
    }

    private void SetDay(int index)
    {
        if (days.Count <= index) return;

        currentDay = days[index];
        visualizer.Set_Day(currentDay);
        currentTurn = 0;
        visualizer.SetTurnSlider(0);
        
        ordersGenerator.Generate();
        ResetRovers();
        
        OnNextDay?.Invoke(currentDay.id);
    }

    private void ResetRovers()
    {
        if (roverManager == null || startBase == null) return;

        var allRovers = roverManager.GetAllRovers();
        Vector2Int startHex = new Vector2Int((int)startBase.position.x, (int)startBase.position.y);

        foreach (var rover in allRovers)
        {
            rover.currentHex = startHex;
            rover.transform.position = roverManager.HexToWorld(startHex);
            rover.currentEnergy = rover.roverData.energy;
            rover.RemoveAddedWeight();
            rover.CancelRoute();
        }
    }

    public void NextDay()
    {
        SetDay(currentDay.id + 1);
    }

    public void btn_NextTurn()
    {
        currentTurn++;
        visualizer.SetTurnSlider((float)currentTurn / turnsPerDay);
        
        if (currentTurn >= turnsPerDay)
        {
            visualizer.DayIsOver();
        }
        else
        {
            roverManager.OnTurn();
        }
    }
}