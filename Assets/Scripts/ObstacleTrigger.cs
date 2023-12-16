using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        ParticleManager.Instance.Play(ParticleType.Explosion, transform.position);
        GameManager.Instance.ChangeState(GameManager.State.GameOver);
    }
}
