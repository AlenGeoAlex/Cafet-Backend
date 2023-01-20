namespace Cafet_Backend.Dto;

public class DailyStockDto
{
    public int StockId { get; set; }
    public int FoodId { get; set; }
    public string FoodName { get; set; }
    public string FoodImage { get; set; }
    public string FoodCategory { get; set; }
    public long TotalInStock { get; set; }
    public long CurrentInStock { get; set; }

    public string FoodDescription { get; set; }
    public bool FoodType { get; set; }
    public double FoodPrice { get; set; }
}