using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayVisualizer : MonoBehaviour
{
    [SerializeField] TMP_Text txt_quota_max, txt_quota_current,txt_day;
    [SerializeField] Button btn_next;
    Day day;


    void OnEnable()
    {
        Inventory.OnOrderComplete+=OnOrderComplete;
    }
    void OnDisable()
    {
        Inventory.OnOrderComplete-=OnOrderComplete;
    }

    public void Set_Day(Day _day)
    {
        day=_day;
        txt_day.text="day "+_day.id+1;
        txt_quota_max.text="/"+_day.quota;
        txt_quota_current.text="000";
    }
    void OnOrderComplete(Order order)
    {
        Inventory.Innstance.coins+=order.reward_coins;
        Add_CurrentQuota(order.reward_coins);
    }

    void Add_CurrentQuota(int value)
    {
        txt_quota_current.text+=value.ToString();

        if(Inventory.Innstance.coins>=day.quota)btn_next.interactable=true;
        else btn_next.interactable=false;
    }
    public void btn_NextDay()
    {
        DaySystem.i.NextDay();
    }
}
