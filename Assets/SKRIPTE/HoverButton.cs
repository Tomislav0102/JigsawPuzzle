using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FirstCollection;

namespace PuzzleShape
{
    public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        GameManager _gm;
        [SerializeField] ButtonType buttonType;
        [SerializeField] Image offImage;

        public void Ini(GameManager gm)
        {
            _gm = gm;
            switch (buttonType)
            {
                case ButtonType.Backgound:
                    break;
                case ButtonType.ShowFinalImage:
                    break;
                case ButtonType.Quit:
                    break;
                case ButtonType.Sound:
                    offImage.enabled = !_gm.setting.soundOn;
                    break;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _gm.HoverToolTip(true, buttonType);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _gm.HoverToolTip(false, buttonType);
        }
    }
}
