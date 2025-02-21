using A1QSystem;
using A1QSystem.Model;
using A1QSystem.Model.Meta;
using A1QSystem.View;
using A1QSystem.View.Maintenance;
using A1QSystem.View.Production;
using A1QSystem.View.Quoting;
using A1QSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace A1QSystem.View
{
	public partial class MainMenu : UserControl
	{
      
        string username;
        string state;

        public MainMenu(string uName, string uState, List<UserPrivilages> uPriv,List<MetaData> metaData)
		{            
			this.InitializeComponent();

            DataContext = new MainMenuViewModel(uName, uState, uPriv, metaData);

            username = uName;
            state = uState;



		}

        private void ExitMainMenuTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           
            MessageBoxImage icon = MessageBoxImage.Warning;
            if (MessageBox.Show("Do you want to exit from A1 Rubber System?", "Exit Confirmation", MessageBoxButton.YesNo, icon) == MessageBoxResult.Yes)
            {
                //Window parentWindow = (Window)this.Parent;
                //parentWindow.Close();

                Application.Current.Shutdown();
            }
            else
            {
                // Do not close the window
            }
        }       
	}
}