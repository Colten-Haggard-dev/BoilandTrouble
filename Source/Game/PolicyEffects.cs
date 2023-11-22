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
        sim.MonthlyIncome += 10;
        sim.DailyIncome += 1;
    }

    public static void SocialMedia(Simulation sim)
    {
        sim.DailySocInc += 10;
    }
}
