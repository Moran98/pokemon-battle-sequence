using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;  // for stringbuilder
using UnityEngine;
using UnityEngine.Windows.Speech;   // grammar recogniser

public class Navigation : MonoBehaviour
{

    private GrammarRecognizer gr;
    private string valueString;

    private void Start()
    {
        gr = new GrammarRecognizer(Path.Combine(Application.streamingAssetsPath, 
                                                "SimpleGrammar.xml"), 
                                    ConfidenceLevel.Low);
        Debug.Log("Grammar loaded!");
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
        switch (valueString)
        {
            case "START THE GAME":
                Selection();
                gr.Stop();
                break;
            case "LEARN TO PLAY":
                Tutorial();
                gr.Stop();
                break;
            case "RETURN TO MENU":
                Return();
                gr.Stop();
                break;
            default:
                break;
        }
    }

    public void NewGame(){
        SceneManager.LoadScene("Game");
    }

    public void Tutorial(){
        SceneManager.LoadScene("Tutorial");
    }

    public void Return(){
        SceneManager.LoadScene("MainMenu");
    }

    public void Selection(){
        SceneManager.LoadScene("BattleSelection");
    }
}