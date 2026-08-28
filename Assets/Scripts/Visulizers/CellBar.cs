using UnityEngine;
using UnityEngine.UI;

public class CellBar : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    private int count;
    private Image[] cells;

    public void SetCount(int _count)
    {
        count = _count;
        Visualize();
    }

    private void Visualize()
    {
        // Удаляем лишние
        while (transform.childCount > count)
        {
            Transform child = transform.GetChild(transform.childCount - 1);
            DestroyImmediate(child.gameObject);
        }

        // Добавляем недостающие
        while (transform.childCount < count)
        {
            Instantiate(cellPrefab, transform);
        }

        // Сохраняем ссылки на все Image
        cells = GetComponentsInChildren<Image>();
        // Первый Image может быть у самого объекта, пропускаем его если он есть
        // Лучше хранить ссылки на дочерние Image
        var childImages = new System.Collections.Generic.List<Image>();
        foreach (Transform child in transform)
        {
            var img = child.GetComponent<Image>();
            if (img != null)
                childImages.Add(img);
        }
        cells = childImages.ToArray();

        // Включаем все ячейки по умолчанию
        foreach (var cell in cells)
        {
            if (cell != null)
                cell.enabled = true;
        }
    }

    public void ChangeCells(int currentEnergy)
    {
        if (cells == null || cells.Length == 0)
        {
            // Если массив пуст, инициализируем
            var childImages = new System.Collections.Generic.List<Image>();
            foreach (Transform child in transform)
            {
                var img = child.GetComponent<Image>();
                if (img != null)
                    childImages.Add(img);
            }
            cells = childImages.ToArray();
        }

        // Количество ячеек для отображения = currentEnergy (но не больше count)
        int visibleCount = Mathf.Clamp(currentEnergy, 0, count);

        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i] != null)
            {
                // Включаем ячейку, если её индекс меньше visibleCount
                cells[i].enabled = i < visibleCount;
            }
        }
    }
}