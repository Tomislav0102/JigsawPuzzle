using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FirstCollection;
using UnityEngine.Rendering;

namespace PuzzleShape
{
    public class PuzzleTile : MonoBehaviour
    {
        GameManager _gm;
        JigsawManager _jigsawMan;
        public BoxCollider2D boxCollider;
        public Transform myTransform;
        [HideInInspector] public Vector2 returnPos;
        public Vector2Int poz;
        [HideInInspector] public SlotTile slot;
        [SerializeField] SpriteMask spriteMask;
        [SerializeField] SpriteRenderer maskedImage;
        [SerializeField] SortingGroup sortingGroup;
 
        int _rotAmount = 0;
        public string Edges = "xxxx";

        const float CONST_OFFSET = 0.16f;
        const float CONST_SIZE = 0.33f;
        public PuzzleState PuzzleState
        {
            get => _puzzleState;
            set
            {
                _puzzleState = value;
                sortingGroup.sortingOrder = 5;
                switch (value)
                {
                    case PuzzleState.Free:
                        _gm.PlaySound(_gm.setting.puzzleReturnToOutside);
                        if (slot != null) slot.edges = "xxxx";
                        slot = null;
                        break;
                    case PuzzleState.Dragging:
                        _gm.PlaySound(_gm.setting.puzzlePickup);
                        sortingGroup.sortingOrder = 10;
                        if (slot != null) slot.edges = "xxxx";
                        slot = null;
                        break;
                    case PuzzleState.SetInSlot:
                        _gm.PlaySound(_gm.setting.puzzleSet);
                        if (slot != null)
                        {
                            slot.SlotHl = SlotHighlight.Set;
                            slot.edges = Edges;
                        }
                        else print("Error! Puzzle piece doesn't have a slot assigned.");
                        break;
                }
            }
        }
        PuzzleState _puzzleState;

        public void Ini(GameManager gm, Sprite sprite, string edges)
        {
            _gm = gm;
            _jigsawMan = _gm.jigsawManager;
            spriteMask.sprite = sprite;
            Edges = edges;
            DefineBoxCollider();

            DefineMaskedPicture();
        }

        private void DefineMaskedPicture()
        {
            float spacing = _jigsawMan.spacing;
            float width = _jigsawMan.dimGrid.x * spacing;
            float height = _jigsawMan.dimGrid.y * spacing;


            maskedImage.sprite = _jigsawMan.MainPicture;
            maskedImage.size = new Vector2(width, height);
            maskedImage.transform.localScale = new Vector3(1 / spacing, 1 / spacing, 1f);

            float posX = (width * 0.5f - spacing * poz.x - spacing * 0.5f) / spacing;
            float posY = (height * 0.5f - spacing * poz.y - spacing * 0.5f) / spacing;
            maskedImage.transform.localPosition = new Vector3(posX, posY, 0f);
        }

        void DefineBoxCollider() //nepotrebno, osim za precizniji BeginDrag
        {
            Vector2 offset = Vector2.zero;
            Vector2 size = Vector2.one;
            if (Edges[0].ToString() == "1")
            {
                offset = new Vector2(offset.x, offset.y + CONST_OFFSET);
                size = new Vector2(size.x, size.y + CONST_SIZE);
            }
            if (Edges[1].ToString() == "1")
            {
                offset = new Vector2(offset.x + CONST_OFFSET, offset.y);
                size = new Vector2(size.x + CONST_SIZE, size.y);
            }
            if (Edges[2].ToString() == "1")
            {
                offset = new Vector2(offset.x, offset.y - CONST_OFFSET);
                size = new Vector2(size.x, size.y + CONST_SIZE);
            }
            if (Edges[3].ToString() == "1")
            {
                offset = new Vector2(offset.x - CONST_OFFSET, offset.y);
                size = new Vector2(size.x + CONST_SIZE, size.y);
            }
            boxCollider.offset = offset;
            boxCollider.size = size;
            boxCollider.edgeRadius = 0.01f;
        }

        public void PuzzleStartPos(Vector2 targetPos)
        {
            returnPos = targetPos;
           // PuzzleState = PuzzleState.Free;
            myTransform.DOMove(targetPos, 0.1f)
                .OnComplete(EndStartPos);

        }
        void EndStartPos()
        {
            int rotations = Random.Range(0, 3);
            for (int i = 0; i < rotations; i++)
            {
                RotatePuzzle(false);
            }

        }
        public void RotatePuzzle(bool useTween)
        {
            _rotAmount = (1 + _rotAmount) % 4;

            string endString = "";
            endString += Edges[3];
            endString += Edges[0];
            endString += Edges[1];
            endString += Edges[2];
            Edges = endString;

            Vector3 rotTarget = -_rotAmount * 90f * Vector3.forward;
            if (useTween)
            {
                myTransform.DORotate(rotTarget, 1f)
                    .SetEase(Ease.OutExpo);
            }
            else myTransform.localEulerAngles = rotTarget;

        }
    }

}
