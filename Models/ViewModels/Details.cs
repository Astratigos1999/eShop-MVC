using eShop.Models.Database;

namespace eShop.Models.ViewModels
{
    public class Details
    {
        public Details()
        {
            Part = new Part();
        }
        public Part Part { get; set; }
    }
}
