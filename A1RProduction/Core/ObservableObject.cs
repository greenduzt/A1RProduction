using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace A1QSystem.Core
{
    /// <summary>
    /// Encapsulates INotifyPropertyChanged implementation for View Models to use
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed
        /// </summary>
        /// <param name="propertyExpression">property</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var body = propertyExpression.Body as MemberExpression;
          if (body != null)
          {
              var property = body.Member as PropertyInfo;
              if (property != null)
              {
                  PropertyChanged(this, new PropertyChangedEventArgs(property.Name));
              }
          }
        }
    }
   
}
