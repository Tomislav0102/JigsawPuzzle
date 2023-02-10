using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.UI;

namespace PuzzleShape
{
    public class JigsawManager : MonoBehaviour
    {
        [SerializeField] GameManager gm;
        [SerializeField] Sprite replacementPic;
        public Sprite MainPicture
        {
            get => _mainPic;
            set
            {
                _mainPic = value;
                if (_mainPic == null)
                {
                    _mainPic = replacementPic;
                }

                float ratio = _mainPic.rect.width / _mainPic.rect.height;
                if (ratio < 0.67f) dimGrid = new Vector2Int(13, 18);
                else if (ratio > 1.33f) dimGrid = new Vector2Int(20, 12);
                else dimGrid = new Vector2Int(14, 14);

               // dimGrid = new Vector2Int(2, 2);
            }
        }
        Sprite _mainPic;
        Image _finalImageRend;
        [SerializeField] LayerMask lmPuzzlePieces, lmSlots;
        Transform _parPuzzleGrid, _parSlotGrid;
        [SerializeField] PuzzleTile prefabPuzzle;
        [SerializeField] SlotTile prefabSlot;
        [HideInInspector] public Vector2Int dimGrid = new Vector2Int(8, 8);
        [HideInInspector] public float spacing = 0.5f;
        [SerializeField] Sprite[] spitesPuzzle;
        #region //up, right, down, left
        readonly string p00 = "0220";
        readonly string p01 = "0210";
        readonly string p02 = "0120";
        readonly string p03 = "0110";
        readonly string p04 = "2200";
        readonly string p05 = "2220";
        readonly string p06 = "2210";
        readonly string p07 = "2100";
        readonly string p08 = "2120";
        readonly string p09 = "2110";
        readonly string p10 = "1200";
        readonly string p11 = "1220";
        readonly string p12 = "1210";
        readonly string p13 = "1100";
        readonly string p14 = "1120";
        readonly string p15 = "1110";
        readonly string p16 = "0022";
        readonly string p17 = "0012";
        readonly string p18 = "0222";
        readonly string p19 = "0212";
        readonly string p20 = "0122";
        readonly string p21 = "0112";
        readonly string p22 = "2002";
        readonly string p23 = "2022";
        readonly string p24 = "2012";
        readonly string p25 = "2202";
        readonly string p26 = "2222";
        readonly string p27 = "2212";
        readonly string p28 = "2102";
        readonly string p29 = "2122";
        readonly string p30 = "2112";
        readonly string p31 = "1002";
        readonly string p32 = "1022";
        readonly string p33 = "1012";
        readonly string p34 = "1202";
        readonly string p35 = "1222";
        readonly string p36 = "1212";
        readonly string p37 = "1102";
        readonly string p38 = "1122";
        readonly string p39 = "1112";
        readonly string p40 = "0021";
        readonly string p41 = "0011";
        readonly string p42 = "0221";
        readonly string p43 = "0211";
        readonly string p44 = "0121";
        readonly string p45 = "0111";
        readonly string p46 = "2001";
        readonly string p47 = "2021";
        readonly string p48 = "2011";
        readonly string p49 = "2201";
        readonly string p50 = "2221";
        readonly string p51 = "2211";
        readonly string p52 = "2101";
        readonly string p53 = "2121";
        readonly string p54 = "2111";
        readonly string p55 = "1001";
        readonly string p56 = "1021";
        readonly string p57 = "1011";
        readonly string p58 = "1201";
        readonly string p59 = "1221";
        readonly string p60 = "1211";
        readonly string p61 = "1101";
        readonly string p62 = "1121";
        readonly string p63 = "1111";
        List<string> _allEdges = new List<string>();
        #endregion
        PuzzleTile[,] _puzzleTilesTable;
        SlotTile[,] _slotTilesTable;
        string _edges = "xxxx";

        PuzzleTile _selectedPuzzle;
        Vector2 _startPos, _dragOffset;
        bool _activeMove, _refreshSlotHighlights;
        Camera cam;
        Vector2 GetMousePos()
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            return mousePos;
        }
        readonly Vector2 _limits = new Vector2(8f, 4f);
        Vector2 RdnSpawPos(Vector2 rangeXleft, Vector2 rangeXright, Vector2 rangeYdown, Vector2 rangeYup)
        {

            float posX = Random.Range(-_limits.x, _limits.x);
            float posY = Random.Range(-_limits.y, _limits.y);

            if (rangeXleft == Vector2.zero || rangeXright == Vector2.zero) FindPos(false);
            else if (rangeYdown == Vector2.zero || rangeYup == Vector2.zero) FindPos(true);
            else FindPos(Random.value < 0.5f);

            void FindPos(bool isX)
            {
                if (isX)
                {
                    if (Random.value < 0.5f) posX = Random.Range(rangeXleft.x, rangeXleft.y);
                    else posX = Random.Range(rangeXright.x, rangeXright.y);
                }
                else
                {
                    if (Random.value < 0.5f) posY = Random.Range(rangeYdown.x, rangeYdown.y);
                    else posY = Random.Range(rangeYup.x, rangeYup.y);
                }
            }

            return new Vector2(posX, posY);
        }
        [SerializeField] GameObject boardBorder;
        BoxCollider2D _bbCollider;
        Vector2 bbSize;


        private void Awake()
        {
            IniEdges();
            cam = gm.cam;
            _finalImageRend = gm.finalImage;
            _parPuzzleGrid = gm.parPuzzles.transform;
            _parSlotGrid = gm.parSlots.transform;
        }

        private void Start()
        {
            MainPicture = gm.setting.chosenSprite;
            _finalImageRend.sprite = MainPicture;
            _finalImageRend.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(dimGrid.x * spacing, dimGrid.y * spacing);
            //   _finalImageRend.size = new Vector2(dimGrid.x * spacing, dimGrid.y * spacing);
          //  NewBoard();
        }
        private void OnEnable()
        {
            HelperScript.GameStart += CallEv_GameStart;
            HelperScript.GameOver += CallEv_GameOver;
        }
        private void OnDisable()
        {
            HelperScript.GameStart -= CallEv_GameStart;
            HelperScript.GameOver -= CallEv_GameOver;
        }
        void CallEv_GameStart()
        {
            _activeMove = true;
        }
        void CallEv_GameOver()
        {
            _activeMove = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) gm.Btn_Quit();

            MovingPuzzles();
        }

        private void IniEdges()
        {
            _allEdges.Add(p00);
            _allEdges.Add(p01);
            _allEdges.Add(p02);
            _allEdges.Add(p03);
            _allEdges.Add(p04);
            _allEdges.Add(p05);
            _allEdges.Add(p06);
            _allEdges.Add(p07);
            _allEdges.Add(p08);
            _allEdges.Add(p09);
            _allEdges.Add(p10);
            _allEdges.Add(p11);
            _allEdges.Add(p12);
            _allEdges.Add(p13);
            _allEdges.Add(p14);
            _allEdges.Add(p15);
            _allEdges.Add(p16);
            _allEdges.Add(p17);
            _allEdges.Add(p18);
            _allEdges.Add(p19);
            _allEdges.Add(p20);
            _allEdges.Add(p21);
            _allEdges.Add(p22);
            _allEdges.Add(p23);
            _allEdges.Add(p24);
            _allEdges.Add(p25);
            _allEdges.Add(p26);
            _allEdges.Add(p27);
            _allEdges.Add(p28);
            _allEdges.Add(p29);
            _allEdges.Add(p30);
            _allEdges.Add(p31);
            _allEdges.Add(p32);
            _allEdges.Add(p33);
            _allEdges.Add(p34);
            _allEdges.Add(p35);
            _allEdges.Add(p36);
            _allEdges.Add(p37);
            _allEdges.Add(p38);
            _allEdges.Add(p39);
            _allEdges.Add(p40);
            _allEdges.Add(p41);
            _allEdges.Add(p42);
            _allEdges.Add(p43);
            _allEdges.Add(p44);
            _allEdges.Add(p45);
            _allEdges.Add(p46);
            _allEdges.Add(p47);
            _allEdges.Add(p48);
            _allEdges.Add(p49);
            _allEdges.Add(p50);
            _allEdges.Add(p51);
            _allEdges.Add(p52);
            _allEdges.Add(p53);
            _allEdges.Add(p54);
            _allEdges.Add(p55);
            _allEdges.Add(p56);
            _allEdges.Add(p57);
            _allEdges.Add(p58);
            _allEdges.Add(p59);
            _allEdges.Add(p60);
            _allEdges.Add(p61);
            _allEdges.Add(p62);
            _allEdges.Add(p63);
        }
        public void NewBoard()
        {
            for (int i = 0; i < _parPuzzleGrid.childCount; i++)
            {
                Destroy(_parPuzzleGrid.GetChild(i).gameObject);
            }
            for (int i = 0; i < _parSlotGrid.childCount; i++)
            {
                Destroy(_parSlotGrid.GetChild(i).gameObject);
            }
            _parPuzzleGrid.position = _parSlotGrid.position = Vector3.zero;

            _puzzleTilesTable = new PuzzleTile[dimGrid.x, dimGrid.y];
            _slotTilesTable = new SlotTile[dimGrid.x, dimGrid.y];
            for (int j = 0; j < dimGrid.y; j++)
            {
                for (int i = 0; i < dimGrid.x; i++)
                {
                    PuzzleTile tl = Instantiate(prefabPuzzle, _parPuzzleGrid);
                    _puzzleTilesTable[i, j] = tl;
                    tl.poz = new Vector2Int(i, j);
                    tl.transform.position = new Vector3(i * spacing, j * spacing, 0f);
                    tl.name = $"Puzzle tile {tl.poz.x} - {tl.poz.y}";

                    SlotTile sl = Instantiate(prefabSlot, _parSlotGrid);
                    _slotTilesTable[i, j] = sl;
                    sl.poz = new Vector2Int(i, j);
                    sl.transform.position = new Vector3(i * spacing, j * spacing, 0f);
                    sl.name = $"Slot tile {sl.poz.x} - {sl.poz.y}";
                    sl.SlotHl = SlotHighlight.Neutral;

                    if (i == 0)
                    {
                        _edges = _edges[0].ToString() + Random.Range(1, 3).ToString() + _edges[2].ToString() + "0";
                    }
                    else if (i == dimGrid.x - 1)
                    {
                        _edges = _edges[0].ToString() + "0" + _edges[2].ToString() + ((_puzzleTilesTable[i - 1, j].Edges[1]) % 2 + 1).ToString();
                    }
                    else
                    {
                        _edges = _edges[0].ToString() + Random.Range(1, 3).ToString() + _edges[2].ToString() + ((_puzzleTilesTable[i - 1, j].Edges[1]) % 2 + 1).ToString();
                    }

                    if (j == 0)
                    {
                        _edges = Random.Range(1, 3).ToString() + _edges[1].ToString() + "0" + _edges[3].ToString();
                    }
                    else if (j == dimGrid.y - 1)
                    {
                        _edges = "0" + _edges[1].ToString() + ((_puzzleTilesTable[i, j - 1].Edges[0]) % 2 + 1).ToString() + _edges[3].ToString();
                    }
                    else
                    {
                        _edges = Random.Range(1, 3).ToString() + _edges[1].ToString() + ((_puzzleTilesTable[i, j - 1].Edges[0]) % 2 + 1).ToString() + _edges[3].ToString();
                    }

                    tl.transform.localScale = sl.transform.localScale = spacing * Vector3.one;
                    for (int k = 0; k < _allEdges.Count; k++)
                    {
                        if (_allEdges[k] == _edges)
                        {
                            tl.Ini(gm, spitesPuzzle[k], _edges);
                            break;
                        }
                    }
                }
            }
            _parPuzzleGrid.position = _parSlotGrid.position = new Vector3(spacing * 0.5f * (-dimGrid.x + 1), spacing * 0.5f * (-dimGrid.y + 1), 0f);
            InvokeRepeating(nameof(RemoveAllHighlightsInSlots), 1f, 0.15f);
            StartCoroutine(SpreadPuzzles());

        }
        IEnumerator SpreadPuzzles()
        {
            Vector2 rangeXleft = new Vector2(-_limits.x, -dimGrid.x * spacing * 0.5f - spacing);
            Vector2 rangeXright = new Vector2(dimGrid.x * spacing * 0.5f + spacing, _limits.x);
            Vector2 rangeYdown = new Vector2(-_limits.y, -dimGrid.y * spacing * 0.5f - spacing);
            Vector2 rangeYup = new Vector2(dimGrid.y * spacing * 0.5f + spacing, _limits.y);
            if (rangeXleft.x >= rangeXleft.y) rangeXleft = Vector2.zero;
            if (rangeXright.x >= rangeXright.y) rangeXright = Vector2.zero;
            if (rangeYdown.x >= rangeYdown.y) rangeYdown = Vector2.zero;
            if (rangeYup.x >= rangeYup.y) rangeYup = Vector2.zero;

            bbSize = new Vector2(dimGrid.x * spacing + spacing, dimGrid.y * spacing + spacing);
            boardBorder.GetComponent<SpriteRenderer>().size = bbSize;
            _bbCollider = boardBorder.GetComponent<BoxCollider2D>();
            _bbCollider.size = bbSize;
            _bbCollider.enabled = true;

            //#region//PRIVREMENO
            //for (int j = 0; j < dimGrid.y; j++)
            //{
            //    for (int i = 0; i < dimGrid.x; i++)
            //    {
            //        _puzzleTilesTable[i, j].PuzzleStartPos(RdnSpawPos(rangeXleft, rangeXright, rangeYdown, rangeYup));
            //    }
            //}
            //HelperScript.GameStart?.Invoke();
            //yield return null;
            //#endregion

            for (int j = 0; j < dimGrid.y; j++)
            {
                for (int i = 0; i < dimGrid.x; i++)
                {
                    _puzzleTilesTable[i, j].PuzzleStartPos(RdnSpawPos(rangeXleft, rangeXright, rangeYdown, rangeYup));
                    yield return new WaitForFixedUpdate();
                }
            }

            HelperScript.GameStart?.Invoke();
        }

        bool CanFit(PuzzleTile puzzleTile, SlotTile slot)
        {
            if (slot.edges != "xxxx") return false;

            string puzzleString = puzzleTile.Edges;
            Vector2Int slotPosition = slot.poz;
            for (int j = -1; j < 2; j++)
            {
                for (int i = -1; i < 2; i++)
                {
                    Vector2Int borderSlotPos = new Vector2Int(i + slotPosition.x, j + slotPosition.y);
                    if (!(borderSlotPos.x >= 0 && borderSlotPos.x < dimGrid.x && borderSlotPos.y >= 0 && borderSlotPos.y < dimGrid.y) || (Mathf.Abs(i) == Mathf.Abs(j))) continue;

                    string borderSlotString = _slotTilesTable[borderSlotPos.x, borderSlotPos.y].edges;
                    if (borderSlotString == "xxxx") continue;

                    if (i == -1)
                    {
                        if (int.Parse(puzzleString[3].ToString()) == 0 || int.Parse(borderSlotString[1].ToString()) == 0) return false;
                        if (int.Parse(borderSlotString[1].ToString()) != (int.Parse(puzzleString[3].ToString()) % 2 + 1)) return false;
                    }
                    else if (i == 1)
                    {
                        if (int.Parse(puzzleString[1].ToString()) == 0 || int.Parse(borderSlotString[3].ToString()) == 0) return false;
                        if (int.Parse(borderSlotString[3].ToString()) != (int.Parse(puzzleString[1].ToString()) % 2 + 1)) return false;
                    }
                    if (j == -1)
                    {
                        if (int.Parse(puzzleString[2].ToString()) == 0 || int.Parse(borderSlotString[0].ToString()) == 0) return false;
                        if (int.Parse(borderSlotString[0].ToString()) != (int.Parse(puzzleString[2].ToString()) % 2 + 1)) return false;
                    }
                    else if (j == 1)
                    {
                        if (int.Parse(puzzleString[0].ToString()) == 0 || int.Parse(borderSlotString[2].ToString()) == 0) return false;
                        if (int.Parse(borderSlotString[2].ToString()) != (int.Parse(puzzleString[0].ToString()) % 2 + 1)) return false;
                    }

                }
            }

            return true;
        }
        bool PickUpPositioOnGrid(Vector2 puPos)
        {
            if (puPos.x < bbSize.x * 0.5f && puPos.x > -bbSize.x * 0.5f && puPos.y < bbSize.y * 0.5f && puPos.y > -bbSize.y * 0.5f)
            {
                return true;
            }

            return false;
        }
        private void MovingPuzzles()
        {
            if (!_activeMove) return;

            _refreshSlotHighlights = false;
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D coll = Physics2D.OverlapPoint(GetMousePos(), lmPuzzlePieces);
                if (coll != null && coll.TryGetComponent(out PuzzleTile puzzle))
                {
                    _selectedPuzzle = puzzle;
                    if (_selectedPuzzle.slot != null) _selectedPuzzle.slot.edges = "xxxx";
                    _selectedPuzzle.PuzzleState = PuzzleState.Dragging;
                    Physics2D.IgnoreCollision(_selectedPuzzle.boxCollider, _bbCollider, true);

                    _startPos = (Vector2)_selectedPuzzle.myTransform.position;
                    if (!PickUpPositioOnGrid(_startPos)) _selectedPuzzle.returnPos = _startPos;
                    _dragOffset = _startPos - GetMousePos();
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedPuzzle == null) return;

                Collider2D collSlot = Physics2D.OverlapPoint(_selectedPuzzle.myTransform.position, lmSlots);
                if (collSlot != null && collSlot.TryGetComponent(out SlotTile st))
                {
                    if (CanFit(_selectedPuzzle, st))
                    {
                        _selectedPuzzle.slot = st;
                        _selectedPuzzle.PuzzleState = PuzzleState.SetInSlot;
                        _selectedPuzzle.myTransform.position = st.transform.position;
                        CheckCompleted();
                    }
                    else
                    {
                        _selectedPuzzle.myTransform.position = _selectedPuzzle.returnPos;
                        _selectedPuzzle.PuzzleState = PuzzleState.Free;
                    }
                }
                else _selectedPuzzle.PuzzleState = PuzzleState.Free;

                Physics2D.IgnoreCollision(_selectedPuzzle.boxCollider, _bbCollider, false);
                _selectedPuzzle = null;
                for (int j = 0; j < dimGrid.y; j++)
                {
                    for (int i = 0; i < dimGrid.x; i++)
                    {
                        _slotTilesTable[i, j].SlotHl = SlotHighlight.Neutral;
                    }
                }
            }

            if (_selectedPuzzle == null) return;

            Vector2 targetPos = GetMousePos() + _dragOffset;
            targetPos = new Vector2(Mathf.Clamp(targetPos.x, -_limits.x, _limits.x), Mathf.Clamp(targetPos.y, -_limits.y, _limits.y));
            _selectedPuzzle.myTransform.position = targetPos;

            _refreshSlotHighlights = true;
            Collider2D collSlotHover = Physics2D.OverlapPoint(_selectedPuzzle.myTransform.position, lmSlots);
            if (collSlotHover != null && collSlotHover.TryGetComponent(out SlotTile stHover))
            {
                stHover.SlotHl = SlotHighlight.Positive;
            }

            //rotation of puzzles
            if (Input.GetKeyDown(KeyCode.Space)) _selectedPuzzle.RotatePuzzle(true);

        }

        private void RemoveAllHighlightsInSlots()
        {
            if (!_refreshSlotHighlights) return;

            for (int j = 0; j < dimGrid.y; j++)
            {
                for (int i = 0; i < dimGrid.x; i++)
                {
                    if (_slotTilesTable[i, j].SlotHl != SlotHighlight.Set) _slotTilesTable[i, j].SlotHl = SlotHighlight.Neutral;
                }
            }
        }

        void CheckCompleted()
        {
            for (int j = 0; j < dimGrid.y; j++)
            {
                for (int i = 0; i < dimGrid.x; i++)
                {
                    if (_puzzleTilesTable[i, j].PuzzleState != PuzzleState.SetInSlot) return;
                }
            }
            for (int j = 0; j < dimGrid.y; j++)
            {
                for (int i = 0; i < dimGrid.x; i++)
                {
                    if (_puzzleTilesTable[i, j].slot.poz != _puzzleTilesTable[i, j].poz) return;
                }
            }

            HelperScript.GameOver?.Invoke();
        }

    }




}
