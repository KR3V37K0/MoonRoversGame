using System.Collections.Generic;
using UnityEngine;

public class Boot : MonoBehaviour
{
    public GameObject[] bootableObjects;

    void Start()
    {
        foreach (var obj in bootableObjects)
        {
            var bootable = obj.GetComponent<IBootable>();
            if (bootable != null)
            {
                bootable.Boot();
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} does not implement IBootable");
            }
        }
    }
}
public interface IBootable
{
    public void Boot();
}
