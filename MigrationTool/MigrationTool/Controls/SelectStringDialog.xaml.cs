using System;
using System.Windows;
using System.Windows.Controls;

namespace MigrationTool.Controls
{
    public partial class SelectStringDialog : UserControl
    {
        public static readonly DependencyProperty SelectedValueProperty = DependencyProperty.Register("SelectedValue", typeof(string), typeof(SelectStringDialog), new UIPropertyMetadata(string.Empty));

        public string SelectedValue
        {
            get { return (string)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public SelectStringDialog()
        {
            InitializeComponent();
        }

        protected virtual string ShowDialog()
        {
            throw new NotImplementedException();
        }

        private void SelectButtonClick(object sender, RoutedEventArgs e)
        {
            var dlgResult = ShowDialog();
            if (dlgResult != null)
            {
                SelectedValue = dlgResult;
            }
        }

        private void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedValue = string.Empty;
        }
    }
}