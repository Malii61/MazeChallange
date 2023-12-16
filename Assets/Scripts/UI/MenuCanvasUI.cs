using UnityEngine;
using UnityEngine.UI;

public class MenuCanvasUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    private void Start()
    {
        Utils.SetMouseLockedState(false);
        startBtn.onClick.AddListener(() => StartGame());
    }
    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
