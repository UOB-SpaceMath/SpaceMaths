using UnityEngine;
using UnityEngine.UI;

public class SelectionGridManager : MonoBehaviour
{
    // game board to use
    [SerializeField] GameObject _gameBoard;

    // color of wall cell
    [SerializeField] Color _wallColor;

    // color of empty cell
    [SerializeField] Color _emptyColor;

    // color of player icon
    [SerializeField] Color _playerIconColor;

    // color of empty cell
    [SerializeField] Color _enemyIconColor;

    [SerializeField] GameObject _playerIcon;

    [SerializeField] GameObject _enemyIcon;

    // 5x5 selection cells    
    ActionType[,] _selectionCells;

    GameBoardManager _gameBoardManager;

    GameObject[] _buttons;

    SelectionOutput _finalOutput;

    void Start()
    {
        _gameBoardManager = _gameBoard.GetComponent<GameBoardManager>();
        _selectionCells = new ActionType[5, 5];
        // find all buttons
        var buttonsGroup = GetComponentInChildren<GridLayoutGroup>();
        var buttonsCount = buttonsGroup.transform.childCount;
        _buttons = new GameObject[buttonsCount];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = buttonsGroup.transform.GetChild(i).gameObject;
        }
        UpdateSelectionUI();
    }

    // Returns what player should do to selected cell
    public SelectionOutput GetFinalResult()
    {
        return _finalOutput;
    }

    public void ResetFinalResult()
    {
        _finalOutput = null;
    }

    void UpdateSelectionUI()
    {
        UpdateSelectionCells();
        UpdateButtonsView();
        UpdateButtonListener();
    }

    void UpdateSelectionCells()
    {
        // retrieve actual player position to determine 5x5 squares around player; at the moment player.position returns cell position
        for (int selectX = 0; selectX < 5; selectX++)
        {
            for (int selectY = 0; selectY < 5; selectY++)
            {
                var wholeVec = GetSelectionIndexFromWhole(selectX, selectY);
                _selectionCells[selectX, selectY] = _gameBoardManager.GetCellType(wholeVec.x, wholeVec.y) switch
                {
                    GameBoardManager.CellType.Empty => ActionType.Move,
                    GameBoardManager.CellType.Ship => ActionType.Attack,
                    GameBoardManager.CellType.Wall => ActionType.None,
                    _ => throw new System.Exception("Invalid cell type")
                };
            }
        }
        // the middle is the player's ship, should be set to None
        _selectionCells[2, 2] = ActionType.None;
    }

    void UpdateButtonsView()
    {
        var xLength = _selectionCells.GetLength(0);
        var yLenght = _selectionCells.GetLength(1);
        for (int i = 0; i < _buttons.Length; i++)
        {
            // convert to 2d array index
            var currentX = i % xLength;
            var currentY = i / yLenght;
            // set button
            SetButtonView(_buttons[i], _selectionCells[currentX, currentY]);
        }
        // set middle cell to player specific color;
        var playerButton = _buttons[_buttons.Length / 2];
        SetPlayerButton(playerButton);
        // set corner buttons invisible and not interact-able
        SetButtonInvisible(_buttons[0]); // left-up
        SetButtonInvisible(_buttons[xLength - 1]); // right-up
        SetButtonInvisible(_buttons[_buttons.Length - yLenght]);  // left-down
        SetButtonInvisible(_buttons[_buttons.Length - 1]);  // right-down
    }

    void UpdateButtonListener()
    {
        var xLength = _selectionCells.GetLength(0);
        var yLenght = _selectionCells.GetLength(1);
        for (int i = 0; i < _buttons.Length; i++)
        {
            // convert to 2d array index
            var currentX = i % xLength;
            var currentY = i / yLenght;
            _buttons[i].GetComponent<Button>().onClick.AddListener(
                () => ClickAction(new Vector2Int(currentX, currentY), _selectionCells[currentX, currentY]));
        }
    }

    void SetButtonView(GameObject button, ActionType type)
    {
        var images = button.GetComponentsInChildren<Image>();
        var buttonImage = images[0];
        var iconImage = images[1];
        // disable icon as default
        iconImage.enabled = false;
        switch (type)
        {
            case ActionType.None:
                SetButtonColor(buttonImage, _wallColor);
                break;
            case ActionType.Attack:
                SetButtonColor(buttonImage, _emptyColor);
                SetButtonIcon(iconImage, _enemyIcon, _enemyIconColor);
                break;
            case ActionType.Move:
                SetButtonColor(buttonImage, _emptyColor);
                break;
        }
    }

    void SetButtonColor(Image buttonImage, Color color)
    {
        buttonImage.color = color;
    }

    void SetButtonIcon(Image iconImage, GameObject imagePrefab, Color color)
    {
        iconImage.enabled = true;
        iconImage.sprite = imagePrefab.GetComponent<SpriteRenderer>().sprite;
        iconImage.color = color;
    }

    void SetPlayerButton(GameObject button)
    {
        var images = button.GetComponentsInChildren<Image>();
        var buttonImage = images[0];
        var iconImage = images[1];
        SetButtonColor(buttonImage, _emptyColor);
        SetButtonIcon(iconImage, _playerIcon, _playerIconColor);
    }

    void SetButtonInvisible(GameObject button)
    {
        button.GetComponent<Button>().interactable = false;
        foreach (var image in button.GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }

    }

    // convert the index of whole cells grid into the selection cells index
    Vector2Int GetSelectionIndexFromWhole(int wholeX, int wholeY)
    {
        var origin = new Vector2Int(
            _gameBoardManager.GetPlayer().CellIndex.x + 2,
            _gameBoardManager.GetPlayer().CellIndex.y + 2);
        return new Vector2Int(origin.x - wholeY, origin.y - wholeX);
    }

    Vector2Int GetWholeIndexFromSelection(int selectionX, int selectionY)
    {
        var origin = new Vector2Int(
            _gameBoardManager.GetPlayer().CellIndex.x + 2,
            _gameBoardManager.GetPlayer().CellIndex.y + 2);
        return new Vector2Int(origin.y - selectionY, origin.x - selectionX);
    }

    void ClickAction(Vector2Int selectionIndex, ActionType type)
    {
        _finalOutput = new SelectionOutput(GetWholeIndexFromSelection(selectionIndex.x, selectionIndex.y), type);
        Debug.Log(string.Format("{0} {1}", _finalOutput.Type, _finalOutput.TargetIndex));
    }
}

public enum ActionType { None, Move, Attack };

public class SelectionOutput
{
    Vector2Int _targetIndex;
    readonly ActionType _type;

    public SelectionOutput(Vector2Int targetIndex, ActionType type)
    {
        _targetIndex = targetIndex;
        _type = type;
    }

    public ActionType Type { get => _type; }
    public Vector2Int TargetIndex { get => _targetIndex; }
}