using UnityEngine;

public class Prize : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameManager.Instance.ChangeState(GameManager.State.GameFinished);
    }
}
