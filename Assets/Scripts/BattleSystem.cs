using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text; 
using UnityEngine.Windows.Speech;  

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST}

public class BattleSystem : MonoBehaviour
{

	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

    public System.Random rnd = new System.Random();

	Unit playerUnit;
	Unit enemyUnit;

    public static bool isPaused=false;
    public GameObject pauseMenuUI;

	public Text dialogText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

    private GrammarRecognizer gr;
    [SerializeField] private string valueString;

    private void Start()
    {
        gr = new GrammarRecognizer(Path.Combine(Application.streamingAssetsPath, 
                                                "Battle.xml"), 
                                    ConfidenceLevel.Low);
        Debug.Log("Grammar loaded! - Battle.xml");
        gr.OnPhraseRecognized += GR_OnPhraseRecognized;
        gr.Start();
        if (gr.IsRunning) Debug.Log("Recogniser running");

        state = BattleState.START;
        StartCoroutine(SetupBattle());
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

    // Setting up the battle and instantiating the player and enemy prefabs
    // HUDS are displayed depending on which oponent is chosen.
    IEnumerator SetupBattle()
    {
        playerUnit = Instantiate(playerPrefab, playerBattleStation).GetComponent<Unit>();
        enemyUnit = Instantiate(enemyPrefab, enemyBattleStation).GetComponent<Unit>();

		dialogText.text = "A wild " + enemyUnit.unitName + " appeared!";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state =  BattleState.PLAYERTURN;
        PlayerTurn();

    }

    void Update()
    {
       Commands();
    }

    private void Commands(){
        switch (valueString)
        {
            case "ATTACK USING HYPER BEAM":
                onAttackButton();
                valueString = "";
                break;
            case "ATTACK USING SELF DESTRUCT":
                onHyperBeam();
                valueString = "";
                break;
            case "HEAL UP":
                onHealingButton();
                valueString = "";
                break;
            case "FLEE THE BATTLE":
                onFleeingButton();
                valueString = "";
                break;
            case "PAUSE THE GAME":
                PauseGame();
                Debug.Log("PAUSE");
                break;
            case "RESUME THE GAME":
                ResumeGame();
                break;
            default:
                break;
        }
    }

    void PlayerTurn(){
        dialogText.text = "Choose an action:";
        Debug.Log("Players Turn");
    }

    IEnumerator PlayerAttack()
    {
        // Damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        // Calculate the HP
        enemyHUD.SetHP(enemyUnit.currentHP);
        dialogText.text = "Self Destruct is critical!";
        Debug.Log("Successful attack");

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            // End
            state = BattleState.WON;
            EndBattle();
        } else{
            // Enemy turn
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack2()
    {
        bool isDead = false;

        int num = rnd.Next(0, 10);
        Debug.Log(num);

        if(num >= 6 && num <= 10){
            dialogText.text = "Attack Failed!";

        } else{
            // Damage the enemy
            enemyUnit.TakeDamage(playerUnit.hyperBeam);

            // Calculate the HP
            enemyHUD.SetHP(enemyUnit.currentHP);

            dialogText.text = "Hyper Beam is successful!";
            Debug.Log("Successful attack");
        }
        

        yield return new WaitForSeconds(2f);

        if(isDead)
        {
            // End
            state = BattleState.WON;
            EndBattle();
        } else{
            // Enemy turn
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerFleeing()
    {

        dialogText.text = "Fleeing the battle!";
        Debug.Log("Fleeing Battle");

        yield return new WaitForSeconds(2f);

        // Random number to decide if allowed to flee or not
        int num = rnd.Next(0, 10);
        Debug.Log(num);

        if(num >= 5 && num <= 10){
            dialogText.text = "Unable to Flee the battle!";

        } else{
            SceneManager.LoadScene("MainMenu");
        }

    }

    IEnumerator PlayerHealing()
    {
        // Damage the enemy

        playerUnit.Heal(15);

        playerHUD.SetHP(playerUnit.currentHP);
        dialogText.text = "You healed up by 15!";

        yield return new WaitForSeconds(2f);

        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    void EndBattle()
    {
        if(state == BattleState.WON)
        {
            dialogText.text = "You won the battle!";
        } else if(state == BattleState.LOST)
        {
            dialogText.text = "You were defeated";
        }
    }

    IEnumerator EnemyTurn()
    {
        dialogText.text = enemyUnit.unitName + " attacked!";
        Debug.Log("Enemys Turn");

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        playerHUD.SetHP(playerUnit.currentHP);

        if(isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        } else{
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }

    public void onAttackButton()
    {
        if(state!=BattleState.PLAYERTURN)
        {
            return;
        }else{
            StartCoroutine(PlayerAttack());
            Debug.Log("Attacked");
        }

    }

    public void onHyperBeam()
    {
        if(state!=BattleState.PLAYERTURN)
        {
            return;
        }else{
            StartCoroutine(PlayerAttack2());
            Debug.Log("Attacked");
        }

    }

    public void onFleeingButton()
    {
        if(state!=BattleState.PLAYERTURN)
        {
            return;
        }else{
            StartCoroutine(PlayerFleeing());
        }

    }

    public void onHealingButton()
    {
        if(state!=BattleState.PLAYERTURN)
        {
            return;
        }else{
            StartCoroutine(PlayerHealing());
        }

    }

    // Update is called once per frame
    void PauseGame()
    {
        isPaused = true;
        Debug.Log("PAUSE CALLED");
        pauseMenuUI.SetActive(true);
        Time.timeScale=0f;
    }

    // Update is called once per frame
    void ResumeGame()
    {
        isPaused = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale=1f;
    }


}
