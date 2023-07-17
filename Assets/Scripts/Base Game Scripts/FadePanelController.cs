using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanelController : MonoBehaviour
{


    public Animator panelAnimator;
    public Animator gameInfoAnimator;

    // Triggers animations of panel
    public void OKButton()
    {
        if (panelAnimator != null && gameInfoAnimator != null)
        {
            panelAnimator.SetBool("Out", true);
            gameInfoAnimator.SetBool("Out", true);
        }
    }
}
