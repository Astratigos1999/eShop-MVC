using eShop.Models.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace eShop.Models.ViewModels
{
    public class AddEdit
    {
        public Part Part { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }
    }
}
