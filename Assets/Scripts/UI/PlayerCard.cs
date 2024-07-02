using UnityEngine;
using TMPro;

public class PlayerCard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI username;
    public ulong clientID = 0;
    public void SetUsernameText(string text)
    {
        username.text = text;
    }
}
