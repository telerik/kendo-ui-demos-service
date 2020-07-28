using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KendoCRUDService.Models
{
    public class DetailProductSubmitViewModel
    {
        public IList<DetailProductModel> Created { get; set; }

        public IList<DetailProductModel> Destroyed { get; set; }

        public IList<DetailProductModel> Updated { get; set; }
    }
}