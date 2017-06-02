using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Proyecto_21351029;

namespace Proyecto_21351029.ViewModels
{
    public class TutorialByRequestViewModel
    {
        public Tutorial Tutorial { get; set; }
        public DateTime RequestDate { get; set; }
        public string ClassCode { get; set; }
    }
}