using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class OrdersGenerator : MonoBehaviour,IBootable
{
    [SerializeField] OrderList list;

    [SerializeField] Vector2Int potentialWeight;
    [SerializeField] int CardChance;

    List<Base> bases;
    public void Boot()
    {
        SetupBases();
        Generate();
    }
    void SetupBases()
    {
        bases = DataBaseSystem.i.GetBases();
        if (bases == null || bases.Count < 2)
        {
            Debug.LogWarning("Need more Bases");
            return;
        }
    }
    public void Generate()
    {
        if (bases == null || bases.Count < 2)
        {
            Debug.LogWarning("Need more Bases");
            return;
        }

        List<Order> orders = new List<Order>();
        for (int i = 0; i < DaySystem.i.currentDay.OrderPerDay; i++)
        {
            orders.Add(GenerateRandomOrder());
        }
        list.Set_NewList(orders);
    }
    private Order GenerateRandomOrder()
    {
        // Выбираем случайные базы (from и to должны быть разными)
        Base fromBase = bases[Random.Range(0, bases.Count)];
        Base toBase;
        do
        {
            toBase = bases[Random.Range(0, bases.Count)];
        } while (toBase == fromBase);

        // Случайное имя груза
        List<string> potential_names= fromBase.order_names.Concat(toBase.order_names).ToList();
        string name = potential_names[Random.Range(0, potential_names.Count)];

        // Случайный вес 
        int weight = Random.Range(potentialWeight.x, potentialWeight.y);

        // Награда: зависит от веса и расстояния 
        // Чем тяжелее груз и дальше расстояние — тем выше награда
        int distance = Mathf.RoundToInt(Vector2.Distance(fromBase.position, toBase.position));
        int rewardCoins = Mathf.RoundToInt(weight * 0.8f + distance * 0.5f + Random.Range(-10, 70));
        rewardCoins = Mathf.Max(10, rewardCoins); 

        // Награда карточками: иногда выпадает 
        int rewardCards = Random.Range(0, 100) < CardChance ? Random.Range(1, 3) : 0;

        return new Order(name, fromBase, toBase, weight, rewardCoins, rewardCards);
    }
}
