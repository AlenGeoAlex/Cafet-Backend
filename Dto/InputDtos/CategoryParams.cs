namespace Cafet_Backend.QueryParams;

public class CategoryParams
{
    public string CategoryName { get; set; }
    public string CategoryDescription { get; set; }
}

public class CategoryUpdateParams
{
    public int id { get; set; }
    public string categoryName { get; set; }
    public string categoryDescription { get; set; }
}