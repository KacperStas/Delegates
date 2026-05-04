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
        private Action _multicastStyleDelegate;

        public MainWindow()
        {
            InitializeComponent();
        }

        // --- Method Definitions (Points 1 & 2) ---

        private double Square(double number) => number * number;
        private double SquareRoot(double number) => Math.Sqrt(number);
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

            if (SquareRadio.IsChecked == true) _selectedOperation = Square;
            else if (SquareRootRadio.IsChecked == true) _selectedOperation = SquareRoot;
            else if (ReciprocalRadio.IsChecked == true) _selectedOperation = Reciprocal;
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
                    MessageBox.Show($"Invalid number detected: '{str.Trim()}'.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (inputNumbers.Count == 0) return;

            List<double> results = ProcessCollection(inputNumbers, _selectedOperation);
            List<string> formattedResults = results.ConvertAll(r => r.ToString("F3"));
            ListResultTextBlock.Text = $"Processed List: {string.Join(", ", formattedResults)}";
        }

        // --- Point 4: Multicast Delegates (UI Styling) ---

        private void ChangeBackgroundColor() => this.Background = Brushes.LightSteelBlue;
        private void ChangeFontColor() => MulticastTargetLabel.Foreground = Brushes.DarkRed;
        private void ChangeFontSize() => MulticastTargetLabel.FontSize = 24;

        private void ApplyStylesButton_Click(object sender, RoutedEventArgs e)
        {
            _multicastStyleDelegate = null;
            _multicastStyleDelegate += ChangeBackgroundColor;
            _multicastStyleDelegate += ChangeFontColor;
            _multicastStyleDelegate += ChangeFontSize;
            _multicastStyleDelegate?.Invoke();
        }

        private void ResetStylesButton_Click(object sender, RoutedEventArgs e)
        {
            this.Background = SystemColors.WindowBrush;
            MulticastTargetLabel.Foreground = SystemColors.ControlTextBrush;
            MulticastTargetLabel.FontSize = 14;
        }

        // --- Point 5: Custom Sorting (Numbers) ---

        // Comparison methods matching Func<double, double, bool>
        // These return 'true' if the elements are out of order and need to be swapped
        private bool ShouldSwapForAscending(double a, double b)
        {
            return a > b;
        }

        private bool ShouldSwapForDescending(double a, double b)
        {
            return a < b;
        }

        /// <summary>
        /// Custom Bubble Sort algorithm. 
        /// Relies entirely on the comparisonDelegate to know how to sort.
        /// </summary>
        private List<double> CustomBubbleSort(List<double> list, Func<double, double, bool> comparisonDelegate)
        {
            // Clone the list so we don't modify the original collection directly
            List<double> sortedList = new List<double>(list);
            int n = sortedList.Count;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    // INVOKE DELEGATE: Ask the delegate if these two specific elements should be swapped
                    if (comparisonDelegate(sortedList[j], sortedList[j + 1]))
                    {
                        // Standard array swap
                        double temp = sortedList[j];
                        sortedList[j] = sortedList[j + 1];
                        sortedList[j + 1] = temp;

                        swapped = true;
                    }
                }

                // If no elements were swapped in the inner loop, the array is sorted
                if (!swapped)
                    break;
            }

            return sortedList;
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            // Parse the input
            string[] inputStrings = SortInputTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<double> inputNumbers = new List<double>();

            foreach (string str in inputStrings)
            {
                if (double.TryParse(str.Trim(), out double num))
                {
                    inputNumbers.Add(num);
                }
                else
                {
                    MessageBox.Show($"Invalid number detected: '{str.Trim()}'.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (inputNumbers.Count == 0)
            {
                MessageBox.Show("Please enter numbers to sort.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Determine which delegate to pass into our custom sort function
            Func<double, double, bool> selectedComparisonMethod;

            if (AscendingRadio.IsChecked == true)
            {
                selectedComparisonMethod = ShouldSwapForAscending;
            }
            else
            {
                selectedComparisonMethod = ShouldSwapForDescending;
            }

            // Execute custom sorting algorithm
            List<double> sortedResults = CustomBubbleSort(inputNumbers, selectedComparisonMethod);

            // Display results
            List<string> formattedResults = sortedResults.ConvertAll(r => r.ToString("G")); // 'G' removes trailing zeros
            SortResultTextBlock.Text = $"Sorted List: {string.Join(", ", formattedResults)}";
        }
    }
}