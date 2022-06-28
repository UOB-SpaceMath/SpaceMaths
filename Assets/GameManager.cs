using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private AttackManager am;
    [SerializeField]
    private GameBoardManager gbm;
    [SerializeField]
    private UIManager uim;
    [SerializeField]
    private SelectionGridManager sgm;
    [SerializeField]
    private Ships player;

    private List<GameObject> enemies;

    // Store the game stage.
    public enum Stages { Question, Player, Enemies };
    private Stages stage;

    // Drag the canvas into these variables
    [SerializeField]
    private GameObject questionCanvas;
    [SerializeField]
    private GameObject selectionCanvas;


    void Start()
    {
        // Question stage
        stage = Stages.Question;
        questionCanvas.SetActive(true);
        selectionCanvas.SetActive(false);
        player = gbm.GetPlayer();
    }

    void Update()
    {
        switch (stage)
        {
            // Question stage
            case Stages.Question:
                switch (uim.GetAnswerState())
                {
                    // Answer was correct
                    case UIManager.AnswerStates.Right:
                        uim.SetAnswerState(UIManager.AnswerStates.Suspension);
                        StartCoroutine(QuestionToPlayerTurn());
                        break;

                    // Answer was incorrect
                    case UIManager.AnswerStates.Wrong:
                        uim.SetAnswerState(UIManager.AnswerStates.Suspension);
                        StartCoroutine(QuestionToEnemiesTurn());
                        break;

                    default:
                        break;
                }
                break;

            // Player's turn
            case Stages.Player:
                switch (sgm.GetSelectionResult().Type)
                {
                    case Move:
                        StartCoroutine(Move(player, sgm.GetSelectionResult().TargetIndex.x, sgm.GetSelectionResult().TargetIndex.y));
                        break;
                    case Attack:
                        StartCoroutine(AttackEnemy(player.shipObject, gbm.GetEnemy(sgm.GetSelectionResult().TargetIndex.x, sgm.GetSelectionResult().TargetIndex.y)));
                        break;
                    default:
                        break;
                }

                break;

            // Enemies' turn
            case Stages.Enemies:
                enemies = gbm.GetEnemies();
                StartCoroutine(AttackPlayer(enemies, player.shipObject));
                break;

            default:
                break;
        }

    }

    private IEnumerator QuestionToPlayerTurn()
    {
        yield return new WaitForSeconds(0.8f);
        questionCanvas.SetActive(false);
        selectionCanvas.SetActive(true);
        stage = Stages.Player;
    }

    private IEnumerator QuestionToEnemiesTurn()
    {
        yield return new WaitForSeconds(0.8f);
        questionCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        stage = Stages.Enemies;
    }

    private IEnumerator AttackEnemy(GameObject attacker, GameObject victim)
    {
        am.Attack(attacker, victim);
        yield return new WaitForSeconds(1.0f);
        stage = Stages.Enemies;
    }

    private IEnumerator AttackPlayer(List<GameObject> attckers, GameObject victim)
    {
        for (int i = 0; i < attckers.Count; i++)
        {
            am.Attack(attckers[i], victim);
            yield return new WaitForSeconds(1.0f);
        }
        stage = Stages.Question;
    }

    private IEnumerator Move(Ships ship, int x, int y)
    {
        gbm.MoveShip(player, x, y);
        // TODO make the movement animation
        yield return new WaitForSeconds(1.0f);
        stage = Stages.Enemies;
    }
}
