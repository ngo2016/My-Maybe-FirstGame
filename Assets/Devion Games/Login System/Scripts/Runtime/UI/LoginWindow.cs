﻿using DevionGames.UIWidgets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DevionGames.LoginSystem
{
    public class LoginWindow : UIWidget
    {
        public override string[] Callbacks
        {
            get
            {
                List<string> callbacks = new List<string>(base.Callbacks);
                callbacks.Add("OnLogin");
                callbacks.Add("OnFailedToLogin");
                callbacks.Add("OnFailedToLoginAccountNotExist");
                return callbacks.ToArray();
            }
        }

        [Header("Reference")]
        /// <summary>
		/// Referenced UI field
		/// </summary>
		[SerializeField]
        protected InputField username;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected InputField password;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected Toggle rememberMe;
        /// <summary>
        /// Referenced UI field
        /// </summary>
        [SerializeField]
        protected Button loginButton;
        [SerializeField]
        protected GameObject loadingIndicator;

        protected override void OnStart()
        {
            base.OnStart();
            username.text = PlayerPrefs.GetString("username", string.Empty);
            password.text = PlayerPrefs.GetString("password", string.Empty);

            if (rememberMe != null)
            {
                rememberMe.isOn = string.IsNullOrEmpty(username.text) ? false : true;
            }
          
            if (loadingIndicator != null){
                loadingIndicator.SetActive(false);
            }

            EventHandler.Register("OnLogin", OnLogin);
            EventHandler.Register("OnFailedToLogin", OnFailedToLogin);
            EventHandler.Register("OnFailedToLoginAccountNotExist", OnFailedToLoginAccountNotExist);

            loginButton.onClick.AddListener(LoginUsingFields);
        }

        public void LoginUsingFields()
        {
            if (string.IsNullOrEmpty(username.text) ||
                string.IsNullOrEmpty(password.text))
            {
                LoginManager.Notifications.emptyField.Show(delegate (int result) { Show(); }, "OK");
                Close();
                return;
            }
            if (!LoginManager.ValidateEmail(username.text))
            {
                LoginManager.Notifications.invalidEmail.Show(delegate (int result) { Show(); }, "OK");
                Close();
                return;
            }
            LoginManager.LoginAccount(username.text, password.text);
            loginButton.interactable = false;
            if (loadingIndicator != null) {
                loadingIndicator.SetActive(true);
            }
        }

        private void OnLogin() {
            if (rememberMe != null && rememberMe.isOn)
            {
                PlayerPrefs.SetString("username", username.text);
                PlayerPrefs.SetString("password", password.text);
            }
            else
            {
                PlayerPrefs.DeleteKey("username");
                PlayerPrefs.DeleteKey("password");
            }
            Execute("OnLogin",new CallbackEventData());
            if (LoginManager.DefaultSettings.loadSceneOnLogin)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(LoginManager.DefaultSettings.sceneToLoad);
            }
        }

        private void OnFailedToLogin() {
            Execute("OnFailedToLogin", new CallbackEventData());
            password.text = "";
            LoginManager.Notifications.loginFailed.Show( delegate (int result) { Show(); }, "OK");
            loginButton.interactable = true;
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }
            Close();
        }

        private void OnFailedToLoginAccountNotExist()
        {
            Execute("OnFailedToLoginAccountNotExist", new CallbackEventData());
            LoginManager.Notifications.loginFailedAccountNotExist.Show(delegate (int result) { Show(); }, "OK");
            loginButton.interactable = true;
            if (loadingIndicator != null)
            {
                loadingIndicator.SetActive(false);
            }
            Close();
        }
    }
}