using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, offsetColor, borderColor;
    [SerializeField] private SpriteRenderer renderer;
    public void Init(bool isOffset, bool isBorder)
    {
        
        if (isBorder)
        {
            renderer.color = borderColor;
        }
        else
        {
            renderer.color = isOffset ? offsetColor : baseColor;
        }
    }

}
