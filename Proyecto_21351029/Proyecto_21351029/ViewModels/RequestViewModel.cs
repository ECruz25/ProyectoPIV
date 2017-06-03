using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proyecto_21351029.ViewModels
{
    public class RequestViewModel
    {
        public List<SelectListItem> class_codes { get; set; }
        public DateTime Date_Hour { get; set; }
        public int MyProperty { get; set; }
    }
}