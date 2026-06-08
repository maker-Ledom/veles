using UnityEngine;
public class ArtifactItem : MonoBehaviour
{
    public void GrantLifesaver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerModel.AcquireLifesaverPerk();
            Debug.Log("Вы получили артефакт: Сердце Феникса!");
            Destroy(gameObject);
        }
    }
}