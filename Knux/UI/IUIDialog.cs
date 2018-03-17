using System.Collections;

public interface IUIDialog
{
    bool MoveOnScreen();
    bool MoveOffScreen();
    IEnumerator MoveDialog(float targetX, float duration, System.Action cbOnFinish = null);
    void SetStatus(bool status);
}
