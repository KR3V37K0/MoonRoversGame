using System.ComponentModel;
using UnityEngine;

public class Order
{
    public string name {get;private set;}
    public Base from {get;private set;}
    public Base to {get;private set;}
    public int weight {get;private set;}
    public int reward_coins {get;private set;}
    public int reward_cards {get;private set;}

        public Order(string name, Base from, Base to, int weight, int reward_coins, int reward_cards)
    {
        this.name = name;
        this.from = from;
        this.to = to;
        this.weight = weight;
        this.reward_coins = reward_coins;
        this.reward_cards = reward_cards;
    }
}
