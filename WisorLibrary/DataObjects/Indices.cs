﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisorLib
{
    class IndicesDB
    {
        private static int yearBeginMeasure = 2000;
        public enum monthsList { JAN, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC };
        public enum yearsList { _2000, _2001, _2002, _2003, _2004, _2005, _2006, _2007, _2008, _2009, _2010, _2011, _2012, _2013, _2014, _2015 };
        public static double[,] primeRatesDB = {
                                                            //  Jan    Feb      Mar     Apr     May    Jun     Jul     Aug     Sep     Oct     Nov     Dev
                                                    /*2000*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 },
                                                    /*2001*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 }, 
                                                    /*2002*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 }, 
                                                    /*2003*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 },
                                                    /*2004*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 }, 
                                                    /*2005*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 },
                                                    /*2006*/ {-1,     -1,      -1,      -1,     -1,     -1,    -1,      -1,    -1,     -1,    -1,     -1 },
                                                    /*2007*/ {0.0600, 0.0575, 0.0550, 0.0550, 0.0525, 0.0500, 0.0500, 0.0525, 0.0550, 0.0550, 0.0550, 0.0550 }, 
                                                    /*2008*/ {0.0575, 0.0575, 0.0525, 0.0475, 0.0475, 0.0500, 0.0525, 0.0550, 0.0575, 0.0525, 0.0450, 0.0400 },
                                                    /*2009*/ {0.0325, 0.0250, 0.0225, 0.0200, 0.0200, 0.0200, 0.0200, 0.0200, 0.0225, 0.0225, 0.0225, 0.0250 },
                                                    /*2010*/ {0.0275, 0.0275, 0.0275, 0.0300, 0.0300, 0.0300, 0.0300, 0.0325, 0.0325, 0.0350, 0.0350, 0.0350 },
                                                    /*2011*/ {0.0350, 0.0375, 0.0400, 0.0450, 0.0450, 0.0475, 0.0475, 0.0475, 0.0475, 0.0450, 0.0450, 0.0425 },
                                                    /*2012*/ {0.0425, 0.0400, 0.0400, 0.0400, 0.0400, 0.0400, 0.0375, 0.0375, 0.0375, 0.0375, 0.0350, 0.0350 },
                                                    /*2013*/ {0.0325, 0.0325, 0.0325, 0.0325, 0.0300, 0.0275, 0.0275, 0.0275, 0.0275, 0.0250, 0.0250, 0.0250 },
                                                    /*2014*/ {0.0250, 0.0250, 0.0225, 0.0225, 0.0225, 0.0225, 0.0225, 0.0200, 0.0175, 0.0175, 0.0175, 0.0175 },
                                                    /*2015*/ {0.0175, 0.0175, 0.0160, 0.0160, 0.0160, 0.0160, 0.0160,   -1,    -1,     -1,    -1,     -1 }
                                                 };


        public static double GetPrimeRateForMonth(int monthToCheck, int yearToCheck)
        {
            return primeRatesDB[yearToCheck, monthToCheck];
        }

        public static void PrintPrimeRateForMonth(int monthToCheck, int yearToCheck)
        {
            Console.WriteLine((monthToCheck + 1) + "/" + (yearToCheck + yearBeginMeasure) + " - " + primeRatesDB[yearToCheck, monthToCheck]);
        }


    }
}