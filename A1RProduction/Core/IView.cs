namespace A1QSystem.Core
{
    public interface IView<out TViewModel> where TViewModel : IViewModel
    {
        /// <summary>
        /// Get's the ViewModel
        /// </summary>
        TViewModel ViewModel { get; }
    }
}
