using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class FoodItem
    {
        
        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string Customise { get; set; }

        public FoodItem(string itemName, string itemDesc, double itemPrice, string customise = "")
        {
            ItemName = itemName;
            ItemDesc = itemDesc;
            ItemPrice = itemPrice;
            Customise = customise;
        }

        public override string ToString()
        {
            return $"{ItemName} - {ItemDesc} (${ItemPrice:0.00})" +
                   (string.IsNullOrWhiteSpace(Customise) ? "" : $" | Customise: {Customise}");
        }
    }
}
