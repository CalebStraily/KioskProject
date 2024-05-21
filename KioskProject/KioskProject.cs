﻿using System.Linq.Expressions;
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
        //declares a structure called Bank and its variables
        public class Kiosk
        {
            //FIELDS
            public int[] _currencyAmount;
            public int[] _userPayment;
            public string[] _currencyName;
            public decimal[] _currencyValue;

            public DateTime _now;
            public string _transactionFilePath;
            public string _transactionNumberFilePath;
            public string _cardVendor;
            public decimal _subtotal;
            public decimal _cashPayment;
            public decimal _cardPayment;
            public decimal _changeGiven;

            //CONSTRUCTOR
            public Kiosk()
            {
                _currencyAmount = new int[12];
                _userPayment = new int[12];
                _currencyName = new string[] {"Hundreds", "Fifties", "Twenties", "Tens", "Fives", "Twos",
                                              "Ones", "Half-Dollar", "Quarters", "Dimes", "Nickels", "Pennies"};
                _currencyValue = new decimal[] { 100, 50, 20, 10, 5, 2, 1, .50M, .25M, .10M, .05M, .01M };
                _now = DateTime.Now;
                _transactionFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\" + _now.ToString("MMM-dd-yy") + "-Transactions.log";
                _transactionNumberFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\TransactionNumber.log";
                _cardVendor = "0";
                _subtotal = 0;
                _cardPayment = 0;
                _changeGiven = 0;
            }
            //END CONSTRUCTOR

            //GETTERS
            public int[] CurrencyAmount { get { return _currencyAmount; } }
            public int[] UserPayment { get { return _userPayment; } }
            public string[] CurrencyName { get { return _currencyName; } }
            public decimal[] CurrencyValue { get { return _currencyValue; } }
            public DateTime Now { get { return _now; } }
            public string TransactionFilePath { get { return _transactionFilePath; } }
            public string TransactionNumberFilePath { get { return _transactionNumberFilePath; } }
            public string CardVendor { get { return _cardVendor; } }
            public decimal SubTotal { get { return _subtotal; } }
            public decimal CardPayment { get { return _cardPayment; } }
            public decimal ChangeGiven { get { return _changeGiven; } }
            //END GETTERS

            //METHODS
            //sets all currency types to have an amount of 5
            public void InitializeKioskInventory()
            {
                //repeat the length of int array currencyAmount
                for (int i = 0; i < CurrencyAmount.Length; i++)
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
                bool wantsCashBack = false;
                bool changePaymentMethod = false;

                //will repeat while the user wants to change payment method
                do
                {
                    //resets changePaymentMethod to false to loop properly
                    changePaymentMethod = false;

                    //will repeat while the card is not a valid value
                    do
                    {
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

                        Console.WriteLine();

                        //runs if the number has an even amount of digits
                        if (digitCount % 2 == 0)
                        {
                            //multiplies every other digit starting from the left most digit by 2
                            for (int i = 0; i < userDigitsArray.Length; i += 2)
                            {
                                userDigitDoubled = userDigitsArray[i] * 2;

                                if (userDigitDoubled >= 10)
                                {
                                    runningTotal += (1 + (userDigitDoubled % 10));
                                }
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

                                if (userDigitDoubled >= 10)
                                {
                                    runningTotal += (1 + (userDigitDoubled % 10));
                                }
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
                            validCard = false;
                            Console.WriteLine("Error: Card number is invalid. Please re-enter your card number.");

                            //resets the running total and digit count so the loop works properly
                            runningTotal = 0;
                            digitCount = 0;
                        }
                    } while (validCard == false);

                    //sets a series of variables at specific digits of what the user has input
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
                            Console.WriteLine("Card accepted: your card is a Discover Card.");
                            _cardVendor = "Discover Card";
                            break;
                        //checks if the digits are within the parameters for a MasterCard
                        case var expression when (twoDigitUserInput >= 51 && twoDigitUserInput <= 55 && digitCount == 16):
                            Console.WriteLine("Card accepted: your card is a MasterCard.");
                            _cardVendor = "MasterCard";
                            break;
                        //checks if the digits are within the parameters for an American card
                        case var expression when (twoDigitUserInput >= 34 && twoDigitUserInput <= 37 && digitCount == 15):
                            Console.WriteLine("Card accepted: your card is an American Express card.");
                            _cardVendor = "American Express";
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
                                wantsCashBack = true;
                                repeat = false;
                                break;
                            case var expression when (userInput.ToLower() == "no"):
                                repeat = false;
                                wantsCashBack = false;
                                break;
                            default:
                                userInput = GetString("Please type a valid response: ");
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

                    //executes if the value at index 1 of transactionResult is declined case-insensitive AND the user does not want cash back
                    if (transactionResult[1].ToLower() == "declined" && wantsCashBack == false)
                    {
                        Console.WriteLine("Card declined.");
                        Console.WriteLine("Remaining Balance: ");
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("${0:F2}", totalCost);
                        Console.WriteLine();
                        Console.WriteLine("Would you like to change payment, pay the remaining in cash, or cancel the order? \n" +
                                          "Please enter 'change', 'cash', or 'cancel' to proceed.");

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
                                case var expression when (userInput.ToLower() == "change"):
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
                                    System.Environment.Exit(0);
                                    break;
                                //defaults if input was not valid
                                default:
                                    userInput = GetString("Please enter a valid input (change/cash/cancel): ");
                                    repeat = true;
                                    Console.Clear();
                                    break;
                            }
                        } while (repeat == true);
                    }
                    //executes if the value at index 1 of transactionResult is declined case in-sensitive AND the user does want cash back 
                    else if (transactionResult[1].ToLower() == "declined" && wantsCashBack == true)
                    {
                        Console.WriteLine("Card declined.");
                        Console.WriteLine("Remaining Balance: ");
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("${0:F2}", totalCost);
                        Console.WriteLine();
                        Console.WriteLine("Would you like to change payment method or cancel the order? \n" +
                                          "Please enter 'change' or 'cancel' to proceed.");
                        userInput = Console.ReadLine();

                        repeat = false;

                        //repeats while boolean repeat is true
                        do
                        {
                            repeat = false;

                            //validates user input
                            switch (userInput)
                            {
                                //executes if the user wants to change payment method
                                case var expression when (userInput.ToLower() == "change"):
                                    repeat = false;
                                    changePaymentMethod = true;
                                    Console.Clear();
                                    break;
                                //executes if the user want to cancel payment
                                case var expression when (userInput.ToLower() == "cancel"):
                                    Console.Clear();
                                    return;
                                //executes if the user's input was invalid
                                default:
                                    userInput = GetString("Please enter a valid input (change/cancel): ");
                                    repeat = true;
                                    Console.Clear();
                                    break;
                            }
                            //repeats while user input is not 'retry' or 'cash'
                        } while (repeat == true);
                    }
                    //executes if the user's card payment is approved
                    else
                    {
                        //declare variables
                        decimal subTotal;
                        decimal actualCashBack;

                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        DispenseChange(cashBack, ref kiosk);
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("Account Number: " + accountNumberResult);
                        Console.WriteLine("Total: +${0:F2}", totalCost);
                        Console.WriteLine("Cash Back Credited: +${0:F2}", cashBack);
                        subTotal = totalCost + cashBack;
                        actualCashBack = decimal.Parse(transactionResultMessage);
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

            //will dispense the change using a greedy algorithm, where the bills/coins dispensed are from highest to lowest possible
            public void DispenseChange(decimal changeDue, ref Kiosk kiosk)
            {
                //rounds the change due so the statements will run correctly
                decimal changeDueRounded = Math.Round(changeDue, 2);

                //repeats for each element of the currencyValue array
                for (int i = 0; i < kiosk.CurrencyValue.Length; i++)
                {
                    //executes if change due rounded is divisible
                    if ((changeDueRounded / kiosk.CurrencyValue[i]) >= 1)
                    {
                        switch (kiosk.CurrencyAmount[i])
                        {
                            //executes if the currencyAmount at current index is greater than zero
                            case var expression when (kiosk.CurrencyAmount[i] > 0):

                                //loops while changeDueRounded is greater than zero AND changeDueRounded divided by current index of currencyValue is still greater than or equal to 1
                                while (changeDueRounded > 0 && (changeDueRounded / kiosk.CurrencyValue[i]) >= 1)
                                {
                                    //executes if changeDueRounded minus current index of currencyValue is positive or zero
                                    if ((changeDueRounded - kiosk.CurrencyValue[i]) >= 0)
                                    {
                                        //subtracts current index of currencyValue from changeDue outputs that the current index of currencyValue was dispensed
                                        changeDue -= kiosk.CurrencyValue[i];
                                        Console.WriteLine("${0:F2} dispensed.", kiosk.CurrencyValue[i]);
                                        //add to a running to for change given to the customer
                                        kiosk._changeGiven += kiosk.CurrencyValue[i];
                                        //deducts the bill/coin taken from kiosk to give customer their change
                                        kiosk.CurrencyAmount[i]--;
                                    }

                                    //rounds change again after new calculations have been made
                                    changeDueRounded = Math.Round(changeDue, 2);
                                }
                                break;

                            //default execution if previous case is not true
                            default:
                                //outputs to the console if there are not enough bill/money to pay customer's change
                                Console.WriteLine("Error: the kiosk does not have enough physical money to supply your change.");
                                Console.WriteLine("Please use another method of payment.");
                                Console.WriteLine("Cancelling transaction...");
                                RefundTransaction(kiosk);
                                return;
                        }
                    }
                }
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

                //sets the payment in cash to totalCost and sets card payment to zero since transaction is in cash only
                kiosk._cashPayment = totalCost;
                kiosk._cardPayment = 0;

                //will get user input while there is still total remaining to pay
                do
                {
                    //gets a payment amount from the user
                    userPayment = GetDecimal("Payment " + count + ": $");

                    //runs while boolean validCurrencyValue is false. validates that the value inputted is a valid currency value and is of double value type
                    do
                    {
                        validCurrencyValue = false;

                        for (int i = 0; i < kiosk.CurrencyValue.Length; i++)
                        {
                            if (userPayment == kiosk.CurrencyValue[i])
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
                            userPayment = GetDecimal("Please enter a valid currency value.\n" +
                                                     "Payment " + count + ": $");
                        }

                    } while (validCurrencyValue == false);

                } while (totalRemaining > 0);

                //flips change due to positive for displaying on console and further use in the program
                changeDue = (totalRemaining * -1);

                //outputs change due to the console
                Console.WriteLine();
                Console.WriteLine("Change Due: ${0:F2}", changeDue);
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

                DispenseChange(changeDue, ref kiosk);

                Console.ReadKey();
                Console.Clear();
            }

            //simulates a request for funds from a banking account
            public string[] MoneyRequest(string account_number, decimal amount)
            {
                Random rnd = new Random();
                //50% CHANCE TRANSACTION PASSES OR FAILS
                bool pass = rnd.Next(100) < 50;

                //50% CHANCE THAT A FAILED TRANSACTION IS DECLINEED
                bool declined = rnd.Next(100) < 50;

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

            //refunds the transactions made by the user
            public void RefundTransaction(Kiosk kiosk)
            {
                //loops for each index of the userPayment array and removes bills from the kiosk for every bill input by the user
                for (int i = 0; i < kiosk.UserPayment.Length; i++)
                {
                    switch (kiosk.UserPayment)
                    {
                        case var expression when (kiosk.UserPayment[i] > 0):

                            kiosk.CurrencyAmount[i] -= kiosk.UserPayment[i];

                            break;
                    }
                }
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

            //validates user input of long type variables
            public long GetLong(string dataRequest)
            {
                string userInput = "";
                bool tester = true;

                Console.Write(dataRequest);

                do
                {
                    if (!tester)
                    {
                        Console.Write("Input must be a nonnegative decimal number. Try Again.");
                    }

                    userInput = Console.ReadLine();

                    tester = double.TryParse(userInput, out _);

                    if (tester)
                    {
                        if (decimal.Parse(userInput) < 0)
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
            //creates a customer objects that inherits the variables of the Kiosk class
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
                        validInput = double.TryParse(userInput, out _);

                        //prevents input from being valid if the user entered a negative number
                        if (validInput)
                        {
                            if (double.Parse(userInput) < 0)
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
                        //if false, prompts the user that the input is not a decimal number and to reinput their variable
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
            userInput = customer.GetString("Would you like to pay with cash or a card?: ");

            Console.Clear();

            //repeats while boolean variable repeat is true
            do
            {
                switch (userInput)
                {
                    //executes when user enters card, case-insensitive
                    case var expression when (userInput.ToLower() == "card"):
                        //proceeds to card payment function if the user wants to pay with card
                        customer.CardTransaction(totalCost, ref customer);
                        repeat = false;
                        break;
                    //executes when user enters cash, case-insensitive
                    case var expression when (userInput.ToLower() == "cash"):
                        Console.WriteLine("Enter your payments for your total: ");
                        Console.WriteLine();
                        //runs a user payment function if the user wants to pay with cash
                        customer.CashTransaction(totalCost, ref customer);
                        repeat = false;
                        break;
                    //defaults to prompt the user to re-enter a valid value
                    default:
                        userInput = customer.GetString("Please enter a valid payment method (cash/card): ");
                        repeat = true;
                        break;
                }
            } while (repeat == true);

            //prints out a post transaction kio inventory report
            Console.WriteLine("Post Transaction Kiosk Inventory Report: ");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            //loops the length of the currencyAmount array and outputs the amount of money in the kiosk
            for (int i = 0; i < customer.CurrencyAmount.Length; i++)
            {
                Console.WriteLine("Amount of " + customer.CurrencyName[i] + " in kiosk: " + customer.CurrencyAmount[i]);
            }

            Console.ReadKey();
            Console.Clear();
            
            //below process assigns a series of variables to multiple arguments in string format to send to program "LogTransaction to create/update a .log file for every transaction made at the kiosk

            //transaction number
            string arg1 = "1";
            //transaction date
            string arg2 = customer._now.ToShortDateString();
            //transaction time
            string arg3 = customer._now.ToShortTimeString();
            //payment amount (cash)
            string arg4 = customer._cashPayment.ToString();
            //card vendor
            string arg5 = customer._cardVendor.ToString();
            //payment amount (card)
            string arg6 = customer._cardPayment.ToString();
            //change given
            string arg7 = customer._changeGiven.ToString();

            //creates and starts a process to run the LogTransaction program with arguments
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = @"C:\Users\Caleb\source\repos\KioskProject\LogTransaction\bin\Debug\net8.0\LogTransaction.exe";
            startInfo.Arguments = $"{arg1} \"{arg2}\" \"{arg3}\" {arg4} {arg5} {arg6} {arg7}";
            Process.Start(startInfo);

        }//END MAIN

    }//END CLASS

}//END NAMESPACE
