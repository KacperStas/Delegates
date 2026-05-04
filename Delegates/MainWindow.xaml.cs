using System;
using System.Windows;

namespace DelegateTester
{
    public partial class MainWindow : Window
    {
        // The delegate variable that will hold the reference to the selected method
        private Func<double, double> _selectedOperation;

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- Method Definitions ---

        private double Square(double number)
        {
            return number * number;
        }

        private double SquareRoot(double number)
        {
            return Math.Sqrt(number);
        }

        private double Reciprocal(double number)
        {
            // Note: In a full implementation, you would want to handle division by zero here.
            return 1.0 / number;
        }

        // --- Delegate Assignment Logic ---

        private void Operation_Checked(object sender, RoutedEventArgs e)
        {
            // Ensure the UI elements have initialized
            if (!IsLoaded) return;

            // Assign the corresponding method to the delegate variable based on the selection
            if (SquareRadio.IsChecked == true)
            {
                _selectedOperation = Square;
            }
            else if (SquareRootRadio.IsChecked == true)
            {
                _selectedOperation = SquareRoot;
            }
            else if (ReciprocalRadio.IsChecked == true)
            {
                _selectedOperation = Reciprocal;
            }
        }
    }
}