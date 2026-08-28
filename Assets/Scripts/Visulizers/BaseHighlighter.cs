using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class BaseHighlighter : MonoBehaviour, IBootable
{
    [SerializeField] private Tilemap overlayTilemap;
    [SerializeField] private TileBase fromTile; 
    [SerializeField] private TileBase toTile;  

    private Dictionary<Vector3Int, Base> basePositions = new Dictionary<Vector3Int, Base>();

    public void Boot()
    {
        var allBases = DataBaseSystem.i.GetBases();
        foreach (var baseData in allBases)
        {
            Vector3Int pos = new Vector3Int((int)baseData.position.x, (int)baseData.position.y, 0);
            basePositions[pos] = baseData;
        }
    }

    public void HighlightRoute(Base from, Base to)
    {
        ClearHighlights();

        Vector3Int fromPos = new Vector3Int((int)from.position.x, (int)from.position.y, 0);
        Vector3Int toPos = new Vector3Int((int)to.position.x, (int)to.position.y, 0);

        if (fromTile != null)
            overlayTilemap.SetTile(fromPos, fromTile);
        if (toTile != null)
            overlayTilemap.SetTile(toPos, toTile);

    }

    public void ClearHighlights()
    {
        overlayTilemap.ClearAllTiles();
    }
}