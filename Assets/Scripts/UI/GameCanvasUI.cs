using System.Collections;
using UnityEngine;

public class GameCanvasUI : MonoBehaviour
{
    [SerializeField] private Transform gameFinishedUI;
    [SerializeField] private Transform gameOverUI;
    [SerializeField] private Transform gameControlsUI;
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameInput.Instance.OnOpenedOrClosedPanel += GameInput_OnOpenedOrClosedPanel;
    }

    private void GameInput_OnOpenedOrClosedPanel(object sender, System.EventArgs e)
    {
        gameControlsUI.gameObject.SetActive(!gameControlsUI.gameObject.activeSelf);
    }
    private void OnDestroy()
    {
        GameManager.Instance.OnStateChanged -= GameManager_OnStateChanged;
        GameInput.Instance.OnOpenedOrClosedPanel -= GameInput_OnOpenedOrClosedPanel;
    }
    private void GameManager_OnStateChanged(object sender, GameManager.OnStateChangedEventArgs e)
    {
        switch (e.CurrentState)
        {
            case GameManager.State.GameOver:
                ChangeActivityOfTransform(gameOverUI, true, 1f);
                break;
            case GameManager.State.GameFinished:
                ChangeActivityOfTransform(gameFinishedUI, true, 2f);
                break;
        }
    }
    private void ChangeActivityOfTransform(Transform t, bool isActive, float changeActivityBackTimer = 0f)
    {
        t.gameObject.SetActive(isActive);
        if (changeActivityBackTimer > 0f)
        {
            StartCoroutine(ChangeActivityAfterDelay(t, !isActive, new WaitForSeconds(changeActivityBackTimer)));
        }
    }
    private IEnumerator ChangeActivityAfterDelay(Transform t, bool isActive, WaitForSeconds delay)
    {
        yield return delay;
        t.gameObject.SetActive(isActive);

        //if the game finished successfully go back to the menu scene
        if (t == gameFinishedUI)
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
