﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;
using WisorLib;
using WisorLibrary.DataObjects;
using WisorLibrary.Utilities;
using static WisorLib.GenericProduct;
using static WisorLib.MiscConstants;
using WisorLibrary.Logic;

namespace WisorAppWpf
{
  
  
    public class SampleData
    {

        public ObservableCollection<DataGridRowModel> DataGridCollection1 { get; set; } = new ObservableCollection<DataGridRowModel>();
        public ObservableCollection<DataGridRowModel> DataGridCollection2 { get; set; } = new ObservableCollection<DataGridRowModel>();

        // make the class singltone
        private static SampleData instance;
        private static SelectionType selectionType;

        public static SampleData Instance
        {
            get
            {
                // since he same window serve several faunclionalities, ensure there is a need to reload
                if (instance == null || selectionType != Share.theSelectionType)
                {
                    instance = new SampleData();
                }
                return instance;
            }
        }

        private SampleData()
        {
            selectionType = Share.theSelectionType;

            string filename = MiscUtilities.GetFilenameFromUser();

            // Load data from file
            if (!string.IsNullOrEmpty(filename))
            {
                if (SelectionType.ReadCretiria == Share.theSelectionType)
                {
                    FieldList cntls = FileUtils.LoadXMLFileData(filename);
       
                    // show the controls in the grid
                    if (null != cntls)
                    {
                        foreach (var c in Share.theSelectedCriteriaFields)
                        {
                            DataGridRowModel dg = new DataGridRowModel(c.ID /*, c.value */);
                            this.DataGridCollection2.Add(dg);
                        }

                        foreach (var c in Share.theLoadedCriteriaFields)
                        {
                            DataGridRowModel dg = new DataGridRowModel(c.ID /*, c.value */);
                            this.DataGridCollection1.Add(dg);
                        }

                        // save the location and name for the cretiria filename
                        if (SelectionType.ReadCretiria == Share.theSelectionType)
                            Share.theCriteriaFilename = Path.GetDirectoryName(filename) +
                                    Path.DirectorySeparatorChar + MiscConstants.CRITERIA_FILENAME;
                    }
                }
                else if (SelectionType.ReadProducts == Share.theSelectionType)
                {
                    ProductsList products = GenericProduct.LoadXMLProductsFile(filename);
                    // show the controls in the grid
                    if (null != products)
                    {
                        foreach (var p in products)
                        {
                            this.DataGridCollection1.Add(new DataGridRowModel(p.Value.name));   // Key.ToString() /*p.ID*/));
                        }
                    }
                }
            }
            else
            {
                WindowsUtilities.loggerMethod("SampleData: Empty filename");
            }
        }
    }


    public class DataGridRowModel 
    {
        public string ID { get; set; }
        //public string Value1 { get; set; }
        //public string Value2 { get; set; }

        public DataGridRowModel(string id
            /*, string value1 = MiscConstants.UNDEFINED_STRING, string value2 = MiscConstants.UNDEFINED_STRING*/)
        {
            ID = id;
            //Value1 = value1;
            //Value2 = value2;
        }
    }


    //////////////////////////////////////////////

    public class Utilities
    {

        /// <summary>
        /// Manage the various file loading
        /// </summary>
        /// <returns></returns>
        /// 


        public static FieldList GetCriteriaFromFile()
        {
            //WindowsUtilities.loggerMethod("Select the critiria and order for the loan");
            FieldList fields = null;
            Share.theSelectionType = SelectionType.ReadCretiria;

            if (Share.shouldShowCriteriaSelectionWindow)
            {
                try
                {
                    Window sw = new SelectionWindow(SelectionType.ReadCretiria);
                    sw.ShowDialog();

                    fields = Share.theSelectedCriteriaFields;
                }
                catch (Exception e)
                {
                    WindowsUtilities.loggerMethod("ERROR: GetCriteriaFromFile got Exception: " + e.ToString());
                }
            }
            else
            {
                string filename = MiscUtilities.GetFilenameFromUser();
                //string filename = @"..\..\..\Data\Gui.xml.txt";

                if (null != filename)
                {
                    fields = FileUtils.LoadXMLFileData(filename);
                }
                WindowsUtilities.loggerMethod("Successfully upload: " + fields.Count + " criteria definitions from file: " +
                    filename.Substring(filename.LastIndexOf(Path.DirectorySeparatorChar) + 1));
            }

            return fields;
        }

        
        public static ProductsList GetProductsFromFile()
        {
            //WindowsUtilities.loggerMethod("Select the Products from file");
            ProductsList products = null;
            Share.theSelectionType = SelectionType.ReadProducts;
            
            if (Share.shouldShowProductSelectionWindow)
            {
                try
                {
                    Window sw = new SelectionWindow(SelectionType.ReadProducts);
                    sw.ShowDialog();

                }
                catch (Exception e)
                {
                    WindowsUtilities.loggerMethod("ERROR: GetCriteriaFromFile got Exception: " + e.ToString());
                }
            }
            else
            {
                string filename = MiscUtilities.GetFilenameFromUser();
                //string filename = @"..\..\..\Data\MortgageProducts.xml";

                if (null != filename)
                {
                    products = GenericProduct.LoadXMLProductsFile(filename);
                }
            }

            return products;
        }

        

        /// <summary>
        /// Print the results in a sorted manner
        /// </summary>
        /// <param name="env"></param>
        /// 
        public static void PrintResultsInList(RunEnvironment env)
        {
            if (0 < Share.numberOfPrintResultsInList && null != env.listOfSelectedCompositions && 0 < env.listOfSelectedCompositions.Count)
            {
                string summery = "MaxProfit: " + env.MaxProfit + ", MaxBankPay: " + env.MaxBankPay + ", MinBorrowerPay: " + env.MinBorrowerPay;
                Console.WriteLine(summery);

                DisplayCombinations displayForm = new DisplayCombinations();
                List<ChosenComposition> newList = null, newAllList = null;

                // set the header
                List<string> newHeader = new List<string>();
                int indexOfBorrower = 0, count = 0, indexOfBank = 0, indexOfProfit = 0;
                foreach (string str in env.headerOfListOfSelectedCompositions)
                {
                    if (0 <= str.IndexOf("Borrower pay"))
                    {
                        newHeader.Add("Min Borrower pay: " + env.MinBorrowerPay);
                        indexOfBorrower = count;
                    }
                    else if (0 <= str.IndexOf("Bank amount"))
                    {
                        newHeader.Add("Max Bank amount: " + env.MaxBankPay);
                        indexOfBank = count;
                    }
                    else if (0 <= str.IndexOf("Profit"))
                    {
                        newHeader.Add("Max Profit: " + env.MaxProfit);
                        indexOfProfit = count;
                    }
                    else
                    {
                        newHeader.Add(str);
                    }
                    count++;
                }

                newList = MiscUtilities.OrderCompositionListByProfit(env.listOfSelectedCompositions);
                displayForm.Text = "Sorting by max profit. " + summery;
                displayForm.SetPrintResultsInListData(
                    newHeader, newList.GetRange(0, Share.numberOfPrintResultsInList), indexOfProfit);
                displayForm.ShowDialog();
                newAllList = newList.GetRange(0, Share.numberOfPrintResultsInList);

                newList = MiscUtilities.OrderCompositionListByBank(env.listOfSelectedCompositions);
                displayForm.Text = "Sorting by max bank pay. " + summery;
                displayForm.SetPrintResultsInListData(
                    newHeader, newList.GetRange(0, Share.numberOfPrintResultsInList), indexOfBank);
                displayForm.ShowDialog();
                newAllList.AddRange(newList.GetRange(0, Share.numberOfPrintResultsInList));

                newList = MiscUtilities.OrderCompositionListByBorrower(env.listOfSelectedCompositions);
                displayForm.Text = "Sorting by min borrower pay. " + summery;
                displayForm.SetPrintResultsInListData(
                    newHeader, newList.GetRange(0, Share.numberOfPrintResultsInList), indexOfBorrower, indexOfBorrower);
                displayForm.ShowDialog();
                newAllList.AddRange(newList.GetRange(0, Share.numberOfPrintResultsInList));

                // all together
                displayForm.Text = "All together. " + summery;
                displayForm.SetPrintResultsInListData(
                    newHeader, newAllList, MiscConstants.UNDEFINED_INT);
                displayForm.ShowDialog();
            }
        }

        /// <summary>
        /// Run the loans by single/multi thread
        /// </summary>
        

        public static void Ask4Input()
        {
            // bool rc = Utilities.PrepareRun();
            string reasoning;
            bool rc = MiscUtilities.PrepareRunningFull(out reasoning);
            if (!rc)
                WindowsUtilities.loggerMethod("NOTICE Ask4Input failed in PrepareRunningFull. reasoning: " + reasoning);

            if (rc)
            {
                BuildControls bc = new BuildControls(Share.theSelectedCriteriaFields);
            }

         }



        public static void RunTheLogic()
        {
            MiscUtilities.RunTheLogic(null, true);
        }




        //private static void RunTheLoansSync2(LoanList loans/*, FieldList fields*/)
        //{
        //    WindowsUtilities.loggerMethod("Running the engine for the: " + loans.Count + " loans.");
        //    List<Task> tasks = new List<Task>();

        //    // start the time elapse counter
        //    Utilities.StartPerformanceCalculation();

        //    int count = 1;
        //    foreach (loanDetails loan in loans)
        //    {

        //        tasks.Add(Task.Factory.StartNew(/*async*/ () =>
        //        {
        //            try
        //        {
        //            RunLoanDetails result = LoanCalculation(loan);
        //            //WindowsUtilities.loggerMethod("--- Complete SYNC running the engine with: " + loan.ToString() + ", result: " + result.ToString());
        //        }
        //        catch (Exception ex)
        //        {
        //            WindowsUtilities.loggerMethod("NOTICE: RunTheLoansSync Exception occured: " + ex.ToString());
        //        }

        //        }
        //        ));

        //        count++;

        //        // fake completion
        //        //if (3 <= count)
        //        //    break;
        //    }

        //    Console.WriteLine("--- BEFORE SYNC Task.WaitAll");
        //    Task.WaitAll(tasks.ToArray());
        //    Console.WriteLine("--- AFTER SYNC Task.WaitAll");
        //    Utilities.StopPerformanceCalculation();

        //    //SetButtonEnable(true);

        //    // WindowsUtilities.loggerMethod("Complete calculate the entire " + loans.Count + " loans");
        //}





    }

    //public class MainViewModel : ViewModelBase
    //{
    //    private SampleData _data;

    //    /// <summary>
    //    /// Initializes a new instance of the MainViewModel class.
    //    /// </summary>
    //    public MainViewModel()
    //    {
    //        this.Data = SampleData.Instance; //    new SampleData.;
    //    }

    //    public SampleData Data
    //    {
    //        get { return _data; }
    //        set
    //        {
    //            if (Equals(value, _data)) return;
    //            _data = value;
    //            OnPropertyChanged();
    //        }
    //    }

    //}


    //public class ViewModelBase : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler PropertyChanged;

    //    [NotifyPropertyChangedInvocator]
    //    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }
    //}



    public class BuildControls
    {

        UserInputLoan uil;

        public BuildControls(FieldList fields)
        {
            uil = new UserInputLoan();
            uil.AutoSize = true;
            uil.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            System.Windows.Forms.Control comp = null; //  new Control();
            int positionX = 50, positionY = 30;
            const int yShift = 40;

            foreach (CriteriaField f in fields)
            {
                if (!String.IsNullOrEmpty(f.type))
                {
                    switch (f.type)
                    {
                        case "Button":
                            comp = new System.Windows.Forms.Button();
                            break;
                        case "TextBox":
                            comp = new System.Windows.Forms.TextBox();
                            //(comp as System.Windows.Forms.TextBox).TextChanged += new EventHandler(TextBox_CheckedChanged);
                             break;
                        case "ComboBox":
                            comp = new System.Windows.Forms.ComboBox();
                            // loop the options
                            foreach (string op in f.options)
                            {
                                (comp as System.Windows.Forms.ComboBox).Items.Add(op);
                            }
                            (comp as System.Windows.Forms.ComboBox).SelectedIndex = 0;

                            //(comp as System.Windows.Forms.ComboBox).SelectedIndexChanged /*.SelectedIndexChanged*/ += new EventHandler(ComboBox_CheckedChanged);
                            break;
                        case "DatePicker":
                            //comp = new DateTimePicker();
                            //setText = false;
                            break;

                    }

                    comp.Name = f.ID;
                    comp.Tag = f.ID;
                    comp.Text = f.value;

                    //// add the label
                    System.Windows.Forms.Label lbl = new System.Windows.Forms.Label();
                    lbl.Font = new Font("Arial", 11);
                    lbl.Text = f.ID; 
                    lbl.Width = 250; // w + 20;
                    lbl.Location = new System.Drawing.Point(positionX + 20, positionY);
                    uil.Controls.Add(lbl);

                    comp.Location = new System.Drawing.Point(positionX + 450, positionY);
                    comp.Width = 200;
                    positionY += yShift;
                    uil.Controls.Add(comp);

                    //// add to the memory
                    //ManageControlsAdd(comp);

                    //// check legal fields value
                    //LoadRange(item);
                }
            }

            //// Add the customer optional name
            ////// add the label
            //System.Windows.Forms.Label lbl2 = new System.Windows.Forms.Label();
            //lbl2.Font = new Font("Arial", 11);
            //lbl2.Text = "Customer name";
            //lbl2.Width = 250; // w + 20;
            //lbl2.Location = new XPoint(positionX + 20, positionY);
            //uil.Controls.Add(lbl2);

            //comp = new System.Windows.Forms.TextBox();
            //comp.Name = Share.CustomerName;
            //////comp.Tag = f.ID;
            ////comp.Text = MiscConstants.CUSTOMER_NAME;
            //comp.Location = new XPoint(positionX + 450, positionY);
            //comp.Width = 200;
            //positionY += yShift;
            //uil.Controls.Add(comp);

            // Add the start button
            System.Windows.Forms.Control startButton = new System.Windows.Forms.Button();
            startButton.ClientSize = new System.Drawing.Size(70, 40);
            startButton.BackColor = Color.GreenYellow;
            startButton.Font = new Font("Arial", 12);
            startButton.Text = "Start";
            startButton.Location = new System.Drawing.Point(positionX, positionY);
            startButton.Click += new EventHandler(StartButton_Clicked);
            uil.Controls.Add(startButton);

            uil.Text = "Please choose the load details:";
            uil.ShowDialog();
        }

        protected void StartButton_Clicked(object sender, EventArgs e)
        {
            LoanList loans = new LoanList();
            int id = MiscUtilities.GetLoanID();
            uint loanAmount, desiredMonthlyPayment, propertyValue, yearlyIncome,
                borrowerAge;
            int fico;
            DateTime dateTaken = DateTime.Now;
            uint desireTerminationMonth, sequentialNumber;
            //string originalProduct = MiscConstants.UNDEFINED_STRING;
            double originalRate = MiscConstants.UNDEFINED_DOUBLE, originalRate2 = MiscConstants.UNDEFINED_DOUBLE;
            double originalMargin = MiscConstants.UNDEFINED_DOUBLE, originalMargin2 = MiscConstants.UNDEFINED_DOUBLE;
            uint originalTime = MiscConstants.UNDEFINED_UINT;
            Risk risk = Risk.NONERisk;
            Liquidity liquidity = Liquidity.NONELiquidity;
            ProductID product = null;

            loanAmount = desiredMonthlyPayment = propertyValue = yearlyIncome = borrowerAge =
                desireTerminationMonth = sequentialNumber = MiscConstants.UNDEFINED_UINT;
            fico = MiscConstants.UNDEFINED_INT;

            foreach (System.Windows.Forms.Control c in uil.Controls)
            {
                Type t = c.GetType();
                
                if (! (c is System.Windows.Forms.Button || c is System.Windows.Forms.Label))
                {
                    AnalayzeParameters.AnalayzeCretiriaParameters(
                        c.Text, c.Name,
                        ref loanAmount, ref desiredMonthlyPayment, ref propertyValue,
                        ref yearlyIncome, ref borrowerAge, ref fico, ref dateTaken,
                        ref desireTerminationMonth, ref sequentialNumber,
                        ref originalRate, ref originalRate2, ref originalMargin, ref originalMargin2, 
                        ref originalTime, ref risk, ref liquidity, ref product);

                }
            }
            uil.Hide();
            uil.Close();
            
            loanDetails loan = new loanDetails(id.ToString(), loanAmount, desiredMonthlyPayment,
                propertyValue, yearlyIncome, borrowerAge, fico,
                dateTaken, product, true /*shouldCalculate*/, originalRate, originalRate2, originalTime,
                originalMargin, originalMargin2, sequentialNumber, risk, liquidity);
           
            WindowsUtilities.runSingleLoanSyncMethod(loan);
        }
        
    }
}
