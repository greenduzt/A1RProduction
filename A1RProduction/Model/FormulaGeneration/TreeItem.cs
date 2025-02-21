using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A1QSystem.Model.FormulaGeneration
{
    public class TreeItem
    {
        private readonly List<TreeItem> children = new List<TreeItem>();

        public int ID { get; set; }
        public int ParentID { get; set; }
        public string TestText { get; set; }
        public List<TreeItem> Children
        {
            get
            {
                return children;
            }
        }

        private bool m_isSelected;
        public bool IsSelected
        {
            get { return m_isSelected; }
            set
            {
                m_isSelected = value;
            }
        }


    }
}
