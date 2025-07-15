using System;
using System.IO;
using System.Linq;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace FahrzeugVerwaltung
{
    public class PdfService
    {
        /// <summary>
        /// Erstellt ein PDF-Infoblatt für ein Fahrzeug
        /// </summary>
        /// <param name="fahrzeug">Das Fahrzeug für das PDF</param>
        /// <param name="dateiPfad">Der Pfad wo das PDF gespeichert werden soll</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        public bool ErstelleFahrzeugPdf(Fahrzeug fahrzeug, string dateiPfad)
        {
            try
            {
                // PDF-Dokument erstellen
                using (var document = new iTextSharp.text.Document(PageSize.A4, 50, 50, 50, 50))
                {
                    using (var writer = PdfWriter.GetInstance(document, new FileStream(dateiPfad, FileMode.Create)))
                    {
                        document.Open();

                        // Titel
                        var titelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                        var titel = new iTextSharp.text.Paragraph("FAHRZEUG-INFOBLATT", titelFont)
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 20
                        };
                        document.Add(titel);

                        // Datum
                        var datumFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);
                        var datum = new iTextSharp.text.Paragraph($"Erstellt am: {DateTime.Now:dd.MM.yyyy HH:mm}", datumFont)
                        {
                            Alignment = Element.ALIGN_RIGHT,
                            SpacingAfter = 30
                        };
                        document.Add(datum);

                        // Fahrzeugdaten als Tabelle
                        var table = new PdfPTable(2)
                        {
                            WidthPercentage = 100,
                            SpacingAfter = 20
                        };
                        table.SetWidths(new float[] { 1f, 2f });

                        // Tabellen-Style
                        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                        var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                        var labelFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.DARK_GRAY);

                        // Header
                        var headerCell1 = new PdfPCell(new Phrase("Eigenschaft", headerFont))
                        {
                            BackgroundColor = BaseColor.DARK_GRAY,
                            Padding = 10,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        var headerCell2 = new PdfPCell(new Phrase("Wert", headerFont))
                        {
                            BackgroundColor = BaseColor.DARK_GRAY,
                            Padding = 10,
                            HorizontalAlignment = Element.ALIGN_CENTER
                        };
                        table.AddCell(headerCell1);
                        table.AddCell(headerCell2);

                        // Fahrzeugdaten hinzufügen
                        AddTableRow(table, "Marke", fahrzeug.Marke, labelFont, contentFont);
                        AddTableRow(table, "Modell", fahrzeug.Modell, labelFont, contentFont);
                        AddTableRow(table, "Baujahr", fahrzeug.Baujahr.ToString(), labelFont, contentFont);
                        AddTableRow(table, "Leistung", $"{fahrzeug.Leistung} PS", labelFont, contentFont);
                        AddTableRow(table, "Kilometerstand", fahrzeug.KilometerstandFormatiert, labelFont, contentFont);
                        AddTableRow(table, "Farbe", fahrzeug.Farbe, labelFont, contentFont);
                        AddTableRow(table, "Kaufpreis", fahrzeug.KaufpreisFormatiert, labelFont, contentFont);

                        // Berechnete Werte
                        var calculatedFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLUE);
                        AddTableRow(table, "Aktueller Wert", fahrzeug.AktuellerWertFormatiert, labelFont, calculatedFont);
                        AddTableRow(table, "Alter", $"{fahrzeug.GetAlter()} Jahre", labelFont, calculatedFont);
                        AddTableRow(table, "Wertverlust", $"{((fahrzeug.Kaufpreis - fahrzeug.AktuellerWert) / fahrzeug.Kaufpreis * 100):F1}%", labelFont, calculatedFont);

                        document.Add(table);

                        // Zusätzliche Informationen
                        var infoTitel = new iTextSharp.text.Paragraph("ZUSÄTZLICHE INFORMATIONEN",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY))
                        {
                            SpacingBefore = 20,
                            SpacingAfter = 10
                        };
                        document.Add(infoTitel);

                        var infoText = new StringBuilder();
                        infoText.AppendLine($"• Fahrzeugtyp: {(fahrzeug.IsOldtimer() ? "Oldtimer" : "Gebrauchtwagen")}");
                        infoText.AppendLine($"• Vollständige Bezeichnung: {fahrzeug.GetDetailedDescription()}");
                        infoText.AppendLine($"• Durchschnittliche Laufleistung pro Jahr: {(fahrzeug.Kilometerstand / Math.Max(fahrzeug.GetAlter(), 1)):N0} km");

                        if (fahrzeug.IsOldtimer())
                        {
                            infoText.AppendLine("• Oldtimer-Status: Dieses Fahrzeug ist über 30 Jahre alt und gilt als Oldtimer.");
                        }

                        var info = new iTextSharp.text.Paragraph(infoText.ToString(),
                            FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK))
                        {
                            SpacingAfter = 20
                        };
                        document.Add(info);

                        // Bewertungshinweise
                        var bewertungTitel = new iTextSharp.text.Paragraph("BEWERTUNGSHINWEISE",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.DARK_GRAY))
                        {
                            SpacingBefore = 20,
                            SpacingAfter = 10
                        };
                        document.Add(bewertungTitel);

                        var bewertungText = @"Der angegebene aktuelle Wert ist eine automatische Schätzung basierend auf:
• Alter des Fahrzeugs (15% Wertverlust pro Jahr in den ersten 3 Jahren, dann 10% pro Jahr)
• Kilometerstand (2% Wertverlust pro 10.000 km)
• Mindestwert von 5% des ursprünglichen Kaufpreises

Diese Bewertung ersetzt keine professionelle Fahrzeugbewertung und dient nur als Richtwert.
Faktoren wie Zustand, Wartungshistorie, Unfallschäden und Marktlage werden nicht berücksichtigt.";

                        var bewertung = new iTextSharp.text.Paragraph(bewertungText,
                            FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.GRAY))
                        {
                            SpacingAfter = 30
                        };
                        document.Add(bewertung);

                        // Fußzeile
                        var fusszeile = new iTextSharp.text.Paragraph("Erstellt mit Fahrzeugverwaltung - Alle Angaben ohne Gewähr",
                            FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.LIGHT_GRAY))
                        {
                            Alignment = Element.ALIGN_CENTER
                        };
                        document.Add(fusszeile);

                        document.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Erstellen des PDFs: {ex.Message}");
            }
        }

        /// <summary>
        /// Hilfsmethode zum Hinzufügen einer Tabellenzeile
        /// </summary>
        private void AddTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            var labelCell = new PdfPCell(new Phrase(label, labelFont))
            {
                Padding = 8,
                BackgroundColor = BaseColor.LIGHT_GRAY,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            var valueCell = new PdfPCell(new Phrase(value, valueFont))
            {
                Padding = 8,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        /// <summary>
        /// Erstellt ein PDF mit einer Fahrzeugliste
        /// </summary>
        /// <param name="fahrzeuge">Liste der Fahrzeuge</param>
        /// <param name="dateiPfad">Pfad für das PDF</param>
        /// <returns>True wenn erfolgreich</returns>
        public bool ErstelleFahrzeuglistePdf(System.Collections.Generic.List<Fahrzeug> fahrzeuge, string dateiPfad)
        {
            try
            {
                using (var document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 30, 30, 30, 30))
                {
                    using (var writer = PdfWriter.GetInstance(document, new FileStream(dateiPfad, FileMode.Create)))
                    {
                        document.Open();

                        // Titel
                        var titel = new iTextSharp.text.Paragraph("FAHRZEUGLISTE",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.DARK_GRAY))
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 20
                        };
                        document.Add(titel);

                        // Datum und Anzahl
                        var info = new iTextSharp.text.Paragraph($"Erstellt am: {DateTime.Now:dd.MM.yyyy HH:mm} | Anzahl Fahrzeuge: {fahrzeuge.Count}",
                            FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY))
                        {
                            Alignment = Element.ALIGN_CENTER,
                            SpacingAfter = 20
                        };
                        document.Add(info);

                        // Tabelle mit allen Fahrzeugen
                        var table = new PdfPTable(8) { WidthPercentage = 100 };
                        table.SetWidths(new float[] { 1f, 1.2f, 0.8f, 0.8f, 1f, 1f, 0.8f, 1.2f });

                        // Header
                        var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                        var headers = new[] { "Marke", "Modell", "Baujahr", "Leistung", "Kilometer", "Kaufpreis", "Farbe", "Akt. Wert" };

                        foreach (var header in headers)
                        {
                            var headerCell = new PdfPCell(new Phrase(header, headerFont))
                            {
                                BackgroundColor = BaseColor.DARK_GRAY,
                                Padding = 5,
                                HorizontalAlignment = Element.ALIGN_CENTER
                            };
                            table.AddCell(headerCell);
                        }

                        // Datenzeilen
                        var contentFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);
                        foreach (var fahrzeug in fahrzeuge)
                        {
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.Marke, contentFont)) { Padding = 5 });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.Modell, contentFont)) { Padding = 5 });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.Baujahr.ToString(), contentFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                            table.AddCell(new PdfPCell(new Phrase($"{fahrzeug.Leistung} PS", contentFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.KilometerstandFormatiert, contentFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.KaufpreisFormatiert, contentFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.Farbe, contentFont)) { Padding = 5 });
                            table.AddCell(new PdfPCell(new Phrase(fahrzeug.AktuellerWertFormatiert, contentFont)) { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                        }

                        document.Add(table);

                        // Zusammenfassung
                        var gesamtKaufpreis = fahrzeuge.Sum(f => f.Kaufpreis);
                        var gesamtAktuellerWert = fahrzeuge.Sum(f => f.AktuellerWert);
                        var gesamtWertverlust = gesamtKaufpreis - gesamtAktuellerWert;

                        var zusammenfassung = new iTextSharp.text.Paragraph($"\nZUSAMMENFASSUNG:\n" +
                            $"Gesamter Kaufpreis: {gesamtKaufpreis:C}\n" +
                            $"Gesamter aktueller Wert: {gesamtAktuellerWert:C}\n" +
                            $"Gesamter Wertverlust: {gesamtWertverlust:C} ({(gesamtWertverlust / gesamtKaufpreis * 100):F1}%)",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.DARK_GRAY))
                        {
                            SpacingBefore = 20
                        };
                        document.Add(zusammenfassung);

                        document.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Erstellen der Fahrzeugliste: {ex.Message}");
            }
        }
    }
}