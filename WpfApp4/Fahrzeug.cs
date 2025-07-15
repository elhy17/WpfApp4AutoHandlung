using System;
using System.ComponentModel;

namespace FahrzeugVerwaltung
{
    public class Fahrzeug : INotifyPropertyChanged
    {
        private int _id;
        private string _marke;
        private string _modell;
        private int _baujahr;
        private int _leistung;
        private int _kilometerstand;
        private decimal _kaufpreis;
        private string _farbe;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Marke
        {
            get => _marke;
            set
            {
                _marke = value;
                OnPropertyChanged(nameof(Marke));
            }
        }

        public string Modell
        {
            get => _modell;
            set
            {
                _modell = value;
                OnPropertyChanged(nameof(Modell));
            }
        }

        public int Baujahr
        {
            get => _baujahr;
            set
            {
                _baujahr = value;
                OnPropertyChanged(nameof(Baujahr));
                OnPropertyChanged(nameof(AktuellerWert));
                OnPropertyChanged(nameof(AktuellerWertFormatiert));
            }
        }

        public int Leistung
        {
            get => _leistung;
            set
            {
                _leistung = value;
                OnPropertyChanged(nameof(Leistung));
            }
        }

        public int Kilometerstand
        {
            get => _kilometerstand;
            set
            {
                _kilometerstand = value;
                OnPropertyChanged(nameof(Kilometerstand));
                OnPropertyChanged(nameof(KilometerstandFormatiert));
                OnPropertyChanged(nameof(AktuellerWert));
                OnPropertyChanged(nameof(AktuellerWertFormatiert));
            }
        }

        public decimal Kaufpreis
        {
            get => _kaufpreis;
            set
            {
                _kaufpreis = value;
                OnPropertyChanged(nameof(Kaufpreis));
                OnPropertyChanged(nameof(KaufpreisFormatiert));
                OnPropertyChanged(nameof(AktuellerWert));
                OnPropertyChanged(nameof(AktuellerWertFormatiert));
            }
        }

        public string Farbe
        {
            get => _farbe;
            set
            {
                _farbe = value;
                OnPropertyChanged(nameof(Farbe));
            }
        }

        // Formatierte Eigenschaften für die Anzeige
        public string KilometerstandFormatiert => $"{Kilometerstand:N0} km";
        public string KaufpreisFormatiert => $"{Kaufpreis:C}";
        public string AktuellerWertFormatiert => $"{AktuellerWert:C}";

        // Berechnet den aktuellen Wert basierend auf Alter und Kilometerstand
        public decimal AktuellerWert
        {
            get
            {
                return BerechneAktuellenWert();
            }
        }

        /// <summary>
        /// Berechnet den aktuellen Wert des Fahrzeugs
        /// Formel: Wertverlust basierend auf Alter und Kilometerstand
        /// </summary>
        /// <returns>Aktueller Wert des Fahrzeugs</returns>
        public decimal BerechneAktuellenWert()
        {
            if (Kaufpreis <= 0 || Baujahr <= 0)
                return 0;

            int aktuellesJahr = DateTime.Now.Year;
            int alter = aktuellesJahr - Baujahr;

            // Wertverlust pro Jahr: 15% in den ersten 3 Jahren, dann 10% pro Jahr
            decimal wertverlustProzent = 0;

            for (int i = 0; i < alter; i++)
            {
                if (i < 3)
                    wertverlustProzent += 0.15m; // 15% pro Jahr in den ersten 3 Jahren
                else
                    wertverlustProzent += 0.10m; // 10% pro Jahr danach
            }

            // Zusätzlicher Wertverlust basierend auf Kilometerstand
            // Pro 10.000 km: 2% Wertverlust
            decimal kmWertverlust = (Kilometerstand / 10000m) * 0.02m;

            wertverlustProzent += kmWertverlust;

            // Maximal 90% Wertverlust
            if (wertverlustProzent > 0.90m)
                wertverlustProzent = 0.90m;

            decimal aktuellerWert = Kaufpreis * (1 - wertverlustProzent);

            // Mindestwert: 5% des Kaufpreises
            decimal mindestwert = Kaufpreis * 0.05m;
            if (aktuellerWert < mindestwert)
                aktuellerWert = mindestwert;

            return Math.Round(aktuellerWert, 2);
        }

        /// <summary>
        /// Gibt eine detaillierte Beschreibung des Fahrzeugs zurück
        /// </summary>
        /// <returns>Formatierte Fahrzeugbeschreibung</returns>
        public string GetDetailedDescription()
        {
            return $"{Marke} {Modell} ({Baujahr}) - {Leistung} PS - {KilometerstandFormatiert} - {Farbe}";
        }

        /// <summary>
        /// Berechnet das Alter des Fahrzeugs in Jahren
        /// </summary>
        /// <returns>Alter in Jahren</returns>
        public int GetAlter()
        {
            return DateTime.Now.Year - Baujahr;
        }

        /// <summary>
        /// Prüft, ob das Fahrzeug als Oldtimer gilt (älter als 30 Jahre)
        /// </summary>
        /// <returns>True wenn Oldtimer, sonst False</returns>
        public bool IsOldtimer()
        {
            return GetAlter() >= 30;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return $"{Marke} {Modell} ({Baujahr})";
        }
    }
}