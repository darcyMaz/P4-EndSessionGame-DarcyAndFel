using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    private Image panelImg;
    private bool HasImg = false;

    private void Awake()
    {
        if (!TryGetComponent(out panelImg)) Debug.Log("The PausePanel could not find its Image UI component.");
        else HasImg = true;
    }

    private void Start()
    {
        TurnOffPanel();
    }

    private void TurnOffPanel()
    {
        gameObject.SetActive(false);
    }
    private void TurnOnPanel()
    {
        gameObject.SetActive(true);
    }

    // i have the bools wrong rn lol otherwise works
    public void FlipPanel(bool PauseSwitch)
    {
        if (HasImg)
        {
            if (PauseSwitch)
            {
                TurnOnPanel();
                Time.timeScale = 0;
            }
            else
            {
                TurnOffPanel();
                Time.timeScale = 1;
            }
        }
    }
}
