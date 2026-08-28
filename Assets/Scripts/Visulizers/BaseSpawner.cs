using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BaseSpawner : MonoBehaviour,IBootable
{
    [SerializeField] private Tilemap basesTilemap;
    public void Boot()
    {
        PlaceAllBases();
        
    }
    private void PlaceAllBases()
    {
        List<Base> bases = DataBaseSystem.i.GetBases();

        foreach (var baseData in bases)
        {
            // Координаты
            Vector3Int cellPosition = new Vector3Int(
                (int)baseData.position.x,
                (int)baseData.position.y,
                0
            );

            // Загружаем спрайт из Resources/Bases/ + имя базы
            Sprite baseSprite = Resources.Load<Sprite>($"Bases/{baseData.name}");
            if (baseSprite == null)
            {
                Debug.LogWarning($"Sprite not found for base: {baseData.name}");
                continue;
            }

            // Создаём тайл из спрайта
            Tile baseTile = ScriptableObject.CreateInstance<Tile>();
            baseTile.sprite = baseSprite;

            // Устанавливаем тайл
            basesTilemap.SetTile(cellPosition, baseTile);
        }
    }
}
