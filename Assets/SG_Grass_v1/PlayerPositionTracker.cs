using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionTracker : MonoBehaviour
{
    [Tooltip("Материал, который будет получать позицию игрока")]
    public Material targetMaterial;

    [Tooltip("Радиус взаимодействия с травой")]
    public float interactionRadius = 1.5f;

    [Tooltip("Имя свойства позиции в шейдере")]
    public string positionPropertyName = "_PlayerPosition";

    [Tooltip("Имя свойства радиуса в шейдере")]
    public string radiusPropertyName = "_RadiusInteract";

    void Update()
    {
        if (targetMaterial != null)
        {
            targetMaterial.SetVector(positionPropertyName, transform.position);
            targetMaterial.SetFloat(radiusPropertyName, interactionRadius);
        }
    }

    public void SetInteractionRadius(float newRadius)
    {
        interactionRadius = newRadius;
    }
}