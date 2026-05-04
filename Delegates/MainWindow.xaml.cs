using System;
using System.Collections.Generic;
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
            if (!IsLoaded) return;

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
            if (_selectedOperation == null)
            {
                MessageBox.Show("Please select an operation first.", "Missing Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (double.TryParse(InputTextBox.Text, out double inputNumber))
            {
                double result = _selectedOperation(inputNumber);
                ResultTextBlock.Text = $"Result: {result:F4}";
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric value.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // --- Point 3: Collection Processing ---

        /// <summary>
        /// This method accepts a collection of numbers and a delegate.
        /// It iterates through the collection, applying the delegate to each item.
        /// </summary>
        private List<double> ProcessCollection(List<double> numbers, Func<double, double> operation)
        {
            List<double> processedNumbers = new List<double>();

            foreach (double number in numbers)
            {
                // Invoke the delegate passed into the method parameter
                processedNumbers.Add(operation(number));
            }

            return processedNumbers;
        }

        private void ProcessListButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Verify a method is selected
            if (_selectedOperation == null)
            {
                MessageBox.Show("Please select an operation first.", "Missing Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // 2. Parse the comma-separated input into a List<double>
            string[] inputStrings = InputListTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<double> inputNumbers = new List<double>();

            foreach (string str in inputStrings)
            {
                if (double.TryParse(str.Trim(), out double num))
                {
                    inputNumbers.Add(num);
                }
                else
                {
                    MessageBox.Show($"Invalid number detected: '{str.Trim()}'. Please enter only numbers separated by commas.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (inputNumbers.Count == 0)
            {
                MessageBox.Show("Please enter a valid list of numbers.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Call the processing method, passing the collection AND the delegate
            List<double> results = ProcessCollection(inputNumbers, _selectedOperation);

            // 4. Format and display the results
            List<string> formattedResults = results.ConvertAll(r => r.ToString("F3"));
            ListResultTextBlock.Text = $"Processed List: {string.Join(", ", formattedResults)}";
        }
    }
}