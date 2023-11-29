using System;
using FlaxEngine;
using FlaxEngine.GUI;

namespace Game;

public enum InfluenceBranches
{
    Oganizational,
    Social,
    Economic,
    Structural
}

/// <summary>
/// Simulation Script.
/// </summary>
public class Simulation : Script
{
    public double TotalBalance { get; set; } = 100;
    public double MonthlyIncome { get; set; } = 0;
    public double DailyIncome { get; set; } = 0;
    public double DailySocInc { get; set; } = 0;
    public double DailyEcoInc { get; set; } = 0;
    public double DailyStructInc { get; set; } = 0;
    public uint[] Influence { get; set; } = new uint[4];
    public uint Day { get; private set; } = 0;
    public uint Month { get; private set; } = 0;
    public uint TotalDays { get; private set; } = 0;
    public float PlanetHealth { get; set; } = 50;
    public float LifetimeTotalPolution { get; set; } = 0;
    public float Pollution { get; set; } = 0;
    public float DailyPollution { get; set; } = 10;
    public float DamagePollution { get; set; } = 100;
    public float SpeedControl { get; set; } = 1;
    public UICanvas UI { get; set; }
    public UIControl[] Screens { get; set; } = new UIControl[4];
    public UIControl[] Labels { get; set; }
    public EmptyActor ScreenParent { get; set; }

    public readonly PolicyTree[] Policies = new PolicyTree[4];

    private float DayTimer = 0;
    private bool Rotate = false;
    private readonly Float3[] AngleIdentities = new Float3[] { Float3.Forward, Float3.Left, Float3.Backward, Float3.Right };
    private int CurrentId = 0;
    //Quaternion NewRot = Quaternion.Identity;

    public Simulation()
    {
        Influence[(int)InfluenceBranches.Oganizational] = 0;
        Influence[(int)InfluenceBranches.Social] = 0;
        Influence[(int)InfluenceBranches.Economic] = 0;
        Influence[(int)InfluenceBranches.Structural] = 0;
    }

    public override void OnEnable()
    {
        PolicyTree org_pol = new(Screens[(int)InfluenceBranches.Oganizational]);
        org_pol.AddPolicy(new("Start Organization", 3, 5, 100, 0, 0, 0, this, PolicyEffects.CreateOrg));
        org_pol.AddPolicy(new("Raise Funds", 2, 4, 200, 0, 0, 0, this, PolicyEffects.RaiseFunds, new Policy[] { org_pol.Tree[0] }));
        org_pol.AddPolicy(new("Social Media", 4, 4, 200, 0, 0, 0, this, PolicyEffects.SocialMedia, new Policy[] { org_pol.Tree[0] }));
        org_pol.EnablePolicy(org_pol.Tree[0]);
        Policies[(int)InfluenceBranches.Oganizational] = org_pol;

        PolicyTree gov_pol = new(Screens[(int)InfluenceBranches.Social]);
        gov_pol.AddPolicy(new("Subsidize\nGreen Energy", 2, 5, 1000, 0, 0, 0, this, PolicyEffects.FundGreenEnergy));
        gov_pol.AddPolicy(new("Lobby Against\nOil Subsidies", 4, 5, 1500, 0, 0, 0, this, PolicyEffects.DepriveOil));
        gov_pol.EnablePolicy(gov_pol.Tree[0]);
        gov_pol.EnablePolicy(gov_pol.Tree[1]);
        Policies[(int)InfluenceBranches.Social] = gov_pol;

        PolicyTree eco_pol = new(Screens[(int)InfluenceBranches.Economic]);
        eco_pol.AddPolicy(new("Advertisement\nCampaign", 2, 5, 2000, 0, 0, 0, this, null));
        eco_pol.AddPolicy(new("Fund Startups", 4, 5, 2500, 0, 0, 0, this, PolicyEffects.FundStartups));
        eco_pol.EnablePolicy(eco_pol.Tree[0]);
        eco_pol.EnablePolicy(eco_pol.Tree[1]);
        Policies[(int)InfluenceBranches.Economic] = gov_pol;

        PolicyTree struct_pol = new(Screens[(int)InfluenceBranches.Structural]);
        struct_pol.AddPolicy(new("Install\nSolar Panels", 2, 5, 5000, 0, 0, 0, this, PolicyEffects.InstallSolarPanels));
        struct_pol.AddPolicy(new("Research\nTerraforming", 4, 5, 5500, 0, 0, 0, this, PolicyEffects.ResearchTerraforming));
        struct_pol.EnablePolicy(struct_pol.Tree[0]);
        struct_pol.EnablePolicy(struct_pol.Tree[1]);
        Policies[(int)InfluenceBranches.Structural] = struct_pol;
    }

    public override void OnFixedUpdate()
    {
        if (Time.GameTime - DayTimer > SpeedControl)
        {
            Day += 1;
            TotalBalance += DailyIncome;
            DayTimer = Time.GameTime;
            Pollution += DailyPollution;
            LifetimeTotalPolution += DailyPollution;

            if (Pollution == DamagePollution)
            {
                Pollution = 0;
                PlanetHealth--;
            }
        }

        if (Day >= 30)
        {
            TotalBalance += MonthlyIncome;
            Month += 1;
            TotalDays += Day;
            Day = 0;
        }

        SpeedControl = Input.InputText.Length > 0 && Input.InputText[0] != '0' && char.IsNumber(Input.InputText[0]) ? 1f / float.Parse(Input.InputText) : SpeedControl;

        if (Input.InputText.Length > 0 && Input.InputText[0] == '0')
            SpeedControl = float.NaN;

        if (Input.GetAction("RotateLeft") && !Rotate)
        {
            Rotate = true;

            if (CurrentId - 1 < 0)
            {
                CurrentId = 3;
            }
            else
            {
                CurrentId -= 1;
            }
        }

        if (Input.GetAction("RotateRight") && !Rotate)
        {
            Rotate = true;

            if (CurrentId + 1 > 3)
            {
                CurrentId = 0;
            }
            else
            {
                CurrentId += 1;
            }
        }

        if (Rotate)
        {
            ScreenParent.Direction = Float3.Lerp(ScreenParent.Direction, AngleIdentities[CurrentId], 5 * Time.DeltaTime);

            Rotate = Float3.Distance(AngleIdentities[CurrentId], ScreenParent.Direction) >= 0.01f;

            if (!Rotate)
                ScreenParent.Direction = AngleIdentities[CurrentId];
        }

        Display();
    }

    public void Display()
    {
        ((Label)Labels[0].Control).Text = string.Format("Daily Income:\n${0}", DailyIncome);
        ((Label)Labels[1].Control).Text = string.Format("Monthly Income:\n${0}", MonthlyIncome);
        ((Label)Labels[2].Control).Text = string.Format("Balance:\n${0}", TotalBalance);
        ((Label)Labels[3].Control).Text = string.Format("Day:\n{0}", Day);
        ((Label)Labels[4].Control).Text = string.Format("Month:\n{0}", Month);
        ((Label)Labels[4].Control).Text = string.Format("Year:\n{0}", DateTime.Now.Year);
        ((Label)Labels[5].Control).Text = string.Format("Speed:\n{0}x", 1f/SpeedControl);
        ((Label)Labels[6].Control).Text = string.Format("Planet Health:\n{0}%", PlanetHealth);
        ((Label)Labels[7].Control).Text = string.Format("Pollution:\n{0}u", LifetimeTotalPolution);
    }
}
