using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using Penates.Database;
using Penates.Utils.Enums;

namespace Penates.Models.ViewModels.Inventory
{
    public class InventoryViewModel {
        
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha del inventario")]
        public DateTime? InventoryDate { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Nombre del inventario")]
        public string InventoryName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Stock inventeado")]
        public ICollection<Stock_Invented> Stock_Invented { get; set; }

        [Display(Name = "Centro de distribucion")]
        public DistributionCenter DistributionCenter { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Id del centro de distribucion")]
        public long IDDistributionCenter { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Codigo")]
        public string Code { get; set; }

        [Display(Name = "Tipo de metodo")]
        public PredifinedMethodsTypes? MethodType { get; set; } 
    }
}
