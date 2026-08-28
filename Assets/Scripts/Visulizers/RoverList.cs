using UnityEngine;

public class RoverList : MonoBehaviour,IBootable
{
    [SerializeField]GameObject rover_pref;
    public void Boot()
    {
        foreach(RoverVisualizer child in transform.GetComponentsInChildren<RoverVisualizer>())
        {
            DestroyImmediate(child.gameObject);
        }
        foreach(Rover rover in DataBaseSystem.i.GetRovers())
        {
            AddRover(rover);
        }
    }
    
    public void AddRover(Rover _rover)
    {
        Instantiate(rover_pref,transform).GetComponent<RoverVisualizer>().Init(_rover);
    }
}
