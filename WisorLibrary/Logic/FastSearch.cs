﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WisorLibrary.DataObjects;
using WisorLibrary.Utilities;

namespace WisorLib
{
    public class FastSearch
    {

        public bool CanRunCalculation { get; set; }

        RunEnvironment env;

        // constructor
        public FastSearch(RunEnvironment Env)
        {
            CanRunCalculation = false;
            env = Env;

            if (MiscConstants.UNDEFINED_STRING == env.CheckInfo.orderID || 0 >= env.CalculationParameters.loanAmtWanted || 
                0 >= env.CalculationParameters.monthlyPmtWanted || 0 >= env.CalculationParameters.propertyValue || 
                0 >= env.CalculationParameters.income || 0 >= env.CalculationParameters.youngestLenderAge)
            {
                Console.WriteLine("NOTICE: Should load the user' parameters already");
            }
            else
            {
                CanRunCalculation = true;
           }
        }

        public RunLoanDetails runSearch()
        {
            //long elapsedMs = 0;
            // ensure there are any combinations
            string[,] combinations = MiscUtilities.GetCombinationsNames(env.theLoan.LoanAmount, env.CalculationParameters.ltv);
            // Combinations.GetCombinationByAmount(Share.theMarket, env.theLoan.LoanAmount);

            if (CanRunCalculation && null != combinations)
            {
                // for debug
                //if (Share.shouldDebugLoans)
                //{
                //    MiscUtilities.PrintMiscLogger("\nRuning calculation with loan:");
                //    MiscUtilities.PrintMiscLogger(env.theLoan.ToString());
                //}

                // Get start time for software
                env.CheckInfo.softwareOpenTime = DateTime.Now;
                    
                if (env.PrintOptions.printMainInConsole == true)
                {
                    Console.WriteLine("\n\tBegin Fast Three Option Check - Version 3.2.1\n\tSoftware Started at " + env.CheckInfo.softwareOpenTime
                                        + "\n\tAll Rights Reserved - Wisor Technologies Ltd. 2014-2015 \n");
                }

                if (env.BorrowerProfile.ShowBorrowerProfile() == (int)CalculationConstants.borrowerProfiles.NOTOK)
                {
                    Console.WriteLine("\nBorrower profile not ok - check yourself....");
                }
                if (env.PrintOptions.printMainInConsole == true)
                {
                    Console.WriteLine("\nLoan Amount = " + env.CalculationParameters.loanAmtWanted + "\nTarget monthly payment = "
                                        + env.CalculationParameters.monthlyPmtWanted + "\n\nThere are " + (combinations.GetUpperBound(0) + 1)
                                        + " combinations possible :");

                    CalculationConstants.PrintCombination(combinations /*Share.theMarket*/);
                        
                }
                
                env.CheckInfo.searchStartTime = DateTime.Now;

                // Run through each combination possible for three options
                for (uint combinationCounter = 0; combinationCounter <= combinations.GetUpperBound(0); combinationCounter++)
                {
                    // should each combination write a different file
                    if (Share.ShouldEachCombinationRunSeparetly)
                    {
                        string com0 = Regex.Replace(combinations[combinationCounter, 0], "[^0-9]", ""); 
                        string com1 = Regex.Replace(combinations[combinationCounter, 1], "[^0-9]", "");
                        string com2 = Regex.Replace(combinations[combinationCounter, 2], "[^0-9]", "");
                        // make the name shorten - keep only the numbers
                        string additionalName = MiscConstants.NAME_SEP_CHAR + com0 + MiscConstants.NAME_SEP_CHAR + 
                            com1 + MiscConstants.NAME_SEP_CHAR + com2 + MiscConstants.NAME_SEP_CHAR;
                        // change the output file
                        env.CreateTheOutputFiles(env.theLoan, env.BorrowerProfile.profile, additionalName);
                    }

                    // Perform three option search for one combination of option types
                    env.CheckInfo.calculationStartTime = DateTime.Now;

                    // Set the search range by the user' Risk and Liquidity
                    bool rc = DefineOptionTypes(combinationCounter, env.theLoan.LoanAmount, env);
                    if (! rc)
                    {
                        WindowsUtilities.loggerMethod("NOTICE runSearch::DefineOptionTypes failed.");
                        continue;
                    }
                    Console.WriteLine();

#if SHOULD_DEBUG_LUCHSILUKIN
                    //// TBD debug - should remove from here
                    Share.ShouldPrintLog = true;
                    MiscUtilities.PrintMiscLogger("\nProducts: "
                        + env.CalculationParameters.optTypes.optionTypes[0].product.productID.stringTypeId
                        + ", " + env.CalculationParameters.optTypes.optionTypes[1].product.productID.stringTypeId
                        + ", " + (MiscUtilities.Use3ProductsInComposition() ? 
                            env.CalculationParameters.optTypes.optionTypes[2].product.productID.stringTypeId : MiscConstants.UNDEFINED_STRING));
                    // debug should remove till here
#endif

                    ThreeOptionSearch search = new ThreeOptionSearch(env);

#if SHOULD_DEBUG_LUCHSILUKIN
                    //// TBD debug - should remove from here
                    Share.ShouldPrintLog = false;
                    // debug should remove till here
#endif

                    env.CheckInfo.calculationEndTime = DateTime.Now;
                    // End of three option search for one combination of option types

                    // Print summary to file
                    PrintSummary(search.numOfCalculations, combinations, combinationCounter);
                  }

                // Get end time for software
                env.CheckInfo.softwareCloseTime = DateTime.Now;

                PrintResultsToFile();

                // manage the selected composition ordering
                SelectBestCompositions();
                
                // Close output file before end.
                env.CloseTheOutputFiles();

                if (env.PrintOptions.printMainInConsole == true)
                    {
                        if (env.resultsOutput.bestComposition != null)
                        {
                            Console.WriteLine("\nBest composition in search is for combination : "
                                            + env.resultsOutput.bestComposition.ToString());
                        }
                        else
                        {
                            // TBD: Shuky - what is the meaning?
                            Console.WriteLine("\nNo composition found in search");
                        }
                    }
                //}
            } // CanRunCalculation
            else
            {
                Console.WriteLine("NOTICE: can't run the calculation. CanRunCalculation is: " + CanRunCalculation);
            }

            PrintCounters();

            return new RunLoanDetails(env.theLoan.resultReportData);
                // RunLoanDetails(env.CheckInfo.orderID, Convert.ToInt32(CanRunCalculation), elapsedMs, env.GetOutputFileName());
        } 


        // ************************************** Define option types according to combination chosen ********************************* //

        private bool DefineOptionTypes(uint combinationToDefine, uint amount, RunEnvironment env)
        {
            int[,] combinations = MiscUtilities.GetCombinations(amount, env.CalculationParameters.ltv);
            bool rc = false;

            if (null != combinations)
            {
                int size1 = combinations.GetUpperBound(0);
                int size2 = combinations.GetUpperBound(1);


                if (MiscUtilities.Use3ProductsInComposition())
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        combinations[combinationToDefine, 0],
                        combinations[combinationToDefine, 1],
                        combinations[combinationToDefine, 2],
                        env);
                }
                else
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        combinations[combinationToDefine, 0],
                        combinations[combinationToDefine, 1],
                        MiscConstants.UNDEFINED_INT,
                        env);
                }

                rc = env.CalculationParameters.optTypes.Status;
            }

#if _OLD_CODE_
            if (MiscConstants.markets.UK == Share.theMarket)
            {
                int[,] combinations = Combinations.GetCombinationByAmountI(Share.theMarket, amount);

                if (MiscUtilities.Use3ProductsInComposition())
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        combinations[combinationToDefine, 0],
                        combinations[combinationToDefine, 1],
                        combinations[combinationToDefine, 2],
                        env);
                }
                else
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        combinations[combinationToDefine, 0],
                        combinations[combinationToDefine, 1],
                        MiscConstants.UNDEFINED_INT, 
                        env);
                }

            }
            else
            {
                if (MiscUtilities.Use3ProductsInComposition())
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        Share.combinations4market[combinationToDefine, 0],
                        Share.combinations4market[combinationToDefine, 1],
                        Share.combinations4market[combinationToDefine, 2],
                        env);
                }
                else
                {
                    env.CalculationParameters.optTypes = new OptionTypes(
                        Share.combinations4market[combinationToDefine, 0],
                        Share.combinations4market[combinationToDefine, 1],
                        MiscConstants.UNDEFINED_INT, env);
                }

            }
#endif

            if (!rc)
            {
                WindowsUtilities.loggerMethod("NOTICE DefineOptionTypes failed for combination: " + combinations[combinationToDefine, 0] +
                    ", " + combinations[combinationToDefine, 1] + ", " +
                    (MiscUtilities.Use3ProductsInComposition() ? combinations[combinationToDefine, 2].ToString() : MiscConstants.UNDEFINED_STRING));
              
            }
            else
            {
                if (rc && env.PrintOptions.printFunctionsInConsole == true)
                {
                    if (MiscUtilities.Use3ProductsInComposition())
                        Console.WriteLine("\nDefining combination for check - "
                            + env.CalculationParameters.optTypes.optionTypes[(int)Options.options.OPTX].product.productID.numberID + " "
                            + env.CalculationParameters.optTypes.optionTypes[(int)Options.options.OPTY].product.productID.numberID + " "
                            + env.CalculationParameters.optTypes.optionTypes[(int)Options.options.OPTZ].product.productID.numberID + " :\n");
                    else
                        Console.WriteLine("\nDefining combination for check - "
                             + env.CalculationParameters.optTypes.optionTypes[(int)Options.options.OPTX].product.productID.numberID + " "
                             + env.CalculationParameters.optTypes.optionTypes[(int)Options.options.OPTY].product.productID.numberID + " :\n");

                }
            }
   
            return rc;
        }


        // manage the selected composition ordering
        void SelectBestCompositions()
        {
            // store in DB and create the reports
            env.CompleteCalculation();
        }

        void PrintResultsToFile()
        {
            // write the best composition founded
            env.WriteToOutputFile("\n\nMaxProfit: " + env.MaxProfit + ", MaxBankPay: " + env.MaxBankPay + ", MinBorrowerPay: " + env.MinBorrowerPay);
            if (null != env.bestDiffComposition)
                env.WriteToOutputFile("\nBest bank profit composition:\n" + env.bestDiffComposition.ToString());
            if (null != env.bestBankComposition)
                env.WriteToOutputFile("Best bank payment composition:\n" + env.bestBankComposition.ToString());
            if (null != env.bestBorrowerComposition)
                env.WriteToOutputFile("Best borrower composition:\n" + env.bestBorrowerComposition.ToString());
            if (null != env.bestAllProfitCompositionBank)
                env.WriteToOutputFile("Best All Profit Composition Bank:\n" + env.bestAllProfitCompositionBank.ToString());
            if (null != env.bestAllProfitCompositionBorrower)
                env.WriteToOutputFile("Best All Profit Composition Borrower:\n" + env.bestAllProfitCompositionBorrower.ToString());
            if (null != env.bestAllProfitComposition)
                env.WriteToOutputFile("Best All Profit Composition:\n" + env.bestAllProfitComposition.ToString());

            env.WriteToOutputFile("\nCalculation ended at " + env.CheckInfo.softwareCloseTime);
            env.WriteToOutputFile("Software runtime " + (env.CheckInfo.softwareCloseTime - env.CheckInfo.softwareOpenTime));
            env.WriteToOutputFile("Search runtime " + (env.CheckInfo.softwareCloseTime - env.CheckInfo.searchStartTime));
        }

        void PrintCounters()
        {
            if (Share.shouldPrintCounters)
            {
                Console.WriteLine("\nSavedCompositionsCounter: " + String.Format("{0:#,###,###}", Share.SavedCompositionsCounter) +
                    "\n, CalculateLuahSilukinCounter: " + String.Format("{0:#,###,###}", Share.Option_CalculateLuahSilukinCounter) +
                    "\n, CalculatePmtCounter: " + String.Format("{0:#,###,###}", Share.CalculatePmtCounter) +
                    "\n, CalculatePmtFromCalculateLuahSilukinCounter: " + String.Format("{0:#,###,###}", Share.CalculatePmtFromCalculateLuahSilukinCounter) +
                    "\n, CalculateLuahSilukinCounterNOTInFirstTimePeriod: " + String.Format("{0:#,###,###}", Share.CalculateLuahSilukinCounterNOTInFirstTimePeriod) +
                    "\n, CalculateLuahSilukinCounterInFirstTimePeriod: " + String.Format("{0:#,###,###}", Share.CalculateLuahSilukinCounterInFirstTimePeriod) +
                    "\n, CalculateLuahSilukinCounterIndexUsedFirstTimePeriod: " + String.Format("{0:#,###,###}", Share.CalculateLuahSilukinCounterIndexUsedFirstTimePeriod) +
                    "\n, RateCounter: " + String.Format("{0:#,###,###}", Share.RateCounter) +
                    "\n, counterOfOneDivisionOfAmounts: " + String.Format("{0:#,###,###}", Share.counterOfOneDivisionOfAmounts) +
                    "\n, OptionObjectCounter: " + String.Format("{0:#,###,###}", Share.OptionObjectCounter));
            }
        }

        void PrintSummary(uint numOfCalculations, string[,] comb, uint combinationCounter)
        {
            // Print summary to console
            if (env.PrintOptions.printMainInConsole == true)
            {
                Console.WriteLine("\nDone checking combination - " + (combinationCounter + 1).ToString() + " out of: " +
                    (comb.GetUpperBound(0) + 1) + " : " +
                    comb[combinationCounter, 0] + " " +
                    comb[combinationCounter, 1] + " " +
                    (MiscUtilities.Use3ProductsInComposition() ?
                        comb[combinationCounter, 2] : MiscConstants.UNDEFINED_STRING)
                        + " :");

                Console.WriteLine("\nnumOfCalculations: " + numOfCalculations);
                Console.WriteLine("\n OptionCalculateLuahSilukinCounter: " + String.Format("{0:#,###,###}", Share.Option_CalculateLuahSilukinCounter +
                    ", CalculateLuahSilukinBorrowerCounter: " + Share.Calculation_CalculateLuahSilukinCounter +
                    ", CalculateLuahSilukinBankCounter: " + Share.Calculation_CalculateLuahSilukinBankCounter +
                    ", CalculateLuahSilukinUKCounter: " + Share.Calculation_CalculateLuahSilukinUKCounter));


                if (env.resultsOutput.bestCompositionSoFar != null)
                {
                    Console.WriteLine("\nBest composition so far :\n" + env.resultsOutput.bestCompositionSoFar.ToString());
                }
                else
                {
                    // TBD: Shuky - what is the meaning?
                    Console.WriteLine("\nNo composition found");
                }
            }

            if (env.PrintOptions.printToOutputFile == true)
            {
                string summaryToFile = null;
                if (env.resultsOutput.bestCompositionSoFar != null)
                {
                    summaryToFile += env.resultsOutput.bestCompositionSoFar.ToString();
                }
                else
                {
                    summaryToFile += (comb[combinationCounter, 0])
                                        + "," + "," + "," + "," + (comb[combinationCounter, 1])
                                        + "," + "," + "," + "," +
                                        (MiscUtilities.Use3ProductsInComposition() ? (comb[combinationCounter, 2]) : "");
                }
                env.WriteToOutputFile(summaryToFile);
            }
            if (env.resultsOutput.bestCompositionSoFar != null)
            {
                if ((env.resultsOutput.bestComposition == null) ||
                    ((env.resultsOutput.bestComposition != null) && (env.resultsOutput.bestCompositionSoFar.ttlPay < env.resultsOutput.bestComposition.ttlPay)))
                {
                    env.resultsOutput.bestComposition = env.resultsOutput.bestCompositionSoFar;
                }
                env.resultsOutput.bestCompositionSoFar = null;
            }
        }


    }
}
