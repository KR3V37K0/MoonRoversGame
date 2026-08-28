using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class RoverManager : MonoBehaviour, IBootable
{
    public static RoverManager Instance { get; private set; }

    [SerializeField] private GameObject roverPrefab;
    [SerializeField] private Transform roverParent;
    [SerializeField] private Grid grid;
    [SerializeField] private Tilemap overlayHexTilemap;
    [SerializeField] private Camera mainCamera;
    [SerializeField]GameObject panel_tips;

    private Dictionary<int, RoverController> roverMap = new Dictionary<int, RoverController>();
    private List<RoverController> roverControllers = new List<RoverController>();
    private RoverController selectedRover;

    public System.Action<RoverController> OnRoverSelected;
    public System.Action<RoverController> OnRoverMoved;
    public System.Action<RoverController> OnRoverEnergyChanged;

    public void Boot()
    {
        Instance = this;

        var roversData = DataBaseSystem.i.GetRovers();
        var bases = DataBaseSystem.i.GetBases();

        foreach (var roverData in roversData)
        {
            Base startBase = bases.Count > 0 ? bases[0] : null;
            if (startBase == null) continue;

            Vector2Int startHex = new Vector2Int((int)startBase.position.x, (int)startBase.position.y);

            GameObject roverObj = Instantiate(roverPrefab, roverParent);
            var controller = roverObj.GetComponent<RoverController>();
            controller.Init(roverData, startHex, grid, overlayHexTilemap);
            controller.OnRouteBuilt += OnRouteBuilt;
            controller.OnMovementComplete += OnMovementComplete;

            roverControllers.Add(controller);
            roverMap[roverData.id] = controller;
        }
    }
    public System.Action OnRoverDeselected;
    public List<RoverController> GetAllRovers()
    {
        return roverControllers;
    }
    public void DeselectRover()
    {
        if (selectedRover != null)
        {
            panel_tips.SetActive(false);
            selectedRover.isRouteBuilding = false;
            //selectedRover.HideHighlight();
            selectedRover = null;
            OnRoverDeselected?.Invoke();
        }
    }

    public void SelectRover(Rover rover)
    {
        if (!roverMap.TryGetValue(rover.id, out var roverController))
        {
            Debug.LogWarning($"Ровер с ID {rover.id} не найден!");
            return;
        }

        // Если выбрали того же ровера — снимаем выбор
        if (selectedRover == roverController)
        {
            DeselectRover();
            return;
        }

        // Если есть выбранный ровер — выходим из режима построения
        if (selectedRover != null && selectedRover.isRouteBuilding)
        {
            selectedRover.isRouteBuilding = false;
            //selectedRover.HideHighlight();
        }

        selectedRover = roverController;

        if (Inventory.Innstance.SelectedCardID != -1)
        {
            Inventory.Innstance.UseCard(selectedRover);
        }

        selectedRover.StartBuildingRoute();
        panel_tips.SetActive(true);

        OnRoverSelected?.Invoke(roverController);
    }

    public RoverController GetSelectedRover()
    {
        return selectedRover;
    }

    public RoverController GetRoverAtHex(Vector2Int hex)
    {
        return roverControllers.Find(r => r.currentHex == hex);
    }

    public RoverController GetRoverById(int id)
    {
        roverMap.TryGetValue(id, out var controller);
        return controller;
    }

    private void OnRouteBuilt(RoverController rover)
    {
        DeselectRover();
        //Debug.Log($"Ровер {rover.roverData.id} построил маршрут длиной {rover.route.Count} гексов.");
    }

    private void OnMovementComplete(RoverController rover)
    {
        Debug.Log($"Ровер {rover.roverData.id} прибыл на гекс {rover.currentHex}.");
        OnRoverMoved?.Invoke(rover);
    }

    public void OnTurn()
    {
        DeselectRover();
        foreach (var rover in roverControllers)
        {
            if (rover.route.Count > 1 && !rover.isMoving)
            {
                rover.MoveOneStep();
            }
        }
    }

    public Vector3 HexToWorld(Vector2Int hex)
    {
        Vector3Int cellPos = new Vector3Int(hex.x, hex.y, 0);
        return grid.CellToWorld(cellPos);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (selectedRover == null || !selectedRover.isRouteBuilding)
        {
            //Debug.Log("Нет выбранного ровера или ровер не строит маршрут.");
            return;
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0));
        Vector3Int cellPos = grid.WorldToCell(worldPos);
        Vector2Int hexPos = new Vector2Int(cellPos.x, cellPos.y);

        selectedRover.AddPointToRoute(hexPos);
    }

    public void OnFinishRoute(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (selectedRover != null && selectedRover.isRouteBuilding)
        {
            selectedRover.FinishRoute();
        }
    }

    public void OnCancelRoute(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (selectedRover != null && selectedRover.isRouteBuilding)
        {
            selectedRover.CancelRoute();
            selectedRover = null;
        }
    }
}