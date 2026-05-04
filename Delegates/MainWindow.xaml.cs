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

        // =================================================================================
        // --- POINTS 5 & 6: Generalized Custom Sorting (Generics) ---
        // =================================================================================

        /// <summary>
        /// Point 6: Generic Custom Bubble Sort algorithm. 
        /// Uses <T> to allow sorting of any data type.
        /// Relies entirely on the comparisonDelegate (Func<T, T, bool>) to know how to sort.
        /// </summary>
        private List<T> CustomBubbleSort<T>(List<T> list, Func<T, T, bool> comparisonDelegate)
        {
            List<T> sortedList = new List<T>(list);
            int n = sortedList.Count;
            bool swapped;

            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    // INVOKE DELEGATE: Ask if these two specific elements should be swapped
                    if (comparisonDelegate(sortedList[j], sortedList[j + 1]))
                    {
                        T temp = sortedList[j];
                        sortedList[j] = sortedList[j + 1];
                        sortedList[j + 1] = temp;

                        swapped = true;
                    }
                }
                if (!swapped) break;
            }

            return sortedList;
        }

        // --- Comparison Methods for Numbers (double) ---
        private bool ShouldSwapForAscendingNumbers(double a, double b) => a > b;
        private bool ShouldSwapForDescendingNumbers(double a, double b) => a < b;

        // --- Comparison Methods for Strings (string) ---
        private bool ShouldSwapForAscendingStrings(string a, string b) => string.Compare(a, b) > 0;
        private bool ShouldSwapForDescendingStrings(string a, string b) => string.Compare(a, b) < 0;


        // --- Event Handlers for Sorting UI ---

        private void SortNumbersButton_Click(object sender, RoutedEventArgs e)
        {
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

            if (inputNumbers.Count == 0) return;

            // Determine delegate
            Func<double, double, bool> selectedComparisonMethod = AscendingRadio.IsChecked == true
                ? ShouldSwapForAscendingNumbers
                : ShouldSwapForDescendingNumbers;

            // Execute generic sorting algorithm for Doubles
            List<double> sortedResults = CustomBubbleSort(inputNumbers, selectedComparisonMethod);

            List<string> formattedResults = sortedResults.ConvertAll(r => r.ToString("G"));
            SortResultTextBlock.Text = $"Sorted Numbers: {string.Join(", ", formattedResults)}";
        }

        private void SortStringsButton_Click(object sender, RoutedEventArgs e)
        {
            string[] inputStrings = StringSortInputTextBox.Text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> inputWords = new List<string>();

            foreach (string str in inputStrings)
            {
                if (!string.IsNullOrWhiteSpace(str))
                {
                    inputWords.Add(str.Trim());
                }
            }

            if (inputWords.Count == 0) return;

            // Determine delegate
            Func<string, string, bool> selectedComparisonMethod = StringAscendingRadio.IsChecked == true
                ? ShouldSwapForAscendingStrings
                : ShouldSwapForDescendingStrings;

            // Execute generic sorting algorithm for Strings
            List<string> sortedResults = CustomBubbleSort(inputWords, selectedComparisonMethod);

            StringSortResultTextBlock.Text = $"Sorted Strings: {string.Join(", ", sortedResults)}";
        }
    }
}
