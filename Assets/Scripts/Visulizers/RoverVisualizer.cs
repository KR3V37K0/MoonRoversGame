using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;

public class RoverVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text txt_weight;
    [SerializeField] private Image img_ico;
    [SerializeField] private CellBar cell_bar;

    private Rover rover;
    private RoverController roverController;

    public void Init(Rover _rover)
    {
        rover = _rover;
        GetComponent<Button>().onClick.AddListener(() => RoverManager.Instance.SelectRover(rover));

        roverController = RoverManager.Instance?.GetRoverById(rover.id);
        if (roverController != null)
        {
            roverController.OnStatsChanged += OnStatsChanged;
        }
        else
        {
            Debug.LogWarning($"roverController для ровера {rover.id} не найден!");
        }

        Visualize();
        RoverManager.Instance.OnRoverEnergyChanged += ChangeEnergy;
    }

    public void Visualize()
    {
        if (rover == null) return;

        img_ico.sprite = Resources.Load<Sprite>("Rovers/" + rover.ico);
        cell_bar.SetCount(rover.energy);
        txt_weight.text = rover.max_weight.ToString() + " kg";
    }

    private void ChangeEnergy(RoverController controller)
    {
        if (controller.roverData.id != rover.id) return;
        cell_bar.ChangeCells(controller.currentEnergy);
    }

    private async void OnStatsChanged(RoverController controller)
    {
        await Task.Delay(100);
        if (controller.roverData.id != rover.id) return;
        // Обновляем вес
        txt_weight.text = controller.roverData.max_weight.ToString() + " kg";
        // Обновляем энергию (если изменилась)
        cell_bar.ChangeCells(controller.currentEnergy);
    }

    private void OnDisable()
    {
        RoverManager.Instance.OnRoverEnergyChanged -= ChangeEnergy;
        if (roverController != null)
        {
            roverController.OnStatsChanged -= OnStatsChanged;
        }
    }

    public void OnClick()
    {
        Inventory.OnRoverSelected?.Invoke(rover);
    }
}