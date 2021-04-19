using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text; 
using UnityEngine.Windows.Speech;  

public class PauseMenu : MonoBehaviour
{

    public static bool isPaused=false;
    public GameObject pauseMenuUI;

    private GrammarRecognizer gr;
    private string valueString;

    private void Start()
    {
        gr = new GrammarRecognizer(Path.Combine(Application.streamingAssetsPath, 
                                                "Pause.xml"), 
                                    ConfidenceLevel.Low);
        Debug.Log("Grammar loaded! - Pause.xml");
        gr.OnPhraseRecognized += GR_OnPhraseRecognized;
        gr.Start();
        if (gr.IsRunning) Debug.Log("Recogniser running");
    }

    private void GR_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        StringBuilder message = new StringBuilder();
        Debug.Log("Recognised a phrase");
        // read the semantic meanings from the args passed in.
        SemanticMeaning[] meanings = args.semanticMeanings;
        
        // use foreach to get all the meanings.
        foreach(SemanticMeaning meaning in meanings)
        {
            string keyString = meaning.key.Trim();
            valueString = meaning.values[0].Trim();
            message.Append("Key: " + keyString + ", Value: " + valueString + " ");
        }

        
        // use a string builder to create the string and out put to the user
        Debug.Log(message);
    }

    private void OnApplicationQuit()
    {
        if (gr != null && gr.IsRunning)
        {
            gr.OnPhraseRecognized -= GR_OnPhraseRecognized;
            gr.Stop();
        }
    }

    void Update()
    {
       Commands();
    }

    private void Commands(){
        switch (valueString)
        {
            case "PAUSE THE GAME":
                PauseGame();
                Debug.Log("PAUSE");
                break;
            case "RESUME THE GAME":
                ResumeGame();
                break;
            case "QUIT THE GAME":
                QuitGame();
                break;
            default:
                break;
        }
    }


    void PauseGame()
    {
        isPaused = true;
        Debug.Log("PAUSE CALLED");
        pauseMenuUI.SetActive(true);
        Time.timeScale=0f;
    }

    void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale=1f;
    }

    void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
