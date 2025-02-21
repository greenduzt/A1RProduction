using A1QSystem.DB;
using A1QSystem.Model;
using A1QSystem.Model.FormulaGeneration;
using A1QSystem.PDFGeneration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MsgBox;

namespace A1QSystem.ViewModel.Manufacturing.FormulaGeneration
{
    public class RubberFormulaTreeViewModel : BaseViewModel
    {
        public List<TreeItem> TestList { get; private set; }
        public List<TreeItem> Tree1 { get; private set; }


        public List<TreeItem> Tree2 { get; private set; }
        public List<TreeItem> Tree3 { get; private set; }

        public RubberFormulaTreeViewModel()
        {


            Tree1 = GetFormulaTree(1, Tree1);
            Tree2 = GetFormulaTree(2, Tree2);
            Tree3 = GetFormulaTree(3, Tree3);


        }

        public List<TreeItem> GetFormulaTree(int listNo, List<TreeItem> LIST)
        {


            List<TreeItem> treeList = new List<TreeItem>();
            List<FormulaItems> allItems = DBAccess.LoadFormulaTree();

            int maxLevel = FindMaxLevel(allItems);

            for (int v = listNo; v <= listNo; v++)
            {
                foreach (var x in allItems)
                {
                    if (x.ID == x.ParentID && x.BID == v)
                    {
                        TestList = new List<TreeItem>();
                        var top = new TreeItem { ID = x.ID, ParentID = x.ParentID, TestText = x.FormulaName };
                        TestList.Add(top);

                        foreach (var fL in allItems)
                        {
                            if (fL.BID == v && fL.TreeLevel == 1)
                            {
                                var second1 = new TreeItem { ID = fL.ID, ParentID = fL.ParentID, TestText = fL.FormulaName };
                                TestList.Add(second1);


                                foreach (var sL in allItems)
                                {
                                    if (fL.ID == sL.ParentID && sL.BID == v && sL.TreeLevel == 2)
                                    {
                                        var second2 = new TreeItem { ID = sL.ID, ParentID = sL.ParentID, TestText = sL.FormulaName };
                                        TestList.Add(second2);

                                        foreach (var tL in allItems)
                                        {
                                            if (sL.ID == tL.ParentID && tL.BID == v && tL.TreeLevel == 3)
                                            {
                                                var second3 = new TreeItem { ID = tL.ID, ParentID = tL.ParentID, TestText = tL.FormulaName };
                                                TestList.Add(second3);

                                                foreach (var ffL in allItems)
                                                {
                                                    if (tL.ID == ffL.ParentID && ffL.BID == v && ffL.TreeLevel == 4)
                                                    {
                                                        var level4 = new TreeItem { ID = ffL.ID, ParentID = ffL.ParentID, TestText = ffL.FormulaName };
                                                        TestList.Add(level4);

                                                        foreach (var ssL in allItems)
                                                        {
                                                            if (ffL.ID == ssL.ParentID && ssL.BID == v && ssL.TreeLevel == 5)
                                                            {
                                                                var level5 = new TreeItem { ID = ssL.ID, ParentID = ssL.ParentID, TestText = ssL.FormulaName };
                                                                TestList.Add(level5);
                                                                level4.Children.Add(level5);
                                                            }
                                                        }

                                                        second3.Children.Add(level4);
                                                    }
                                                }
                                                second2.Children.Add(second3);
                                            }
                                        }
                                        second1.Children.Add(second2);
                                    }
                                }
                                top.Children.Add(second1);
                            }
                        }
                        LIST = new List<TreeItem> { top };
                        treeList = LIST;
                    }
                }
            }

            return treeList;
        }

        private TreeItem _selectedTreeItem;
        public TreeItem SelectedTreeItem
        {
            get { return _selectedTreeItem; }
            set
            {
                _selectedTreeItem = value;
                RaisePropertyChanged("SelectedTreeItem");
                ViewFormula(SelectedTreeItem.ID,SelectedTreeItem.TestText);
            }
        }

        public void ViewFormula(int ID, String FormulaName)
        {
            int Binder = 0;
            string BinderType = string.Empty;
            int MixingMinutes = 0;
            string HeaderName = string.Empty;
            string FormulaType = string.Empty;
            string MouldType= string.Empty;
            int NoOfBins = 0;
            int Mesh4 = 0;
            int Mesh12 = 0;
            int Mesh16 = 0;
            int Mesh16To30 = 0;
            int Mesh30 = 0;
            int Mesh3040 = 0;
            int Mesh12mg = 0;
            int MeshRegrind = 0;
            string SpecialInstructions= string.Empty;
            string ColourInstructions= string.Empty;
            string MethodPS= string.Empty;
            string HeaderColours = string.Empty;
            decimal HeaderFontSize=12;
            decimal SpecialInsHeight = 1;
            decimal TopicFontSize = 26;
            decimal SpecialInsTextPosHeight = 3;
            bool Enable = false;
            string Lift1 = string.Empty;
            string Lift2 = string.Empty;
            string MixingNotes = string.Empty;

            ObservableCollection<RubberProduction> formula = DBAccess.SearchProductFormula(ID);
            if (formula.Count > 0)
            {
                BindingList<ProductColourDetails> fColours = DBAccess.GetFormulaColours(ID);

                ObservableCollection<FormulaColourTableHeaders> formulaColourTable = DBAccess.GetFormulaColourTable(ID);

               
                foreach (var x in formula)
                {
                    Binder = x.Binder;
                    BinderType = x.BinderType;
                    MixingMinutes = x.Minutes;
                    HeaderName = x.HeaderName;
                    MouldType = x.MouldType;
                    NoOfBins = x.NoOfBins;
                    Mesh4 = x.GradingSize4;
                    Mesh12 = x.GradingSize12;
                    Mesh16 = x.GradingSize16;
                    Mesh16To30 = x.GradingSize1620;
                    Mesh30 = x.GradingSize30;
                    Mesh3040 = x.GradingSize3040;
                    Mesh12mg = x.GradingSize12mg;
                    MeshRegrind = x.GradingSizeRegrind;
                    SpecialInstructions = x.SpecialInstructions;
                    ColourInstructions = x.ColourInstructions;
                    MethodPS = x.MethodPS;
                    HeaderColours = x.HeaderColours;
                    HeaderFontSize = x.HeaderFontSize;
                    TopicFontSize = x.TopicFontSize;
                    SpecialInsHeight = x.SpecialInsHeight;
                    SpecialInsTextPosHeight = x.SpecialInsTextPosHeight;
                    Enable = x.Enable;
                    Lift1 = x.Lift1;
                    Lift2 = x.Lift2;
                    MixingNotes = x.MixingNotes;
                    FormulaType = x.FormulaType;
                }
                if (Enable)
                {
                    PrintFormulaPDF pf = new PrintFormulaPDF();
                    pf.CreateFormula(fColours,formulaColourTable, Binder, BinderType, HeaderName, MixingMinutes, MouldType, NoOfBins, Mesh4, Mesh12, Mesh16, Mesh16To30, Mesh30, Mesh3040, Mesh12mg, MeshRegrind, SpecialInstructions, ColourInstructions, MethodPS, HeaderColours, HeaderFontSize, TopicFontSize, SpecialInsHeight, SpecialInsTextPosHeight, Enable, Lift1, Lift2, MixingNotes, FormulaType);
                }
                else
                {
                    Msg.Show("This formula does not available!", "Formula Not Available", MsgBoxButtons.OK, MsgBoxImage.Information_Red, MsgBoxResult.Yes);
                }

            }
        }

        public int FindMaxLevel(List<FormulaItems> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Empty list");
            }
            int maxL = int.MinValue;
            foreach (FormulaItems type in list)
            {
                if (type.TreeLevel > maxL)
                {
                    maxL = type.TreeLevel;
                }
            }
            return maxL;
        }

        public int FindMaxParent(List<FormulaItems> list)
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Empty list");
            }
            int maxP = int.MinValue;
            foreach (FormulaItems type in list)
            {
                if (type.BID > maxP)
                {
                    maxP = type.BID;
                }
            }
            return maxP;
        }

    }
}
