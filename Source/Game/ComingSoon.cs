using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

/// <summary>
/// ComingSoon Script.
/// </summary>
public class ComingSoon : Script
{
    public UIControl btn_control;
    public UIControl fadein;
    public Material color;
    public CharacterController PlayerController;

    private Button button;
    private AlphaPanel panel;
    private bool clicked = false;

    public override void OnEnable()
    {
        button = btn_control.Get<Button>();
        panel = fadein.Get<AlphaPanel>();
        button.Clicked += OnClick;
    }

    public override void OnDisable()
    {
        button.Clicked -= OnClick;
        color.SetParameterValue("Color", Color.Black);
    }

    public void OnClick()
    {
        clicked = true;
        //PlayerScript p_script = (PlayerScript)PlayerController.Scripts[0];
        //p_script.ComingSoon();
    }

    public override void OnFixedUpdate()
    {
        if (clicked)
        {
            color.SetParameterValue("Color", Color.Lerp((Color)color.GetParameter("Color").Value, Color.White, 0.5f * Time.DeltaTime));

            if (((Color)color.GetParameter("Color").Value).R >= 0.6)
                panel.Alpha = Mathf.Lerp(panel.Alpha, 1, Time.DeltaTime);
        }
    }
}
