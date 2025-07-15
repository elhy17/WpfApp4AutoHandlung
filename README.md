# WpfApp4AutoHandlung
# 🚗 Fahrzeugverwaltung

Ein modernes C# WPF-Anwendungssystem zur effizienten Verwaltung von Fahrzeugen mit automatischer Wertermittlung und PDF-Berichtserstellung.

## 📋 Projektübersicht

Die Fahrzeugverwaltung ist eine Desktop-Anwendung, die es Autohändlern, Fuhrparkmanagern und Privatpersonen ermöglicht, ihre Fahrzeuge strukturiert zu verwalten. Das System bietet eine intuitive Benutzeroberfläche zur Erfassung von Fahrzeugdaten, automatische Wertberechnung basierend auf Alter und Kilometerstand sowie professionelle PDF-Berichte.

## ✨ Features

### 🔧 Fahrzeugverwaltung
- **Vollständige Datenverwaltung**: Marke, Modell, Baujahr, Leistung, Kilometerstand, Farbe und Kaufpreis
- **Automatische Wertberechnung**: Intelligente Ermittlung des aktuellen Fahrzeugwerts
- **Oldtimer-Erkennung**: Automatische Klassifizierung von Fahrzeugen über 30 Jahren
- **Formatierte Anzeige**: Benutzerfreundliche Darstellung von Preisen und Kilometerständen

### 📊 Berichte & Export
- **Einzelfahrzeug-PDF**: Detaillierte Infoblätter mit allen Fahrzeugdaten
- **Fahrzeugliste-PDF**: Übersichtliche Tabelle aller verwalteten Fahrzeuge
- **Zusammenfassungen**: Gesamtwerte, Wertverluste und statistische Auswertungen
- **Professionelles Design**: Moderne PDF-Layouts mit strukturierten Tabellen

### 🧮 Wertermittlung
- **Altersbasierte Bewertung**: 15% Wertverlust in den ersten 3 Jahren, dann 10% jährlich
- **Kilometerstand-Faktor**: 2% Wertverlust pro 10.000 km
- **Mindestwert-Schutz**: Automatische Begrenzung auf mindestens 5% des Kaufpreises
- **Transparente Berechnung**: Nachvollziehbare Bewertungslogik

## 🛠️ Technologie-Stack

- **Framework**: .NET Framework / .NET Core
- **UI-Technologie**: WPF (Windows Presentation Foundation)
- **PDF-Generierung**: iTextSharp
- **Datenbank** : SQLite
- **Programmiersprache**: C#
- **Architektur**: MVVM Pattern

## 📦 Installation

### Voraussetzungen
- Windows 10 oder höher
- .NET Framework 4.8 oder .NET 6.0+
- Visual Studio 2019+ (für Entwicklung)

## 🚀 Verwendung

### Neues Fahrzeug hinzufügen
1. Anwendung starten
2. "Neues Fahrzeug" Button klicken
3. Fahrzeugdaten eingeben
4. Speichern - automatische Wertberechnung erfolgt

## 🎯 Kernkomponenten

### Fahrzeug-Klasse
- Eigenschaften für alle Fahrzeugdaten
- Automatische Formatierung von Preisen und Kilometerständen
- Berechnungsmethoden für Alter und aktuellen Wert
- Oldtimer-Statusprüfung

### PdfService
- Erstellung detaillierter Fahrzeug-Infoblätter
- Generierung von Fahrzeuglisten im Tabellenformat
- Professionelle Formatierung mit iTextSharp
- Automatische Zusammenfassungen und Statistiken

## 🔄 Wertberechnungslogik

```csharp
// Vereinfachtes Beispiel der Wertberechnung
public decimal BerechneAktuellenWert()
{
    decimal wert = Kaufpreis;
    
    // Altersbasierte Abschreibung
    int alter = GetAlter();
    for (int i = 0; i < alter; i++)
    {
        decimal abschreibung = (i < 3) ? 0.15m : 0.10m;
        wert *= (1 - abschreibung);
    }
    
    // Kilometerstand-Abschreibung
    decimal kmAbschreibung = (Kilometerstand / 10000) * 0.02m;
    wert *= (1 - kmAbschreibung);
    
    // Mindestwert sicherstellen
    decimal mindestwert = Kaufpreis * 0.05m;
    return Math.Max(wert, mindestwert);
}
```

## 🤝 Beitragen

Beiträge sind willkommen! Bitte beachten Sie:

1. Fork des Repositories
2. Feature-Branch erstellen (`git checkout -b feature/AmazingFeature`)
3. Änderungen committen (`git commit -m 'Add some AmazingFeature'`)
4. Branch pushen (`git push origin feature/AmazingFeature`)
5. Pull Request erstellen

## 📋 Roadmap

- [ ] Datenbankintegration (SQLite/SQL Server)
- [ ] Erweiterte Suchfunktionen
- [ ] Fahrzeugbilder-Verwaltung
- [ ] Export nach Excel
- [ ] Wartungshistorie
- [ ] Benachrichtigungen für TÜV/Wartung
- [ ] Multi-User-Unterstützung
- [ ] Web-API für externe Systeme

## 🐛 Bekannte Probleme

- PDF-Generierung kann bei sehr großen Listen langsam sein
- PDF-Öffnung direkt nach dem Saving funktioniert nicht
- Währungsformatierung ist derzeit nur für EUR optimiert
- Keine Unterstützung für Fahrzeuge vor 1900

## 📜 Lizenz

Dieses Projekt ist unter der YSA Lizenz lizenziert - siehe [LICENSE](LICENSE) Datei für Details.

## 👥 Autor

**El Haddoury Younes Ayman Alshian Saad Akki ** - *Initial work* 

## 🙏 Danksagungen

- iTextSharp Team für die ausgezeichnete PDF-Bibliothek
- Microsoft für das WPF Framework
- Allen Testern und Contributoren

---

⭐ **Gefällt Ihnen das Projekt? Geben Sie uns einen Stern!** ⭐