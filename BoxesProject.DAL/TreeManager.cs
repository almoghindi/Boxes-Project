using BoxesProject.Conf;
using BoxesProject.DAL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace BoxesProject.Models
{
    public class TreeManager
    {
        public static BaseTree root;


        //function that receive width, height and quantity and will insert box in the suitable place
        public static BaseTree InsertIntoBaseAndHeightTree(double widthVal, double heightVal, int quantity)
        {
            if (root == null || root.Val == 0)
            {
                root = new BaseTree(widthVal, new HeightTree(heightVal, new Box(widthVal, heightVal, quantity)));
            }
            else
            {
                root = root.Insert(widthVal);

                // Find the node in BaseTree where the widthVal was inserted
                BaseTree node = root.Search(widthVal);
                if (node != null)
                {
                    if (node.HeightTree == null)
                    {
                        node.HeightTree = new HeightTree(heightVal, new Box(widthVal, heightVal, quantity));
                    }
                    else
                    {
                        // Instead of calling Insert on the existing HeightTree, create a new HeightTree node
                        node.HeightTree.Insert(heightVal, new Box(widthVal, heightVal, quantity));
                    }
                }
            }
            LoadAndSaveJson.SaveData();
            return root;
        }


        //function that receive width, height and quantity and will return the most suitables boxes

        public static List<Box> FindMustSuitableBoxes(double width, double height, int quantity)
        {
            List<Box> suitableBoxes = new List<Box>();
            BaseTree widthTree = FindClosestBiggerValue(width, root);

            while (widthTree != null && suitableBoxes.Count < quantity)
            {
                HeightTree heightTree = FindClosestBiggerValue(height, widthTree.HeightTree);

                if (heightTree != null && heightTree.GiftBox.Quantity > 0)
                {
                    Box suitableBox = heightTree.GiftBox;
                    suitableBoxes.Add(suitableBox);
                    heightTree.UseBox(suitableBox.Height);

                    if (heightTree.GiftBox.Quantity == 0)
                    {
                        BaseTree widthNode = FindClosestBiggerValue(width, root);
                        if (widthNode != null)
                        {
                            widthNode.HeightTree = widthNode.HeightTree.Remove(heightTree.Val);
                            if (widthNode.HeightTree == null)
                            {
                                root = root.Remove(widthNode.Val);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No suitable width found.");
                        }
                    }
                }
                else
                {
                    // Find the next width tree with suitable height
                    widthTree = root.BiggerValues(widthTree.Val + 1)
                                     .FirstOrDefault(node => node.HeightTree != null && node.HeightTree.GiftBox.Quantity > 0);
                }
            }

            if (suitableBoxes.Count > 0)
            {
                List<Box> filteredBoxes = DisplayBoxesWithoutDuplicates(suitableBoxes);

                Console.Write("Do you want to use these boxes? (Y/N): ");
                string userResponse = Console.ReadLine();

                if (userResponse.Trim().ToUpper() == "Y")
                {
                    foreach (Box box in filteredBoxes)
                    {
                        if (box.Quantity < box.MinBoxes)
                            Console.WriteLine($"The Box: (Width: {box.Width}, Height:{box.Height}) Stock is low !");


                        if (box.Quantity == 0)
                            Console.WriteLine($"The Box: (Width: {box.Width}, Height:{box.Height}) is now out of stock !");

                    }
                    Console.WriteLine("Boxes have been used and removed.");
                }
                else
                {
                    foreach (Box box in suitableBoxes)
                    {
                        InsertIntoBaseAndHeightTree(box.Width, box.Height, 1);
                    }
                    Console.WriteLine("Boxes were not used.");
                }
            }

            else
            {
                Console.WriteLine("No suitable boxes found.");
            }
            LoadAndSaveJson.SaveData();

            return suitableBoxes;
        }




        //function that receive tree and value and eturn the leaf with the closet bigger value 
        private static BaseTree FindClosestBiggerValue(double value, BaseTree tree)
        {
            BaseTree closestBiggerNode = null;
            BaseTree currentNode = tree;

            while (currentNode != null)
            {
                if (currentNode.Val >= value)
                {
                    if (IsInDeviationRange(currentNode.Val, value))
                    {
                        closestBiggerNode = currentNode;
                    }
                    currentNode = currentNode.Left;
                }
                else
                {
                    currentNode = currentNode.Right;
                }
            }

            return closestBiggerNode;
        }

        //function that receive tree and value and eturn the leaf with the closet bigger value 
        private static HeightTree FindClosestBiggerValue(double height, HeightTree tree)
        {
            HeightTree closestBiggerNode = null;
            HeightTree currentNode = tree;

            while (currentNode != null)
            {
                if (currentNode.Val >= height && currentNode.GiftBox.Quantity > 0)
                {
                    if (IsInDeviationRange(currentNode.Val, height))
                        closestBiggerNode = currentNode;
                    currentNode = currentNode.Left;
                }
                else
                {
                    currentNode = currentNode.Right;
                }
            }

            return closestBiggerNode;
        }

        
        //function that received 2 values and return if in range
        private static bool IsInDeviationRange(double value, double originalValue)
        {
            Configurations _config = Configurations.Instance;
            double range = _config.Data.DEVIATION_PERCANTANGE / 100 + 1;
            return originalValue * range >= value && !(value < originalValue);
        }

        //function that receive list of boxes and return the list without duplicates
        public static List<Box> DisplayBoxesWithoutDuplicates(List<Box> boxes)
        {
            List<Box> filteredBoxes = new List<Box>();
            int amount = boxes.Count;
            int countSame = 1;
            for (int i = 0; i < boxes.Count - 1; i++)
            {
                if (boxes[i].Width == boxes[i + 1].Width && boxes[i].Height == boxes[i + 1].Height)
                {
                    countSame++;
                }
                else
                {
                    Console.WriteLine($"Width:{boxes[i].Width}, Height:{boxes[i].Height}, Quantity: {countSame}");
                    filteredBoxes.Add(boxes[i]);
                    amount -= countSame;
                    countSame = 1;
                }
            }
            if (amount > 0)
            {
                Console.WriteLine($"Width:{boxes[boxes.Count - 1].Width}, Height:{boxes[boxes.Count - 1].Height}, Quantity: {amount}");
                filteredBoxes.Add(boxes[boxes.Count - 1]);
            }

            return filteredBoxes;

        }

        //function that receive days until expiration and invoke the delete function, and save the changes
        public static void DeleteExpiredBoxes(int expirationTime)
        {
            DeleteExpiredBoxesFromTree(root, expirationTime);
            LoadAndSaveJson.SaveData();
        }

        //function that receive days until expiration and tree and with recursive loops on the width tree
        private static void DeleteExpiredBoxesFromTree(BaseTree tree, int expirationTime)
        {
            if (tree == null)
                return;

            tree.HeightTree = DeleteExpiredBoxesFromHeightTree(tree.HeightTree, expirationTime);

            DeleteExpiredBoxesFromTree(tree.Left, expirationTime);
            DeleteExpiredBoxesFromTree(tree.Right, expirationTime);
        }


        //function that receive days until expiration and tree and delete box that expired
        private static HeightTree DeleteExpiredBoxesFromHeightTree(HeightTree heightTree, int expirationTime)
        {
            if (heightTree == null)
                return null;

            if (heightTree.GiftBox.PurchaseDate.AddDays(expirationTime) < DateTime.Now)
            {
                Console.WriteLine($"Box (Width: {heightTree.Val}, Height: {heightTree.Height}) has expired and will be removed.");
                return DeleteExpiredBoxesFromHeightTree(heightTree.Right, expirationTime); // Continue searching in the right subtree
            }

            heightTree.Left = DeleteExpiredBoxesFromHeightTree(heightTree.Left, expirationTime);
            heightTree.Right = DeleteExpiredBoxesFromHeightTree(heightTree.Right, expirationTime);

            return heightTree;
        }

        //function that receive t time and print if not found
        public static void ShowBoxesNotPurchasedForMoreThan(int T)
        {
            bool found = ShowExpiredBoxesFromTree(root, T);
            if (!found)
            {
                Console.WriteLine("No boxes found that have not been purchased for more than the specified days.");
            }
        }

        // Function that receives days until expiration and tree and recursively loops on the width tree
        private static bool ShowExpiredBoxesFromTree(BaseTree tree, int T)
        {
            if (tree == null)
                return false;

            bool found = ShowExpiredBoxesFromHeightTree(tree.HeightTree, T);

            bool leftFound = ShowExpiredBoxesFromTree(tree.Left, T);
            bool rightFound = ShowExpiredBoxesFromTree(tree.Right, T);

            return found || leftFound || rightFound;
        }

        // Function that receives days until expiration and tree and shows if boxes haven't been bought for this time
        private static bool ShowExpiredBoxesFromHeightTree(HeightTree heightTree, int T)
        {
            if (heightTree == null)
                return false;

            bool found = false;

            if (heightTree.GiftBox.PurchaseDate.AddDays(T) < DateTime.Now)
            {
                Console.WriteLine($"Box (Width: {heightTree.GiftBox.Width}, Height: {heightTree.Val}) has not been purchased for more than {T} days.");
                found = true;
            }

            bool leftFound = ShowExpiredBoxesFromHeightTree(heightTree.Left, T);
            bool rightFound = ShowExpiredBoxesFromHeightTree(heightTree.Right, T);

            return found || leftFound || rightFound;
        }



        //functio that receive tree and return list with all the boxes
        public static List<Box> GetAllBoxes(BaseTree tree)
        {
            List<Box> allBoxes = new List<Box>();
            if (tree != null)
            {
                // Traverse the width dimension
                allBoxes.AddRange(GetAllBoxes(tree.Left)); // Traverse left subtree
                allBoxes.AddRange(GetAllBoxes(tree.Right)); // Traverse right subtree

                // Traverse the height dimension
                if (tree.HeightTree != null)
                {
                    allBoxes.AddRange(GetAllHeightBoxes(tree.HeightTree));
                }
            }
            return allBoxes;
        }


        //function that receive height tree and return the list of all the boxes inside
        private static List<Box> GetAllHeightBoxes(HeightTree heightTree)
        {
            List<Box> heightBoxes = new List<Box>();
            if (heightTree != null)
            {
                heightBoxes.Add(heightTree.GiftBox);
                heightBoxes.AddRange(GetAllHeightBoxes(heightTree.Left)); // Traverse left subtree
                heightBoxes.AddRange(GetAllHeightBoxes(heightTree.Right)); // Traverse right subtree
            }
            return heightBoxes;
        }


        //function that print all the boxes
        public static void ShowAllBoxes()
        {
            List<Box> allBoxes = GetAllBoxes(root);

            if (allBoxes.Count == 0)
            {
                Console.WriteLine("No boxes found.");
            }
            else
            {
                Console.WriteLine("List of all boxes:");
                foreach (Box box in allBoxes)
                {
                    Console.WriteLine($"Width: {box.Width}, Height: {box.Height}, Quantity: {box.Quantity}");
                }
            }
        }

    }


}
