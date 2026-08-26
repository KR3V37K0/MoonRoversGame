using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderVisualizer : MonoBehaviour
{
    [SerializeField] private TMP_Text txt_name, txt_from, txt_to, txt_weigth, txt_coins, txt_cards;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage; // фон кнопки

    [Header("Colors")]
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color availableColor = new Color(0.2f, 0.8f, 0.2f); // зелёный
    [SerializeField] private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f); // серый
    [SerializeField] private Color inDeliveryColor = new Color(0.8f, 0.6f, 0f); // оранжевый/жёлтый

    private Order order;
    private bool isInDelivery = false;

    public System.Action<Order> OnOrderSelected;

    public void Init(Order _order)
    {
        order = _order;
        Visualize();
        SetState(OrderState.Default);
    
    }

    public void OnSelected()
    {
        OnOrderSelected?.Invoke(order);
    }

    private void Visualize()
    {
        if (order == null) return;

        txt_name.text = order.name;
        txt_from.text = order.from.name;
        txt_to.text = order.to.name;
        txt_weigth.text = order.weight.ToString();
        txt_coins.text = order.reward_coins.ToString();
        txt_cards.text = order.reward_cards.ToString();
    }

    // === СОСТОЯНИЯ ===

    public void SetState(OrderState state)
    {
        switch (state)
        {
            case OrderState.Available:
                backgroundImage.color = availableColor;
                button.interactable = true;
                break;

            case OrderState.Unavailable:
                backgroundImage.color = unavailableColor;
                button.interactable = false;
                break;

            case OrderState.InDelivery:
                backgroundImage.color = inDeliveryColor;
                button.interactable = false;
                isInDelivery = true;
                break;

            case OrderState.Default:
            default:
                backgroundImage.color = defaultColor;
                button.interactable = true;
                isInDelivery = false;
                break;
        }
    }

    public void SetInDelivery(bool inDelivery)
    {
        SetState(inDelivery ? OrderState.InDelivery : OrderState.Default);
    }

    public void SetAvailable(bool available)
    {
        if (isInDelivery) return;
        SetState(available ? OrderState.Available : OrderState.Unavailable);
    }

    public void ResetState()
    {
        isInDelivery = false;
        SetState(OrderState.Default);
    }

    public Order GetOrder() => order;

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}

public enum OrderState
{
    Default,
    Available,
    Unavailable,
    InDelivery
}