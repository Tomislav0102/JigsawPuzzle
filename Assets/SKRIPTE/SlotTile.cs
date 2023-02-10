using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

namespace PuzzleShape
{
    public class SlotTile : MonoBehaviour
    {
        public Vector2Int poz;
        public string edges = "xxxx";
        [SerializeField] SpriteRenderer spriteRend;
        [SerializeField] Color[] colorsHl;
        SlotHighlight _slotHl;
        public SlotHighlight SlotHl
        {
            get => _slotHl;
            set
            {
                _slotHl = value;

                spriteRend.color = colorsHl[(int)_slotHl];
                switch (_slotHl)
                {
                    case SlotHighlight.Neutral:
                        break;
                    case SlotHighlight.Positive:
                        break;
                    case SlotHighlight.Negative:
                        break;
                    case SlotHighlight.Set:
                        break;
                }
            }
        }



    }

}
