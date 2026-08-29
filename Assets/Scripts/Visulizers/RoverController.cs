using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoverController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Tile availableTile;
    [SerializeField] private Tilemap terrainTilemap;

    [Header("State")]
    public Rover roverData;
    public int currentEnergy;
    public Vector2Int currentHex;
    public List<Vector2Int> route = new List<Vector2Int>();
    public int currentStepIndex = 0; 
    public bool isMoving = false;
    public bool isRouteBuilding = false;
    private Tilemap routeOverlay;
    private Grid grid;

    public System.Action<RoverController> OnRouteBuilt;
    public System.Action<RoverController> OnMovementComplete;
    public System.Action<RoverController> OnStepCompleted;
    public System.Action<RoverController> OnStatsChanged;

    void OnEnable()
    {
        DaySystem.OnNextDay+=OnNewDay;
    }
    void OnDisable()
    {
        DaySystem.OnNextDay-=OnNewDay;
    }
    private void OnNewDay(int day)
    {
        RemoveAddedWeight();
    }
    public void Init(Rover data, Vector2Int startHex, Grid gridReference, Tilemap overlay, Tilemap terrain)
    {
        routeOverlay = overlay;
        roverData = data;
        currentHex = startHex;
        grid = gridReference;
        transform.position = HexToWorld(startHex);
        lineRenderer.positionCount = 0;
        currentStepIndex = 0;
        SetImage();
        currentEnergy=roverData.energy;
        terrainTilemap = terrain;
    }

    void SetImage()
    {
        Sprite sprite = Resources.Load<Sprite>("Rovers/" + roverData.ico);
        if (sprite != null)
            GetComponent<SpriteRenderer>().sprite = sprite;
        else
            Debug.LogWarning($"Спрайт для ровера {roverData.ico} не найден!");
    }

    // === ПОСТРОЕНИЕ МАРШРУТА ===

    public void StartBuildingRoute()
    {
        if (isMoving) return;

        // НЕ вызываем CancelRoute() — чтобы не сбросить подсветку заказа
        // Просто очищаем только маршрут
        route.Clear();
        route.Add(currentHex);
        currentStepIndex = 0;
        isRouteBuilding = true;

        UpdateRouteVisuals();
        HighlightAvailableNeighbors();

        //Debug.Log($"Начато построение маршрута для ровера {roverData.id}. Текущий гекс: {currentHex}");
    }
    

    public void AddPointToRoute(Vector2Int newHex)
    {
        if (!isRouteBuilding || isMoving) return;

        Vector2Int lastHex = route[route.Count - 1];

        if (!HexHelper.AreNeighbors(lastHex, newHex))
        {
           // Debug.Log("Можно кликать только по соседним гексам!");
            return;
        }

        if (route.Count > 1 && route[route.Count - 2] == newHex)
        {
            route.RemoveAt(route.Count - 1);
            UpdateRouteVisuals();
            HighlightAvailableNeighbors();
            return;
        }

        route.Add(newHex);
        UpdateRouteVisuals();
        HighlightAvailableNeighbors();
    }

    private void HighlightAvailableNeighbors()
    {
        routeOverlay.ClearAllTiles(); // чистим только этот слой

        Vector2Int lastHex = route[route.Count - 1];
        var neighbors = HexHelper.GetNeighbors(lastHex);

        foreach (var hex in neighbors)
        {
            Vector3Int cellPos = new Vector3Int(hex.x, hex.y, 0);
            routeOverlay.SetTile(cellPos, availableTile);
        }
    }


    public void CancelRoute()
    {
        isRouteBuilding = false;
        route.Clear();
        lineRenderer.positionCount = 0;
        routeOverlay.ClearAllTiles(); // чистим только подсветку, НЕ трогаем заказы
    }

    public void FinishRoute()
    {
        if (!isRouteBuilding || route.Count < 2)
        {
            Debug.Log("Маршрут должен содержать минимум 2 гекса (текущий + следующий)!");
            return;
        }

        isRouteBuilding = false;
        routeOverlay?.ClearAllTiles();
        OnRouteBuilt?.Invoke(this);

        //Debug.Log($"Маршрут завершён. Длина: {route.Count} гексов.");
    }

    private void UpdateRouteVisuals()
    {
        lineRenderer.positionCount = route.Count;
        for (int i = 0; i < route.Count; i++)
        {
            Vector3 worldPos = HexToWorld(route[i]);
            lineRenderer.SetPosition(i, worldPos);
        }
    }

    // === ДВИЖЕНИЕ ПОШАГОВО (С ИНДЕКСОМ) ===

    public async System.Threading.Tasks.Task MoveOneStep()
    {
        if (isMoving || route.Count < 2) return;

        // Проверяем стоимость следующего гекса
        Vector2Int targetHex = route[1];
        int hexCost = GetHexCost(targetHex);
        
        if (currentEnergy < hexCost)
        {
            Debug.Log($"Не хватает энергии для прохода через этот гекс! Нужно {hexCost}, есть {currentEnergy}");
            FinishRoute();
            CancelRoute();
            return;
        }

        isMoving = true;

        Vector3 targetWorld = HexToWorld(targetHex);

        float t = 0;
        Vector3 startPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(startPos, targetWorld, t);
            await System.Threading.Tasks.Task.Yield();
        }

        transform.position = targetWorld;
        currentHex = targetHex;

        route.RemoveAt(0);

        currentEnergy -= hexCost; // <-- тратим энергию в зависимости от типа местности
        RoverManager.Instance?.OnRoverEnergyChanged.Invoke(this);

        UpdateRouteVisuals();

        isMoving = false;

        if (route.Count == 1)
        {
            route.Clear();
            lineRenderer.positionCount = 0;
            routeOverlay?.ClearAllTiles();
            OnMovementComplete?.Invoke(this);
        }

        OnStepCompleted?.Invoke(this);
    }
    // === ХЕЛПЕРЫ ===
    private int GetHexCost(Vector2Int hex)
    {
        if (terrainTilemap == null) return 1;
        
        Vector3Int cellPos = new Vector3Int(hex.x, hex.y, 0);
        TileBase tile = terrainTilemap.GetTile(cellPos);
        
        if (tile is TerrainTile terrainTile)
        {
            return terrainTile.energyCost;
        }
        
        return 1;
    }
    private Vector3 HexToWorld(Vector2Int hex)
    {
        Vector3Int cellPos = new Vector3Int(hex.x, hex.y, 0);
        return grid.CellToWorld(cellPos);
    }
    
    public void AddEnergy(int amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0, roverData.energy);
        OnStatsChanged?.Invoke(this);
        RoverManager.Instance?.OnRoverEnergyChanged?.Invoke(this);
    }

    private int addedWeight = 0;

    public void AddWeight(int amount)
    {
        addedWeight += amount;
        roverData.max_weight += amount;
        OnStatsChanged?.Invoke(this);
    }

    public void RemoveAddedWeight()
    {
        roverData.max_weight -= addedWeight;
        addedWeight = 0;
        OnStatsChanged?.Invoke(this);
    }
    


}
// === STATIC HELPER ДЛЯ ГЕКСОВ ===
public static class HexHelper
{
    // Соседи для чётной строки (y % 2 == 0)
    private static Vector2Int[] evenNeighbors = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // право
        new Vector2Int(0, 1),   // верх-право
        new Vector2Int(-1, 1),  // верх-лево
        new Vector2Int(-1, 0),  // лево
        new Vector2Int(-1, -1), // низ-лево
        new Vector2Int(0, -1)   // низ-право
    };

    // Соседи для нечётной строки (y % 2 == 1)
    private static Vector2Int[] oddNeighbors = new Vector2Int[]
    {
        new Vector2Int(1, 0),   // право
        new Vector2Int(1, 1),   // верх-право
        new Vector2Int(0, 1),   // верх-лево
        new Vector2Int(-1, 0),  // лево
        new Vector2Int(0, -1),  // низ-лево
        new Vector2Int(1, -1)   // низ-право
    };

    public static bool AreNeighbors(Vector2Int a, Vector2Int b)
    {
        var neighbors = GetNeighbors(a);
        return neighbors.Contains(b);
    }

    public static List<Vector2Int> GetNeighbors(Vector2Int hex)
    {
        var result = new List<Vector2Int>();

        // Выбираем массив соседей в зависимости от чётности строки
        Vector2Int[] neighbors = hex.y % 2 == 0 ? evenNeighbors : oddNeighbors;

        foreach (var offset in neighbors)
        {
            result.Add(hex + offset);
        }

        return result;
    }
}
