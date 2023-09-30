using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxesProject.Models
{
    public class BaseTree
    {
        public double Val { get; set; }
        public int Height { get; set; }
        public HeightTree HeightTree { get; set; }
        public BaseTree Left { get; set; }
        public BaseTree Right { get; set; }

        public BaseTree(double val, HeightTree heightTree)
        {
            Val = val;
            HeightTree = heightTree;
            Height = 1;
        }


        //function that receive tree and return his height
        private int GetHeight(BaseTree node)
        {
            if (node == null)
                return 0;
            return node.Height;
        }


        //function to rotate right the self balancing tree
        public BaseTree RotateRight()
        {
            BaseTree x = Left;
            BaseTree T2 = x.Right;

            x.Right = this;
            Left = T2;

            Height = Math.Max(GetHeight(Left), GetHeight(Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        //function to rotate left the self balancing tree
        public BaseTree RotateLeft()
        {
            BaseTree y = Right;
            BaseTree T2 = y.Left;

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
        public BaseTree Insert(double val)
        {
            if (this == null)
                return new BaseTree(val, null);

            if (val < Val)
            {
                if (Left == null)
                    Left = new BaseTree(val, null);
                else
                    Left = Left.Insert(val);
            }
            else if (val > Val)
            {
                if (Right == null)
                    Right = new BaseTree(val, null);
                else
                    Right = Right.Insert(val);
            }
           

            Height = 1 + Math.Max(GetHeight(Left), GetHeight(Right));

            int balance = GetBalance();

            if (balance > 1 && val < Left.Val)
                return RotateRight();

            if (balance < -1 && val > Right.Val)
                return RotateLeft();

            if (balance > 1 && val > Left.Val)
            {
                Left = Left.RotateLeft();
                return RotateRight();
            }

            if (balance < -1 && val < Right.Val)
            {
                Right = Right.RotateRight();
                return RotateLeft();
            }

            return this;
        }

        //function that receive a value and remove it to the tree
        public BaseTree Remove(double val)
        {
            if (this == null)
                return this;

            if (val < Val)
                Left = Left?.Remove(val);
            else if (val > Val)
                Right = Right?.Remove(val);
            else
            {
                if (Left == null || Right == null)
                    return Left ?? Right;

                BaseTree temp = FindMinNode(Right);
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

        //function to return the balance of the tree
        private int GetBalance(BaseTree node)
        {
            if (node == null)
                return 0;
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        //function that receive tree and return the minimum valuein the tree
        private BaseTree FindMinNode(BaseTree node)
        {
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        //function that receive tree and search a leaf with that value
        public BaseTree Search(double val)
        {
            if (this == null || Val == val)
            {
                return this;
            }

            if (val < Val)
            {
                return Left?.Search(val);
            }
            else
            {
                return Right?.Search(val);
            }
        }

        //function that receive value and return enumrable of the result
        public IEnumerable<BaseTree> BiggerValues(double value)
        {
            List<BaseTree> nodes = new List<BaseTree>();
            BiggerValuesRecursive(this, value, nodes);
            return nodes;
        }

        //function that receive list, tree and value and add the all biggest values
        private void BiggerValuesRecursive(BaseTree node, double value, List<BaseTree> nodes)
        {
            if (node == null)
                return;

            if (node.Val >= value)
            {
                BiggerValuesRecursive(node.Left, value, nodes);
                nodes.Add(node);
                BiggerValuesRecursive(node.Right, value, nodes);
            }
            else
            {
                BiggerValuesRecursive(node.Right, value, nodes);
            }
        }

    }
}
