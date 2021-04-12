using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Navigation : MonoBehaviour
{
    public void NewGame(){
        SceneManager.LoadScene("Game");
    }

    public void Tutorial(){
        SceneManager.LoadScene("Tutorial");
    }
}
