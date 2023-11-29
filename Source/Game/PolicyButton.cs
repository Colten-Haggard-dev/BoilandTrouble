using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

/// <summary>
/// PolicyButton Script.
/// </summary>
public class PolicyButton : Button
{
    private readonly Policy Policy = null;
    private readonly Label HoverLabel = null;
    private readonly UICanvas UI = null;
    private readonly UIControl ParentButton = null;
    private readonly UIControl HoverText = null;
    private readonly UIControl ButtonText = null;

    public PolicyButton(Policy policy, UIControl parent_btn, UICanvas ui)
    {
        Policy = policy;
        UI = ui;
        ParentButton = parent_btn;

        Clicked += PurchasePolicy;
        HoverLabel = new Label
        {
            Text = Policy.ToString(),
            AnchorPreset = AnchorPresets.MiddleLeft,
            Size = new(25, 25),
            Visible = false
        };
        HoverLabel.Font.Size = 4;
        HoverText = UI.AddChild<UIControl>();
        HoverText.Control = HoverLabel;
        HoverText.LocalPosition = parent_btn.LocalPosition + new Vector3(40f, -5f, 0);
        HoverBegin += ButtonEnter;
        HoverEnd += ButtonExit;
        SizeChanged += ChangeTextSize;

        Label btn_lbl = new()
        {
            Text = policy.Name,
            AnchorPreset = AnchorPresets.MiddleCenter,
            Size = parent_btn.Control.Size
        };

        ButtonText = ParentButton.AddChild<UIControl>();
        ButtonText.Control = btn_lbl;
    }

    public void ChangeTextSize(Control control)
    {
        //Debug.Log(control);
        ((Label)ButtonText.Control).AutoFitText = true;
        //button_text.LocalPosition = ParentButton.LocalPosition;
    }

    public void ButtonEnter()
    {
        HoverLabel.Text = Policy.ToString();
        HoverLabel.Visible = true;
    }

    public void ButtonExit()
    {
        HoverLabel.Visible = false;
    }

    public void PurchasePolicy()
    {   
        if (Policy != null && Policy.Purchase())
            HoverLabel.Text = Policy.ToString();
    }

    ~PolicyButton()
    {
        Clicked -= PurchasePolicy;
        HoverBegin -= ButtonEnter;
        HoverEnd -= ButtonExit;
        SizeChanged -= ChangeTextSize;
    }
}
