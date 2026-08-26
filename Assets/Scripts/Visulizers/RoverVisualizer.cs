using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoverVisualizer : MonoBehaviour
{
    [SerializeField]TMP_Text txt_weigth;
    [SerializeField]Image img_ico;
    [SerializeField]CellBar cell_bar;
    Rover rover;

    public void Init(Rover _rover)
    {
        rover = _rover;
        GetComponent<Button>().onClick.AddListener(() => RoverManager.Instance.SelectRover(rover)); 
        Visualize();
        RoverManager.Instance.OnRoverEnergyChanged+=ChangeEnergy;
    }
    void Visualize()
    {
        if(rover==null)return;

        img_ico.sprite = Resources.Load<Sprite>("Rovers/"+rover.ico);
        cell_bar.SetCount(rover.energy);
        txt_weigth.text=rover.max_weight.ToString()+" kg";
    }
    void ChangeEnergy(RoverController controller)
    {
        if(controller.roverData.id!=rover.id)return;
        cell_bar.ChangeCells(controller.currentEnergy);
    }
    public void OnClick()
    {
        Inventory.OnRoverSelected?.Invoke(rover);
    }
    void OnDisable()
    {
        RoverManager.Instance.OnRoverEnergyChanged-=ChangeEnergy;
    }
}
