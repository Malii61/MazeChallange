using UnityEngine;

public class Prize : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Player got the prize 
        GameManager.Instance.ChangeState(GameManager.State.GameFinished);
    }
}
