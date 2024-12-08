using UnityEngine;

public class LobbyRoomScrollViewModifier : MonoBehaviour, ISaferioLayoutModifier
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform viewArea;

    private Vector2 _canvasSize;

    public void Modify()
    {
        _canvasSize = canvas.sizeDelta;

        UIUtil.SetSize(viewArea, 0.9f * _canvasSize.x, 0.6f * _canvasSize.y);

        UIUtil.SetLocalPositionOfRectToAnotherRectVertically(viewArea, canvas, 0.5f, -0.5f);
    }
}
