using System.Linq.Expressions;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Transactions;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace KioskProject
{
    internal class KioskProject
    {
        //declares a called called Kiosk and its variables
        public class Kiosk
        {
            //FIELDS
            private DateTime _now;
            private string _cardVendor;
            private decimal _cashPayment;
            private decimal _cardPayment;
            private decimal _changeGiven;
            private int[] _currencyAmount;
            private string[] _currencyName;
            private decimal[] _currencyValue;

            //CONSTRUCTOR
            public Kiosk()
            {
                _currencyAmount = new int[12];
                _currencyName = new string[] {"Hundreds", "Fifties", "Twenties", "Tens", "Fives", "Twos",
                                              "Ones", "Half-Dollar", "Quarters", "Dimes", "Nickels", "Pennies"};
                _currencyValue = new decimal[] { 100, 50, 20, 10, 5, 2, 1, .50M, .25M, .10M, .05M, .01M };
                _now = DateTime.Now;
                _cardVendor = "0";
                _cardPayment = 0;
                _changeGiven = 0;
            }
            //END CONSTRUCTOR

            //METHODS
            //sets all currency types to have an amount of 5
            public void InitializeKioskInventory()
            {
                //repeat the length of int array currencyAmount
                for (int i = 0; i < _currencyAmount.Length; i++)
                {
                    _currencyAmount[i] = 5;
                }
            }

            //runs the processes for card payments
            public void CardTransaction(decimal totalCost, ref Kiosk kiosk)
            {
                //declares variables
                long longUserInput;
                string userInput;
                string cardNumber;
                int digitCount = 0;
                int userDigitDoubled;
                int runningTotal = 0;
                decimal cashBack = 0;
                bool validCard = false;
                bool repeat = false;
                bool changePaymentMethod = false;

                //will repeat while the user wants to change payment method
                do
                {
                    //resets changePaymentMethod to false to loop properly
                    changePaymentMethod = false;

                    //will repeat while the card is not a valid value
                    do
                    {
                        
                        digitCount = 0;

                        Console.Clear();

                        //prompts the user to enter their card number and stores the value
                        longUserInput = GetLong("Please enter your card number: ");

                        Console.Clear();

                        //counts how many digits the user's input is
                        for (long i = longUserInput; i > 0; i = i / 10)
                        {
                            digitCount++;
                        }

                        //converts user's integer input into a string value
                        userInput = longUserInput.ToString();

                        //stores the input for card number for future use
                        cardNumber = userInput;

                        //sets the length of an integer array called userDigitsArray to the length of the userInput string value
                        int[] userDigitsArray = new int[userInput.Length];

                        //assigns each index of userDigitsArray to a character in userInput
                        for (int i = 0; i < userInput.Length; i++)
                        {
                            userDigitsArray[i] = int.Parse(userInput[i].ToString());
                        }

                        //runs if the number has an even amount of digits
                        if (digitCount % 2 == 0)
                        {
                            //multiplies every other digit starting from the left most digit by 2
                            for (int i = 0; i < userDigitsArray.Length; i += 2)
                            {
                                userDigitDoubled = userDigitsArray[i] * 2;

                                //checks if the number at the current iteration multiplied by two more than one digit
                                if (userDigitDoubled >= 10)
                                {
                                    //add the remainder of the number divided by 10, plus one to the running total
                                    runningTotal += (1 + (userDigitDoubled % 10));
                                }
                                //adds the number multiplied by two to the running total if it is a single digit number
                                else if (userDigitDoubled < 10)
                                {
                                    runningTotal += userDigitDoubled;
                                }
                            }

                            //adds the numbers between the digits just multiplied to an overall total
                            for (int i = 1; i < userDigitsArray.Length; i += 2)
                            {
                                runningTotal += userDigitsArray[i];
                            }
                        }
                        //runs if the number has an odd amount of digit
                        else if (digitCount % 2 != 0)
                        {
                            //multiplies every other digit start from the second most right digit by 2
                            for (int i = userDigitsArray.Length - 2; i >= 0; i -= 2)
                            {
                                userDigitDoubled = userDigitsArray[i] * 2;

                                //checks if the number at the current iteration multiplied by two more than one digit
                                if (userDigitDoubled >= 10)
                                {
                                    //add the remainder of the number divided by 10, plus one to the running total
                                    runningTotal += (1 + (userDigitDoubled % 10));
                                }
                                //adds the number multiplied by two to the running total if it is a single digit number
                                else if (userDigitDoubled < 10)
                                {
                                    runningTotal += userDigitDoubled;
                                }
                            }

                            //adds the numbers between the digits just multiplied to an overall total
                            for (int i = userDigitsArray.Length - 1; i >= 0; i -= 2)
                            {
                                runningTotal += userDigitsArray[i];
                            }
                        }

                        //if the total is divisible by 10, then the card is determined to be valid
                        if (runningTotal % 10 == 0)
                        {
                            validCard = true;
                        }
                        //otherwise if the total is not divisible by 10, then the card is not valid
                        else if (runningTotal % 10 != 0)
                        {
                            Console.WriteLine("Error: Card number is invalid. Please re-enter your card number.");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();

                            validCard = false;

                            //resets the running total and digit count so the loop works properly
                            runningTotal = 0;
                            digitCount = 0;
                        }
                    } while (validCard == false);

                    //sets a series of variables at specific digits of what the user has input to determine the card vendor
                    int sixDigitUserInput = int.Parse(longUserInput.ToString().Substring(0, Math.Min(6, longUserInput.ToString().Length)));
                    int fourDigitUserInput = int.Parse(longUserInput.ToString().Substring(0, Math.Min(4, longUserInput.ToString().Length)));
                    int threeDigitUserInput = int.Parse(longUserInput.ToString().Substring(0, Math.Min(3, longUserInput.ToString().Length)));
                    int twoDigitUserInput = int.Parse(longUserInput.ToString().Substring(0, Math.Min(2, longUserInput.ToString().Length)));
                    int oneDigitUserInput = int.Parse(longUserInput.ToString().Substring(0, Math.Min(1, longUserInput.ToString().Length)));

                    Console.Clear();

                    switch (longUserInput)
                    {
                        //checks if the digits are within the parameters for a discover card
                        case var expression when (sixDigitUserInput >= 622126 && sixDigitUserInput <= 622925 || threeDigitUserInput >= 644 && threeDigitUserInput <= 649 || fourDigitUserInput == 6011 || twoDigitUserInput == 65 && digitCount == 16):
                            Console.WriteLine("Card accepted: your card is a Discover card.");
                            _cardVendor = "Discover";
                            break;
                        //checks if the digits are within the parameters for a MasterCard
                        case var expression when (twoDigitUserInput >= 51 && twoDigitUserInput <= 55 && digitCount == 16):
                            Console.WriteLine("Card accepted: your card is a MasterCard.");
                            _cardVendor = "MasterCard";
                            break;
                        //checks if the digits are within the parameters for an American card
                        case var expression when (twoDigitUserInput >= 34 && twoDigitUserInput <= 37 && digitCount == 15):
                            Console.WriteLine("Card accepted: your card is an American Express card.");
                            _cardVendor = "AmericanExpress";
                            break;
                        //checks if the digits are within the parameters for a Discover card
                        case var expression when (oneDigitUserInput == 4 && digitCount >= 13 && digitCount <= 16):
                            Console.WriteLine("Card accepted: your card is a Visa.");
                            _cardVendor = "Visa";
                            break;
                    }

                    //prompts the user on if they would like cash back and stores the value
                    userInput = GetString("Would you like cash back (yes/no)?: ");

                    Console.Clear();

                    //repeats while boolean repeat equals true
                    do
                    {
                        //checks for valid input from the user
                        switch (userInput)
                        {
                            case var expression when (userInput.ToLower() == "yes"):
                                cashBack = GetDecimal("Please enter the amount you want on cash back: $");
                                Console.Clear();
                                repeat = false;
                                break;
                            case var expression when (userInput.ToLower() == "no"):
                                repeat = false;
                                break;
                            default:
                                userInput = GetString("Please type a valid response. Do you want cash back (yes/no): ");
                                Console.Clear();
                                repeat = true;
                                break;
                        }
                    } while (repeat == true);

                    //transitions to the payment processing phase
                    Console.WriteLine("Processing payment...");
                    Console.ReadKey();
                    Console.Clear();

                    //sets a string variable to a function money request which simulates a card potentially declining
                    string[] transactionResult = MoneyRequest(cardNumber, cashBack);

                    //sets each index of the new array to separate variables
                    string accountNumberResult = transactionResult[0];
                    string transactionResultMessage = transactionResult[1];

                    //executes if the value at index 1 of transactionResult is declined case-insensitive
                    if (transactionResult[1].ToLower() == "declined")
                    {
                        Console.WriteLine("Card declined.");
                        Console.WriteLine("Remaining Balance: ");
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("${0:F2}", totalCost);
                        Console.WriteLine();
                        Console.WriteLine("Would you like to try a different card, pay the remaining in cash, or cancel the order? \n" +
                                          "Please enter 'card', 'cash', or 'cancel' to proceed.");

                        userInput = Console.ReadLine();

                        Console.Clear();

                        //safety sets repeat variable to false so do while loop below functions properly
                        repeat = false;

                        //will repeat while boolean repeat is true
                        do
                        {
                            repeat = false;

                            //verifies that user input is valid
                            switch (userInput)
                            {
                                //will break out of the loop so that the code will execute back to where the user was prompted to enter their card number
                                case var expression when (userInput.ToLower() == "card"):
                                    repeat = false;
                                    changePaymentMethod = true;
                                    break;
                                //will change payment method and call the CashPayment function for processes
                                case var expression when (userInput.ToLower() == "cash"):
                                    Console.Clear();
                                    repeat = false;
                                    CashTransaction(totalCost, ref kiosk);
                                    return;
                                //will exit the program
                                case var expression when (userInput.ToLower() == "cancel"):
                                    Console.Clear();
                                    System.Environment.Exit(-1);
                                    break;
                                //defaults if input was not valid
                                default:
                                    userInput = GetString("Please enter a valid input (card/cash/cancel): ");
                                    repeat = true;
                                    Console.Clear();
                                    break;
                            }
                        } while (repeat == true);
                    }

                    //executes if the user's card payment is accepted
                    else
                    {
                        //declare variables
                        decimal subTotal;
                        decimal actualCashBack;
                        bool changeDuePossible;

                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        
                        //sets a boolean variable to true or false depending on if the kiosk can credit change due to the customer
                        changeDuePossible = DispenseChange(cashBack, ref kiosk);

                        //sets cashBack and _changeGiven back to zero if crediting change due is not possible
                        if (changeDuePossible == false)
                        {
                            cashBack = 0;
                            _changeGiven = 0;
                        }

                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("Account Number: " + accountNumberResult);
                        Console.WriteLine("Total: +${0:F2}", totalCost);
                        Console.WriteLine("Cash Back Credited: +${0:F2}", cashBack);
                        subTotal = totalCost + cashBack;
                        actualCashBack = decimal.Parse(transactionResultMessage);

                        //sets actualCashBack back to zero if crediting change due is not possible
                        if (changeDuePossible == false)
                        {
                            actualCashBack = 0;
                        }

                        Console.WriteLine("Cash Back Received from Bank: -${0:F2}", actualCashBack);
                        subTotal -= actualCashBack;
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("Subtotal: ${0:F2}", subTotal);

                        //sets the card payment to the subtotal for logging and cash payment to 0 since transaction was paid with a card
                        _cardPayment = subTotal;
                        _cashPayment = 0;

                        Console.ReadKey();
                        Console.Clear();

                        changePaymentMethod = false;
                    }
                } while (changePaymentMethod == true);
            }

            //will dispense the change using a greedy algorithm, where the bills/coins dispensed are from the highest to lowest possible value
            private bool DispenseChange(decimal changeDue, ref Kiosk kiosk)
            {
                string userInput = "";
                bool repeat = false;
                bool changeDuePossible = false;

                //rounds the change due so the statements will run correctly
                decimal changeDueRounded = Math.Round(changeDue, 2);

                //repeats for each element of the currencyValue array
                for (int i = 0; i < _currencyValue.Length; i++)
                {
                    //executes if change due rounded is divisible by the current currency value
                    if ((changeDueRounded / _currencyValue[i]) >= 1)
                    {
                        switch (_currencyAmount[i])
                        {
                            //executes if the currencyAmount at current index is greater than zero
                            case var expression when (_currencyAmount[i] > 0):

                                //loops while changeDueRounded is greater than zero
                                //AND changeDueRounded divided by current index of currencyValue is still greater than or equal to 1
                                //AND there are still bills/coins that are distributable within the kiosk
                                while (changeDueRounded > 0 && (changeDueRounded / _currencyValue[i]) >= 1 && _currencyAmount[i] > 0)
                                {
                                    //executes if changeDueRounded minus current index of currencyValue is positive or zero
                                    if ((changeDueRounded - _currencyValue[i]) >= 0)
                                    {
                                        //subtracts current index of currencyValue from changeDue outputs that the current index of currencyValue was dispensed
                                        changeDue -= _currencyValue[i];
                                        Console.WriteLine("${0:F2} dispensed.", _currencyValue[i]);
                                        //add to a running to for change given to the customer
                                        kiosk._changeGiven += _currencyValue[i];
                                        //deducts the bill/coin taken from kiosk to give customer their change
                                        _currencyAmount[i]--;
                                    }

                                    //rounds change again after new calculations have been made
                                    changeDueRounded = Math.Round(changeDue, 2);
                                }
                                break;
                        }
                    }
                }

                //checks if there is still change due to the customer after all money from the kiosk is exhausted
                if (changeDueRounded > 0)
                {
                    changeDuePossible = false;

                    //redeposits the cash dispensed for change back to the kiosk since full cash back compensation is not possible
                    InitializeKioskInventory();

                    Console.Clear();
                    Console.WriteLine("Error: the kiosk does not have enough physical money to supply your change.");

                    userInput = GetString("Would you like to proceed with transaction without receiving cash back? (yes/no): ");

                    Console.Clear();

                    //repeats while boolean repeat equals true
                    do
                    {
                        //checks for valid input from the user
                        switch (userInput)
                        {
                            case var expression when (userInput.ToLower() == "yes"):
                                changeDue = 0;
                                return changeDuePossible;
                            case var expression when (userInput.ToLower() == "no"):
                                Console.Clear();
                                Console.WriteLine("Cancelling transaction...");
                                Environment.Exit(-1);
                                repeat = false;
                                break;
                            default:
                                userInput = GetString("Please type a valid response. Would you like to proceed with transaction without receiving cash back? (yes/no): ");
                                Console.Clear();
                                repeat = true;
                                break;
                        }
                    } while (repeat == true);
                }
                else
                {
                    changeDuePossible = true;
                }

                return changeDuePossible;
            }

            //gets a series of payments in bills/coins to pay the total expense on items
            public void CashTransaction(decimal totalCost, ref Kiosk kiosk)
            {
                //declare variables
                decimal totalRemaining = totalCost;
                decimal userPayment;
                decimal changeDue;
                int count = 1;
                int arrayIteration = 0;
                bool validCurrencyValue = false;
                bool changeDuePossible;

                //sets the payment in cash to totalCost and sets card payment to zero since transaction is in cash only
                kiosk._cashPayment = totalCost;
                kiosk._cardPayment = 0;

                Console.WriteLine("Enter your payments for your total: ");
                Console.WriteLine();

                //will get user input while there is still total remaining to pay
                do
                {
                    Console.Clear();
                    Console.WriteLine("Remaining: ${0:F2}", totalRemaining);

                    //gets a payment amount from the user
                    userPayment = GetDecimal("Payment " + count + ": $");

                    //runs while boolean validCurrencyValue is false. validates that the value inputted is a valid currency value and is of double value type
                    do
                    {
                        validCurrencyValue = false;

                        for (int i = 0; i < _currencyValue.Length; i++)
                        {
                            if (userPayment == _currencyValue[i])
                            {
                                validCurrencyValue = true;

                                totalRemaining -= userPayment;

                                if (totalRemaining > 0)
                                {
                                    Console.WriteLine("Remaining: ${0:F2}", totalRemaining);
                                }

                                arrayIteration++;
                                count++;
                            }
                        }

                        if (validCurrencyValue == false)
                        {
                            Console.Clear();
                            Console.WriteLine("Please enter a valid currency value.");
                            Console.WriteLine("Remaining: ${0:F2}", totalRemaining);

                            userPayment = GetDecimal("Payment " + count + ": $");
                        }

                    } while (validCurrencyValue == false);

                } while (totalRemaining > 0);

                //flips change due to positive for displaying on console and further use in the program
                changeDue = (totalRemaining * -1);

                //outputs change due to the console
                Console.WriteLine();
                Console.WriteLine("Change Due: ${0:F2}", changeDue);
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                //calls a method to dispense change owed to the user
                changeDuePossible = DispenseChange(changeDue, ref kiosk);

                //outputs to the console if the kiosk was not able to dispense the change due
                if (changeDuePossible == false)
                {
                    Console.WriteLine("No bills were dispensed. Proceeding with transaction...");
                    Console.ReadKey();
                    Console.Clear();

                    _changeGiven = 0;
                }
                else
                {
                    Console.ReadKey();
                    Console.Clear();
                }
            }

            //simulates a request for funds from a banking account
            static string[] MoneyRequest(string account_number, decimal amount)
            {
                Random rnd = new Random();
                //50% CHANCE TRANSACTION PASSES OR FAILS
                bool pass = rnd.Next(100) < 50;

                //50% CHANCE THAT A FAILED TRANSACTION IS DECLINED
                bool declined = rnd.Next(100) < 50;

                //****TESTING FOR CARD DECLINES. PLEASE COMMENT OUT WHEN MAKING COMMITS****
                /*
                pass = false;
                declined = true;
                */

                if (pass)
                {
                    return new string[] { account_number, amount.ToString() };
                }
                else
                {
                    if (!declined)
                    {
                        return new string[] { account_number, (amount / rnd.Next(2, 6)).ToString() };
                    }
                    else
                    {
                        return new string[] { account_number, "declined" };
                    }//end if

                }//end if

            }//end if

            //outputs a post transaction kiosk report to the console
            public void KioskInventoryReport()
            {
                //prints out a post transaction kio inventory report
                Console.WriteLine("Post Transaction Kiosk Inventory Report: ");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine();

                //loops the length of the currencyAmount array and outputs the amount of money in the kiosk
                for (int i = 0; i < _currencyAmount.Length; i++)
                {
                    Console.WriteLine("Amount of " + _currencyName[i] + " in kiosk: " + _currencyAmount[i]);
                }

                Console.ReadKey();
                Console.Clear();
            }

            //calls a separate program to log the transaction made by the customer
            public void LogTransaction()
            {
                //below process assigns a series of variables to multiple arguments in string format to send to program "LogTransaction" to create/update a .log file for every transaction made at the kiosk

                //transaction number
                string arg1 = "1";
                //transaction date
                string arg2 = _now.ToShortDateString();
                //transaction time
                string arg3 = _now.ToShortTimeString();
                //payment amount (cash)
                string arg4 = _cashPayment.ToString();
                //card vendor
                string arg5 = _cardVendor.ToString();
                //payment amount (card)
                string arg6 = _cardPayment.ToString();
                //change given
                string arg7 = _changeGiven.ToString();

                //creates and starts a process to run the LogTransaction program with arguments
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Users\Caleb\source\repos\KioskProject\LogTransaction\bin\Debug\net8.0\LogTransaction.exe";
                startInfo.Arguments = $"{arg1} \"{arg2}\" \"{arg3}\" {arg4} {arg5} {arg6} {arg7}";
                Process.Start(startInfo);
            }

            //receives user input of string type variables
            public string GetString (string dataRequest)
            {
                string userInput = "";

                Console.Write(dataRequest);

                userInput = Console.ReadLine();

                return userInput;
            }

            //validates user input of double type variables
            public decimal GetDecimal(string dataRequest)
            {
                string userInput = "";
                bool tester = true;

                Console.Write(dataRequest);

                do
                {
                    if (!tester)
                    {
                        Console.WriteLine("Input must be a nonnegative decimal number. Try Again.");
                    }

                    userInput = Console.ReadLine();

                    tester = decimal.TryParse(userInput, out _);

                    if (tester)
                    {
                        if (decimal.Parse(userInput) < 0)
                        {
                            tester = false;
                        }
                    }

                } while (!tester);

                return decimal.Parse(userInput);
            }

            //validates user input of long type variables. in this program, card numbers are the only long variable being validated.
            public long GetLong(string dataRequest)
            {
                string userInput = "";
                bool tester = true;

                Console.Write(dataRequest);

                do
                {
                    if (!tester)
                    {
                        Console.Clear();
                        Console.Write("Input was not a valid card number. Try Again. \n" +
                                      "Please enter your card number: ");
                    }

                    userInput = Console.ReadLine();

                    tester = long.TryParse(userInput, out _);

                    if (tester)
                    {
                        if (long.Parse(userInput) < 0)
                        {
                            tester = false;
                        }
                    }

                } while (!tester);

                return long.Parse(userInput);
            }
            //END METHODS

        }//END CLASS

        static void Main(string[] args)
        {
            //creates a customer object that inherits the variables of the Kiosk class
            Kiosk customer = new Kiosk();

            //sets amounts of each currency type in the kiosk
            customer.InitializeKioskInventory();
            
            //declare variables
            string userInput;
            decimal convertedInput;
            decimal totalCost = 0;
            int count = 1;
            bool repeat;
            bool validInput = false;

            List<decimal> itemList = new List<decimal>();

            //welcomes the user to the program
            Console.WriteLine("Welcome to Kiosk checkout! Please enter the price of your items. \n" +
                              "After all item prices are input, press the 'Enter' key while current item price input whitespace is empty to proceed to checkout.");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            //will run until the user's response null/empty
            do
            {
                //resets repeat to false to properly reuse the loop
                repeat = false;

                userInput = customer.GetString("Item " + count + ": $");

                //executes if the user input is null or empty
                if (String.IsNullOrEmpty(userInput))
                {
                    repeat = false;
                }

                //executes otherwise
                else
                {
                    //while the boolean variable valid 
                    while (validInput != true)
                    {
                        //attempts to convert user input to a double value type and sets the bool variable to true or false
                        validInput = decimal.TryParse(userInput, out _);

                        //prevents input from being valid if the user entered a negative number
                        if (validInput)
                        {
                            if (decimal.Parse(userInput) < 0)
                            {
                                validInput = false;
                            }
                        }
                        //if true, sets userInput to a double variable, adds the input to a list, adds to a running total, and sets repeat to true to move to the next item input
                        if (validInput == true)
                        {
                            convertedInput = decimal.Parse(userInput);

                            itemList.Add(convertedInput);

                            totalCost += convertedInput;

                            repeat = true;
                        }
                        //if false, prompts the user that the input is invalid and to reinput their variable
                        else if (validInput == false)
                        {
                            Console.WriteLine("Input must be a nonnegative decimal number. Try Again.");
                            Console.Write("Item " + count + ": $");
                            userInput = Console.ReadLine();

                            if (userInput == "")
                            {
                                repeat = false;
                                break;
                            }
                        }
                    }
                    //resets the validInput to false to reuse in loop
                    validInput = false;
                }

                //iterates count to accurately represent which item input the console is on
                count++;

            } while (repeat == true);

            //checks if the user did not input any items into the Kiosk and exits the program if this is true
            if (totalCost == 0)
            {
                Console.Clear();
                Console.WriteLine("No item prices were input into the Kiosk. \n"
                                + "Cancelling transaction...");
                Environment.Exit(-1);
            }

            //prints out the total price of items and prompts the user to enter payments for their total
            Console.WriteLine("~~~~~~~~~~~~~~");
            Console.WriteLine("Total: ${0:F2}", totalCost);
            Console.WriteLine();

            //proceeds to payment method screen when a key is pressed
            Console.WriteLine("Press any key to proceed to payment method...");
            Console.ReadKey();
            Console.Clear();

            //prompts the user on which payment method they would like to use and stores input
            userInput = customer.GetString("Would you like to pay with cash or a card? (enter 'cancel' if no payment method is viable): ");

            Console.Clear();

            //repeats while boolean variable repeat is true
            do
            {
                switch (userInput)
                {
                    //executes when user enters card, case-insensitive
                    case var expression when (userInput.ToLower() == "card"):
                        //proceeds to card payment method if the user wants to pay with card
                        customer.CardTransaction(totalCost, ref customer);
                        repeat = false;
                        break;
                    //executes when user enters cash, case-insensitive
                    case var expression when (userInput.ToLower() == "cash"):
                        //runs a user payment method if the user wants to pay with cash
                        customer.CashTransaction(totalCost, ref customer);
                        repeat = false;
                        break;
                    //executes when user enters cancel, case-insensitive
                    case var expression when (userInput.ToLower() == "cancel"):
                        //cancels the transaction by exiting the program
                        Console.WriteLine("Cancelling transaction...");
                        System.Environment.Exit(-1);
                        break;
                    //defaults to prompt the user to re-enter a valid value
                    default:
                        Console.Clear();
                        userInput = customer.GetString("Please enter a valid input (card/cash/cancel): ");
                        repeat = true;
                        break;
                }
            } while (repeat == true);

            //calls a method to output a post transaction kiosk inventory report to show the change in cash flow after the customer transaction
            customer.KioskInventoryReport();

            //starts a separate program and passes arguments to be used in the program for logging the transaction
            customer.LogTransaction();

        }//END MAIN

    }//END CLASS

}//END NAMESPACE
