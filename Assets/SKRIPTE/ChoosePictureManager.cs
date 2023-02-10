using FirstCollection;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using Coffee.UIEffects;

namespace PuzzleShape
{
    public class ChoosePictureManager : MonoBehaviour
    {
        public SO_settings setting;
        [SerializeField] UITransitionEffect transition;
        [SerializeField] TextMeshProUGUI displayChosenPicName, displayLoading;
        float _loadingPercent;
        [SerializeField] Button btnStartGame, btnRefresh, btnQuit;
        [SerializeField] Transform parImages;
        [SerializeField] Scrollbar scrollbar;
        [SerializeField] ChoosePicture picturePrefab;
        List<ChoosePicture> _images = new List<ChoosePicture>();
        ChoosePicture _selectedImage;
        List<string> _fileNames = new List<string>();
        float _cellSize;
        string GetFileName(string st)
        {
            string s1 = "";
            for (int i = 0; i < st.Length; i++)
            {
                s1 += st[st.Length - 1 - i];
            }
            string s2 = "";
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].ToString() == @"\")
                {
                    break;
                }
                s2 += s1[i];
            }
            string s3 = "";
            for (int i = 0; i < s2.Length; i++)
            {
                s3 += s2[s2.Length - 1 - i];
            }
            string s4 = "";
            for (int i = 0; i < s3.Length; i++)
            {
                if (s3[i].ToString() == @".")
                {
                    break;
                }
                s4 += s3[i];
            }

            return s4;
        }
        bool _btnsActive;
        AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }
        IEnumerator Start()
        {
            setting.chosenSprite = null;

            btnStartGame.interactable = false;

            List<string> _filePaths = new List<string>();
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.jpg"));
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.png"));
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.tif"));
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.bmp"));
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.psd"));
            _filePaths.AddRange(System.IO.Directory.GetFiles("file:///..\\Images\\", "*.tga"));
            _filePaths.Sort();

            Texture2D[] _textureList = new Texture2D[_filePaths.Count];
            int counter = 0;

            _cellSize = parImages.GetComponent<GridLayoutGroup>().cellSize.x;
            foreach (string item in _filePaths)
            {
                var www = UnityWebRequestTexture.GetTexture(item);
                yield return www.SendWebRequest();
                Texture2D texTemp = DownloadHandlerTexture.GetContent(www);

                _textureList[counter] = texTemp;
                Sprite sprajt = Sprite.Create(texTemp, new Rect(0, 0, texTemp.width, texTemp.height), new Vector2(0.5f, 0.5f));
                _images.Add(Instantiate(picturePrefab, parImages));
                _images[counter].Ini(this, counter, sprajt, _cellSize);

                _fileNames.Add(GetFileName(item));
                _loadingPercent = 100f * counter / _fileNames.Count;
                displayLoading.text = $"Loading pictures, {_loadingPercent.ToString("F0")}% complete.";
                counter++;

            }
            displayLoading.enabled = false;
            scrollbar.value = 1f;
            transition.Hide();
            yield return new WaitForSeconds(1f);
            _btnsActive = true;
        }
        private void OnEnable()
        {
            btnStartGame.onClick.AddListener(Btn_StartGame);
            btnRefresh.onClick.AddListener(Btn_Refresh);
            btnQuit.onClick.AddListener(Btn_Quit);
        }
        private void OnDisable()
        {
            btnStartGame.onClick?.RemoveAllListeners();
            btnRefresh.onClick?.RemoveAllListeners();
            btnQuit.onClick?.RemoveAllListeners();
        }
        void Btn_StartGame()
        {
            if (!_btnsActive) return;
            _btnsActive = false;
            transition.Show();
            Invoke(nameof(NewGameContinuation), 1f);

            PlaySound(setting.buttonClick);
        }
        void NewGameContinuation()
        {
            SceneManager.LoadScene(1);
        }
        void Btn_Refresh()
        {
            if (!_btnsActive) return;
            PlaySound(setting.buttonClick);
            SceneManager.LoadScene(0);

        }
        void Btn_Quit()
        {
            if (!_btnsActive) return;
            PlaySound(setting.buttonClick);
            Application.Quit();

        }
        public void PickOnePicture(int index)
        {
            DeselectPic();
            _selectedImage = _images[index];
            _selectedImage.Selected = true;
            btnStartGame.interactable = true;
            displayChosenPicName.text = $"PICTURE CHOSEN - {_fileNames[index]}";
            setting.chosenSprite = _selectedImage.picture.sprite;

            PlaySound(setting.choosePicture);
        }
        public void DeselectPic()
        {
            for (int i = 0; i < _images.Count; i++)
            {
                _images[i].Selected = false;
            }
            _selectedImage = null;
            displayChosenPicName.text = "CHOOSE PICTURE";
            btnStartGame.interactable = false;
            setting.chosenSprite = null;
        }

        public void PlaySound(AudioPack klipPack)
        {
            if (!setting.soundOn /*|| _source.isPlaying*/ || !_btnsActive) return;

            _source.clip = klipPack.klip;
            _source.volume = klipPack.volume;
            _source.pitch = klipPack.pitch;
            _source.Play();
        }


    }
}
