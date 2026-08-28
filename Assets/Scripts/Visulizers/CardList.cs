using System.Collections.Generic;
using UnityEngine;

public class CardList : MonoBehaviour
{
    [SerializeField]GameObject prefab;
    List<CardVisualizer>cards=new List<CardVisualizer>();
   public void AddCard(Card _card)
    {
        GameObject obj = Instantiate(prefab, transform);
        cards.Add(obj.GetComponent<CardVisualizer>());
        cards[cards.Count - 1].SetInfo(_card);
    }
    public void DeleteCard(Card _card)
    {
        CardVisualizer target = null;
        foreach (var visualizer in cards)
        {
            if (visualizer.GetCard() == _card)
            {
                target = visualizer;
                break;
            }
        }

        if (target != null)
        {
            cards.Remove(target);
            Destroy(target.gameObject);
        }
        else
        {
            Debug.LogWarning($"Карта '{_card?.id+" "+_card?.type}' не найдена в списке!");
        }
    }
}
