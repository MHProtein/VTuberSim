using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.UI
{
    public class VMultiplierBlockUI : VUIBehaviour
    {
        [SerializeField] private Image image;
        public Color Color => _color;
        private Color _color;

        public void SetColor(Color color)
        {
            this._color = color;
            image.color = color;
        }
    }
}