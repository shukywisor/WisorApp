﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using WisorLibrary.Utilities;

namespace WisorLib
{
    class ThreeOptionSearch
    {
        // General Parameters
        private double minAmtOptX = -1;
        private double maxAmtOptX = -1;
        private double minAmtOptY = -1;
        public double maxAmtOptY = -1;
        private double minAmtOptZ = -1;
        public double maxAmtOptZ = -1;
        private OneDivisionOfAmounts searchOneDivisionOfAmounts = null;
        
        // Counters
        private double amtYCounter = 0;
        public uint numOfCalculations;




        public ThreeOptionSearch(RunEnvironment env)
        {
            minAmtOptX = env.CalculationParameters.minAmts[(int)Options.options.OPTX];
            maxAmtOptX = env.CalculationParameters.maxAmts[(int)Options.options.OPTX]; 
            minAmtOptY = env.CalculationParameters.minAmts[(int)Options.options.OPTY];
            maxAmtOptY = env.CalculationParameters.maxAmts[(int)Options.options.OPTY];

            // number of products in composition
            if (MiscUtilities.Use3ProductsInComposition())
            {
                minAmtOptZ = env.CalculationParameters.minAmts[(int)Options.options.OPTZ];
                maxAmtOptZ = env.CalculationParameters.maxAmts[(int)Options.options.OPTZ];
            }
            if (env.PrintOptions.printFunctionsInConsole == true)
            {
                Console.WriteLine("\nPerforming three option search ...\n");
            }

            if (MiscUtilities.Use3ProductsInComposition())
                PerformFullThreeOptionSearch(env);
            else
                PerformFull2OptionSearch(env);
        }






        // **************************************************************************************************************************** //
        // ***************** PRIVATE - Performs Full Search for Three Options According to Limit Amounts for Option X ***************** //

        
        private void PerformFull2OptionSearch(RunEnvironment env)
        {
            double opt2Amt = MiscConstants.UNDEFINED_DOUBLE;
            double percentDone = 0;
            double counter = 0;
            double loopCount = ((maxAmtOptX - minAmtOptX) / CalculationConstants.jumpBetweenAmounts) + 1;
            Console.WriteLine("Max Amount = " + maxAmtOptX + "\nMin Amount = " + minAmtOptX + "\nLoop Counter = " + loopCount);
            numOfCalculations = 0;

            // Begin loop for Option 1 amount
            for (double opt1Amt = minAmtOptX; opt1Amt <= maxAmtOptX; opt1Amt += CalculationConstants.jumpBetweenAmounts)
            {
                numOfCalculations++;
                counter++;
                if (((env.CalculationParameters.loanAmtWanted - opt1Amt) >= minAmtOptY) &&
                        ((env.CalculationParameters.loanAmtWanted - opt1Amt) <= maxAmtOptY))
                {
                    opt2Amt = env.CalculationParameters.loanAmtWanted - opt1Amt;
                    // Perform main thing here ...
                    searchOneDivisionOfAmounts = new OneDivisionOfAmounts(opt1Amt, opt2Amt, MiscConstants.UNDEFINED_DOUBLE, env);
                }

                // Show progress and  calculate remaining time
                if (env.PrintOptions.printPercentageDone == true)
                {
                    percentDone = (counter / loopCount) * 100;
                    percentDone = ((percentDone * 100) - ((percentDone * 100) % 1)) / 100;
                    Console.Write("\n" + counter + " / " + loopCount + " -> AmountX = " + opt1Amt + " | AmountY = " + opt2Amt + " (" + percentDone + " % done)");
                }

            }
        }

        private void PerformFullThreeOptionSearch(RunEnvironment env)
        {
            DateTime tStart = DateTime.Now;

            double percentDone = 0;
            double loopCount = ((maxAmtOptY - minAmtOptY) / CalculationConstants.jumpBetweenAmounts) + 1;
            double percentForOneLoop = ((((1 / loopCount) * 100) * 100) - ((((1 / loopCount) * 100) * 100) % 1)) / 100;
            numOfCalculations = 0;

            // Begin loop for Option 1 amount
            for (double opt1Amt = minAmtOptX; opt1Amt <= maxAmtOptX; opt1Amt += CalculationConstants.jumpBetweenAmounts)
            {
                // Begin loop for Option 2 amount
                for (double opt2Amt = minAmtOptY; opt2Amt <= maxAmtOptY; opt2Amt += CalculationConstants.jumpBetweenAmounts)
                {
                    numOfCalculations++;
                    // Show progress and  calculate remaining time
                    if (env.PrintOptions.printPercentageDone == true)
                    {
                        amtYCounter++;
                        TimeSpan tLoop = DateTime.Now - tStart;
                        tStart = DateTime.Now;
                        percentDone = (amtYCounter / loopCount) * 100;
                        percentDone = ((percentDone * 100) - ((percentDone * 100) % 1)) / 100;
                        Console.Write("\r" + amtYCounter + " / " + loopCount + " - AmountX checked = "
                                            + opt1Amt + " ( " + percentDone + " % done ) - took " + tLoop);
                    }

                    // number of products in composition (notice: minAmtOptZ is not set...
                    // if (MiscUtilities.Use3ProductsInComposition())
                    if (((env.CalculationParameters.loanAmtWanted - opt1Amt - opt2Amt) >= minAmtOptZ) &&
                            ((env.CalculationParameters.loanAmtWanted - opt1Amt - opt2Amt) <= maxAmtOptZ))
                    {
                        double opt3Amt = env.CalculationParameters.loanAmtWanted - opt1Amt - opt2Amt;
                        // Perform main thing here ...
                        // Probably the right place to run in different task here
                        if (Share.shouldRunLogicSync)
                        {
                            // Omri - what do we do with the searchOneDivisionOfAmounts object?
                            try
                            {
                                searchOneDivisionOfAmounts = new OneDivisionOfAmounts(opt1Amt, opt2Amt, opt3Amt, env);
                            }
                            catch (ArgumentOutOfRangeException /*aoore*/)
                            {
                                Console.WriteLine("NOTICE: PerformFullThreeOptionSearch ArgumentOutOfRangeException occured: " /* + aoore.ToString() */ +
                                    " for opt1Amt: " + opt1Amt + ", opt2Amt: " + opt2Amt + ", opt3Amt: " + opt3Amt);
                                continue;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("NOTICE: PerformFullThreeOptionSearch Exception occured: "  + ex.Message  +
                                    " for opt1Amt: " + opt1Amt + ", opt2Amt: " + opt2Amt + ", opt3Amt: " + opt3Amt);
                                continue;
                            }
                        }
                        else
                        {
                            try
                            {
                                ManageASync(opt1Amt, opt2Amt, opt3Amt, env);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("NOTICE: PerformFullThreeOptionSearch loopCount: " + 
                                    loopCount + ", numOfCalculations: " + numOfCalculations);
                            }
                            //Console.WriteLine("RETURN DoComputeASync searchOneDivisionOfAmounts: " + searchOneDivisionOfAmounts.plane.totalColumnSearchChecks);
                        }

                        // stop the printing
#if SHOULD_DEBUG_LUCHSILUKIN
                        //// TBD debug - should remove from here
                        Share.ShouldPrintLog = false;
                        // debug should remove till here
#endif
                    }
                }
            }

            Task.WaitAll(TaskList.ToArray());
        }

        /// <summary>
        /// Manage the logic calculation async
        /// </summary>
        /// <returns></returns>


        List<Task> TaskList = new List<Task>();

        private async Task ManageASync(double opt1Amt, double opt2Amt, double opt3Amt, RunEnvironment env)
        {
            TaskList.Add(DoComputeASync2(opt1Amt, opt2Amt, opt3Amt, env));
            //Console.WriteLine("BEFORRRR TaskList contain: " + TaskList.Count);

            //Task.WaitAll(TaskList.ToArray());
            ////Console.WriteLine("AFTERRRRR TaskList.ToArray ");
        }
       

        private async Task DoComputeASync2(double opt1Amt, double opt2Amt, double opt3Amt, RunEnvironment env)
        {
            //string debugStr = opt1Amt + ":" + opt2Amt + ":" + opt3Amt;
            //Console.WriteLine("++++ BEFORE DoComputeASync2: " + debugStr);
            var result =  await Task.Run(() => new OneDivisionOfAmounts(opt1Amt, opt2Amt, opt3Amt, env));
            // Console.WriteLine("**** GetCurrentProcess" + Process.GetCurrentProcess().Threads.Count);
            //Console.WriteLine("**** AFTER DoComputeASync2: " + debugStr);
        }

   

    }
}
