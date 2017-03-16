﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WisorLibrary.Logic;
using static WisorLib.MiscConstants;

namespace WisorLib
{
    public class Option
    {

        // General Parameters
        public int optType;
        public double optAmt = 0;
        public uint optTime = 0;
        public double optPmt = -1;

        //private double[] optRateList = new double[360];
        public double optTtlPay = 0;
        public double optTtlRatePay = 0;
        public double optTtlPrincipalPay = 0;
        private bool printOrNo = false;
        
        //private double optRate = -1;
        public double optRateFirstPeriod = -1;
        public double optRateSecondPeriod = -1;

        public double optRateForRemainingAmount = -1;

        // Shuky changes:
        //public double inflation = -1;
        public double indexFirstPeriod { get; set; }
        public double indexSecondPeriod { get; set; }

        private double optBankRateFirstPeriod = -1;
        private double optBankRateSecondPeriod = -1;
        public double optBankTtlPay = 0;

        public GenericProduct product;

        public Option(int optionType, double optionAmount, uint optionTime, RunEnvironment env)
        {
            //Console.WriteLine("Option ctor optionType: " + optionType + ", optionAmount: " + optionAmount + ", optionTime: " + optionTime);
            Interlocked.Add(ref Share.OptionObjectCounter, 1);

            optType = optionType;
            optAmt = optionAmount;
            optTime = optionTime;

            product = MiscConstants.GetProduct(optionType);
            bool rc = GetInflationsForOption(optType);

            optRateFirstPeriod = FindInterestRate();
            if (-1 == optRateFirstPeriod)
            {
                // TBD: Uston we have a problem
            }
            optPmt = CalculatePmt(optAmt, optTime, optRateFirstPeriod, env);
        }

   




        // **************************************************************************************************************************** //
        // **************************************** Getting Inflation According to Option Type **************************************** //

        //should be called GetIndex4Option
        // and return 2 values for: indexUsedFirstTimePeriod and indexUsedSecondTimePeriod
        // according to the Product -> typeId
        //indices { MADAD, PRIME, CPI, FED, LIBOR, EUROBOR, BBBR, OTHER, NONE

        private bool GetInflationsForOption(int id)
        {
            bool rc = false;
            GenericProduct product = GetProduct(id);
            if (null != product)
            {
                indices indexUsedFirstTimePeriod = product.indexUsedFirstTimePeriod;
                indices indexUsedSecondTimePeriod = product.indexUsedSecondTimePeriod;
                indexFirstPeriod = MiscConstants.GetIndexRateForOption(indexUsedFirstTimePeriod);
                indexSecondPeriod = MiscConstants.GetIndexRateForOption(indexUsedSecondTimePeriod);
                rc = true;
            }
            else
            {
                WindowsUtilities.loggerMethod("GetInflationsForOption: failed to find product id: " + id);
                indexFirstPeriod = indexSecondPeriod = ILLEGAL_RATE_VALUE;
            }
            return rc;
        }


  

        // **************************************************************************************************************************** //
        // ***************************** Calculating Interest Rate According to Option Type and Option Time *************************** //

        // get the rate by the product, the borrower profile and time
        private double FindInterestRate()
        {
            double rate = MiscConstants.UNDEFINED_DOUBLE;

            if ((MiscConstants.UNDEFINED_INT != optType ) && (optTime >= CalculationConstants.minimumTimeForLoan))
            {
                // Instead of looking for product in local class -> lookup via xml
                // Go to local Rates class, lookup value and plug in
                 rate = Rates.FindRateForKey(product.productID.numberID, BorrowerProfile.borrowerProfile, (int)optTime/12-4);
            }
            else
            {
                if (MiscConstants.UNDEFINED_INT == optType)
                {
                    throw new System.ArgumentOutOfRangeException("Option Type Out of Range. optType: " + optType);

                }
                else
                {
                    throw new System.ArgumentOutOfRangeException("Option Time Out of Range. optTime: " + optTime.ToString());
                }
            }

            return rate;
        }





        // **************************************************************************************************************************** //
        // *************************************************** Show Interest Rate ***************************************************** //

        public double ShowRate()
        {
            if ((optTime >= CalculationConstants.minimumTimeForLoan) && (MiscConstants.UNDEFINED_INT != optType))
            {
                if (optRateFirstPeriod == -1)
                {
                    optRateFirstPeriod = FindInterestRate();
                    return optRateFirstPeriod;
                }
                else if (optRateFirstPeriod >= 0)
                {
                    return optRateFirstPeriod;
                }              
                else
                {
                    throw new System.ArgumentOutOfRangeException("Option Rate Out of Range");
                }
            }
            else
            {
                if (MiscConstants.UNDEFINED_INT == optType)
                {
                    throw new System.ArgumentOutOfRangeException("Option Type Out of Range");
                }
                else
                {
                    throw new System.ArgumentOutOfRangeException("Option Time Out of Range");
                }
            }
        }





        // **************************************************************************************************************************** //
        // ******************************* Calculating PMT According to Option Type and Time and Rate ********************************* //

        public double CalculatePmt(double amtForCalc, uint timeForCalc, double interestRateForCalc,
            RunEnvironment env)
        {
            double currInterest = MiscConstants.UNDEFINED_DOUBLE;

            Interlocked.Add(ref Share.CalculatePmtCounter, 1);
            //env.CalculatePmtCounter++;

            if (false) // fake the calculation examine the performance
            {
                double i2 = ((indexFirstPeriod /*inflation*/ / 12 * 100000000) - ((indexFirstPeriod /*inflation*/ / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                double monthlyPmt2 = ((amtForCalc * (1 + i2)) / timeForCalc);
                monthlyPmt2 = ((monthlyPmt2 * 100000) - ((monthlyPmt2 * 100000) % 1)) / 100000; // Instead of Math.Round
                return monthlyPmt2;
            }

            //Console.WriteLine("CalculatePmt amtForCalc: " + amtForCalc + ", timeForCalc: " + timeForCalc + ", interestRateForCalc: " + interestRateForCalc);
            if ((MiscConstants.UNDEFINED_INT != optType) && (amtForCalc > 0) && (timeForCalc > 0))                                          
            {
                if ((amtForCalc > 0) && (timeForCalc > 0))
                {
                    if (product.firstTimePeriod <= timeForCalc)
                    {
                        currInterest = optRateFirstPeriod = interestRateForCalc; //  FindInterestRate();
                    }
                    else
                    {
                        currInterest = optRateSecondPeriod = interestRateForCalc; //  FindInterestRate();
                    }
                }
                if (printOrNo == true)
                {
                    WindowsUtilities.loggerMethod("Option:\nType = " + product.name /*Options.optionTypes[optType]*/ + "\nAmount = " + amtForCalc
                                            + "\nTime in months = " + timeForCalc + "\nYearly Rate = " + interestRateForCalc
                                            + "\nYearly Inflation = " + indexFirstPeriod /*inflation*/ + " \n");
                }
                double i = ((indexFirstPeriod /*inflation*/ / 12 * 100000000) - ((indexFirstPeriod /*inflation*/ / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round

                if (0 == currInterest)
                {

                    double monthlyPmt = ((amtForCalc * (1 + i)) / timeForCalc);
                    monthlyPmt = ((monthlyPmt * 100000) - ((monthlyPmt * 100000) % 1)) / 100000; // Instead of Math.Round
                    if (printOrNo == true)
                    {
                        double r = 0;
                        double calcPow = 0;
                        WindowsUtilities.loggerMethod("Monthly Rate = " + r + "\nPower = " + calcPow + "\nMonthly Inflation = " + i + "\nMonthly Payment = " + monthlyPmt + " \n");
                    }
                    return monthlyPmt;
                }
                else if (currInterest > 0)
                {
                    double r = ((currInterest / 12 * 100000000) - ((currInterest / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                    double calcPow = Math.Pow((1 + r), timeForCalc);
                    double monthlyPmt = ((amtForCalc * (1 + i)) * (r * calcPow) / (calcPow - 1));
                    monthlyPmt = ((monthlyPmt * 100000) - ((monthlyPmt * 100000) % 1)) / 100000; // Instead of Math.Round

                    if (printOrNo == true)
                    {
                        WindowsUtilities.loggerMethod("Monthly Rate = " + r + "\nPower = " + calcPow + "\nMonthly Inflation = " + i + "\nMonthly Payment = " + monthlyPmt + " \n");
                    }
                    return monthlyPmt;
                }
                else
                {
                    throw new System.ArgumentOutOfRangeException("Rate Out of Range");
                }                

            }
            else
            {
                if (MiscConstants.UNDEFINED_INT == optType)
                {
                    throw new System.ArgumentOutOfRangeException("Option Type Out of Range");
                }
                else if (optAmt <= CalculationConstants.optionMinimumAmount)
                {
                    throw new System.ArgumentOutOfRangeException("Option Amount Out of Range");
                }
                else
                {
                    throw new System.ArgumentOutOfRangeException("Option Time Out of Range");
                }
            }
        }





        // **************************************************************************************************************************** //
        // ********************************** PRIVATE - Calculating full luah silukin for option ************************************** //

  
        private void CalculateLuahSilukin(double rateFirstPeriod, double rateSecondPeriod, out double ttlPay, RunEnvironment env)
        {
            //Console.WriteLine("CalculateLuahSilukin product.firstTimePeriod: " + product.firstTimePeriod);
            ttlPay = 0;
            Interlocked.Add(ref Share.CalculateLuahSilukinCounter, 1);
            //env.CalculateLuahSilukinCounter++;

            if (product.firstTimePeriod == 0)
            {
                Interlocked.Add(ref Share.CalculateLuahSilukinCounterInFirstTimePeriod, 1);
                if (product.indexUsedFirstTimePeriod == 0)
                {
                    optTtlPrincipalPay = optAmt;
                    ttlPay = optTime * Math.Round(optPmt, 2);
                    optTtlRatePay = ttlPay - optTtlPrincipalPay;
                    Interlocked.Add(ref Share.CalculateLuahSilukinCounterIndexUsedFirstTimePeriod, 1);
                }
                else
                {
                    double r = ((rateFirstPeriod / 12 * 100000000) - ((rateFirstPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                    double i = ((indexFirstPeriod / 12 * 100000000) - ((indexFirstPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round

                    double monthlyPmt = Math.Round(optPmt, 2);
                    double startingAmount = optAmt;

                    for (uint months = 1; months <= optTime; months++)
                    {
                        double ratePmt = Math.Round((startingAmount * (1 + i) * r), 2);
                        double principalPmt = monthlyPmt - ratePmt;
                        optTtlPrincipalPay += principalPmt;
                        optTtlRatePay += ratePmt;
                        ttlPay += monthlyPmt;
                        startingAmount = Math.Round((((startingAmount) * (1 + i)) - principalPmt), 2);
                        if (months < optTime)
                        {
                            Interlocked.Add(ref Share.CalculatePmtFromCalculateLuahSilukinCounter, 1);
                            monthlyPmt = Math.Round(CalculatePmt(startingAmount, (optTime - months), rateFirstPeriod, env), 2);
                        }
                    }
                }
            }
            else
            {
                Interlocked.Add(ref Share.CalculateLuahSilukinCounterNOTInFirstTimePeriod, 1);

                double r = ((rateFirstPeriod / 12 * 100000000) - ((rateFirstPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                double i = ((indexFirstPeriod / 12 * 100000000) - ((indexFirstPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round

                double monthlyPmt = Math.Round(optPmt, 2);
                double startingAmount = optAmt;
                rateSecondPeriod = rateFirstPeriod;
                // TBD Omri
                //optRateSecondPeriod = CalculateInterestRate4SecondPeriod()

                for (uint months = 1; months <= product.firstTimePeriod; months++)
                {
                    double ratePmt = Math.Round((startingAmount * (1 + i) * r), 2);
                    double principalPmt = monthlyPmt - ratePmt;
                    optTtlPrincipalPay += principalPmt;
                    optTtlRatePay += ratePmt;
                    ttlPay += monthlyPmt;
                    startingAmount = Math.Round((((startingAmount) * (1 + i)) - principalPmt), 2);
                    if (months < optTime)
                    {
                        Interlocked.Add(ref Share.CalculatePmtFromCalculateLuahSilukinCounter, 1);
                        monthlyPmt = Math.Round(CalculatePmt(startingAmount, (optTime - months), rateFirstPeriod, env), 2);
                    }
                }
                r = ((rateSecondPeriod / 12 * 100000000) - ((rateSecondPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                i = ((indexSecondPeriod / 12 * 100000000) - ((indexSecondPeriod / 12 * 100000000) % 1)) / 100000000; // Instead of Math.Round
                for (uint months = product.firstTimePeriod + 1; months <= optTime; months++)
                {
                    double ratePmt = Math.Round((startingAmount * (1 + i) * r), 2);
                    double principalPmt = monthlyPmt - ratePmt;
                    optTtlPrincipalPay += principalPmt;
                    optTtlRatePay += ratePmt;
                    ttlPay += monthlyPmt;
                    startingAmount = Math.Round((((startingAmount) * (1 + i)) - principalPmt), 2);
                    if (months < optTime)
                    {
                        Interlocked.Add(ref Share.CalculatePmtFromCalculateLuahSilukinCounter, 1);
                        monthlyPmt = Math.Round(CalculatePmt(startingAmount, (optTime - months), rateSecondPeriod, env), 2);
                    }
                }
            }
        }





        // **************************************************************************************************************************** //
        // ************************************* PUBLIC - Calculating full luah silukin for option ************************************ //

        public double GetTtlPay(RunEnvironment env)
        {
            if (optTtlPay > 0)
            {
                return optTtlPay;
            }
            else
            {
                CalculateLuahSilukin(optRateFirstPeriod, optRateSecondPeriod, out optTtlPay, env);
                return optTtlPay;
            }
        }
        
        public void SetBankRate(double margin)
        {
            optBankRateFirstPeriod = optRateFirstPeriod - margin;
            optBankRateSecondPeriod = optRateSecondPeriod - margin;
        }

        public double GetBankTtlPay()
        {
           return Calculations.CalculateLuahSilukin2(optBankRateFirstPeriod, optBankRateSecondPeriod,
                (int) product.firstTimePeriod, product.indexUsedFirstTimePeriod,
                optAmt, (int) optTime, optPmt,
                indexFirstPeriod, indexSecondPeriod, optType, printOrNo);
         }
        







        // **************************************************************************************************************************** //
        // *************************************************** Print Option To String ************************************************* //

        public override string ToString()
        {
            //return "(" + optType + "," + optAmt + "," + optTime + "," + optRate + "," + optPmt + ")";

            //return (optType + 4).ToString() + "," + optAmt + "," + optTime + "," + inflationStr + "," + optRate
            //            + "," + (int)optPmt + "," + (int)optTtlPay;
            string name = GenericProduct.GetProductName(optType);
            return name + "," + optAmt + "," + optTime + "," + optRateFirstPeriod;
            // return (optType + 4).ToString() + "," + optAmt + "," + optTime + "," + optRateFirstPeriod;
        }

        public string ToString2()
        {
            //return "(" + optType + "," + optAmt + "," + optTime + "," + optRate + "," + optPmt + ")";

            //return (optType + 4).ToString() + "," + optAmt + "," + optTime + "," + inflationStr + "," + optRate
            //            + "," + (int)optPmt + "," + (int)optTtlPay;
            return (optType + 4).ToString() + ":" + optAmt + ":" + optTime + ":" + optRateFirstPeriod;
        }


    }
}
