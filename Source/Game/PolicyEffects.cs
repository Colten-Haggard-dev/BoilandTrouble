namespace Game;


static class PolicyEffects
{
    public static void CreateOrg(Simulation sim)
    {
        sim.MonthlyIncome += 50;
        sim.DailyIncome += 5;
    }

    public static void RaiseFunds(Simulation sim)
    {
        sim.MonthlyIncome += 50;
        sim.DailyIncome += 2;
    }

    public static void SocialMedia(Simulation sim)
    {
        sim.DailySocInc += 10;
        sim.Policies[(int)InfluenceBranches.Social].Tree[0].Cost -= 250;
        sim.Policies[(int)InfluenceBranches.Social].Tree[1].Cost -= 250;
    }

    public static void FundGreenEnergy(Simulation sim)
    {
        sim.Influence[(int)InfluenceBranches.Social] += 200;
        sim.Policies[(int)InfluenceBranches.Economic].Tree[1].Cost -= 1000;
        sim.Policies[(int)InfluenceBranches.Structural].Tree[0].Cost -= 2000;
        sim.Policies[(int)InfluenceBranches.Structural].Tree[1].Cost -= 1000;
        //sim.Policies[(int)InfluenceBranches.Social].Tree[0].Cost -= 10;
    }

    public static void DepriveOil(Simulation sim)
    {
        sim.Influence[(int)InfluenceBranches.Social] += 200;
        sim.Policies[(int)InfluenceBranches.Social].Tree[0].Cost -= 500;
        sim.DailyPollution -= 2;
        sim.DamagePollution -= 50;
    }

    public static void FundStartups(Simulation sim)
    {
        sim.Influence[(int)InfluenceBranches.Economic] += 200;
        sim.Influence[(int)InfluenceBranches.Structural] += 200;
        sim.Policies[(int)InfluenceBranches.Structural].Tree[0].Cost -= 1000;

    }

    public static void InstallSolarPanels(Simulation sim)
    {
        sim.DailyPollution -= 5;
    }

    public static void ResearchTerraforming(Simulation sim)
    {
        sim.PlanetHealth += 10;
    }
}
