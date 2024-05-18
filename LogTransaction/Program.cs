using System.IO;

namespace LogTransaction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //declare variables
            int maxReportNumber;
            decimal cardPayment = 0;
            string transactionNumber = "";
            DateTime transactionDate = DateTime.Now;
            string transactionTime = "";
            string cashPaymentAmount = "";
            string cardVendor = "";
            string cardPaymentAmount = "";
            string changeGiven = "";

            //runs if there are arguments passed to main
            if (args.Length > 0) 
            {
                //repeats for every index in the args array
                for (int i = 0; i < args.Length; i++)
                {
                    switch (i)
                    {
                        //assigns transaction number
                        case 0:
                            transactionNumber = args[i];
                            break;
                        //assigns transaction date
                        case 1:
                            transactionDate = DateTime.Parse(args[i]);
                            break;
                        //assigns transaction time
                        case 2:
                            transactionTime = args[i];
                            break;
                        //assigns pay amount in cash
                        case 3:
                            if (args[i] == "0")
                            {
                                cashPaymentAmount = "0.00";
                            }
                            else
                            {
                                cashPaymentAmount = args[i];
                            }
                            break;
                        //assigns card vendor
                        case 4:
                            if (args[i] == "0")
                            {
                                cardVendor = "N/A";
                            }
                            else
                            {
                                cardVendor = args[i];
                            }
                            break;
                        //assigns payment amount with card
                        case 5:
                            if (args[i] == "0")
                            {
                                cardPaymentAmount = "0.00";
                            }
                            else
                            {
                                cardPaymentAmount = args[i];
                                cardPayment = decimal.Parse(cardPaymentAmount); 
                            }

                            break;
                        //assigns change given
                        case 6:
                            if (args[i] == "0")
                            {
                                changeGiven = "0.00";

                            }
                            else
                            {
                                changeGiven = args[i];
                            }

                            break;
                        default:
                            Console.WriteLine("Something went wrong with argument variable assignment.");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("No arguments received.");
            }

            //declares the file paths for the transaction logs
            string transactionFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\" + transactionDate.ToString("MMM-dd-yy") + "-Transactions.log";
            string transactionNumberFilePath = "C:\\Users\\Caleb\\Desktop\\Visual Studio Files\\TransactionNumber.log";

            //declares a string array to store contents of the file
            string[] text = new string[]
            {
                "\tTransaction Number: #" + transactionNumber,
                "\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy"),
                "\tTransaction Time: " + transactionTime,
                "\tPayment Amount (cash): $" + cashPaymentAmount,
                "\tCard Vendor: " + cardVendor,
                "\tPayment Amount (card): $" + cardPaymentAmount,
                "\tChange Given: $" + changeGiven
            };

            switch (cardPaymentAmount)
            {
                case var expression when (cardPaymentAmount == "N/A"):
                    text = new string[]
                    {
                        "\tTransaction Number: #" + transactionNumber,
                        "\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy"),
                        "\tTransaction Time: " + transactionTime,
                        "\tPayment Amount (cash): $" + cashPaymentAmount,
                        "\tCard Vendor: " + cardVendor,
                        "\tPayment Amount (card): $" + cardPaymentAmount,
                        "\tChange Given: $" + changeGiven
                    };
                    break;
                case var expression when (cardPaymentAmount != "N/A"):
                    text = new string[]
                    {
                        "\tTransaction Number: #" + transactionNumber,
                        "\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy"),
                        "\tTransaction Time: " + transactionTime,
                        "\tPayment Amount (cash): $" + cashPaymentAmount,
                        "\tCard Vendor: " + cardVendor,
                        "\tPayment Amount (card): $" + cardPayment.ToString("0.00"),
                        "\tChange Given: $" + changeGiven
                    };
                    break;
                default:
                    Console.WriteLine("Something went wrong with file text content creation.");
                    break;
            }

            //executes only if the file exists or is reachable
            if (File.Exists(transactionFilePath))
            {
                //reads in the contents of TransactionNumber.log
                using (StreamReader reader = new StreamReader(transactionNumberFilePath))
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
                File.AppendAllText(transactionNumberFilePath, maxReportNumber.ToString() + Environment.NewLine);
                Console.WriteLine("Appended " + maxReportNumber.ToString() + " to: " + transactionNumberFilePath);

                //gives a string variable named attrib with the new maxReport number so it properly is displayed within the Transactions.log
                string attrib =
                "\n\n\tTransaction Number: #" + maxReportNumber +
                "\n\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy") +
                "\n\tTransaction Time: " + transactionTime +
                "\n\tPayment Amount (cash): $" + cashPaymentAmount +
                "\n\tCard Vendor: " + cardVendor +
                "\n\tPayment Amount (card): $" + cardPaymentAmount +
                "\n\tChange Given: $" + changeGiven;

                switch (cardPaymentAmount)
                {
                    case var expression when (cardPaymentAmount == "N/A"):
                        attrib = "\n\n\tTransaction Number: #" + maxReportNumber +
                                 "\n\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy") +
                                 "\n\tTransaction Time: " + transactionTime +
                                 "\n\tPayment Amount (cash): $" + cashPaymentAmount +
                                 "\n\tCard Vendor: " + cardVendor +
                                 "\n\tPayment Amount (card): $" + cardPaymentAmount +
                                 "\n\tChange Given: $" + changeGiven;
                        break;
                    case var expression when (cardPaymentAmount != "N/A"):
                        attrib = "\n\n\tTransaction Number: #" + maxReportNumber +
                                 "\n\tTransaction Date: " + transactionDate.ToString("MMM-dd-yy") +
                                 "\n\tTransaction Time: " + transactionTime +
                                 "\n\tPayment Amount (cash): $" + cashPaymentAmount +
                                 "\n\tCard Vendor: " + cardVendor +
                                 "\n\tPayment Amount (card): $" + cardPayment.ToString("0.00") +
                                 "\n\tChange Given: $" + changeGiven;
                        break;
                    default:
                        Console.WriteLine("Something went wrong with file attrib content creation.");
                        break;
                }

                //appends the string attrib to the Transactions.log
                File.AppendAllText(transactionFilePath, attrib);
                Console.WriteLine("Appended To File: " + transactionFilePath);
            }
            //executes if the file does not exist or is unreachable 
            else
            {
                //attempts the following code
                try
                {
                    //writes the contents of string array text
                    using (StreamWriter writer = new StreamWriter(transactionFilePath))
                    {
                        foreach (string line in text)
                        {
                            writer.WriteLine(line);
                        }
                        Console.WriteLine("Transaction Log File written: " + transactionFilePath);
                    }
                    //writes a 1 on the first line of a new TransactionNumber.log file
                    using (StreamWriter writer = new StreamWriter(transactionNumberFilePath))
                    {
                        writer.WriteLine(1);
                        Console.WriteLine("Report Number Log File Updated: " + transactionNumberFilePath);
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
    }
}
