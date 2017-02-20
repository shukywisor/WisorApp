﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisorLib
{
    class SavedCompositions
    {
        // General Parameters
        private Option fixedOptZ = null;
        private SavedMatchesBeforeCheck savedListForCheck = null;

        // For checking total payment
        private double ttlPayForCheck = -1;
        private enum ttlPayRange { SMALLER, EQUAL, LARGER };

        // List of saved compositions after calculation
        //private Composition compositionToSave = null;




        public SavedCompositions(Option fixedOptionZ, SavedMatchesBeforeCheck savedMatchesListForOnePlane)
        {
            fixedOptZ = fixedOptionZ;
            savedListForCheck = savedMatchesListForOnePlane;
            
            fixedOptZ.GetTtlPay();
            CheckMatchList();
                      
        }



        // **************************************************************************************************************************** //
        // ********************************** Check one point if Total Pay is lower than lowest so far ******************************** //

        private int CheckTotalPay(double pmtForCheck)
        {
            if (ResultsOutput.bestCompositionSoFar == null)
            {
                return (int)ttlPayRange.SMALLER;
            }
            else
            {
                if (pmtForCheck < ResultsOutput.bestCompositionSoFar.ttlPay)
                {
                    return (int)ttlPayRange.SMALLER;
                }
                else if (pmtForCheck > ResultsOutput.bestCompositionSoFar.ttlPay)
                {
                    return (int)ttlPayRange.LARGER;
                }
                else
                {
                    return (int)ttlPayRange.EQUAL;
                }
            }
        }


        
        // **************************************************************************************************************************** //
        // ********************************** Check one point if Total Pay is lower than lowest so far ******************************** //

        public void CheckMatch(Option[] matchingPoint)
        {
            // 1 - Calculate total pay for Option X and Option Y of matching point for check

            Option tempOptX = matchingPoint[(int)Options.options.OPTX];
            Option tempOptY = matchingPoint[(int)Options.options.OPTY];
            tempOptX.GetTtlPay();
            tempOptY.GetTtlPay();
            ttlPayForCheck = tempOptX.optTtlPay + tempOptY.optTtlPay + fixedOptZ.optTtlPay;   

            // 2 - Check total pay if smaller than the lowest so far
            int ttlPayChecker = CheckTotalPay(ttlPayForCheck);
            
            if (ttlPayChecker == (int)ttlPayRange.SMALLER)
            {
                ResultsOutput.bestCompositionSoFar = new Composition(matchingPoint[(int)Options.options.OPTX],
                                                                        matchingPoint[(int)Options.options.OPTY], fixedOptZ);
            }
        }





        // **************************************************************************************************************************** //
        // ***************************** Check list of saved matching points and save lowest total pay ******************************** //

        private void CheckMatchList()
        {
            for (int i = 0; i < savedListForCheck.numOfMatches; i++)
            {
                CheckMatch(savedListForCheck.savedMatchesList[i].savedMatch);
            }        
        }




    }
}
