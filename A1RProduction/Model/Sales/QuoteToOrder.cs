using A1QSystem.Commands;
using A1QSystem.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace A1QSystem.Model.Sales
{
    public class QuoteToOrder
    {
        public string UserName { get; set; }
        public string State { get; set; }
        public string CanvasNew { get; set; }   
        private bool _canExecute;
        private Microsoft.Practices.Prism.Commands.DelegateCommand _editCommand;
        private ICommand _convertToSaleCommand;
        private ICommand _commentCommand;

        public QuoteDetails quoteDetails { get; set; }
        public Quote quote { get; set; }
        public FreightDetails freightDetails { get; set; }  
        
        public QuoteToOrder(string userName, string state)
        {

            UserName = userName;
            State = state;
            _editCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(ShowEditQuote);
            _canExecute = true;           
        }


        private void ShowEditQuote()
        {
            var childWindow = new ChildWindowView();
            childWindow.quoteDetails_Closed += (r => { });
            childWindow.ShowEditQuoteDetails(quote.ID, UserName, State);
        }

        private void ConvertToSale()
        {
           
            var childWindow = new ChildWindowView();
            childWindow.ShowQuoteToSaleApproval(quote.ID, quote.Prefix);
        }

        private void OpenComments()
        {
            var childWindow = new ChildWindowView();
            childWindow.ShowComments(quote.ID, UserName, State);
        }


        #region COMMANDS

        public Microsoft.Practices.Prism.Commands.DelegateCommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand ConvertToSaleCommand
        {
            get
            {
                return _convertToSaleCommand ?? (_convertToSaleCommand = new LogOutCommandHandler(() => ConvertToSale(), _canExecute));
            }
        }
        public ICommand CommentCommand
        {
            get
            {
                return _commentCommand ?? (_commentCommand = new LogOutCommandHandler(() => OpenComments(), _canExecute));
            }
        }

        #endregion

    }
}
