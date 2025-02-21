using A1QSystem.Core;
using A1QSystem.DB;
using A1QSystem.Model.Categories;
using A1QSystem.Model.Products;
using A1QSystem.Model.Products.Rolls;
using A1QSystem.Model.Products.Tiles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model
{
    public class Product : ViewModelBase
    {
        public int ProductID { get; set; }
        public Category Category { get; set; }
        public ProductType ProductType { get; set; }
        public RawProduct RawProduct { get; set; }
        public string CommodityCode { get; set; }
        public string Type { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }  
        public int Size { get; set; }
        public string ProductUnit { get; set; }
        public decimal MaterialCost { get; set; }
        public decimal UnitsPerPack { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal MinimumOrderQty { get; set; }
        public decimal OrderInMultiplesOf { get; set; }
        public string Density { get; set; }
        public decimal Width { get; set; }      
        public decimal MinCutLength { get; set; }
        public decimal SafetyStockQty { get; set; }
        public decimal OrderPoint { get; set; }
        public string MouldType { get; set; }
        public bool IsCustomReRoll { get; set; }
        public string LogoPath { get; set; }
        public string QRVideoPath { get; set; }
        public string QRPDFPath { get; set; }
        public bool Active { get; set; }
        public bool IsManufactured { get; set; }
        public bool IsPurchased { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsRawMaterial { get; set; }
        public bool IsAutoOrder { get; set; }
        public Tile Tile { get; set; }
        public BulkRoll BulkRoll { get; set; }
        public Roll Roll { get; set; }

        private string _lastModifiedBy;
        public string LastModifiedBy
        {
            get
            {
                return _lastModifiedBy;
            }
            set
            {
                _lastModifiedBy = value;
                RaisePropertyChanged(() => this.LastModifiedBy);
            }
        }
    }
}
