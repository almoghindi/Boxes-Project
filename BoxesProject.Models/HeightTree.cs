using System;

namespace BoxesProject.Models
{
    public class HeightTree
    {
        public double Val { get; set; }
        public int Height { get; set; }
        public Box GiftBox { get; set; }
        public HeightTree Right { get; set; }
        public HeightTree Left { get; set; }

        public HeightTree(double val, Box box)
        {
            Val = val;
            GiftBox = box;
            Height = 1;
        }

        //function to rotate right the self balancing tree
        public HeightTree RotateRight()
        {
            HeightTree x = Left;
            HeightTree T2 = x.Right;

            x.Right = this;
            Left = T2;

            Height = Math.Max(GetHeight(Left), GetHeight(Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        //function to rotate left the self balancing tree
        public HeightTree RotateLeft()
        {
            HeightTree y = Right;
            HeightTree T2 = y.Left;

            y.Left = this;
            Right = T2;

            Height = Math.Max(GetHeight(Left), GetHeight(Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        //function to return the balance of the tree
        public int GetBalance()
        {
            if (this == null)
                return 0;
            return GetHeight(Left) - GetHeight(Right);
        }

        //function that receive a value and insert it in the tree
        public HeightTree Insert(double height, Box box)
        {
            if (this == null)
                return new HeightTree(height, box);

            if (height < Val)
            {
                if (Left == null)
                    Left = new HeightTree(height, box);
                else
                    Left.Insert(height, box);
            }
            else if (height > Val)
            {
                if (Right == null)
                    Right = new HeightTree(height, box);
                else
                    Right.Insert(height, box);
            }
            else // height matches
            {
                AddToBox(box);
            }

            Height = 1 + Math.Max(GetHeight(Left), GetHeight(Right));

            int balance = GetBalance();

            if (balance > 1 && height < Left.Val)
                return RotateRight();

            if (balance < -1 && height > Right.Val)
                return RotateLeft();

            if (balance > 1 && height > Left.Val)
            {
                Left = Left.RotateLeft();
                return RotateRight();
            }

            if (balance < -1 && height < Right.Val)
            {
                Right = Right.RotateRight();
                return RotateLeft();
            }

            return this;
        }

        //function that receive a value and remove it to the tree
        public HeightTree Remove(double height)
        {
            if (this == null)
                return this;

            if (height < Val)
                Left = Left?.Remove(height);
            else if (height > Val)
                Right = Right?.Remove(height);
            else
            {
                if (Left == null || Right == null)
                    return Left ?? Right;

                HeightTree temp = FindMinNode(Right);
                Val = temp.Val;
                Right = Right.Remove(temp.Val);
            }

            // Update height and balance
            Height = 1 + Math.Max(GetHeight(Left), GetHeight(Right));
            int balance = GetBalance();

            if (balance > 1 && GetBalance(Left) >= 0)
                return RotateRight();

            if (balance > 1 && GetBalance(Left) < 0)
            {
                Left = Left.RotateLeft();
                return RotateRight();
            }

            if (balance < -1 && GetBalance(Right) <= 0)
                return RotateLeft();

            if (balance < -1 && GetBalance(Right) > 0)
            {
                Right = Right.RotateRight();
                return RotateLeft();
            }

            return this;
        }

        //function that receive tree and return the minimum valuein the tree
        private HeightTree FindMinNode(HeightTree node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        //function to return the balance of the tree
        private int GetBalance(HeightTree node)
        {
            if (node == null)
                return 0;
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        //function that receive tree and return his height
        private int GetHeight(HeightTree node)
        {
            if (node == null)
                return 0;
            return node.Height;
        }


        //function that receive box and add it to the tree
        public void AddToBox(Box box)
        {
            int currentAmount = GiftBox.Quantity;
            if (currentAmount == GiftBox.MaxBoxes)
            {
                Console.WriteLine("You reach the maximum of boxes, we return the stock to the deliver!");
            }
            else if (box.Quantity + currentAmount <= GiftBox.MaxBoxes)
            {
                GiftBox.Quantity += box.Quantity;
                GiftBox.PurchaseDate = DateTime.Now;
            }
            else if (box.Quantity + currentAmount > box.MaxBoxes && currentAmount < box.MaxBoxes)
            {
                int availableAmount = GiftBox.MaxBoxes - currentAmount;
                int returnes = box.Quantity - availableAmount;
                Console.WriteLine($"{returnes} Boxes return to the deliver, you react the limit");
                if (GiftBox.Quantity != GiftBox.MaxBoxes)
                {
                    GiftBox.PurchaseDate = DateTime.Now;
                }
                GiftBox.Quantity = GiftBox.MaxBoxes;
            }
        }


        //function that receive tree and search a leaf with that value
        public HeightTree Search(double height)
        {
            HeightTree currentNode = this;

            while (currentNode != null && currentNode.Val != height)
            {
                if (height < currentNode.Val)
                    currentNode = currentNode.Left;
                else
                    currentNode = currentNode.Right;
            }

            return currentNode;
        }
   
        //function that receive height and use box
        public bool UseBox(double height)
        {
            if (GiftBox.Quantity > 0)
            {
                GiftBox.Quantity--;
                GiftBox.PurchaseDate = DateTime.Now;
                return true;
            }
            else
            {
                Console.WriteLine($"Box with Height {height} is out of stock.");
                return false;
            }
        }

    }
}


