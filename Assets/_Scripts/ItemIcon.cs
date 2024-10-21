using UnityEngine;
using UnityEngine.UI;

public class ItemIcon : MonoBehaviour
{
    [SerializeField] private Image _useImage;
    [SerializeField] private Sprite _coinImage;
    public void ChangeIcon(Sprite sprite = null)
    {
        if (sprite == null)
        {
            sprite = _coinImage;
        }

        _useImage.sprite = sprite;

    }
}
