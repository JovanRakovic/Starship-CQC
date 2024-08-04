using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private InputActionReference escapeAction;
    private CanvasGroup pauseGroup;
    public bool paused { get; private set; } = false;

    private void Awake()
    {
        pauseGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        Resume();
        
        escapeAction.action.Enable();
        escapeAction.action.started += context => { if(paused) Resume(); else Pause(); paused = !paused; };
    }

    public void Pause()
    {
        UITools.ToggleCanvasGroup(pauseGroup, true);
    }

    public void Resume()
    {
        UITools.ToggleCanvasGroup(pauseGroup, false);
    }

    public void Quit()
    {
        NetworkSceneManager.LoadSceneNetwork("MainMenu");
    }
}