using System.Windows.Controls;

namespace A1QSystem.Core
{
    /// <summary>
    /// Base class for all Views that is used in MVVM
    /// </summary>
    /// <typeparam name="TViewModel">ViewModel</typeparam>
    public class ViewBase<TViewModel> : UserControl, IView<TViewModel> where TViewModel : ViewModelBase
    {
        public ViewBase()
        { }

        public ViewBase(TViewModel tViewModel)
        {
            ViewModel = tViewModel;
        }

        /// <summary>
        /// ViewModel
        /// </summary>
        public TViewModel ViewModel
        {
            get
            {
                return (TViewModel)DataContext;
            }
            private set
            {
                DataContext = value;
            }
        }
    }
}
