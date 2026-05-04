using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace DelegateTester
{
    public partial class MainWindow : Window
    {
        // Point 1 & 2: The delegate variable for single operations
        private Func<double, double> _selectedOperation;

        // Point 4: The delegate variable for UI styling (multicast)
        // Action is a built-in delegate type that takes no parameters and returns void.
        private Action _multicastStyleDelegate;

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

        private List<double> ProcessCollection(List<double> numbers, Func<double, double> operation)
        {
            List<double> processedNumbers = new List<double>();

            foreach (double number in numbers)
            {
                processedNumbers.Add(operation(number));
            }

            return processedNumbers;
        }

        private void ProcessListButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOperation == null)
            {
                MessageBox.Show("Please select an operation first.", "Missing Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

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

            List<double> results = ProcessCollection(inputNumbers, _selectedOperation);

            List<string> formattedResults = results.ConvertAll(r => r.ToString("F3"));
            ListResultTextBlock.Text = $"Processed List: {string.Join(", ", formattedResults)}";
        }

        // --- Point 4: Multicast Delegates (UI Styling) ---

        private void ChangeBackgroundColor()
        {
            this.Background = Brushes.LightSteelBlue;
        }

        private void ChangeFontColor()
        {
            MulticastTargetLabel.Foreground = Brushes.DarkRed;
        }

        private void ChangeFontSize()
        {
            MulticastTargetLabel.FontSize = 24;
        }

        private void ApplyStylesButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Clear out any previous assignments to prevent duplicates if clicked multiple times
            _multicastStyleDelegate = null;

            // 2. Add multiple methods to the delegate (Chaining / Multicasting)
            _multicastStyleDelegate += ChangeBackgroundColor;
            _multicastStyleDelegate += ChangeFontColor;
            _multicastStyleDelegate += ChangeFontSize;

            // 3. Invoke all methods simultaneously
            _multicastStyleDelegate?.Invoke();
        }

        private void ResetStylesButton_Click(object sender, RoutedEventArgs e)
        {
            // Bonus requirement: Reset window appearance
            this.Background = SystemColors.WindowBrush;
            MulticastTargetLabel.Foreground = SystemColors.ControlTextBrush;
            MulticastTargetLabel.FontSize = 14;
        }
    }
}