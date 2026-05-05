using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    private Image panelImg;
    private bool HasImg = false;

    private Color PauseColor = new Color(1, 1, 1, 0.5f);
    private Color PlayColor = new Color(1, 1, 1, 0);

    private void Awake()
    {
        if (!TryGetComponent(out panelImg)) Debug.Log("The PausePanel could not find its Image UI component.");
        else HasImg = true;
    }
    public void FlipPanel(bool PauseSwitch)
    {
        if (HasImg)
        {
            if (PauseSwitch)
            {
                panelImg.color = PauseColor;
                Time.timeScale = 0;
            }
            else
            {
                panelImg.color = PlayColor;
                Time.timeScale = 1;
            }
        }
    }
}
