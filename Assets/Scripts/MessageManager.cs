using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    [SerializeField]
    Text _messageBox;
    [SerializeField]
    Button _closeButton;

    bool _isChecked;

    public bool IsChecked { get => _isChecked; set => _isChecked = value; }

    public void SetMessage(string message)
    {
        _messageBox.text = message;
    }

    public void SetCloseAction(UnityAction action)
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(action);
    }
}
