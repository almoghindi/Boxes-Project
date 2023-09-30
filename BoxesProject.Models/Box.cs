using BoxesProject.Conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxesProject.Models
{
    public class Box
    {
        private static Configurations _config = Configurations.Instance;

        private readonly double _width; // X dimension of the square bottom
        private readonly double _height; // Y dimension of the box
        private int MIN_BOXES = _config.Data.MIN_BOXES;   // CONFIG
        private int MAX_BOXES = _config.Data.MAX_BOXES;  // CONFIG
        public int Quantity { get; set; } // Number of boxes available
        private DateTime _purchaseDate; // Purchase date

        public Box(double width, double height, int quantity) 
        {
            _width = width;
            _height = height;
            Quantity = quantity;
            _purchaseDate = DateTime.Now;
        }
        public double Width { get { return _width; } }
        public double Height { get { return _height; } }
        public int MinBoxes { get { return MIN_BOXES; } }
        public int MaxBoxes { get { return MAX_BOXES; } }

        public DateTime PurchaseDate { get {  return _purchaseDate; } set { _purchaseDate = value; } }
        public override string ToString()
        {
            return "Width: "+Width + " Height: "+Height + " Quantity: " +Quantity;
        }
    }
}
