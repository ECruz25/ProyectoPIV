//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Proyecto_21351029
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Request
    {
        public string request_code { get; set; }

        public string account_number { get; set; }

        [Required]
        [Display(Name ="Fecha")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public System.DateTime request_date { get; set; }

        public System.DateTime date_requested { get; set; }

        public string status { get; set; }

        [Required]
        [Display(Name = "Clase")]
        public string class_code { get; set; }
        
        [Required]
        [Display(Name = "Hora")]
        public Nullable<System.TimeSpan> request_time { get; set; }

        [Required]
        [Display(Name = "Hora2")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public Nullable<System.DateTime> hour { get; set; }
    }
}
