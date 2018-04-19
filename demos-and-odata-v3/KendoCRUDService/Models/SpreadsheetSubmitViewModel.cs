using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class SpreadsheetSubmitViewModel
    {
        public IList<ProductModel> Created { get; set; }

        public IList<ProductModel> Destroyed { get; set; }

        public IList<ProductModel> Updated { get; set; }
    }
}