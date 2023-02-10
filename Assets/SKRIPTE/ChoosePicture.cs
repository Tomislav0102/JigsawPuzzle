using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace PuzzleShape
{
    public class ChoosePicture : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        ChoosePictureManager _manager;
        public Image picture;
        [SerializeField] Image selectionFrame;
        RectTransform _picRect;
        Color _colSelected = Color.white;
        Color _colNotSelected = new Color(1f, 1f, 1f, 0.2f);
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                selectionFrame.color = _selected ? _colSelected : _colNotSelected;

            }
        }
        bool _selected;
        int _index;

        public void Ini(ChoosePictureManager man, int index, Sprite sprite, float cellSize)
        {
            _manager= man;
            _index = index;
            picture.sprite = sprite;
            _picRect = picture.GetComponent<RectTransform>();

            Vector2 picSize = sprite.rect.size;
            float omjer = 0f; //omjer i smanjenaDim treba kasnije pobrisati
            float smanjenaDim = 0f;
            if (picSize.x > picSize.y)
            {
                omjer = picSize.y / picSize.x;
                smanjenaDim = omjer * cellSize;
                _picRect.sizeDelta = new Vector2(0f, cellSize * (picSize.y / picSize.x - 1));
            }
            else if (picSize.y > picSize.x)
            {
                omjer = picSize.x / picSize.y;
                smanjenaDim = omjer * cellSize;
                _picRect.sizeDelta = new Vector2(cellSize * (picSize.x / picSize.y - 1), 0f);
            }



            Selected = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selected) _manager.DeselectPic();
            else  _manager.PickOnePicture(_index);
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            _picRect.DOScale(1.1f * Vector3.one, 0.1f);
            _manager.PlaySound(_manager.setting.hoverPicture);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _picRect.DOScale(Vector3.one, 0.3f);
        }

    }

}
