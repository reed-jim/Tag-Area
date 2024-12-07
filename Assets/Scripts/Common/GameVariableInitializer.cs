using UnityEngine;

public class GameVariableInitializer : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;

    [SerializeField] private Vector2Variable canvasSize;

    private void Awake()
    {
        canvasSize.Value = canvas.sizeDelta;
    }
}
