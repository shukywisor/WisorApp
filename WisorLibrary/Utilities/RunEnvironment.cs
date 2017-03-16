﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WisorLib.GenericProduct;

namespace WisorLib
{
    public class RunEnvironment
    {
        public string OrderID { get; }
        //public string OutputFilename { get; }

        public CheckInfo CheckInfo { get; }

        public CalculationParameters CalculationParameters { get; }

        public PrintOptions PrintOptions { get; }

        public OutputFile OutputFile { get; }
        public List<ChosenComposition> listOfSelectedCompositions { get; set; }
        public List<string> headerOfListOfSelectedCompositions  {  get; }

        public int MaxProfit { get; set; }
        public int MaxBankPay { get; set; }
        public int MinBorrowerPay { get; set; }
       
     
        // Hold the entire running environment data
        public RunEnvironment(loanDetails loan)
            //string orderid, double loanAmtWanted, double monthlyPmtWanted,
            //        uint propertyValue, uint income, uint youngestLenderAge, uint fico, uint sequenceID)
        {
            OrderID = loan.ID;
            //OutputFilename = CreateOutputFilename(orderid, loanAmtWanted, monthlyPmtWanted);
            CheckInfo = new CheckInfo(OrderID);
            CalculationParameters = new CalculationParameters(loan.LoanAmount, loan.DesiredMonthlyPayment,
                    loan.PropertyValue, loan.YearlyIncome, loan.BorrowerAge, loan.fico);
            PrintOptions = new PrintOptions();
            if (PrintOptions.printToOutputFile == true)
            {
                OutputFile = new OutputFile(loan, CheckInfo);
                    //loan.ID, loan.LoanAmount, loan.DesiredMonthlyPayment, 
                    //CheckInfo, loan.SequentialNumber);
            }
            listOfSelectedCompositions = new List<ChosenComposition>();
            headerOfListOfSelectedCompositions = new List<string>()
            {
                "ProductX", "ProductY", "ProductZ", "Borrower pay", "Bank amount", "Profit"
            };
            MaxProfit = MaxBankPay = MinBorrowerPay = 0;
  
        }

        public static bool SetMarket(markets market)
        {
            bool rc = false;
            Share.theMarket = market;
            // ensure there are combination for this market
            string[,] combination = CalculationConstants.GetCombination(market);

            if (null == combination || 0 == combination.Length)
            {
                WindowsUtilities.loggerMethod("ERROR: no combination founded for market: " + market.ToString());
            }
            else
                rc = true;
            WindowsUtilities.loggerMethod("NOTICE: running for market: " + market.ToString() + ", #of combination: " + combination.GetUpperBound(0));
            Console.WriteLine("NOTICE: running for market: " + market.ToString() + ", #of combination: " + combination.Length);
            return rc;
        }

     


        /// <summary>
        /// Enable to write logs
        /// </summary>
        /// 
        private StreamWriter fileStream;

        public void PrepareLog2CSV(string ID)
        {
            fileStream = null;

            if (Share.ShouldStoreAllCombinations)
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory // + Path.DirectorySeparatorChar
                    + MiscConstants.OUTPUT_DIR + Path.DirectorySeparatorChar +
                    /*orderid*/ ID +
                    MiscConstants.NAME_SEP_CHAR + "Logger" + MiscConstants.NAME_SEP_CHAR +
                    DateTime.Now.ToString("MM-dd-yyyy-h-mm-tt") + MiscConstants.CSV_EXT;


                // TBD: Shuky - ensure the directory realy exists
                if (!Directory.Exists(Path.GetDirectoryName(filename)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));

                fileStream = new StreamWriter(filename);
            }
        }

        public void PrintLog2CSV(string[] msg)
        {
            if (Share.ShouldStoreAllCombinations && null != fileStream)
            {
                try
                {
                    string msg2write = null;
                    for (int i = 0; i < msg.Length; i++)
                    {
                        msg2write += msg[i] + MiscConstants.COMMA_SEERATOR_STR;
                    }

                    fileStream.WriteLine(msg2write);
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: PrintLog2CSV got Exception: " + e.ToString());
                }
            }
        }

        public void CloseLog2CSV()
        {
            if (Share.ShouldStoreAllCombinations && null != fileStream)
            {
                fileStream.Close();
            }
        }

    }


}
