using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

	public Text dialogText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;


    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        // GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		// playerUnit = playerGO.GetComponent<Unit>();
        playerUnit = Instantiate(playerPrefab, playerBattleStation).GetComponent<Unit>();

        enemyUnit = Instantiate(enemyPrefab, enemyBattleStation).GetComponent<Unit>();



		// GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		// enemyUnit = enemyGO.GetComponent<Unit>();

		dialogText.text = "A wild " + enemyUnit.unitName + " appeared!";

		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

        yield return new WaitForSeconds(2f);

        state =  BattleState.PLAYERTURN;
        PlayerTurn();

    }

    void PlayerTurn(){
        dialogText.text = "Choose an action:";
    }

    IEnumerator PlayerAttack()
    {
        // Damage the enemy
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        // enemyHUD.SetHP(enemyUnit.currentHP);
        dialogText.text = "The attack is successful!";
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

    IEnumerator PlayerFleeing()
    {

        dialogText.text = "Fleeing the battle!";
        Debug.Log("Fleeing Battle");

        yield return new WaitForSeconds(2f);

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
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        // playerHUD.SetHP(playerUnit.currentHP);
        dialogText.text = "You healed up!";

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

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        // playerHUD.SetHP(playerUnit.currentHP);

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


}
