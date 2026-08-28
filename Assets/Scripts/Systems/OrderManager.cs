using UnityEngine;
using System.Collections.Generic;

public class OrderManager : MonoBehaviour, IBootable
{
    [SerializeField] private BaseHighlighter baseHighlighter;
    [SerializeField] private OrderList orderList;
    [SerializeField] private RoverManager roverManager;

    private Order selectedOrder;
    private Dictionary<int, Order> activeDeliveries = new Dictionary<int, Order>();

    public void Boot()
    {
        foreach (var visualizer in orderList.GetComponentsInChildren<OrderVisualizer>())
        {
            visualizer.OnOrderSelected += OnOrderClicked;
        }

        roverManager.OnRoverMoved += OnRoverMoved;
        roverManager.OnRoverSelected += OnRoverSelected;
        roverManager.OnRoverDeselected += OnRoverDeselected;

        UpdateOrderVisuals();
    }

   private void OnOrderClicked(Order order)
    {
        var selectedRover = roverManager.GetSelectedRover();

        // Если заказ уже в пути — игнорируем
        if (activeDeliveries.ContainsValue(order))
        {
            Debug.Log("Заказ уже в пути!");
            return;
        }

        // Если ровер выбран и может взять заказ — берём
        if (selectedRover != null && CanTakeOrder(selectedRover, order))
        {
            TakeOrder(selectedRover, order);
            baseHighlighter.HighlightRoute(order.from, order.to);
            return;
        }

        // Если кликнули на тот же заказ — снимаем выделение
        if (selectedOrder == order)
        {
            DeselectOrder();
            return;
        }

        // Иначе — показываем маршрут
        selectedOrder = order;
        baseHighlighter.HighlightRoute(order.from, order.to); // <-- это должно работать
    }

    private bool CanTakeOrder(RoverController rover, Order order)
    {
        if (rover == null) return false;
        if (activeDeliveries.ContainsKey(rover.roverData.id)) return false;
        if (activeDeliveries.ContainsValue(order)) return false;

        Vector2Int roverHex = rover.currentHex;
        Vector2Int fromHex = new Vector2Int((int)order.from.position.x, (int)order.from.position.y);

        if (roverHex != fromHex) return false;
        if (rover.roverData.max_weight < order.weight) return false;

        return true;
    }

    private void TakeOrder(RoverController rover, Order order)
    {
        activeDeliveries[rover.roverData.id] = order;
        Debug.Log($"Ровер {rover.roverData.id} взял заказ {order.name}");

        DeselectOrder();
        orderList.MarkOrderAsInDelivery(order);
        UpdateOrderVisuals();
    }

    private void OnRoverSelected(RoverController rover)
    {
        // Если есть выбранный заказ, но ровер не может его взять — снимаем выделение заказа
        if (selectedOrder != null && !CanTakeOrder(rover, selectedOrder))
        {
            DeselectOrder();
        }
        UpdateOrderVisuals();
    }

    private void OnRoverDeselected()
    {
        // Если ровер снят с выбора — заказы становятся доступными для просмотра
        // selectedOrder остаётся, чтобы показывать маршрут, если он был
        UpdateOrderVisuals();
    }

    private void OnRoverMoved(RoverController rover)
    {
        if (activeDeliveries.TryGetValue(rover.roverData.id, out Order order))
        {
            Vector2Int roverHex = rover.currentHex;
            Vector2Int toHex = new Vector2Int((int)order.to.position.x, (int)order.to.position.y);

            if (roverHex == toHex)
            {
                CompleteDelivery(rover, order);
            }
        }
    }

    private void CompleteDelivery(RoverController rover, Order order)
    {
Debug.Log($"order {order.name} delivered {rover.roverData.ico}");
        activeDeliveries.Remove(rover.roverData.id);

        orderList.RemoveOrder(order);
        Inventory.OnOrderComplete.Invoke(order);
        UpdateOrderVisuals();
    }

    private void UpdateOrderVisuals()
    {
        var selectedRover = roverManager.GetSelectedRover();

        foreach (Transform child in orderList.transform)
        {
            var visualizer = child.GetComponent<OrderVisualizer>();
            if (visualizer == null) continue;

            var order = visualizer.GetOrder();
            if (order == null) continue;

            // Если заказ в пути
            if (activeDeliveries.ContainsValue(order))
            {
                visualizer.SetInDelivery(true);
                continue;
            }

            // Если ровер не выбран — все заказы в состоянии Default (белые и кликабельные)
            if (selectedRover == null)
            {
                visualizer.SetState(OrderState.Default);
                continue;
            }

            // Если ровер выбран — проверяем доступность
            bool isAvailable = CanTakeOrder(selectedRover, order);
            visualizer.SetAvailable(isAvailable);
        }
    }

    public void DeselectOrder()
    {
        selectedOrder = null;
        baseHighlighter.ClearHighlights(); // <-- убираем иконки с карты
    }

    // Метод для явного сброса выделения заказа (например, при старте нового дня)
    public void ClearOrderSelection()
    {
        DeselectOrder();
        UpdateOrderVisuals();
    }

    private void OnDestroy()
    {
        foreach (var visualizer in orderList.GetComponentsInChildren<OrderVisualizer>())
        {
            visualizer.OnOrderSelected -= OnOrderClicked;
        }
        roverManager.OnRoverMoved -= OnRoverMoved;
        roverManager.OnRoverSelected -= OnRoverSelected;
        roverManager.OnRoverDeselected -= OnRoverDeselected;
    }
}