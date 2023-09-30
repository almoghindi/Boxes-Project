using BoxesProject.Conf;
using BoxesProject.DAL;
using BoxesProject.Models;
using Spectre.Console;
using System;

namespace BoxesProject
{
    public class MainMenu
    {
        //function that show the menu
        public static void ShowMenu()
        {
            Configurations _config = Configurations.Instance;
            LoadAndSaveJson.LoadData();
            TreeManager.DeleteExpiredBoxes(_config.Data.EXPIRE_DAYS);

            string mainMenuChoice = "";

            while (mainMenuChoice != "Exit")
            {
                Console.Clear();
                AnsiConsole.Write(new FigletText("Welcome to Almog's Boxes!").Color(Color.Blue));
                Console.WriteLine("\n\n\n");
                mainMenuChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("What you [green]want to do[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {"Request box","Add Box","Show All Boxes","Check old boxes","Credits", "Settings", "Exit"
                    }));
                switch (mainMenuChoice)
                {
                    case "Request box":
                        Console.Clear();
                        AnsiConsole.Write(new FigletText("Request Box").Color(Color.Blue));
                        double findWidth = InputDouble("width");
                        double findHeight = InputDouble("height");
                        int findQuantity = InputInt("quantity");
                        TreeManager.FindMustSuitableBoxes(findWidth, findHeight, findQuantity);
                        break;

                    case "Add Box":
                        Console.Clear();
                        AnsiConsole.Write(new FigletText("Add Box").Color(Color.Blue));
                        double addWidth = InputDouble("width");
                        double addHeight = InputDouble("height");
                        int addQuantity = InputInt("quantity");
                        TreeManager.InsertIntoBaseAndHeightTree(addWidth, addHeight, addQuantity);
                        Console.WriteLine($"The Box: (Width: {addWidth}, Height:{addHeight}) added succesfully!");
                        break;

                    case "Show All Boxes":
                        TreeManager.ShowAllBoxes();
                        break;

                    case "Check old boxes":
                        Console.Clear();
                        AnsiConsole.Write(new FigletText("Check old boxes").Color(Color.Blue));
                        int timeToCheck = InputInt("time to check");
                        TreeManager.ShowBoxesNotPurchasedForMoreThan(timeToCheck);
                        break;

                    case "Settings":
                        Console.Clear();
                        AnsiConsole.Write(new FigletText("Settings").Color(Color.Blue));
                        Console.WriteLine(_config+"\n");
                        mainMenuChoice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                    .Title("What you [green]want to change[/]?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(new[] {"Max Boxes","Min Boxes","Expire Days","Deviation Percantage", "Back"
                    }));

                        switch (mainMenuChoice)
                        {
                            case "Max Boxes":
                                int maxBox = InputInt("New Max Boxes");
                                _config.Data.MAX_BOXES = maxBox;
                                _config.ChangeConfiguration();
                                Console.WriteLine($"Max Boxes changed successfully for {maxBox}");
                                break;

                            case "Min Boxes":
                                int minBox = InputInt("New Min Boxes");
                                _config.Data.MIN_BOXES = minBox;
                                _config.ChangeConfiguration();
                                Console.WriteLine($"Min Boxes changed successfully for {minBox}");
                                break;

                            case "Expire Days":
                                int expireDate = InputInt("New Expire Days");
                                _config.Data.EXPIRE_DAYS = expireDate;
                                _config.ChangeConfiguration();
                                Console.WriteLine($"Expire Days changed successfully for {expireDate}");
                                break;

                            case "Deviation Percantage":
                                double deviationPercantage = InputDouble("New Deviation Percantage");
                                _config.Data.DEVIATION_PERCANTANGE = deviationPercantage;
                                _config.ChangeConfiguration();
                                Console.WriteLine($"Deviation Percantage changed successfully for {deviationPercantage}");
                                break;

                            case "Back":
                                break;
                        }
                        break;

                    case "Credits":
                        AnsiConsole.Markup("[underline red]CREDITS[/] ");
                        AnsiConsole.MarkupLine("[bold]Credits to Almog Hindi[/]");
                        AnsiConsole.MarkupLine("[blue]Version ---------- 1.0[/]");
                        Console.WriteLine("\nPress any key to return to the menu");
                        break;

                    case "Exit":
                        return;
                }
                Console.ReadKey();
            }
        }

        //function that recive string and shows it on the screen and returns valid double
        public static double InputDouble(string title)
        {
            Console.WriteLine("Please enter " + title);
            string sValue = Console.ReadLine();
            bool isValid = double.TryParse(sValue, out double value);
            while (!isValid || value < 0)
            {
                Console.WriteLine("Please try again to enter " + title);
                sValue = Console.ReadLine();
                isValid = double.TryParse(sValue, out value);
            }
            return value;
        }

        //function that recive string and shows it on the screen and returns valid int
        public static int InputInt(string title)
        {
            Console.WriteLine("Please enter " + title);
            string sValue = Console.ReadLine();
            bool isValid = int.TryParse(sValue, out int value);
            while (!isValid || value < 0)
            {
                Console.WriteLine("Please try again to enter " + title);
                sValue = Console.ReadLine();
                isValid = int.TryParse(sValue, out value);
            }
            return value;
        }
    }
}
