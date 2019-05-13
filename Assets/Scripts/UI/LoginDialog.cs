using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginDialog : Dialog {

    public InputField usernameInput;
    public InputField passwordInput;

    public void Login() {
        string user = usernameInput.text;
        string password = passwordInput.text;
    }

}
