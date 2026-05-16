using UnityEngine;
public class ArtifactItem : MonoBehaviour
{
    public void GrantLifesaver()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerModel.HasLifesaverPerk = true;
            Debug.Log("Вы получили артефакт: Сердце Феникса!");
            Destroy(gameObject); // Артефакт подобран
        }
    }
}