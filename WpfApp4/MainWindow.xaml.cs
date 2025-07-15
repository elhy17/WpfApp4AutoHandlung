using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace FahrzeugVerwaltung
{
    public partial class MainWindow : Window
    {
        private DatabaseService _databaseService;
        private PdfService _pdfService;
        private List<Fahrzeug> _aktuelleFahrzeuge;
        private Fahrzeug _ausgewaehltesFahrzeug;

        public MainWindow()
        {
            InitializeComponent();
            InitializeServices();
            LadeFahrzeuge();
        }

        /// <summary>
        /// Initialisiert die Services
        /// </summary>
        private void InitializeServices()
        {
            try
            {
                _databaseService = new DatabaseService();
                _pdfService = new PdfService();
                _aktuelleFahrzeuge = new List<Fahrzeug>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Initialisieren der Anwendung: {ex.Message}",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Lädt alle Fahrzeuge aus der Datenbank
        /// </summary>
        private void LadeFahrzeuge()
        {
            try
            {
                _aktuelleFahrzeuge = _databaseService.LadeAlleFahrzeuge();
                dgFahrzeuge.ItemsSource = _aktuelleFahrzeuge;
                UpdateStatusText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Fahrzeuge: {ex.Message}",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualisiert den Status-Text
        /// </summary>
        private void UpdateStatusText()
        {
            if (_aktuelleFahrzeuge.Count == 0)
            {
                txtDetails.Text = "Keine Fahrzeuge vorhanden";
            }
            else
            {
                txtDetails.Text = $"{_aktuelleFahrzeuge.Count} Fahrzeug(e) gefunden";
            }
        }

        /// <summary>
        /// Speichert ein neues Fahrzeug
        /// </summary>
        private void BtnSpeichern_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                var fahrzeug = new Fahrzeug
                {
                    Marke = txtMarke.Text.Trim(),
                    Modell = txtModell.Text.Trim(),
                    Baujahr = int.Parse(txtBaujahr.Text.Trim()),
                    Leistung = int.Parse(txtLeistung.Text.Trim()),
                    Kilometerstand = int.Parse(txtKilometer.Text.Trim()),
                    Kaufpreis = decimal.Parse(txtKaufpreis.Text.Trim()),
                    Farbe = txtFarbe.Text.Trim()
                };

                int neueId = _databaseService.SpeichereFahrzeug(fahrzeug);
                fahrzeug.Id = neueId;

                MessageBox.Show($"Fahrzeug '{fahrzeug.Marke} {fahrzeug.Modell}' wurde erfolgreich gespeichert!",
                    "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                // Eingabefelder leeren
                ClearInputFields();

                // Fahrzeugliste aktualisieren
                LadeFahrzeuge();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Fahrzeugs: {ex.Message}",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Validiert die Eingaben
        /// </summary>
        private bool ValidateInput()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtMarke.Text))
                errors.Add("Marke ist erforderlich");

            if (string.IsNullOrWhiteSpace(txtModell.Text))
                errors.Add("Modell ist erforderlich");

            if (!int.TryParse(txtBaujahr.Text, out int baujahr) || baujahr < 1900 || baujahr > DateTime.Now.Year)
                errors.Add($"Baujahr muss zwischen 1900 und {DateTime.Now.Year} liegen");

            if (!int.TryParse(txtLeistung.Text, out int leistung) || leistung <= 0)
                errors.Add("Leistung muss eine positive Zahl sein");

            if (!int.TryParse(txtKilometer.Text, out int kilometer) || kilometer < 0)
                errors.Add("Kilometerstand muss 0 oder größer sein");

            if (!decimal.TryParse(txtKaufpreis.Text, out decimal kaufpreis) || kaufpreis <= 0)
                errors.Add("Kaufpreis muss eine positive Zahl sein");

            if (string.IsNullOrWhiteSpace(txtFarbe.Text))
                errors.Add("Farbe ist erforderlich");

            if (errors.Any())
            {
                MessageBox.Show($"Bitte korrigieren Sie folgende Fehler:\n\n{string.Join("\n", errors)}",
                    "Eingabefehler", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Leert alle Eingabefelder
        /// </summary>
        private void ClearInputFields()
        {
            txtMarke.Clear();
            txtModell.Clear();
            txtBaujahr.Clear();
            txtLeistung.Clear();
            txtKilometer.Clear();
            txtKaufpreis.Clear();
            txtFarbe.Clear();
            txtMarke.Focus();
        }

        /// <summary>
        /// Sucht nach Fahrzeugen
        /// </summary>
        private void BtnSuchen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string suchbegriff = txtSuche.Text.Trim();
                if (string.IsNullOrWhiteSpace(suchbegriff))
                {
                    MessageBox.Show("Bitte geben Sie einen Suchbegriff ein.",
                        "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                _aktuelleFahrzeuge = _databaseService.SucheFahrzeuge(suchbegriff);
                dgFahrzeuge.ItemsSource = _aktuelleFahrzeuge;
                UpdateStatusText();

                if (_aktuelleFahrzeuge.Count == 0)
                {
                    MessageBox.Show($"Keine Fahrzeuge mit dem Suchbegriff '{suchbegriff}' gefunden.",
                        "Suchergebnis", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Suche: {ex.Message}",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Zeigt alle Fahrzeuge an
        /// </summary>
        private void BtnAlle_Click(object sender, RoutedEventArgs e)
        {
            txtSuche.Clear();
            LadeFahrzeuge();
        }

        /// <summary>
        /// Wird aufgerufen wenn ein Fahrzeug ausgewählt wird
        /// </summary>
        private void DgFahrzeuge_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _ausgewaehltesFahrzeug = dgFahrzeuge.SelectedItem as Fahrzeug;

            if (_ausgewaehltesFahrzeug != null)
            {
                txtDetails.Text = $"Ausgewählt: {_ausgewaehltesFahrzeug.GetDetailedDescription()} | " +
                                 $"Aktueller Wert: {_ausgewaehltesFahrzeug.AktuellerWertFormatiert} | " +
                                 $"Wertverlust: {((_ausgewaehltesFahrzeug.Kaufpreis - _ausgewaehltesFahrzeug.AktuellerWert) / _ausgewaehltesFahrzeug.Kaufpreis * 100):F1}%";
                btnPdf.IsEnabled = true;
            }
            else
            {
                UpdateStatusText();
                btnPdf.IsEnabled = false;
            }
        }

        /// <summary>
        /// Erstellt ein PDF für das ausgewählte Fahrzeug
        /// </summary>
        private void BtnPdf_Click(object sender, RoutedEventArgs e)
        {
            if (_ausgewaehltesFahrzeug == null)
            {
                MessageBox.Show("Bitte wählen Sie erst ein Fahrzeug aus.",
                    "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF-Dateien (*.pdf)|*.pdf",
                    FileName = $"Fahrzeug_{_ausgewaehltesFahrzeug.Marke}_{_ausgewaehltesFahrzeug.Modell}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                    Title = "PDF speichern"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    bool erfolg = _pdfService.ErstelleFahrzeugPdf(_ausgewaehltesFahrzeug, saveFileDialog.FileName);

                    if (erfolg)
                    {
                        MessageBox.Show($"PDF wurde erfolgreich erstellt:\n{saveFileDialog.FileName}",
                            "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Fragen ob PDF geöffnet werden soll
                        var result = MessageBox.Show("Möchten Sie das PDF jetzt öffnen?",
                            "PDF öffnen", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (result == MessageBoxResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Erstellen des PDFs: {ex.Message}",
                    "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Aktualisiert die Fahrzeugliste
        /// </summary>
        private void BtnAktualisieren_Click(object sender, RoutedEventArgs e)
        {
            LadeFahrzeuge();
            MessageBox.Show("Fahrzeugliste wurde aktualisiert.",
                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Behandelt das Schließen des Fensters
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        /// <summary>
        /// Behandelt Tastatureingaben für Shortcuts
        /// </summary>
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            // Strg+S zum Speichern
            if (e.Key == System.Windows.Input.Key.S &&
                System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                BtnSpeichern_Click(null, null);
                e.Handled = true;
            }
            // Strg+F zum Suchen
            else if (e.Key == System.Windows.Input.Key.F &&
                     System.Windows.Input.Keyboard.Modifiers == System.Windows.Input.ModifierKeys.Control)
            {
                txtSuche.Focus();
                e.Handled = true;
            }
            // F5 zum Aktualisieren
            else if (e.Key == System.Windows.Input.Key.F5)
            {
                BtnAktualisieren_Click(null, null);
                e.Handled = true;
            }
            // Escape zum Leeren der Eingabefelder
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                ClearInputFields();
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }
    }
}