﻿using Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using WisorLib;
//using System.Windows.Forms;

namespace WisorLib
{

    public class MiscConstants
    {
        public const int        UNDEFINED_INT = -1;
        public const int        ILLEGAL_RATE_VALUE = -1;
        public const uint       UNDEFINED_UINT = 0;
        public const double     UNDEFINED_DOUBLE = 0;
        public const string     UNDEFINED_STRING = "";
        public const bool       UNDEFINED_BOOL = false;

        public static string    MIN_STR = "min";
        public static string    MAX_STR = "max";
        public static string    EQUAL_STR = "=";
        public static char      SEERATOR_STR = ';';
        public static char      COMMA = ',';
        public static char      DOT_STR = '.';
        public static char      PERCANTAGE_STR = '%';
        public static char      DOLLAR_STR = '$';
        public static string    CRITERIA_FILENAME = "criteria.txt";
        public static string    OUTPUT_DIR = @"..\Output";
        public static string    BIN_DIR = @"..";
        public static string    REPORTS_DIR = @"..\Reports";
        public static string    DATA_DIR = @"..\Data"; // @"..\..\..\Data";
        public static string    IMAGES_DIR = @"..\images";
        public static char      NAME_SEP_CHAR = '-';
        public static string    CSV_EXT = ".csv";
        public static string    TEXT_EXT = ".txt";
        public static string    XML_EXT = ".xml";
        public static string    PDF_EXT = ".PDF";
        public static string    HTML_EXT = ".html";
        public static string    DOTS_STR = ":";
        public static string    SEQ_STR = "#";
        public static char      SPACE_STR = ' ';
        public static string    DATE_FORMAT  = "d/M/yyyy";
        public static string    DATE_FORMAT_US = "M/d/yyyy";
        public static string    COMMA_STR = ",";
        public static string    HEB_BRACKETS1 = " ) ";
        public static string    HEB_BRACKETS2 = " ( ";
        public static string    ENG_BRACKETS1 = " ( ";
        public static string    ENG_BRACKETS2 = " ) ";

        // files to load from

        // citi:
        //public static string CRETIRIA_FILE = "CitiGui.xml"; // "Gui.xml"; //  "ClalGui.xml";
        //public static string RATES_FILE = "RateFileGeneric.csv"; // "RateFileClalOnlyApril2017.xlsx"; // ;
        //public static string BANK_RATES_FILE = "CitiRateMarginGeneric.csv"; // "MarginFileClalOnlyApril2017.xlsx";
        //public static string LOAN_FILE = "TestCases.xlsx"; // "ClalPOCDataForFinalCalculation.xlsx"; // "Citi Test cases (2).csv"; // "POC Data - Test Run.csv";
        //public static string COMBINATIONS_FILE = "Combinations.csv"; // "CombinationsIsrael.csv";

        public const string MARKET = "market";
        public static string LOGGER_FILE = "LOGGER";
        //public const string PRODUCTS_FILE = "MortgageProducts - Updated.xml";
        //public const string CRETIRIA_FILE = "ClalGui.xml"; // "Gui.xml"; 
        //public const string RATES_FILE = "RateFileClalOnlyApril2017.xlsx";
        //public const string BANK_RATES_FILE = "MarginFileClalOnlyApril2017.xlsx";
        //public const string LOAN_FILE = "ClalShort.xlsx"; //"ClalPOCDataForFinalCalculation.xlsx"; // "Citi Test cases (2).csv"; // "POC Data - Test Run.csv";
        //public const string COMBINATION_FILENAME = "CombinationsIsrael.csv";
        //public const string COMBINATION_FILENAME_2_PRODUCTS_IN_COMBINATION = "CombinationsIsrael2ProductsInCombination.csv";
        
        public const string HISTORIC_FILE = "PrimeIsrael_RateDB.xlsx"; // "HistoricIsraelPrimeRates - Copy.xlsx";
        public const string HISTORIC_BBBR_FILE = "BBBR_RateDB.xlsx"; // "HistoricIsraelPrimeRates - Copy.xlsx";
        public const string CONFIGURATION_FILE = "WisorConfiguration.xml";
        
        // Loan parameter
        public const string LOAN_AMOUNT = "loan amount";
        public const string MONTHLY_PAYMENT = "monthly payment";
        public const string PROPERTY_VALUE = "property value";
        public const string YEARLY_INCOME = "income";
        public const string AGE = "age";
        public const string MORTGAGE_TYPE = "mortgage type"; // First time buyer, 
        public const string PAYMENT_TYPE = "payment type";
        public const string MORTGAGE_PRODUCT = "mortgage product";
        public const string LOAN_FICO = "fico";
        public const string DATE_TAKEN = "date";
        public const string DESIRE_TERMINATION_MONTH = "desireterminationmonth";
        public const string SEQ_NUMBER = "sequentialnumber";
        public const string ORIGINAL_PRODUCT = "original product";
        public const string ORIGINAL_RATE = "rate";
        public const string ORIGINAL_RATE2 = "rate2";
        public const string ORIGINAL_TIME = "time";
        public const string ORIGINAL_MARGIN = "margin";
        public const string ORIGINAL_MARGIN2 = "margin2";
        public const string CUSTOMER_NAME = "CUSTOMER_NAME";
        public const string RISK_VALUE = "risk";
        public const string LIQUIDITY_VALUE = "liquidity";
        public const string PRODUCT_NAME = "product name";
        public const string CRETIRIA_FILENAME = "CRETIRIA_FILE";
        public const string LOAN_FILENAME = "LOAN_FILE";

        // configuration settings
        public const string RATES_FILENAME = "RATES_FILE";
        public const string BANK_RATES_FILENAME = "BANK_RATES_FILE";
        public const string SECOND_PERIOD_RATES_FILENAME = "SECOND_PERIOD_RATES_FILENAME";
        public const string SECOND_PERIOD_BANK_RATES_FILENAME = "SECOND_PERIOD_BANK_RATES_FILENAME";
        public const string HISTORIC_FILENAME = "HISTORIC_FILE";
        public const string BBBR_HISTORIC_FILENAME = "BBBR_HISTORIC_FILE";
        public const string COMBINATIONS_FILE = "COMBINATIONS_FILE";
        public const string COMBINATIONS_FILE_2_PRODUCTS_IN_COMBINATION = "COMBINATIONS_FILE_2_PRODUCTS_IN_COMBINATION";
        public const string RISK_LIQUIDITY_FILENAME = "RISK_LIQUIDITY_FILE";
        public const string PRODUCTS_FILENAME = "PRODUCTS_FILE";
        public const string RISK_FACTOR = "RISKFACTOR";
        public const string LIQUIDITY_FACTOR = "LIQUIDITYFACTOR";
        public const string BENEFIT_FACTOR = "BENEFITFACTOR";
        public const string BENEFIT_THRESHOLD = "BENEFITTHRESHOLD";
        public const string MAX_COMBINATIONS = "MAX_COMBINATIONS";
        public const string SHOULD_CREATE_REPORT = "SHOULD_CREATE_REPORT";
        public const string SHOULD_STORE_REPORT_IN_DB = "SHOULD_STORE_REPORT_IN_DB";
        public const string SHOULD_STORE_REPORT_AS_LONG_PDF = "SHOULD_STORE_REPORT_AS_LONG_PDF";
        public const string SHOULD_STORE_REPORT_AS_SHORT_PDF = "SHOULD_STORE_REPORT_AS_SHORT_PDF";
        public const string FROM_TO_LINES_TO_LOAD_LOANS = "FROM_TO_LINES_TO_LOAD_LOANS";
        public const string FROM_IDS_LOAD_LOANS = "FROM_IDS_LOAD_LOANS";
        public const string NUMBER_OF_PRODUCTS_IN_COMBINATION = "NUMBER_OF_PRODUCTS_IN_COMBINATION";
        public const string SHOULD_DISPLAY_REPORT_ONLINE = "SHOULD_DISPLAY_REPORT_ONLINE";
        public const string SHOULD_RUN_THE_LOGIC_SYNC = "SHOULD_RUN_THE_LOGIC_SYNC";


        internal static string LENDER_REPORT_PREFIX = "LenderReport";
        internal static string BORROWER_REPORT_PREFIX = "BorrowerReport";
        internal static string HEBREW_PREFIX = "HEB";
        internal static string ENGLISH_PREFIX = "EN";
        public const string _YES_KEY = "yes";
        public const string _NO_KEY = "no";

        internal static string shouldConsider = "shouldConsider";
        internal static string Product = "Product";
        internal static string market = "market";
        internal static string name = "name";
        internal static string hebrewName = "nameHeb";
        internal static string indexUsedFirstTimePeriod = "indexUsedFirstTimePeriod";
        internal static string indexUsedSecondTimePeriod = "indexUsedSecondTimePeriod";
        internal static string indexJumpFirstTimePeriod = "indexJumpFirstTimePeriod";
        internal static string indexJumpSecondTimePeriod = "indexJumpSecondTimePeriod";
        internal static string minTime = "minTime";
        internal static string maxTime = "maxTime";
        internal static string timeJump = "timeJump";
        internal static string firstTimePeriod = "firstTimePeriod";
        internal static string maxPercentageOfLoan = "maxPercentageOfLoan";
        internal static string benefit = "benefit";
        internal static string fixOrAdjustable = "FixOrAdjustable";
        internal static string risk = "risk";
        internal static string liquidity = "liquidity";
        internal static string mustBeUsed = "mustBeUsed";
        internal static string fee = "fee";

        public static uint DEFAULT_PERCANTAGE_OF_MONTHLY_PAYMENT = 30;
        public static uint MAX_LOAN_TIME = 360;

        // public static uint NUM_OF_PRODUCTS_IN_COMBINATION = 3;
        public static uint MAX_NUM_OF_COMBINATION_TO_SELECT = 3;

        public const int NumberOfProfiles = 6;
        public const int NumberOfYearsFrProduct = 32; // 27;

        //public static double BANK_RATE = 0.005;
        public static double MADAD_Inflation = 0.005; /*0.018;*/
        public static double BANK_PRIME_RATE_FACTOR = 0.001;
        //public static double BANK_BBBR_RATE_FACTOR = 0.001;

        // composition headers constants
        public const int MinProductPercantage = 0;
        public const int MidProductPercantage = 50;
        public const int MaxProductPercantage = 100;

        // enable to select the right composition by name
        internal const string BEST_DIFF_COMPOSITION = "bestDiffComposition";
        internal const string BEST_BANK_COMPOSITION = "bestBankComposition";
        internal const string BEST_BORROWER_COMPOSITION = "bestBorrowerComposition";
        internal const string BEST_ALL_PROFIT_COMPOSITION = "allProfitComposition";
        internal const string BEST_ALL_PROFIT_COMPOSITION_BORROWER = "allProfitCompositionBorrower";
        internal const string BEST_ALL_PROFIT_COMPOSITION_BANK = "allProfitCompositionBank";

        // default API parameters
        public static uint DEAFULT_FICO_VALUE = 234;
        public static uint DEAFULT_AGE_VALUE = 34;
        public static double DEAFULT_BORROWER_RATE_VALUE = 0.03625;
        public static double DEAFULT_BANK_RATE_VALUE = -0.015;
        public static uint DEAFULT_LOAN_TIME_VALUE = 240;

        // type of the selection window
        public enum SelectionType { ReadCretiria = 0, ReadProducts , ReadRates , ReadLoansFile};

        // file name types
        public enum FileType { XML = 0, HTML, PDF, CSV};

        // TBD - shuky
        public enum indices { NONE = 0, MADAD = 1, PRIME = 2, CPI = 3, FED = 4, LIBOR = 5,
            EUROBOR = 6, BBBR = 7, MAKAM = 8, OTHER = 9}; // Are the options in the code or pulled from outside DB?

        // Risk and Liquidity
        public const int RISK_LIQUIDITY_HEADER = 3;
        public enum Risk { MinimumRisk1, LessRisk2, MediumRisk3, MoreRisk4, MaximumRisk5, NONERisk}; 
        public enum Liquidity { MinimumLiquidity1, LessLiquidity2, MediumLiquidity3, MoreLiquidity4, MaximumLiquidity5 , NONELiquidity};
        public const string RISK_LIQUIDITY_FILE = "RiskLiquidityCiti.xlsx";

        public static string HEBREW_STR { get { return "he-IL"; }  }
        public static string GENERIC_CONFIG { get { return "GENERIC_CONFIG"; } }

        public static double FIX_PRODUCT_CLOSING_FEE_PERCANTAGE { get { return 0.01; } }
        public static double NOTFIX_PRODUCT_CLOSING_FEE_PERCANTAGE { get { return 0.03; } }

        public static double JUMP_PERCANTAGE_PER_LOAN_AMOUNT = 0.005;
        public static double MIN_JUMP_BETWEEN_AMOUNTS = 1000;

        public const string COMBINATION_PER_AMOUNT = "AmountCombinations";
        public const string GENERIC_COMBINATION = "Combinations";
        

        public const string MARKET_START = "MARKET_START";
        public const string MARKET_END = "MARKET_END";
        public const string MARKET_SETUP = "MARKET_SETUP";
        public const string READ_LOANS_FROM_LINE = "READ_LOANS_FROM_LINE";
        public const string READ_LOANS_TO_LINE = "READ_LOANS_TO_LINE";
            

        public enum Benefit { Benefit1, Benefit2, Benefit3, Benefit4, Benefit5, NONEBenefit };
        public enum FixOrAdjustable { FIX, ADJUSTABLE };
        public enum markets { USA, UK, ISRAEL, OTHER, NONE }; // Are the options in the code or pulled from outside DB?
        public enum indexJumps
        {
            NONE = 0,  WEEK = 1, MONTHS1 = 2, MONTHS3 = 3, MONTHS6 = 4, MONTHS12 = 5, MONTHS24 = 6,
            MONTHS30 = 7, MONTHS36 = 8, MONTHS60 = 9, MONTHS84 = 10, MONTHS120 = 11, OTHER = 12, DAY = 13
        };

    }

}

