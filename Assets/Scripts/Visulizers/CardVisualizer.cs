using UnityEngine.UI;
using UnityEngine;

public class CardVisualizer : MonoBehaviour
{
    Card card;
    [SerializeField]Image img;
    bool isSelected=false;
    public void SetInfo(Card _card)
    {
        card=_card;
        
        img.sprite=Resources.Load<Sprite>("Cards/"+_card.type);
    }
    public void OnSelect()
    {
        Inventory.Innstance.OnSelectCard(card);
        isSelected=!isSelected;
        ChangeColor();
    }
    public Card GetCard()
    {
        return card;
    }
    void ChangeColor()
    {
        if(isSelected)img.color=Color.green;
        else img.color=Color.white;
    }
}
