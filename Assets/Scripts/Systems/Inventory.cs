using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public static System.Action<Order> OnOrderComplete;
    public static System.Action<Rover> OnRoverSelected;
    //public static System.Action<Card> OnCardSelect;
    //public static System.Action<Card> OnCardUse;
    public static Inventory Innstance;
    [SerializeField]CardList list;

    public int coins=0;

    public List<Card> Cards = new List<Card>();
    public int SelectedCardID;
    int unicCards=0;

    void Awake()
    {
        Innstance=this;
        OnOrderComplete+=GiveCards;
        //OnRoverSelected+=OnSelectRover;
    }
    void OnDestroy()
    {
        OnOrderComplete-=GiveCards;
        //OnRoverSelected-=OnSelectRover;
    }
    public void OnSelectCard(Card _card)
    {
        if (SelectedCardID == _card.id)
        {
            SelectedCardID=-1;
        }
        else SelectedCardID=_card.id;
    }
    public void UseCard(RoverController _rover)
    {
        Card card = Cards.FirstOrDefault(c => c.id == SelectedCardID);
        if (card == null)
        {
            Debug.LogWarning($"Card with ID {SelectedCardID} not found!");
            return;
        }

        card.Use(_rover);

        Cards.Remove(card);
        list.DeleteCard(card);

        SelectedCardID = -1;
    }
    void GiveCards(Order _order)
    {
        if(_order.reward_cards==0)return;

        for(int i = 0; i<_order.reward_cards; i++)
        {
            unicCards++;
            Cards.Add(new Card(unicCards,(CardType)Random.Range(0, System.Enum.GetValues(typeof(CardType)).Length)));
            list.AddCard(Cards[Cards.Count-1]);
        }
    }
}
public class Card
{
    public int id;
    public CardType type;
    public void Use(RoverController _rover)
    {
        switch (type)
        {
            case CardType.EnergyCard:
                _rover.AddEnergy(10);                   //СКОЛЬК????
                break;
            case CardType.WeightCard:
                _rover.AddWeight(50);                   // СКОЛЬК????
                break;
            default:
                Debug.LogError("unknown card type");
                break;
        }
    }
    public Card(int _id,CardType _type)
    {
        id=_id;
        type=_type;
    }
}
public enum CardType
{
    EnergyCard,
    WeightCard
}