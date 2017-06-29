﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisorLib;
using WisorLibrary.DataObjects;
using WisorLibrary.Utilities;

/*
 * Collect all the loans with the same ID and calculate the sum of remaining loan, the monthly payment
 * and create a new loan with those parameters
 */
namespace WisorLibrary.Logic
{
 
    class LoanContainer
    {
        LoanList _loans;

        public LoanContainer()
        {
            _loans = new LoanList();
        }

        public void Add(loanDetails loan)
        {
            _loans.Add(loan);
        }

        public LoanList GroupLoansByID()
        {
            LoanList tmpLoans = new LoanList(), returnLoans = new LoanList();
            string id = MiscConstants.UNDEFINED_STRING;
            loanDetails calcLoan;

            // TBD - print log . Should be removed
            //PrintOriginalLoanLuahSilukin(loans);

            for (int i = 0; i < _loans.Count; i++)
            {
                // is it the same ID
                if (MiscConstants.UNDEFINED_STRING == id || id == _loans[i].ID)
                {
                    tmpLoans.Add(_loans[i]);
                }
                else
                {
                    calcLoan = AccumulaLoanData(tmpLoans);
                    returnLoans.Add(calcLoan);
                    tmpLoans.Clear();
                    tmpLoans.Add(_loans[i]);
                }
                id = _loans[i].ID;
            }
            if (0 < tmpLoans.Count)
            {
                calcLoan = AccumulaLoanData(tmpLoans);
                returnLoans.Add(calcLoan);
            }

            WindowsUtilities.loggerMethod("NOTICE: LoanContainer got: " + _loans.Count + " loans, convert to: " + returnLoans.Count);
            Console.WriteLine("The Original loans:");
            for (int i = 0; i < _loans.Count; i++)
            {
                Console.WriteLine("Loan: " + (i+1).ToString() + " : " + _loans[i].ToString());
            }

            Console.WriteLine("\nThe grouped loans:\n");
            for (int i = 0; i < returnLoans.Count; i++)
            {
                Console.WriteLine("Loan: " + (i + 1).ToString() + " : " + returnLoans[i].ToString());
                // debug print
                if (null != Share.theMiscLogger)
                {
                    Share.theMiscLogger.PrintLog("The oroginal Loan: " + (i + 1).ToString() + " : " + returnLoans[i].ToString());
                    Share.theMiscLogger.PrintLog(returnLoans[i].resultReportData.ToString());
                }
            }
            // calculate the luaah silukin 
            //GetTheLuahSilukin(ref returnLoans);


            return returnLoans;
        }


        loanDetails AccumulaLoanData(LoanList collectedLoans)
        {
            ResultReportData resultData = new ResultReportData();
            loanDetails ld = new loanDetails();
            ld.resultReportData = new ResultReportData();

            if (0 < collectedLoans.Count) {
                // ld = collectedLoans[0];

                uint originalAmount = MiscConstants.UNDEFINED_UINT,
                    monthlyPayment = MiscConstants.UNDEFINED_UINT,
                    yearlyIncome = MiscConstants.UNDEFINED_UINT;
                uint PayUntilToday = MiscConstants.UNDEFINED_UINT,
                    PayFuture = MiscConstants.UNDEFINED_UINT,
                    RemaingLoanAmount = MiscConstants.UNDEFINED_UINT,
                    MonthlyPaymentCalc = MiscConstants.UNDEFINED_UINT,
                    BankPayUntilToday = MiscConstants.UNDEFINED_UINT,
                    BankPayFuture = MiscConstants.UNDEFINED_UINT;

                try
                {
                    for (int i = 0; i < collectedLoans.Count; i++)
                    {
                        yearlyIncome = collectedLoans[i].YearlyIncome;
                        originalAmount += collectedLoans[i].OriginalLoanAmount;
                        monthlyPayment += collectedLoans[i].DesiredMonthlyPayment;
                        // calculate luch silukim
                        Calculations.CalculateLuahSilukinFullAll(collectedLoans[i].indices,
                             collectedLoans[i].OriginalRate, collectedLoans[i].OriginalMargin, collectedLoans[i].OriginalInflation,
                             collectedLoans[i].OriginalLoanAmount, collectedLoans[i].OriginalTime,
                             collectedLoans[i].DateTaken, ref resultData);

                        // borrower data
                        PayUntilToday += resultData.PayUntilToday;
                        PayFuture += resultData.PayFuture;
                        RemaingLoanAmount += resultData.RemaingLoanAmount;
                        MonthlyPaymentCalc += resultData.MonthlyPaymentCalc;
                        // bank data
                        BankPayUntilToday += resultData.BankPayUntilToday;
                        BankPayFuture += resultData.BankPayFuture;
                    }

                    UpdateLoanData(ref ld, collectedLoans[0], PayUntilToday, PayFuture, RemaingLoanAmount,
                        MonthlyPaymentCalc, BankPayUntilToday, BankPayFuture, yearlyIncome, originalAmount);

                }
                catch (Exception e)
                {
                    WindowsUtilities.loggerMethod("ERROR: AccumulaLoandata got Exception: " + e.ToString());
                }

            }
            if (null != Share.theMiscLogger)
            {
                Share.theMiscLogger.PrintLog("\n\nThe Accumulated loan:\n");
                Share.theMiscLogger.PrintLog(ld.ToString());
            }
        
            return ld;
        }



        void  UpdateLoanData(ref /*not needed but for the sake of god will...*/ 
                        loanDetails ld, loanDetails loan, uint PayUntilToday, uint PayFuture,
                        uint RemaingLoanAmount, uint MonthlyPaymentCalc, uint BankPayUntilToday, uint BankPayFuture,
                        uint yearlyIncome, uint originalAmount)
        {
            //ld.LoanAmount = originalAmount;
            ld.resultReportData.BankName = Share.CustomerName;
            ld.resultReportData.ID = ld.ID = loan.ID;
            // the age is passing...
            int years = MiscUtilities.CalculateYearsBetweenDates(loan.DateTaken, DateTime.Now);
            ld.resultReportData.BorrowerAge = ld.BorrowerAge = loan.BorrowerAge + (uint) years;
            ld.resultReportData.OriginalRate = ld.OriginalRate = loan.OriginalRate;
            ld.resultReportData.OriginalTime = ld.OriginalTime = loan.OriginalTime;
            ld.resultReportData.fico = ld.fico = loan.fico;
            ld.resultReportData.FirstMonthlyPMT = loan.resultReportData.FirstMonthlyPMT;
            ld.resultReportData.DateTaken = ld.DateTaken = DateTime.Now; // loan.DateTaken;
            ld.resultReportData.PropertyValue = ld.PropertyValue = loan.PropertyValue;
            ld.indices = loan.indices;
            ld.liquidity = loan.liquidity;
            ld.risk = loan.risk;
            ld.OriginalDateTaken = loan.OriginalDateTaken;
            ld.resultReportData.DesireTerminationMonth = ld.DesireTerminationMonth = loan.DesireTerminationMonth;
            ld.resultReportData.OriginalInflation = ld.OriginalInflation = loan.OriginalInflation;
            ld.resultReportData.OriginalMargin = ld.OriginalMargin = loan.OriginalMargin;
            ld.resultReportData.StartTime = loan.resultReportData.StartTime;
            ld.resultReportData.RemaingLoanAmount = ld.resultReportData.LoanAmount = ld.LoanAmount = RemaingLoanAmount;
            ld.resultReportData.YearlyIncome = ld.YearlyIncome = yearlyIncome;
            ld.resultReportData.OriginalLoanAmount = ld.OriginalLoanAmount = originalAmount;
            // the new monthly amount should be the sum of the all monthly payments today
            ld.resultReportData.DesiredMonthlyPayment = ld.DesiredMonthlyPayment = MonthlyPaymentCalc;
            
            if (0 < ld.YearlyIncome)
                ld.resultReportData.PTI = (double)ld.DesiredMonthlyPayment / ld.YearlyIncome;
            ld.resultReportData.LTV = (double)ld.LoanAmount / ld.PropertyValue;
            ld.resultReportData.PayUntilToday = PayUntilToday;
            ld.resultReportData.PayFuture = PayFuture;
            ld.resultReportData.BankPayUntilToday = BankPayUntilToday;
            ld.resultReportData.BankPayFuture = BankPayFuture;
            ld.resultReportData.EstimateFuturePay = ld.resultReportData.PayFuture;
            ld.resultReportData.EstimateProfitSoFar =
                ld.resultReportData.PayUntilToday - ld.resultReportData.BankPayUntilToday;
            ld.resultReportData.EstimateProfitPercantageSoFar =
                (double)ld.resultReportData.EstimateProfitSoFar / ld.OriginalLoanAmount;
            ld.resultReportData.EstimateTotalProfit =
                ld.resultReportData.PayUntilToday + ld.resultReportData.PayFuture -
                ld.resultReportData.BankPayUntilToday - ld.resultReportData.BankPayFuture;
            ld.resultReportData.EstimateTotalProfitPercantage =
                (double)ld.resultReportData.EstimateTotalProfit / ld.OriginalLoanAmount;
            ld.resultReportData.EstimateFutureProfit =
                ld.resultReportData.PayFuture - ld.resultReportData.BankPayFuture;
            ld.resultReportData.EstimateFutureProfitPercantage =
                (double)ld.resultReportData.EstimateFutureProfit / ld.OriginalLoanAmount;
        }

        //void PrintOriginalLoanLuahSilukin(LoanList loans/*, LoanList groupedLoans*/)
        //{
        //    ResultReportData resultData = new ResultReportData();

        //    if (null != Share.theMiscLogger)
        //    {
        //        for (int i = 0; i < loans.Count; i++)
        //        {
        //            Share.theMiscLogger.PrintLog("\nLoan: " + (i + 1).ToString() + " : " + loans[i].ToString() + "\n");
        //            Share.theMiscLogger.PrintLog("\nCalculateLuahSilukinFullAll results:\n");
        //            Calculations.CalculateLuahSilukinFullAll(loans[i].indices,
        //                loans[i].OriginalRate, loans[i].OriginalMargin, loans[i].OriginalInflation,
        //                loans[i].OriginalLoanAmount, loans[i].OriginalTime,
        //                loans[i].DateTaken, ref resultData);
        //            Share.theMiscLogger.PrintLog("\n\n");
        //        }

        //        //Share.theMiscLogger.PrintLog("\nThe grouped loans:\n");
        //        //for (int i = 0; i < groupedLoans.Count; i++)
        //        //{
        //        //    Share.theMiscLogger.PrintLog("\nLoan: " + (i + 1).ToString() + " : " + groupedLoans[i].ToString() + "\n");
        //        //    Share.theMiscLogger.PrintLog("\n\n");
        //        //}
        //    }
        //}


        //void GetTheLuahSilukin(ref LoanList loans)
        //{
        //    ResultReportData resultData = new ResultReportData();

        //    try
        //    {
        //        for (int i = 0; i < loans.Count; i++)
        //        {
        //            Calculations.CalculateLuahSilukinFullAll(loans[i].indices,
        //                loans[i].OriginalRate, loans[i].OriginalMargin, loans[i].OriginalInflation,
        //                loans[i].OriginalLoanAmount, loans[i].OriginalTime,
        //                loans[i].DateTaken, ref resultData);

        //            loans[i].resultReportData.EstimateFuturePay = resultData.EstimateFuturePay;
        //            loans[i].resultReportData.EstimateProfitSoFar = resultData.EstimateProfitSoFar;
        //            loans[i].resultReportData.EstimateProfitPercantageSoFar = resultData.EstimateProfitPercantageSoFar;
        //            loans[i].resultReportData.EstimateTotalProfit = resultData.EstimateTotalProfit;
        //            loans[i].resultReportData.EstimateTotalProfitPercantage = resultData.EstimateTotalProfitPercantage;
        //            loans[i].resultReportData.EstimateFutureProfit = resultData.EstimateFutureProfit;
        //            loans[i].resultReportData.EstimateFutureProfitPercantage = resultData.EstimateFutureProfitPercantage;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        WindowsUtilities.loggerMethod("ERROR: GetTheLuahSilukin got Exception: " + e.ToString());
        //    }
        //}

    }
}
