using SpaceMath;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Sub managers
    [SerializeField]
    private AttackManager am;
    [SerializeField]
    private GameBoardManager gbm;
    [SerializeField]
    private UIManager uim;
    [SerializeField]
    private SelectionGridManager sgm;
    [SerializeField]
    private WatsonManager wm;
    [SerializeField]
    private MessageManager mm;

    private Ships player;
    private List<Ships> enemies;

    // Store the game stage.
    public enum Stages { None, Question, Player, Enemies };
    private Stages stage;

    // Drag the canvas into these variables in the inspector
    [SerializeField]
    private GameObject questionCanvas;
    [SerializeField]
    private GameObject selectionCanvas;
    [SerializeField]
    private GameObject messageCanvas;

    // Game settings
    [SerializeField]
    private int energyConsumption;

    [SerializeField]
    private float panelHigh;

    void Start()
    {
        // Question stage
        stage = Stages.Question;
        player = gbm.GetPlayer();
        SetPanel(PanelType.Question);
    }

    void Update()
    {
        if (player.IsShipDead())
        {
            // Game over
        }
        else if (!gbm.IsEnemiesRemain())
        {
            // You win
        }
        else
        {
            switch (stage)
            {
                // Question stage
                case Stages.Question:
                    SetPanel(PanelType.Question);
                    switch (uim.GetAnswerState())
                    {
                        // Answer was correct
                        case UIManager.AnswerStates.Right:
                            stage = Stages.None;
                            uim.SetAnswerState(UIManager.AnswerStates.Suspension);
                            StartCoroutine(QuestionToPlayerTurn());
                            break;

                        // Answer was incorrect
                        case UIManager.AnswerStates.Wrong:
                            stage = Stages.None;
                            uim.SetAnswerState(UIManager.AnswerStates.Suspension);
                            StartCoroutine(QuestionToEnemiesTurn());
                            break;

                        default:
                            break;
                    }
                    break;

                // Player's turn
                case Stages.Player:
                    SetPanel(PanelType.Selection);
                    // check Watson running
                    if (wm.IsWatsonRunning())
                    {
                        stage = Stages.None;
                        StartCoroutine(RunWatsonCommand());
                        break;
                    }
                    SelectionOutput selectionResult = sgm.GetFinalResult();
                    sgm.ResetFinalResult();
                    if (selectionResult != null && selectionResult.Type != ActionType.None)
                    {
                        stage = Stages.None;
                        switch (selectionResult.Type)
                        {
                            case ActionType.Move:
                                StartCoroutine(Move(player, selectionResult.TargetIndex.x, selectionResult.TargetIndex.y));
                                break;
                            case ActionType.Attack:
                                StartCoroutine(AttackEnemy(player, gbm.GetShip(selectionResult.TargetIndex)));
                                break;
                            //case ActionType.Shield:
                            //   Do something
                            //   break;
                            default:
                                break;
                        }
                    }
                    break;

                // Enemies' turn
                case Stages.Enemies:
                    stage = Stages.None;
                    enemies = gbm.GetEnemiesInRange();
                    StartCoroutine(AttackPlayer(enemies, player));
                    break;
                default:
                    // reset the second selection input occurred in none stage.
                    sgm.ResetFinalResult();
                    break;
            }
        }
    }

    private void SetPanel(PanelType type)
    {
        // set all false
        questionCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        messageCanvas.SetActive(false);
        switch (type)
        {
            case PanelType.Question:
                questionCanvas.SetActive(true);
                break;
            case PanelType.Selection:
                selectionCanvas.SetActive(true);
                break;
            case PanelType.Message:
                messageCanvas.SetActive(true);
                break;
        }
        SetPanelPostion();
    }

    private void ShowMessage(string message, Stages nextStage)
    {
        mm.SetMessage(message);
        SetPanel(PanelType.Message);
        mm.SetCloseAction(() => { stage = nextStage; });
    }

    //// Set "on" to true to turn on the question panel.
    //private void SwitchPanel(bool on)
    //{
    //    questionCanvas.SetActive(on);
    //    selectionCanvas.SetActive(!on);
    //}

    private IEnumerator QuestionToPlayerTurn()
    {
        yield return new WaitForSeconds(0.8f);
        SetPanel(PanelType.Selection);
        stage = Stages.Player;
    }

    private IEnumerator QuestionToEnemiesTurn()
    {
        yield return new WaitForSeconds(0.8f);
        questionCanvas.SetActive(false);
        selectionCanvas.SetActive(false);
        stage = Stages.Enemies;
    }

    private IEnumerator AttackEnemy(Ships player, Ships enemy)
    {
        am.Attack(player, enemy);
        yield return new WaitForSeconds(1.0f);
        stage = Stages.Enemies;
        sgm.UpdateSelectionUI();
    }

    private IEnumerator AttackPlayer(List<Ships> enemies, Ships player)
    {
        player.DecreaseEnergy(energyConsumption);
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                am.Attack(enemies[i], player);
                yield return new WaitForSeconds(1.0f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }
        stage = Stages.Question;
    }

    private IEnumerator Move(Ships ship, int x, int y)
    {
        gbm.MoveShip(ship, x, y);
        // TODO make the movement animation
        yield return new WaitForSeconds(1.0f);
        stage = Stages.Enemies;
        sgm.UpdateSelectionUI();
    }

    private IEnumerator RunWatsonCommand()
    {
        Debug.Log("Running Watson Command.");
        // wait for WatsonManager
        while (wm.GetFinalResult() == null)
        {
            yield return null;
        }
        var output = wm.GetFinalResult();
        wm.ResetFinalResult();
        var index = sgm.GetWholeIndexFromSelection(output.SelectionIndex);
        switch (output.Intent)
        {
            case WatsonIntents.Attack:
                if (gbm.IsEnemy(index))
                {
                    StartCoroutine(AttackEnemy(player, gbm.GetShip(index)));
                }
                else
                {
                    var message = string.Format("Nothing to be attacked on {0}", sgm.GetIndexNameString(output.SelectionIndex));
                    ShowMessage(message, Stages.Player);
                    Debug.Log(string.Format("Fail to attack {0}", index));
                }
                break;
            case WatsonIntents.Move:
                if (gbm.IsEmpty(index))
                {
                    StartCoroutine(Move(player, index.x, index.y));
                }
                else
                {
                    var message = string.Format("You can't move to {0}", sgm.GetIndexNameString(output.SelectionIndex));
                    ShowMessage(message, Stages.Player);
                    Debug.Log(string.Format("Fail to move {0}", index));
                }
                break;
            //case WatsonIntents.Sheild:
            //    // todo
            //    break;
            default: // fail
                     // message box;
                ShowMessage(output.FailMessage, Stages.Player);
                break;
        }

    }

    private void SetPanelPostion()
    {
        if (player.ShipObject != null)
        {
            Vector3 pos = player.ShipObject.transform.position;
            questionCanvas.transform.parent.position = new Vector3(pos.x, pos.y + panelHigh, pos.z);
            selectionCanvas.transform.parent.position = new Vector3(pos.x, pos.y + panelHigh, pos.z);
            messageCanvas.transform.parent.position = new Vector3(pos.x, pos.y + panelHigh, pos.z);
        }
    }

    private enum PanelType { Question, Selection, Message };
}
