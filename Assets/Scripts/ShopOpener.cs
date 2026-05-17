using UnityEngine;

public class ShopOpener : MonoBehaviour
{
    public GameObject shopPanel;

    public void OpenShop()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OpenPanel(shopPanel);
        else
            shopPanel.SetActive(true);
    }
}