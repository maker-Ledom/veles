using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public float interactionDistance = 3f;
    public UnityEvent onInteract;

    private Transform _player;

    private void Start()
    {
        var runtime = Object.FindFirstObjectByType<PlayerRuntime>();
        if (runtime != null) _player = runtime.transform;
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                onInteract?.Invoke();
            }
        }
    }
}