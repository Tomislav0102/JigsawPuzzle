using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirstCollection;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Coffee.UIEffects;

namespace PuzzleShape
{
    public class GameManager : MonoBehaviour
    {
        //public Ease izy;
        //public float vrijeme;
        public SO_settings setting;
        public JigsawManager jigsawManager;
        public Image finalImage;
        UITransitionEffect _finalTransition;
        public GameObject parPuzzles, parSlots;
        public Camera cam;

        [SerializeField] SpriteRenderer[] bkgRends;
        [SerializeField] Transform parButtons;
        Button[] _buttonsInGame;
        bool _showImage;
        Image _offShowImage, _offSound;
        [SerializeField] GameObject toolTipGO;
        TextMeshProUGUI _toolTipText;
        int _counterRend;
        Sprite[] _allSprites;
        int _counter;

        [SerializeField] UITransitionEffect endTransition;

        [SerializeField] GameObject endPanel;
        Image _fadeImageEnd;
        [SerializeField] GameObject introPanel;
        [SerializeField] Button buttonPlay;
        AudioSource _source;
        private void Awake()
        {
            _buttonsInGame = new Button[parButtons.childCount];
            for (int i = 0; i < _buttonsInGame.Length; i++)
            {
                _buttonsInGame[i] = parButtons.GetChild(i).GetComponent<Button>();
                _buttonsInGame[i].GetComponent<HoverButton>().Ini(this);
            }

            parButtons.gameObject.SetActive(false);
            _allSprites = Resources.LoadAll<Sprite>("backgrounds");
            endPanel.SetActive(false);
            _fadeImageEnd = endPanel.transform.GetChild(0).GetComponent<Image>();
            _finalTransition = finalImage.GetComponent<UITransitionEffect>();
            _source = GetComponent<AudioSource>();
        }
        private void Start()
        {
            toolTipGO.SetActive(false);
            _toolTipText = toolTipGO.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            _offShowImage = _buttonsInGame[1].transform.GetChild(1).GetComponent<Image>();
            _offSound = _buttonsInGame[2].transform.GetChild(1).GetComponent<Image>();
            _counter = Random.Range(0, _allSprites.Length);
            bkgRends[0].sprite = _allSprites[_counter];
            _offSound.enabled = !setting.soundOn;
        }
        #region//EVENTS
        private void OnEnable()
        {
            HelperScript.GameStart += CallEv_GameStart;
            HelperScript.GameOver += CallEv_GameOver;
            buttonPlay.onClick.AddListener(Btn_Play);
            _buttonsInGame[0].onClick.AddListener(Btn_NextBackground);
            _buttonsInGame[1].onClick.AddListener(Btn_ShowImage);
            _buttonsInGame[2].onClick.AddListener(Btn_Sound);
            _buttonsInGame[3].onClick.AddListener(Btn_Quit);
        }
        private void OnDisable()
        {
            HelperScript.GameStart -= CallEv_GameStart;
            HelperScript.GameOver -= CallEv_GameOver;
            buttonPlay.onClick.RemoveAllListeners();
            for (int i = 0; i < 4; i++)
            {
                _buttonsInGame[i].onClick.RemoveAllListeners();
            }
        }
        void CallEv_GameStart()
        {
            parButtons.gameObject.SetActive(true);
            _source.loop = false;
        }
        void CallEv_GameOver()
        {
            parButtons.gameObject.SetActive(false);
            toolTipGO.SetActive(false);
            endPanel.SetActive(true);
            _fadeImageEnd.DOFade(0.9f, 1f)
                .From(0f)
                .OnComplete(() => PlaySound(setting.win))
                .SetEase(Ease.OutSine);
        }
        #endregion

        void Btn_Play()
        {
            jigsawManager.NewBoard();
            endTransition.Hide();
            introPanel.SetActive(false);
            if (setting.soundOn) _source.Play();

        }
        public void Btn_NextBackground()
        {
            PlaySound(setting.buttonClick);
            int prevCounterRend = _counterRend;
            _counterRend = (1 + _counterRend) % 2;
            bkgRends[prevCounterRend].DOFade(0f, 0.5f);

            _counter = (1 + _counter) % _allSprites.Length;
            bkgRends[_counterRend].sprite = _allSprites[_counter];
            bkgRends[_counterRend].DOFade(1f, 0.5f);
        }

        public void Btn_ShowImage()
        {
            PlaySound(setting.buttonClick);
            _showImage = !_showImage;
            if (_showImage) _finalTransition.Show();
            else _finalTransition.Hide();
            _toolTipText.text = _showImage ? "Image is shown." : "Image is hidden.";
           // finalImage.enabled = _showImage;
            parPuzzles.SetActive(!_showImage);
            _offShowImage.enabled = _showImage;
        }
        public void Btn_Sound()
        {
            setting.soundOn = !setting.soundOn;
            PlaySound(setting.buttonClick);
            _toolTipText.text = setting.soundOn ? "Sound is on." : "Sound is off.";
            _offSound.enabled = !setting.soundOn;
        }

        public void Btn_Quit()
        {
            PlaySound(setting.buttonClick);
            endTransition.Show();
            Invoke(nameof(QuitContinuation), 1f);
        }
        void QuitContinuation()
        {
            SceneManager.LoadScene(0);
        }

        public void HoverToolTip(bool hoverOn, ButtonType buttonType)
        {
            toolTipGO.SetActive(hoverOn);
            toolTipGO.transform.position = _buttonsInGame[(int)buttonType].transform.position;
            switch (buttonType)
            {
                case ButtonType.Backgound:
                    _toolTipText.text = "Change background.";
                    break;
                case ButtonType.ShowFinalImage:
                    _toolTipText.text = _showImage ? "Image is shown." : "Image is hidden.";
                    break;
                case ButtonType.Sound:
                    _toolTipText.text = setting.soundOn ? "Sound is on." : "Sound is off.";
                    break;
                case ButtonType.Quit:
                    _toolTipText.text = "Quit to main menu.";
                    break;
            }
        }

        public void PlaySound(AudioPack klipPack)
        {
            if (!setting.soundOn) return;

            _source.volume = klipPack.volume;
            _source.pitch = klipPack.pitch;
            _source.PlayOneShot(klipPack.klip);
        }

    }

}

//public class LayoutPuzzle
//{
//    Vector4 _area;
//    const float CONST_AREABUFFER = 0.6f;
//    Vector2Int dim;
//    Vector2 _limits = new Vector2(8.5f, 4.5f);
//    Vector2 _effSpacing;
//    int _totalNum;
//    List<Vector2> _positions = new List<Vector2>();
//    public List<Vector2> finalPositions = new List<Vector2>();
//    bool IsInside(Vector2 poz)
//    {
//        if (poz.x > _area.x && poz.x < _area.y && poz.y > _area.z && poz.y < _area.w)
//        {
//            return true;
//        }
//        return false;
//    }

//    public LayoutPuzzle(Vector2Int dimension, float spacing)
//    {
//        dim = 2 * dimension;

//        _totalNum = dim.x * dim.y;
//        _effSpacing = new Vector2(_limits.x * 2f / dim.x, _limits.y * 2f / dim.y);
//        Vector2 centeredPos = new Vector2(-_limits.x + _effSpacing.x * 0.5f, -_limits.y + _effSpacing.y * 0.5f);
//        _area = new Vector4(-dimension.x * spacing * 0.5f - CONST_AREABUFFER, dimension.x * spacing * 0.5f + CONST_AREABUFFER, -dimension.y * spacing * 0.5f - CONST_AREABUFFER, dimension.y * spacing * 0.5f + CONST_AREABUFFER);

//        for (int j = 0; j < dim.y; j++)
//        {
//            for (int i = 0; i < dim.x; i++)
//            {
//                _positions.Add(new Vector2(i * _effSpacing.x, j * _effSpacing.y));
//            }
//        }

//        for (int i = 0; i < _totalNum; i++)
//        {
//            _positions[i] += centeredPos;
//            if (!IsInside(_positions[i])) finalPositions.Add(_positions[i]);
//        }

//    }


//}

//IEnumerator IniLayoutPuzzle(float durationTween)
//{
//    Vector2[] _pozicije = new Vector2[parPozicije.childCount];
//    for (int i = 0; i < parPozicije.childCount; i++)
//    {
//        _pozicije[i] = parPozicije.GetChild(i).position;
//    }

//    float _totalDistance = 0f;
//    float[] _distances = new float[_pozicije.Length - 1];
//    for (int i = 0; i < _distances.Length; i++)
//    {
//        _distances[i] = Vector2.Distance(_pozicije[i], _pozicije[i + 1]);
//        _totalDistance += _distances[i];
//    }

//    float _segmentLength = _totalDistance / _numPuzzlesTotal;

//    List<Transform> spawns = new List<Transform>();
//    Vector2 dir = (_pozicije[1] - _pozicije[0]).normalized;
//    for (int i = 0; i < _numPuzzlesTotal; i++)
//    {
//        spawns.Add(Instantiate(prefab, _pozicije[0] + dir * i * _segmentLength, Quaternion.identity).transform);
//        spawns[i].name = i.ToString();
//    }

//    List<Transform> lista = new List<Transform>();
//    for (int i = 1; i < _pozicije.Length - 1; i++)
//    {
//        float angle = Vector2.SignedAngle(_pozicije[i] - _pozicije[i - 1], _pozicije[i + 1] - _pozicije[i]);
//        foreach (Transform item in spawns)
//        {
//            if (Vector2.Distance(item.position, _pozicije[i - 1]) > _distances[i - 1]) item.RotateAround(_pozicije[i], Vector3.forward, angle);
//            else lista.Add(item);
//        }
//        foreach (Transform item in lista)
//        {
//            spawns.Remove(item);
//        }
//    }
//    List<Vector2> spawnPositions = new List<Vector2>();
//    for (int i = 0; i < spawns.Count; i++)
//    {
//        spawnPositions.Add(spawns[i].position);
//    }
//    for (int i = 0; i < lista.Count; i++)
//    {
//        spawnPositions.Add(lista[i].position);
//    }

//    yield return new WaitForSeconds(2f);
//    List<Vector2> _finalPositions = HelperScript.RandomListByType<Vector2>(spawnPositions);
//    for (int i = 0; i < _allPuzzles.Length; i++)
//    {
//        _allPuzzles[i].PuzzleStartPos(_finalPositions[i], durationTween);
//    }

//    yield return new WaitForSeconds(durationTween);
//    HelperScript.GameStart?.Invoke();
//}
