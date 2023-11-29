using System;
using System.Collections.Generic;
using System.Linq;

namespace Game;

/// <summary>
/// Policy Script.
/// </summary>
public class Policy
{
    public List<Policy> Requirements { get; private set; } = new();
    public Simulation SimRef { get; private set; } = null;
    public double Cost { get; set; } = 0;
    public uint[] InfluenceCosts { get; private set; } = new uint[4];
    public bool Purchased { get; private set; } = false;
    public PolicyTree ParentTree { get; set; } = null;
    public string Name { get; private set; } = string.Empty;
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    public Action<Simulation> Effect { get; set; } = null;

    public Policy(string name, int x, int y, double cost, uint soc_inf, uint eco_inf, uint struct_inf, Simulation simRef, Action<Simulation> effect, Policy[] requirements = null)
    {
        Name = name;
        X = x;
        Y = y;
        Cost = cost;
        Effect = effect;

        InfluenceCosts[(int)InfluenceBranches.Oganizational] = 0;
        InfluenceCosts[(int)InfluenceBranches.Social] = soc_inf;
        InfluenceCosts[(int)InfluenceBranches.Economic] = eco_inf;
        InfluenceCosts[(int)InfluenceBranches.Structural] = struct_inf;

        SimRef = simRef;

        //Debug.Log(requirements);

        if (requirements == null || Requirements == null)
        {
            return;
        }

        //foreach (Policy p in  requirements)
        //{
        //    Debug.Log(name + ":");
        //    Debug.Log(p.Name);
        //}

        Requirements = requirements.ToList();
    }

    public bool CheckReady()
    {
        return (Requirements == null || Requirements.Count == 0) && !Purchased;
    }

    public bool Purchase()
    {
        bool check = SimRef.Influence == null;
        //Debug.Log(check);
        check = check || SimRef.TotalBalance < Cost;
        //Debug.Log(check);
        check = check || Purchased;
        //Debug.Log(check);
        check = check || (SimRef.Influence[0] < InfluenceCosts[0] || SimRef.Influence[1] < InfluenceCosts[1] || SimRef.Influence[2] < InfluenceCosts[2]);
        //Debug.Log(check);

        if (check)
            return false;

        SimRef.TotalBalance -= Cost;
        Purchased = true;
        ParentTree.RemoveReq(this);
        Effect?.Invoke(SimRef);

        return true;
    }

    public override string ToString()
    {
        return string.Format("Cost: {0}\nSoc req: {1}\nEco req: {2}\nStructural req: {3}\nPurchased: {4}\n", Cost,
            InfluenceCosts[(int)InfluenceBranches.Social], InfluenceCosts[(int)InfluenceBranches.Economic], InfluenceCosts[(int)InfluenceBranches.Structural],
            Purchased);
    }
}
