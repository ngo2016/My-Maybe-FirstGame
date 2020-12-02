﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BayatGames.SaveGameFree;
using BayatGames.SaveGameFree.Serializers;
using BayatGames.SaveGameFree.Encoders;
using System.Text;

namespace SpeedTutorMainMenuSystem
{
    public class MenuController : MonoBehaviour
    {
        #region Default Values
        [Header("Levels To Load")]
        public string stage1; 
        public string stage2; 
        public string stage3;
        private string levelToLoad; //use when load saved game

        private int menuNumber; //1 - default menu || 2 - option || 3 - graphics
        #endregion              //4 - sounds || 5 - gameplay || 6 - controls
                                //7 - new game || 8 - load game || 9 - stage select

        #region Menu Dialogs
        [Header("Main Menu Components")]
        [SerializeField] private GameObject menuDefaultCanvas;
        [SerializeField] private GameObject confirmationBox;
        [SerializeField] private GameObject stageCanvas;
        [Space(10)]
        [Header("Menu Popout Dialogs")]
        [SerializeField] private GameObject noSaveDialog;
        [SerializeField] private GameObject newGameDialog;
        [SerializeField] private GameObject loadGameDialog;
        #endregion        

        #region Initialisation - Button Selection & Menu Order
        private void Start()
        {
            menuNumber = 1;
        }
        #endregion

        //MAIN SECTION
        public IEnumerator ConfirmationBox() //hide confirm box after 2 sec visual
        {
            confirmationBox.SetActive(true);
            yield return new WaitForSeconds(2);
            confirmationBox.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //options, new game, load game, stage select
                if (menuNumber == 2 || menuNumber == 7 || menuNumber == 8 || menuNumber == 9)
                {
                    GoBackToMainMenu();
                    ClickSound();
                }
            }
        }

        private void ClickSound()
        {
            GetComponent<AudioSource>().Play();
        }

        #region Menu Mouse Clicks
        public void MouseClick(string buttonType)
        {            
            if (buttonType == "Exit")
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #endif

                Application.Quit();
            }

            if (buttonType == "LoadGame")
            {
                menuDefaultCanvas.SetActive(false);
                loadGameDialog.SetActive(true);
                menuNumber = 8;
            }

            if (buttonType == "NewGame")
            {
                menuDefaultCanvas.SetActive(false);
                newGameDialog.SetActive(true);
                menuNumber = 7;
                //set time scale back to normal in case return from pause menu or gameover
                Time.timeScale = 1;
            }
        }
        #endregion
        
        #region Dialog Options - This is where we load what has been saved in player prefs!
        public void ClickNewGameDialog(string ButtonType)
        {
            //Hide main menu and show stageCanvas
            if (ButtonType == "Yes")
            {
                menuDefaultCanvas.SetActive(false);
                stageCanvas.SetActive(true);
                menuNumber = 9;
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }

            if (ButtonType == "1")
            {
                SceneManager.LoadScene(stage1);
            }

            if (ButtonType == "2")
            {
                SceneManager.LoadScene(stage2);
            }

            if (ButtonType == "3")
            {
                SceneManager.LoadScene(stage3);
            }
        }

        public enum SaveFormat
        {
            XML,
            JSON,
            Binary
        }

        [Header("Save/Load Settings")]
        [Space]

        [Tooltip("You must specify a value for this to be able to save it.")]
        /// <summary>
        /// The position identifier.
        /// </summary>
        public string positionIdentifier = "enter the position identifier";

        [Tooltip("You must specify a value for this to be able to save it.")]
        /// <summary>
        /// The rotation identifier.
        /// </summary>
        public string rotationIdentifier = "enter the rotation identifier";

        [Tooltip("You must specify a value for this to be able to save it.")]
        /// <summary>
        /// The score identifier.
        /// </summary>
        public string scoreIdentifier = "enter the score identifier";

        [Tooltip("You must specify a value for this to be able to save it.")]
        /// <summary>
        /// The score identifier.
        /// </summary>
        public string flagIdentifier = "enter the flag identifier";

        [Tooltip("You must specify a value for this to be able to save it.")]
        /// <summary>
        /// The score identifier.
        /// </summary>
        public string stageIdentifier = "enter the flag identifier";

        [Tooltip("Encode the data?")]
        /// <summary>
        /// The encode.
        /// </summary>
        public bool encode = false;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encode password.
        /// </summary>
        public string encodePassword = "";

        [Tooltip("Which serialization format?")]
        public SaveFormat format = SaveFormat.JSON;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The serializer.
        /// </summary>
        public ISaveGameSerializer serializer;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encoder.
        /// </summary>
        public ISaveGameEncoder encoder;

        [Tooltip("If you leave it blank this will reset to it's default value.")]
        /// <summary>
        /// The encoding.
        /// </summary>
        public Encoding encoding;

        [Tooltip("Where to save? (PersistentDataPath highly recommended).")]
        /// <summary>
        /// The save path.
        /// </summary>
        public SaveGamePath savePath = SaveGamePath.PersistentDataPath;

        [Tooltip("Reset the empty fields to their default value.")]
        /// <summary>
        /// The reset blanks.
        /// </summary>
        public bool resetBlanks = true;

        private void Awake()
        {
            if (resetBlanks)
            {
                if (string.IsNullOrEmpty(encodePassword))
                {
                    encodePassword = SaveGame.EncodePassword;
                }
                if (serializer == null)
                {
                    serializer = SaveGame.Serializer;
                }
                if (encoder == null)
                {
                    encoder = SaveGame.Encoder;
                }
                if (encoding == null)
                {
                    encoding = SaveGame.DefaultEncoding;
                }
            }
            switch (format)
            {
                case SaveFormat.Binary:
                    serializer = new SaveGameBinarySerializer();
                    break;
                case SaveFormat.JSON:
                    serializer = new SaveGameJsonSerializer();
                    break;
                case SaveFormat.XML:
                    serializer = new SaveGameXmlSerializer();
                    break;
            }
        }

        public void ClickLoadGameDialog(string ButtonType)
        {
            if (ButtonType == "Yes")
            {
                GameSetting.loadType = GameSetting.LoadType.Load;

                if (GameSetting.loadType == GameSetting.LoadType.Load)
                {
                    Debug.Log("I WANT TO LOAD THE SAVED GAME");
                    //LOAD LAST SAVED SCENE
                    levelToLoad = SaveGame.Load<string>(stageIdentifier, "", encode, encodePassword,
                                                        serializer, encoder, encoding, savePath);
                    //if dont have any saved scene then show dialog and return
                    if (levelToLoad == "" || levelToLoad == null)
                    {
                        loadGameDialog.SetActive(false);
                        noSaveDialog.SetActive(true);
                        return;
                    }
                    SceneManager.LoadScene(levelToLoad);
                }
                else
                {
                    Debug.Log("Load Game Dialog");
                    menuDefaultCanvas.SetActive(false);
                    loadGameDialog.SetActive(false);
                    noSaveDialog.SetActive(true);
                }
            }

            if (ButtonType == "No")
            {
                GoBackToMainMenu();
            }
        }
        #endregion

        #region Back to Menus        
        public void GoBackToMainMenu()
        {
            menuDefaultCanvas.SetActive(true);
            newGameDialog.SetActive(false);
            loadGameDialog.SetActive(false);
            noSaveDialog.SetActive(false);
            stageCanvas.SetActive(false);
            menuNumber = 1;
        }

        public void ClickQuitOptions()
        {
            GoBackToMainMenu();
        }

        public void ClickNoSaveDialog()
        {
            GoBackToMainMenu();
        }
        #endregion
    }
}

public static class GameSetting
{
    public enum LoadType
    {
        New,
        Load
    }

    public static LoadType loadType;
    public static string username = "test";
    public static string uid = "123456";
}