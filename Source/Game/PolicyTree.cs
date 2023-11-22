using System.Collections.Generic;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;
public class PolicyTree
{
    public List<Policy> Tree { get; private set; } = new();
    public UIControl Grid { get; private set; } = null;

    public PolicyTree(UIControl grid)
    {
        Grid = grid;
    }

    public void AddPolicy(Policy new_policy)
    {
        new_policy.ParentTree = this;
        Tree.Add(new_policy);
    }

    public void EnablePolicy(Policy policy)
    {
        Actor[] children = Grid.Children;
        UniformGridPanel gp = (UniformGridPanel)Grid.Control;

        //btn.Clicked += policy.Purchase;

        int max = gp.SlotsHorizontally * gp.SlotsVertically - 1;
        int index = policy.Y * gp.SlotsHorizontally + policy.X;//max - (gp.SlotsHorizontally - -policy.X - (gp.SlotsVertically - policy.Y));
        //Debug.Log(index);
        index = Mathf.Clamp(index, 0, max);
        //Mathf.Clamp(policy.Y * (gp.SlotsHorizontally - policy.X), 0, gp.SlotsHorizontally * gp.SlotsVertically);
        UIControl btn_ctrl = (UIControl)children[index];

        PolicyButton btn = new(policy, btn_ctrl, (UICanvas)Grid.Parent)
        {
            AutoFocus = false
        };

        btn_ctrl.Control = btn;
    }

    public void RemoveReq(Policy policy)
    {
        foreach (Policy branch in Tree)
        {
            if (branch.Requirements.Remove(policy) && branch.CheckReady())
            {
                EnablePolicy(branch);
            }
        }
    }
}
