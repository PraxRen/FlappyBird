using System;

public class StartScreen : Window
{
    public event Action PlayButtonClicked;

    public override void Close()
    {
        WindowGroup.alpha = 0f;
        ActionButton.image.raycastTarget = false;
    }

    public override void Open()
    {
        WindowGroup.alpha = 1f;
        ActionButton.image.raycastTarget = true;
    }

    protected override void OnButtonClick()
    {
        PlayButtonClicked?.Invoke();
    }
}
