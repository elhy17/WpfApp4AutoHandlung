using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FahrzeugVerwaltung
{
    public static class Utilities
    {
        /// <summary>
        /// Validiert eine Eingabe für numerische Werte
        /// </summary>
        public static bool IsValidNumber(string input, out double result, double min = double.MinValue, double max = double.MaxValue)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (double.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result >= min && result <= max;
            }

            return false;
        }

        /// <summary>
        /// Validiert eine Eingabe für Ganzzahlen
        /// </summary>
        public static bool IsValidInteger(string input, out int result, int min = int.MinValue, int max = int.MaxValue)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (int.TryParse(input, out result))
            {
                return result >= min && result <= max;
            }

            return false;
        }

        /// <summary>
        /// Validiert eine Eingabe für Dezimalzahlen
        /// </summary>
        public static bool IsValidDecimal(string input, out decimal result, decimal min = decimal.MinValue, decimal max = decimal.MaxValue)
        {
            result = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out result))
            {
                return result >= min && result <= max;
            }

            return false;
        }

        /// <summary>
        /// Bereinigt Eingabetext von überflüssigen Zeichen
        /// </summary>
        public static string CleanInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Mehrfache Leerzeichen durch einzelne ersetzen
            return Regex.Replace(input.Trim(), @"\s+", " ");
        }

        /// <summary>
        /// Zeigt eine Fehlermeldung an
        /// </summary>
        public static void ShowError(string message, string title = "Fehler")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Zeigt eine Warnmeldung an
        /// </summary>
        public static void ShowWarning(string message, string title = "Warnung")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// Zeigt eine Informationsmeldung an
        /// </summary>
        public static void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Zeigt eine Bestätigungsdialog an
        /// </summary>
        public static bool ShowConfirmation(string message, string title = "Bestätigung")
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        /// <summary>
        /// Formatiert einen Preis für die Anzeige
        /// </summary>
        public static string FormatPrice(decimal price)
        {
            return price.ToString("C", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Formatiert einen Kilometerstand für die Anzeige
        /// </summary>
        public static string FormatKilometers(int kilometers)
        {
            return $"{kilometers:N0} km";
        }

        /// <summary>
        /// Berechnet die Differenz zwischen zwei Jahren
        /// </summary>
        public static int CalculateYearsDifference(int fromYear, int toYear)
        {
            return Math.Abs(toYear - fromYear);
        }

        /// <summary>
        /// Prüft, ob ein Baujahr realistisch ist
        /// </summary>
        public static bool IsValidBuildYear(int year)
        {
            int currentYear = DateTime.Now.Year;
            return year >= 1900 && year <= currentYear + 1;
        }

        /// <summary>
        /// Generiert einen Dateinamen basierend auf Fahrzeugdaten
        /// </summary>
        public static string GenerateFileName(Fahrzeug fahrzeug, string extension = "html")
        {
            string marke = CleanInput(fahrzeug.Marke).Replace(" ", "_");
            string modell = CleanInput(fahrzeug.Modell).Replace(" ", "_");
            string datum = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            return $"Fahrzeug_{marke}_{modell}_{datum}.{extension}";
        }

        /// <summary>
        /// Prüft, ob eine Textbox nur numerische Eingaben enthält
        /// </summary>
        public static void SetNumericOnly(TextBox textBox)
        {
            textBox.PreviewTextInput += (sender, e) =>
            {
                e.Handled = !IsNumeric(e.Text);
            };
        }

        /// <summary>
        /// Prüft, ob ein String nur Zahlen enthält
        /// </summary>
        private static bool IsNumeric(string text)
        {
            return Regex.IsMatch(text, @"^[0-9]+$");
        }

        /// <summary>
        /// Konvertiert einen String sicher zu einem Integer
        /// </summary>
        public static int SafeParseInt(string value, int defaultValue = 0)
        {
            if (int.TryParse(value, out int result))
                return result;
            return defaultValue;
        }

        /// <summary>
        /// Konvertiert einen String sicher zu einem Decimal
        /// </summary>
        public static decimal SafeParseDecimal(string value, decimal defaultValue = 0)
        {
            if (decimal.TryParse(value, out decimal result))
                return result;
            return defaultValue;
        }
    }
}