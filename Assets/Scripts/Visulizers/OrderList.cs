using System.Collections.Generic;
using UnityEngine;

public class OrderList : MonoBehaviour
{
    [SerializeField]GameObject order_pref;

    public void Set_NewList(List<Order> orders)
    {
        Clear();
        foreach(Order order in orders)
        {
            AddOrder(order);
        }
    }
    void Clear()
    {
        var children = transform.GetComponentsInChildren<OrderVisualizer>();
        
        foreach(OrderVisualizer child in children)
        {
            Destroy(child.gameObject);
        }
    }
    void AddOrder(Order _order)
    {
        Instantiate(order_pref,transform).GetComponent<OrderVisualizer>().Init(_order);
    }
    public void RemoveOrder(Order order)
    {
        // Находим и удаляем визуализатор
        foreach (Transform child in transform)
        {
            var visualizer = child.GetComponent<OrderVisualizer>();
            if (visualizer != null && visualizer.GetOrder() == order)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }
    public void MarkOrderAsInDelivery(Order order)
    {
        foreach (Transform child in transform)
        {
            var visualizer = child.GetComponent<OrderVisualizer>();
            if (visualizer != null && visualizer.GetOrder() == order)
            {
                visualizer.SetInDelivery(true);
                break;
            }
        }
    }
}
