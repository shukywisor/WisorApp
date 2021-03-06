﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisorLibrary.DataObjects;
using WisorLibrary.Utilities;
using static WisorLib.GenericProduct;
using static WisorLib.MiscConstants;

namespace WisorLib
{
    public class Share
    {
        public static FieldList theLoadedCriteriaFields { get; set; }
        public static FieldList theSelectedCriteriaFields { get; set; }

        public static string theCriteriaFilename { get; set; }
        public static bool shouldShowCriteriaSelectionWindow { get; set; }
        public static bool shouldShowCriteriaSelectionContinue { get; set; }


        public static ProductsList theLoadedProducts { get; set; }
        public static ProductsList theSelectedProducts { get; set; }
        public static ProductsList theAllLoadedProducts { get; set; }
        public static ProductsList theAllLoadedProductsFromFile { get; set; }
        

        public static string theProductsFilename { get; set; }
        public static bool shouldShowProductSelectionWindow { get; set; }
        public static bool shouldShowProductSelectionContinue { get; set; }

        public static bool shouldShowLoansSelectionWindow { get; set; }
        public static bool shouldShowRatesSelectionWindow { get; set; }
        //public static bool ShouldCalcTheBankProfit { get; set; }
        public static bool ShouldStoreAllCombinations { get; set; }

        public static bool shouldRunSync { get; set; }
        public static bool shouldRunLogicSync { get; set; }

        public static SelectionType theSelectionType { get; set; }
       
        public static bool shouldRunFake { get; set; }
        public static int numberOfPrintResultsInList { get; set; }
        public static int numberOfOption { get; set; }

        public static bool ShouldEachCombinationRunSeparetly { get; set; }
        

        public static int[,] combinations4market;
        public static string[,] combinationsAsString;
        public static string[] combinationsAsUniqueString;
        public static string[] theProductsNames { get; set; }
        public static double[] theProductsRates;
        public static double[] theProductsRatesSecondPeriod;
        public static double[] theProductsBankRatesSecondPeriod;
        // bank rates
        public static double[] theBankRates;

        public static double[] riskAndLiquidity { get; set; }

        public static double RiskFactor;
        public static double LiquidityFactor;
        public static double BenefitFactor;

        public static uint ProductBeneficialScoreCriteria { get; set; }
        public static uint MaxCombinationNumber { get; set; }
         

        // Reports
        public static bool ShouldStoreInDB { get; set; }
        public static bool shouldCreateShortPDFReport { get; set; }
        public static bool shouldCreateLongPDFReport { get; set; }
        //public static bool ShouldStoreHTMLReport { get; set; }
        public static uint LoansLoadFromLine { get; set; }
        public static uint LoansLoadToLine { get; set; }
        // load specific loan' IDs
        public static string LoansLoadIDsFromLine { get; set; }

        public static bool shouldDebugLoans { get; set; }
        public static bool shouldDebugLoansCalculation{ get; set; }
        public static bool shouldDebugLoansOnlyWinWin { get; set; }
        
        public static bool shouldDebugLuchSilukin { get; set; }

    /// <summary>
    /// Print some counters for performance benchmark
    /// </summary>
        public static bool shouldPrintCounters { get; set; }

        public static long counterOfOneDivisionOfAmounts;
        public static long SavedCompositionsCounter;
        public static long CalculatePmtCounter;
        public static long Option_CalculateLuahSilukinCounter;
        public static long Calculation_CalculateLuahSilukinCounter;
        public static long Calculation_CalculateLuahSilukinBankCounter;
        public static long Calculation_CalculateLuahSilukinUKCounter;
        public static long RateCounter;
        public static long OptionObjectCounter;
        public static long CalculatePmtFromCalculateLuahSilukinCounter;
        public static long CalculateLuahSilukinCounterNOTInFirstTimePeriod;
        public static long CalculateLuahSilukinCounterInFirstTimePeriod;
        public static long CalculateLuahSilukinCounterIndexUsedFirstTimePeriod;
        public static long OneDivisionOfAmountsCounter;
        public static long Calculation_CalculatePmtCounter;

        public static int TotalNumberOfLoans { get; set; }
        public static int NumberOfCanRefininceLoans { get; set; }
        public static int NumberOfCanRefininceLoansWithFee { get; set; }
        
        public static int NumberOfPositiveBeneficialLoans { get; set; }
        public static int NumberOfNoCompositionFound { get; set; }

        public static string CustomerName { get; set; }

        public static markets theMarket { get; set; }

        public static CultureInfo cultureInfo;

        public static string tempLogFile
        {
            get {
                return MiscUtilities.GetOutputDirectory() + Path.DirectorySeparatorChar + "LogNewLoans";
                /* return @"C:\tmp\LogNewLoans"; */ }
        }
        public static string summaryLogFile
        {
            get { return MiscUtilities.GetOutputDirectory() + Path.DirectorySeparatorChar + "SummaryLoans";
                /*@"C:\tmp\SummaryLoans";*/ }
        }
        public static LoggerFile theMiscLogger { get; set; }
        public static LoggerFile theSummaryFile { get; set; }
        public static LoggerFile theWinWinSummaryFile { get; set; }
        public static LoggerFile theBorrowerWinSummaryFile { get; set; }
        public static LoggerFile theBankWinSummaryFile { get; set; }
        public static LoggerFile theTotalWinSummaryFile { get; set; }

        private static string criteriaFileName;
        public static string CriteriaFileName
        {
            get {
                //if (string.IsNullOrEmpty(criteriaFileName))
                //    return MiscConstants.CRETIRIA_FILE;
                return criteriaFileName;
            }
            set
            {
                //if (File.Exists(value))
                    criteriaFileName = value;
            }
        }

        private static string productsFileName;
        public static string ProductsFileName
        {
            get
            {
                //if (string.IsNullOrEmpty(productsFileName))
                //    return MiscConstants.PRODUCTS_FILE;
                return productsFileName;
            }
            set
            {
                //if (File.Exists(value))
                    productsFileName = value;
            }
        }

        private static string loansFileName;
        public static string LoansFileName
        {
            get
            {
                //if (string.IsNullOrEmpty(loansFileName))
                //    return MiscConstants.LOAN_FILE;
                return loansFileName;
            }
            set
            {
                //if (File.Exists(value))
                    loansFileName = value;
            }
        }

        private static string ratesFileName;
        public static string RatesFileName
        {
            get
            {
                //if (string.IsNullOrEmpty(ratesFileName))
                //    return MiscConstants.RATES_FILE;
                return ratesFileName;
            }
            set
            {
                //if (File.Exists(value))
                    ratesFileName = value;
            }
        }

        private static string bankRatesFileName;
        public static string BankRatesFileName
        {
            get
            {
                //if (string.IsNullOrEmpty(bankRatesFileName))
                //    return MiscConstants.BANK_RATES_FILE;
                return bankRatesFileName;
            }
            set
            {
                //if (File.Exists(value))
                bankRatesFileName = value;
            }
        }

        private static string secondPeriodFilename;
        public static string SecondPeriodFilename
        {
            get
            {
                //if (string.IsNullOrEmpty(secondPeriodFilename))
                //    return MiscConstants.BANK_RATES_FILE;
                return secondPeriodFilename;
            }
            set
            {
                //if (File.Exists(value))
                secondPeriodFilename = value;
            }
        }

        private static string secondPeriodBankRatesFileName;
        public static string SecondPeriodBankRatesFileName
        {
            get
            {
                //    if (string.IsNullOrEmpty(secondPeriodBankRatesFileName))
                //        return MiscConstants.BANK_RATES_FILE;
                return secondPeriodBankRatesFileName;
            }
            set
            {
                //if (File.Exists(value))
                secondPeriodBankRatesFileName = value;
            }
        }

        
        private static string _BBBR_HistoricFileName;
        public static string BBBRHistoricFileName
        {
            get
            {
                if (string.IsNullOrEmpty(_BBBR_HistoricFileName))
                    return MiscConstants.HISTORIC_FILE;
                return _BBBR_HistoricFileName;
            }
            set
            {
                //if (File.Exists(value))
                _BBBR_HistoricFileName = value;
            }
        }

        private static string historicFileName;
        public static string HistoricFileName
        {
            get
            {
                if (string.IsNullOrEmpty(historicFileName))
                    return MiscConstants.HISTORIC_FILE;
                return historicFileName;
            }
            set
            {
                //if (File.Exists(value))
                    historicFileName = value;
            }
        }

        private static string combinationFileName;
        public static string CombinationFileName
        {
            get
            {
                //if (string.IsNullOrEmpty(combinationFileName))
                //    return MiscConstants.COMBINATION_FILENAME;
                return combinationFileName;
            }
            set
            {
                //if (File.Exists(value))
                    combinationFileName = value;
            }
        }

        private static string products2InCombinationFileName;
        public static string Products2InCombinationFileName
        {
            get
            {
                if (string.IsNullOrEmpty(products2InCombinationFileName))
                    return MiscConstants.COMBINATIONS_FILE;
                return products2InCombinationFileName;
            }
            set
            {
                //if (File.Exists(value))
                products2InCombinationFileName = value;
            }
        }
        

        private static string riskAndLiquidityFileName;
        public static string RiskAndLiquidityFileName
        {
            get
            {
                if (string.IsNullOrEmpty(riskAndLiquidityFileName))
                    return MiscConstants.RISK_LIQUIDITY_FILE;
                return riskAndLiquidityFileName;
            }
            set
            {
                //if (File.Exists(value))
                    riskAndLiquidityFileName = value;
            }
        }

        public static bool ShouldCreateCombinationDynamickly { get; set; }
        public static bool ShouldPrintLog { get; internal set; }
        public static string DataDirectory { get; set; }
        public static string ConfigurationFileName { get; set; }

        static int numberOfProductsInCombination;
        public static int NumberOfProductsInCombination {
            get
            {
                return numberOfProductsInCombination;
            }
            set
            {
                numberOfProductsInCombination = value;
                // for performance reasons 

                //// TBD: should be defined by:
                //// 1. the loan amount (percantage)
                //if (3 == numberOfProductsInCombination)
                //    Share.jumpBetweenAmounts = 3000;
                //else
                //    Share.jumpBetweenAmounts = 1000;
            }
        }

        public static bool ShouldCreateReportOnlyWhenWinWin { get; internal set; }
        public static bool ShouldDisplayReportOnline { get; internal set; }
        public static bool CheckPerformanceBySkippingCalculation { get; internal set; }



        // Once the user select the order, update the loaded list accordingly
        public static void OrderTheCriteriaFields()
        {
            FieldList theNewList = new FieldList();

            if (null == theSelectedCriteriaFields)
                theSelectedCriteriaFields = theLoadedCriteriaFields;

            foreach (CriteriaField cf in theSelectedCriteriaFields)
            {
                CriteriaField f = theLoadedCriteriaFields.GetField(cf.ID);
                f.index = cf.index;
                theNewList.Add(f);
            }
            theSelectedCriteriaFields = theNewList;
        }

        public static void OrderTheProductsFields()
        {
            ProductsList theNewList = new ProductsList();

            theNewList = theSelectedProducts;
            //foreach (GenericProduct p in theSelectedProducts)
            //{
            //    GenericProduct gp = theLoadedProducts.GetProduct(p.ID);
            //    theNewList.Add(gp);
            //}
            //theSelectedProducts = theNewList;
        }

        // output log settings
        public static bool printMainInConsole;
        public static bool printToOutputFile;
        public static bool printFunctionsInConsole;
        public static bool printSubFunctionsInConsole;
        public static bool printPercentageDone;

        // Loan settings
        internal static uint maximumTimeForLoan = 360;
        internal const double optionMinimumAmount = 30000;
        public static double jumpBetweenAmounts = 1000;
        internal const uint minimumTimeForLoan = 48;
        public const double largeDev = 20;
        internal const double smallDev = 1;
 
    }
}
