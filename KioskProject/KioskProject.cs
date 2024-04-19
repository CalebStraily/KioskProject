using System.Linq.Expressions;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace KioskProject
{
    internal class KioskProject
    {
        //declares a structure called Bank and its variables
        public struct Bank
        {
            public int[] currencyAmount;
            public int[] userPayment;
            public string[] currencyName;
            public double[] currencyValue;
        }

        public struct TransactionLog
        {
            public DateTime now;
            public string transactionFilePath;
            public string transactionNumberFilePath;
            public string cardVendor;
            public double subtotal;
            public double cashPayment;
            public double cardPayment;
            public double changeGiven;
        }
        static void Main(string[] args)
        {
            //assigns the variables made in the Bank struct
            Bank kiosk;

            kiosk.currencyAmount = new int[12];
            kiosk.userPayment = new int[12];
            kiosk.currencyName = new string[] {"Hundreds", "Fifties", "Twenties", "Tens", "Fives", "Twos",
                                               "Ones", "Half-Dollar", "Quarters", "Dimes", "Nickels", "Pennies"};
            kiosk.currencyValue = new double[] { 100, 50, 20, 10, 5, 2, 1, .50, .25, .10, .05, .01 };
           

            //assigns the variables made in DateAndTransaction
            TransactionLog transaction;

            transaction.now = DateTime.Now;
            transaction.transactionFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\" + transaction.now.ToString("MMM-dd-yy") + "-Transactions.log";
            transaction.transactionNumberFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\TransactionNumber.log";
            transaction.cardVendor = "";
            transaction.subtotal = 0;
            transaction.cashPayment = 0;
            transaction.cardPayment = 0;
            transaction.changeGiven = 0;

            //repeat the length of int array currencyAmount
            for (int i = 0; i < kiosk.currencyAmount.Length; i++) 
            {
                kiosk.currencyAmount[i] = 5;
            }

            //declare variables
            string userInput;
            double convertedInput;
            double totalCost = 0;
            int count = 1;
            bool repeat = false;
            bool validInput = false;

            

            List<double> itemList = new List<double>();

            //welcomes the user to the program
            Console.WriteLine("Welcome to Kiosk! Please enter the price of your items so that change can be dispensed.");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            //will run until the user enters null into the console
            do
            {
                //resets repeat to false to properly reuse the loop
                repeat = false;

                Console.Write("Item " + count + ": $");

                userInput = Console.ReadLine();

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

                        //if true, sets userInput to a double variable, adds the input to a list, adds to a running total, and sets repeat to true to move to the next item input
                        if (validInput == true)
                        {
                            convertedInput = double.Parse(userInput);

                            itemList.Add(convertedInput);

                            totalCost += convertedInput;

                            repeat = true;
                        }
                        //if false, prompts the user that the input is not a decimal number and to reinput their variable
                        else if (validInput == false)
                        {
                            Console.WriteLine("Not a decimal number. Try Again.");
                            Console.Write("Item " + count + ": $");
                            userInput = Console.ReadLine();
                        }
                    }
                    //resets the validInput to false to reuse in loop
                    validInput = false;
                }

                //iterates count to accurately represent which item input the console is on
                count++;

            } while (repeat == true);

            //prints out the total price of items and prompts the user to enter payments for their total
            Console.WriteLine("~~~~~~~~~~~~~~");
            Console.WriteLine("Total: ${0:F2}", totalCost);
            Console.WriteLine();

            //proceeds to payment method screen when a key is pressed
            Console.WriteLine("Press any key to proceed to payment method...");
            Console.ReadKey();
            Console.Clear();

            //prompts the user on which payment method they would like to use and stores input
            Console.WriteLine("Would you like to pay with cash or a card?: ");

            userInput = Console.ReadLine();

            Console.Clear();

            //repeats while boolean variable repeat is true
            do
            {
                switch (userInput)
                {
                    //executes when user enters card, case-insensitive
                    case var expression when (userInput.ToLower() == "card"):
                        //proceeds to card payment function if the user wants to pay with card
                        CardPayment(totalCost, kiosk, ref transaction);
                        repeat = false;
                        break;
                    //executes when user enters cash, case-insensitive
                    case var expression when (userInput.ToLower() == "cash"):
                        Console.WriteLine("Enter your payments for your total: ");
                        Console.WriteLine();
                        //runs a user payment function if the user wants to pay with cash
                        CashPayment(totalCost, kiosk, ref transaction);
                        repeat = false;
                        break;
                    //defaults to prompt the user to re-enter a valid value
                    default:
                        Console.WriteLine("Please enter a valid payment method (cash/card): ");
                        userInput = Console.ReadLine();
                        repeat = true;
                        break;

                }
            } while (repeat == true);

            //prints out a post transaction kio inventory report
            Console.WriteLine("Post Transaction Kiosk Inventory Report: ");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            //loops the length of the currencyAmount array and outputs the amount of money in the kiosk
            for (int i = 0; i < kiosk.currencyAmount.Length; i++)
            {
                Console.WriteLine("Amount of " + kiosk.currencyName[i] + " in kiosk: " + kiosk.currencyAmount[i]);
            }

            Console.ReadKey();
            Console.Clear();

            //prints out a post transaction customer transaction report
            Console.WriteLine("Post Transaction Customer Transaction Report: ");
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
            Console.WriteLine();

            //loops the length of the userPayment array and outputs the amount of bill and coins used 
            for (int i = 0; i < kiosk.userPayment.Length; i++) 
            {
                Console.WriteLine("User's payment in " + kiosk.currencyName[i] + ": " + kiosk.userPayment[i]);
            }

            Console.ReadKey();

            //calls LogTransaction to log purchase
            LogTransaction(totalCost, ref transaction, kiosk);

        }//END MAIN

        //outputs the results of the transaction to a .log file
        static void LogTransaction(double totalCost, ref TransactionLog transaction, Bank kiosk)
        {
            int transactionNumber = 1;
            int maxReportNumber;
            //declares a string array to store contents of the file
            string[] text = new string[]
            {
                "\tTransaction Number: #" + transactionNumber,
                "\tTransaction Date: " + transaction.now.ToShortDateString(),
                "\tTransaction Time: " + transaction.now.ToShortTimeString(),
                "\tPayment Amount (cash): $" + transaction.cashPayment,
                "\tCard Vendor: " + transaction.cardVendor,
                "\tPayment Amount (card): $" + transaction.cardPayment, 
                "\tChange Given: $" + transaction.changeGiven
            };

            //executes only if the file exists or is reachable
            if (File.Exists(transaction.transactionFilePath))
            {
                //reads in the contents of TransactionNumber.log
                using (StreamReader reader = new StreamReader(transaction.transactionNumberFilePath))
                {
                    //declare variables
                    string line;
                    int temp = 0;

                    //reads through every line of the log
                    while ((line = reader.ReadLine()) != null)
                    {
                        //if the current line has a value parsed to int greater than temp, then assigns temp that value
                        if (int.Parse(line) > temp)
                        {
                            temp = int.Parse(line);
                        }
                    }

                    //after the while loop, sets maxReportNumber to temp + 1
                    maxReportNumber = temp += 1;
                }

                //appends the max report number to the TransactionNumber.log
                File.AppendAllText(transaction.transactionNumberFilePath, maxReportNumber.ToString() + Environment.NewLine);
                Console.WriteLine("Appended " + maxReportNumber.ToString() + " to: " + transaction.transactionNumberFilePath);

                //gives a string variable named attrib with the new maxReport number so it properly is displayed within the Transactions.log
                string attrib =
                "\n\n\tTransaction Number: #" + maxReportNumber +
                "\n\tTransaction Date: " + transaction.now.ToShortDateString() +
                "\n\tTransaction Time: " + transaction.now.ToShortTimeString() +
                "\n\tPayment Amount (cash): $" + transaction.cashPayment +
                "\n\tCard Vendor: " + transaction.cardVendor +
                "\n\tPayment Amount (card): $" + transaction.cardPayment +
                "\n\tChange Given: $" + transaction.changeGiven;

                //appends the string attrib to the Transactions.log
                File.AppendAllText(transaction.transactionFilePath, attrib);
                Console.WriteLine("Appended To File: " + transaction.transactionFilePath);
            }
            //executes if the file does not exist or is unreachable 
            else
            {
                //attempts the following code
                try
                {
                    //writes the contents of string array text
                    using (StreamWriter writer = new StreamWriter(transaction.transactionFilePath))
                    {
                        foreach (string line in text)
                        {
                            writer.WriteLine(line);
                        }
                        Console.WriteLine("File written: " + transaction.transactionFilePath);
                    }
                    //writes a 1 on the first line of a new TransactionNumber.log file
                    using (StreamWriter writer = new StreamWriter(transaction.transactionNumberFilePath))
                    {
                        writer.WriteLine(1);
                        Console.WriteLine("Report Amount Updated: " + transaction.transactionNumberFilePath);
                    }
                }
                //throws and exception error if not possible
                catch (Exception error)
                {
                    Console.WriteLine(error.Message);
                    Console.ReadKey();
                }
            }
            //attempts to run its contents
            
        }

        //runs the processes for card payments
        static void CardPayment(double totalCost, Bank kiosk, ref TransactionLog transaction)
        {
            //declares variables
            long longUserInput;
            string userInput;
            string cardNumber;
            int digitCount = 0;
            int userDigitDoubled;
            int runningTotal = 0;
            double cashBack = 0;
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
                    Console.WriteLine("Please enter your card number: ");

                    longUserInput = GetLong();

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
                        transaction.cardVendor = "Discover Card";
                        break;
                    //checks if the digits are within the parameters for a MasterCard
                    case var expression when (twoDigitUserInput >= 51 && twoDigitUserInput <= 55 && digitCount == 16):
                        Console.WriteLine("Card accepted: your card is a MasterCard.");
                        transaction.cardVendor = "MasterCard";
                        break;
                    //checks if the digits are within the parameters for an American card
                    case var expression when (twoDigitUserInput >= 34 && twoDigitUserInput <= 37 && digitCount == 15):
                        Console.WriteLine("Card accepted: your card is an American Express card.");
                        transaction.cardVendor = "American Express";
                        break;
                    //checks if the digits are within the parameters for a Discover card
                    case var expression when (oneDigitUserInput == 4 && digitCount >= 13 && digitCount <= 16):
                        Console.WriteLine("Card accepted: your card is a Visa.");
                        transaction.cardVendor = "Visa";
                        break;
                }

                //prompts the user on if they would like cash back and stores the value
                Console.WriteLine("Would you like cash back (yes/no)?");

                userInput = Console.ReadLine();

                Console.Clear();

                //repeats while boolean repeat equals true
                do
                {
                    //checks for valid input from the user
                    switch (userInput)
                    {
                        case var expression when (userInput.ToLower() == "yes"):
                            Console.Write("Please enter the amount you want on cash back: $");
                            cashBack = GetDouble();
                            Console.Clear();
                            wantsCashBack = true;
                            repeat = false;
                            break;
                        case var expression when (userInput.ToLower() == "no"):
                            repeat = false;
                            wantsCashBack = false;
                            break;
                        default:
                            Console.WriteLine("Please type a valid response: ");
                            userInput = Console.ReadLine();
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
                                CashPayment(totalCost, kiosk, ref transaction);
                                return;
                            //will exit the program
                            case var expression when (userInput.ToLower() == "cancel"):
                                Console.Clear();
                                System.Environment.Exit(0);
                                break;
                            //defaults if input was not valid
                            default:
                                Console.Write("Please enter a valid input (change/cash/cancel): ");
                                userInput = Console.ReadLine();
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
                                Console.Write("Please enter a valid input (change/cancel): ");
                                userInput = Console.ReadLine();
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
                    double subTotal;
                    double actualCashBack;

                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    DispenseChange(cashBack, kiosk, ref transaction);
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("Account Number: " + accountNumberResult);
                    Console.WriteLine("Total: +${0:F2}", totalCost);
                    Console.WriteLine("Cash Back Credited: +${0:F2}", cashBack);
                    subTotal = totalCost + cashBack;
                    actualCashBack = double.Parse(transactionResultMessage);
                    Console.WriteLine("Cash Back: -${0:F2}", actualCashBack);
                    subTotal -= actualCashBack;
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("Subtotal: ${0:F2}", subTotal);

                    //sets the card payment to the subtotal for logging and cash payment to 0 since transaction was paid with a card
                    transaction.cardPayment = subTotal;
                    transaction.cashPayment = 0;

                    Console.ReadKey();
                    Console.Clear();

                    changePaymentMethod = false;
                }

            } while (changePaymentMethod == true);
        }

        //simulates a request from funds from a banking account
        static string[] MoneyRequest(string account_number, double amount)
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

        //gets a series of payments in bills/coins to pay total expense on items
        static void CashPayment(double totalCost, Bank kiosk, ref TransactionLog transaction)
        {
            //declare variables
            double totalRemaining = totalCost;
            double userPayment;
            double changeDue;
            int count = 1;
            int arrayIteration = 0;
            bool validCurrencyValue = false;

            //sets the payment in cash to totalCost and sets card payment to zero since transaction is in cash only
            transaction.cashPayment = totalCost;
            transaction.cardPayment = 0;

            //will get user input while there is still total remaining to pay
            do
            {
                //gets a payment amount from the user
                Console.Write("Payment " + count + ": $");
                userPayment = GetDouble();

                //runs while boolean validCurrencyValue is false. validates that the value inputted is a valid currency value and is of double value type
                do
                {
                    validCurrencyValue = false;

                    for (int i = 0; i < kiosk.currencyValue.Length; i++)
                    {
                        if (userPayment == kiosk.currencyValue[i])
                        {
                            validCurrencyValue = true;

                            AddUserBillOrCoin(userPayment, kiosk);

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
                        Console.WriteLine("Please enter a valid currency value.");
                        Console.Write("Payment " + count + ": $");
                        userPayment = GetDouble();
                    }

                } while (validCurrencyValue == false);

            } while (totalRemaining > 0);

            //flips change due to positive for displaying on console and further use in the program
            changeDue = (totalRemaining * -1);

            //outputs change due to the console
            Console.WriteLine();
            Console.WriteLine("Change Due: ${0:F2}", changeDue);
            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");

            DispenseChange(changeDue, kiosk, ref transaction);

            Console.ReadKey();
            Console.Clear();
        }

        //will dispense the change using a greedy algorithm, where the bills/coins used are from highest to lowest possible
        static void DispenseChange(double changeDue, Bank kiosk, ref TransactionLog transaction)
        {
            //rounds the change due so the statements will run correctly
            double changeDueRounded = Math.Round(changeDue, 2);

            //repeats for each element of the currencyValue array
            for (int i = 0; i < kiosk.currencyValue.Length; i++) 
            {
                //executes if change due rounded is divisible
                if ((changeDueRounded / kiosk.currencyValue[i]) >= 1)
                {
                    switch(kiosk.currencyAmount[i])
                    {
                        //executes if the currencyAmount at current index is greater than zero
                        case var expression when (kiosk.currencyAmount[i] > 0):
                            
                            //loops while changeDueRounded is greater than zero AND changeDueRounded divided by current index of currencyValue is still greater than or equal to 1
                            while (changeDueRounded > 0 && (changeDueRounded / kiosk.currencyValue[i]) >= 1)
                            {
                                //executes if changeDueRounded minus current index of currencyValue is positive or zero
                                if ((changeDueRounded - kiosk.currencyValue[i]) >= 0)
                                {
                                    //subtracts current index of currencyValue from changeDue outputs that the current index of currencyValue was dispensed
                                    changeDue -= kiosk.currencyValue[i];
                                    Console.WriteLine("${0:F2} dispensed.", kiosk.currencyValue[i]);
                                    //add to a running to for change given to the customer
                                    transaction.changeGiven += kiosk.currencyValue[i];
                                    //deducts the bill/coin taken from kiosk to give customer their change
                                    kiosk.currencyAmount[i]--;
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

        //refunds the transactions made by the user
        static void RefundTransaction(Bank kiosk)
        {
            //loops for each index of the userPayment array and removes bills from the kiosk for every bill input by the user
            for (int i = 0; i < kiosk.userPayment.Length; i++)
            {
                switch (kiosk.userPayment)
                {
                    case var expression when (kiosk.userPayment[i] > 0):

                        kiosk.currencyAmount[i] -= kiosk.userPayment[i];

                        break;
                }
            }
        }

        //adds the bill inputted by the user for payment to the amount in the kiosk
        static void AddUserBillOrCoin(double userPayment, Bank kiosk)
        {
            //loops for every index of currencyValue adds the bill/coin input into the kiosk
            for (int i = 0; i < kiosk.currencyValue.Length; i++)
            {
                switch (userPayment)
                {
                    case var express when (userPayment == kiosk.currencyValue[i]):
                        kiosk.currencyAmount[i] += 1;
                        kiosk.userPayment[i] += 1;
                        break;
                }
            }
        }

        //validates user input of double type variables
        static double GetDouble()
        {
            string userInput = "";
            bool tester = true;

            do
            {
                if (!tester)
                {
                    Console.WriteLine("Not a decimal number. Try Again.");
                }

                userInput = Console.ReadLine();

                Console.Clear();

                tester = double.TryParse(userInput, out _);

            } while (!tester);

            return double.Parse(userInput);
        }

        //validates user input of long type variables
        static long GetLong() 
        {
            string userInput = "";
            bool tester = true;

            do
            {
                if (!tester)
                {
                    Console.WriteLine("Not a valid number. Try Again.");
                }

                userInput = Console.ReadLine();

                Console.Clear();

                tester = double.TryParse(userInput, out _);

            } while (!tester);

            return long.Parse(userInput);
        }

    }
}
