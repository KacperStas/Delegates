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
            // Handling division by zero
            if (number == 0)
            {
                MessageBox.Show("Cannot calculate the reciprocal of zero.", "Math Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return 0;
            }
            return 1.0 / number;
        }

        // --- Point 1: Delegate Assignment Logic ---

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

        // --- Point 2: Execution of Stored Method ---

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Verify a method is actually stored in the delegate
            if (_selectedOperation == null)
            {
                MessageBox.Show("Please select an operation first.", "Missing Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Validate the input from the user
            if (double.TryParse(InputTextBox.Text, out double inputNumber))
            {
                // 3. INVOKE the delegate to execute whichever method is currently stored inside it
                double result = _selectedOperation(inputNumber);

                // 4. Display the result
                ResultTextBlock.Text = $"Result: {result:F4}";
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric value.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}