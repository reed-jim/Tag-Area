using UnityEngine;

public class GameVariableInitializer : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;

    [SerializeField] private Vector2Variable canvasSize;
    [SerializeField] private BoolVariable isBotVariable;

    private void Awake()
    {
        canvasSize.Value = canvas.sizeDelta;

        isBotVariable.Value = false;
    }
}
