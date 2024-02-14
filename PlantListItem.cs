namespace garden_planner;

public struct PlantListItem
{
    public Plant plant { get; set; }
    public bool good { get; set; }
    public bool bad { get; set; }
    public int amount { get; set; }
    public bool hasAmount { get; set; }
    
    public PlantListItem(Plant plant, bool good, bool bad, int amount, bool hasAmount)
    {
        this.plant = plant;
        this.good = good;
        this.bad = bad;
        this.amount = amount;
        this.hasAmount = hasAmount;
    }
}