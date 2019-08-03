using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour {

    public Client client;
    const string DB_SIGNUP_URL = "http://localhost/caliburn/signup.php";
    const string DB_LOGIN_URL = "http://localhost/caliburn/login.php";

    
    public Button backButton;
    public GameObject buttonGroup;
    public Button loginButton;
    public Button signupButton;

    public GameObject signupGroup;
    public InputField usernameSignupField;
    public InputField passwordSignupField;
    public InputField confirmSignupField;
    public InputField emailSignupField;
    public Button submitSignupButton;

    public GameObject loginGroup;
    public InputField usernameLoginField;
    public InputField passwordLoginField;
    public Button submitLoginButton;

    void Awake() {
        OnShowButtonGroup();
        OnHideLoginGroup();
        OnHideSignupGroup();
    }

    void Update() {
        if(Mirror.NetworkClient.active) {
            submitLoginButton.interactable = true;
        } else {
            submitLoginButton.interactable = false;
        }
    }

    public void OnShowButtonGroup() {
        buttonGroup.SetActive(true);
        backButton.gameObject.SetActive(false);
        OnHideLoginGroup();
        OnHideSignupGroup();
    }

    public void OnHideButtonGroup() {
        backButton.gameObject.SetActive(true);
        buttonGroup.SetActive(false);
    }

    public void OnShowLoginGroup() {
        loginGroup.SetActive(true);
        OnHideButtonGroup();
        OnHideSignupGroup();
    }

    public void OnHideLoginGroup() {
        loginGroup.SetActive(false);
    }

    public void OnShowSignupGroup() {
        signupGroup.SetActive(true);
        OnHideButtonGroup();
        OnHideLoginGroup();
    }

    public void OnHideSignupGroup() {
        signupGroup.SetActive(false);
    }

    public void OnBack() {
        OnHideSignupGroup();
        OnHideLoginGroup();
        OnShowButtonGroup();
    }

    public void OnLogin() {
        if(usernameLoginField.text == "" || passwordLoginField.text == "") {
            Debug.Log("One or more fields are invalid.");
            return;
        }

        client.Login(usernameLoginField.text, passwordLoginField.text);

        OnHideLoginGroup();
        backButton.gameObject.SetActive(false);
    }

    public void OnLoginComplete() {
        OnShowLoginGroup();
    }

    public void OnSignup() {
        if(usernameSignupField.text == "" || passwordSignupField.text == "" || confirmSignupField.text == "" || emailSignupField.text == "") {
            Debug.Log("One or more fields are invalid.");
            return;
        }

        if(passwordSignupField.text != confirmSignupField.text) {
            Debug.Log("Passwords do not match.");
            return;
        }

        client.Signup(usernameSignupField.text, passwordSignupField.text, confirmSignupField.text, emailSignupField.text);

        OnHideSignupGroup();
        backButton.gameObject.SetActive(false);
    }

    public void OnSignupComplete() {
        OnShowButtonGroup();
    }

    public void SignupRequest(string username, string password, string confirm, string email) {
        if(username == "" || password == "" || confirm == "" || email == "" || password != confirm) {
            Debug.Log("Invalid fields.");
            return;
        }

        StartCoroutine(ESignupRequest(username, password, email));
        OnHideSignupGroup();
        backButton.gameObject.SetActive(false);
    }

    IEnumerator ESignupRequest(string username, string password, string email) {
        Dictionary<string, string> parameters = new Dictionary<string, string>(){ { "uid", username }, { "password", password }, { "email", email } };
        using(UnityWebRequest signupRequest = UnityWebRequest.Post(DB_SIGNUP_URL, parameters)) {
            yield return signupRequest.SendWebRequest();

            if (signupRequest.isNetworkError) {
				Debug.Log("Error: " + signupRequest.error);
			}

            string result = signupRequest.downloadHandler.text;
            Debug.Log(result);
        }

        OnShowSignupGroup();
        yield break;
    }

    public void LoginRequest(string username, string password) {
        if(username == "" || password == "") {
            Debug.Log("Invalid username or password.");
            return;
        }

        StartCoroutine(ELoginRequest(username, password));
        OnHideLoginGroup();
        backButton.gameObject.SetActive(false);
    }

    IEnumerator ELoginRequest(string username, string password) {
        Dictionary<string, string> parameters = new Dictionary<string, string>(){ { "uid", username }, { "password", password } };

        using(UnityWebRequest loginRequest = UnityWebRequest.Post(DB_LOGIN_URL, parameters)) {
            yield return loginRequest.SendWebRequest();

            if (loginRequest.isNetworkError) {
				Debug.Log("Error: " + loginRequest.error);
			}

            string result = loginRequest.downloadHandler.text;
            Debug.Log(result);

        }

        OnShowLoginGroup();
        yield break;
    }


}
