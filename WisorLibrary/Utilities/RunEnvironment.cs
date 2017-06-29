﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisorLibrary.DataObjects;
using static WisorLib.GenericProduct;
using static WisorLib.MiscConstants;

namespace WisorLib
{
    public class RunEnvironment
    {
        public string OrderID { get; }
        //public string OutputFilename { get; }

        public CheckInfo CheckInfo { get; }

        public CalculationParameters CalculationParameters { get; }

        public BorrowerProfile BorrowerProfile { get; }

        public PrintOptions PrintOptions { get; }

        OutputFile OutputFile { get; set; }
        public List<ChosenComposition> listOfSelectedCompositions { get; set; }
        public List<string> headerOfListOfSelectedCompositions  {  get; }

        public int MaxProfit { get; set; }
        public int MaxBankPay { get; set; }
        public int MinBorrowerPay { get; set; }

        public LoggerFile Logger  { get; internal set; }

        public loanDetails theLoan { get; }

        public ResultsOutput resultsOutput { get; set; }

        public Composition bestDiffComposition, bestBankComposition, bestBorrowerComposition;

        public LoggerFile theMiscLogger { get; set; }

        // Hold the entire running environment data
        public RunEnvironment(loanDetails loan)
            //string orderid, double loanAmtWanted, double monthlyPmtWanted,
            //        uint propertyValue, uint income, uint youngestLenderAge, uint fico, uint sequenceID)
        {
            theLoan = loan;
            OrderID = loan.ID;
            //OutputFilename = CreateOutputFilename(orderid, loanAmtWanted, monthlyPmtWanted);
            CheckInfo = new CheckInfo(OrderID);
            CalculationParameters = new CalculationParameters(loan.LoanAmount, loan.DesiredMonthlyPayment,
                    loan.PropertyValue, loan.YearlyIncome, loan.BorrowerAge, loan.fico);

            // Set borrower risk profile for choosing interest rates
            BorrowerProfile = new BorrowerProfile(CalculationParameters, loan.fico);

            PrintOptions = new PrintOptions();

            listOfSelectedCompositions = new List<ChosenComposition>();
            headerOfListOfSelectedCompositions = new List<string>()
            {
                "ProductX", "ProductY", "ProductZ", "Borrower pay", "Bank amount", "Profit"
            };
            MaxProfit = MaxBankPay = MinBorrowerPay = 0;

            resultsOutput = new ResultsOutput();
            
            //bestDiffCompositionList = new List<Composition>();
            //bestBankCompositionList = new List<Composition>();
            //bestBorrowerCompositionList = new List<Composition>();
        }
        
        public void CreateTheOutputFiles(loanDetails loan, int borrowerProfile, string additionalName = MiscConstants.UNDEFINED_STRING) {
            // no need to create output file to each composition
            //CloseTheOutputFiles();
      
            if (null == OutputFile)
            {
                OutputFile = new OutputFile(loan, borrowerProfile /*, additionalName*/);
            }
    
            // create the combination logger file
            if (Share.ShouldStoreAllCombinations)
            {
                if (null != Logger)
                    Logger.CloseLog2CSV();
                Logger = new LoggerFile(OutputFile.OutputFilename, additionalName);
            }
        }

        public string GetOutputFileName()
        {
            if (null == OutputFile)
            {
                CreateTheOutputFiles(theLoan, BorrowerProfile.profile);
            }
            return OutputFile.OutputFilename;
        }

        public void WriteToOutputFile(string msg)
        {
            if (null == OutputFile)
            {
                CreateTheOutputFiles(theLoan, BorrowerProfile.profile);
            }
            OutputFile.WriteToOutputFile(msg);
        }

        public void CloseTheOutputFiles()
        {
            if (null != OutputFile)
            {
                //OutputFile.Remove();
                OutputFile.CloseOutputFile();
            }
            //OutputFile = null;
            if (null != Logger)
                Logger.CloseLog2CSV();
            //if (null != theMiscLogger)
            //    theMiscLogger.CloseLog2CSV();
            //theMiscLogger = null;
            //Logger = null;
        }


        public static bool SetMarket(markets market)
        {
            bool rc = true;
            Share.theMarket = market;
            //// ensure there are combination for this market
            //string[,] combination = CalculationConstants.GetCombination(market);

            //if (null == combination || 0 == combination.Length)
            //{
            //    WindowsUtilities.loggerMethod("ERROR: no combination founded for market: " + market.ToString());
            //}
            //else
            //{
            //    rc = true;
            //    WindowsUtilities.loggerMethod("NOTICE: running for market: " + market.ToString() + ", #of combination: " + combination.Length);
            //    Console.WriteLine("NOTICE: running for market: " + market.ToString() + ", #of combination: " + combination.Length);
            //}
            return rc;
        }

        // store the entire best results
        //public List<Composition> bestDiffCompositionList { get; }
        //public List<Composition> bestBankCompositionList { get; }
        //public List<Composition> bestBorrowerCompositionList { get; }

       

        //public void AddBestDiffComposition(Composition c)
        //{
        //    if (!bestDiffCompositionList.Exists(Composition.CompositionPredicate(c)))
        //        bestDiffCompositionList.Add(c);
        //}
        //public void AddBestBankComposition(Composition c)
        //{
        //    if (!bestBankCompositionList.Exists(Composition.CompositionPredicate(c)))
        //        bestBankCompositionList.Add(c);
        //}
        //public void AddBestBorroweComposition(Composition c)
        //{
        //    if (!bestBorrowerCompositionList.Exists(Composition.CompositionPredicate(c)))
        //        bestBorrowerCompositionList.Add(c);
        //}

        // store the results in the DB and create the reports
        public void CompleteCalculation()
        {
           
            // check if should create the report
            if (Share.shouldCreateHTMLReport || Share.ShouldStoreInDB)
            {
                theLoan.CompleteCalculation(new Composition[] 
                    { bestDiffComposition, bestBankComposition, bestBorrowerComposition }, 
                    Share.ShouldStoreInDB, Share.shouldCreateHTMLReport, Share.shouldCreatePDFReport,
                    this /* enable to print in the output file*/);
            }
            
        }

    }


}
