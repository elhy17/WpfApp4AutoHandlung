using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace FahrzeugVerwaltung
{
    public class DatabaseService
    {
        private readonly string _connectionString;
        private readonly string _dbPath;

        public DatabaseService()
        {
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fahrzeuge.db");
            _connectionString = $"Data Source={_dbPath};Version=3;";
            InitializeDatabase();
        }

        /// <summary>
        /// Initialisiert die Datenbank und erstellt die Tabelle falls sie nicht existiert
        /// </summary>
        private void InitializeDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string createTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Fahrzeuge (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Marke TEXT NOT NULL,
                            Modell TEXT NOT NULL,
                            Baujahr INTEGER NOT NULL,
                            Leistung INTEGER NOT NULL,
                            Kilometerstand INTEGER NOT NULL,
                            Kaufpreis DECIMAL(10,2) NOT NULL,
                            Farbe TEXT NOT NULL,
                            ErstelltAm DATETIME DEFAULT CURRENT_TIMESTAMP
                        )";

                    using (var command = new SQLiteCommand(createTableQuery, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Initialisieren der Datenbank: {ex.Message}");
            }
        }

        /// <summary>
        /// Speichert ein neues Fahrzeug in der Datenbank
        /// </summary>
        /// <param name="fahrzeug">Das zu speichernde Fahrzeug</param>
        /// <returns>Die ID des gespeicherten Fahrzeugs</returns>
        public int SpeichereFahrzeug(Fahrzeug fahrzeug)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string insertQuery = @"
                        INSERT INTO Fahrzeuge (Marke, Modell, Baujahr, Leistung, Kilometerstand, Kaufpreis, Farbe)
                        VALUES (@Marke, @Modell, @Baujahr, @Leistung, @Kilometerstand, @Kaufpreis, @Farbe);
                        SELECT last_insert_rowid();";

                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Marke", fahrzeug.Marke);
                        command.Parameters.AddWithValue("@Modell", fahrzeug.Modell);
                        command.Parameters.AddWithValue("@Baujahr", fahrzeug.Baujahr);
                        command.Parameters.AddWithValue("@Leistung", fahrzeug.Leistung);
                        command.Parameters.AddWithValue("@Kilometerstand", fahrzeug.Kilometerstand);
                        command.Parameters.AddWithValue("@Kaufpreis", fahrzeug.Kaufpreis);
                        command.Parameters.AddWithValue("@Farbe", fahrzeug.Farbe);

                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Speichern des Fahrzeugs: {ex.Message}");
            }
        }

        /// <summary>
        /// Lädt alle Fahrzeuge aus der Datenbank
        /// </summary>
        /// <returns>Liste aller Fahrzeuge</returns>
        public List<Fahrzeug> LadeAlleFahrzeuge()
        {
            var fahrzeuge = new List<Fahrzeug>();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string selectQuery = "SELECT * FROM Fahrzeuge ORDER BY Marke, Modell";

                    using (var command = new SQLiteCommand(selectQuery, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fahrzeug = new Fahrzeug
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Marke = reader["Marke"].ToString(),
                                Modell = reader["Modell"].ToString(),
                                Baujahr = Convert.ToInt32(reader["Baujahr"]),
                                Leistung = Convert.ToInt32(reader["Leistung"]),
                                Kilometerstand = Convert.ToInt32(reader["Kilometerstand"]),
                                Kaufpreis = Convert.ToDecimal(reader["Kaufpreis"]),
                                Farbe = reader["Farbe"].ToString()
                            };

                            fahrzeuge.Add(fahrzeug);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Laden der Fahrzeuge: {ex.Message}");
            }

            return fahrzeuge;
        }

        /// <summary>
        /// Sucht Fahrzeuge basierend auf einem Suchbegriff
        /// </summary>
        /// <param name="suchbegriff">Der Suchbegriff</param>
        /// <returns>Liste der gefundenen Fahrzeuge</returns>
        public List<Fahrzeug> SucheFahrzeuge(string suchbegriff)
        {
            var fahrzeuge = new List<Fahrzeug>();

            if (string.IsNullOrWhiteSpace(suchbegriff))
                return LadeAlleFahrzeuge();

            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string searchQuery = @"
                        SELECT * FROM Fahrzeuge 
                        WHERE Marke LIKE @Suchbegriff 
                           OR Modell LIKE @Suchbegriff 
                           OR Farbe LIKE @Suchbegriff
                           OR CAST(Baujahr AS TEXT) LIKE @Suchbegriff
                        ORDER BY Marke, Modell";

                    using (var command = new SQLiteCommand(searchQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Suchbegriff", $"%{suchbegriff}%");

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var fahrzeug = new Fahrzeug
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Marke = reader["Marke"].ToString(),
                                    Modell = reader["Modell"].ToString(),
                                    Baujahr = Convert.ToInt32(reader["Baujahr"]),
                                    Leistung = Convert.ToInt32(reader["Leistung"]),
                                    Kilometerstand = Convert.ToInt32(reader["Kilometerstand"]),
                                    Kaufpreis = Convert.ToDecimal(reader["Kaufpreis"]),
                                    Farbe = reader["Farbe"].ToString()
                                };

                                fahrzeuge.Add(fahrzeug);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Suchen der Fahrzeuge: {ex.Message}");
            }

            return fahrzeuge;
        }

        /// <summary>
        /// Aktualisiert ein bestehendes Fahrzeug in der Datenbank
        /// </summary>
        /// <param name="fahrzeug">Das zu aktualisierende Fahrzeug</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        public bool AktualisiereFahrzeug(Fahrzeug fahrzeug)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string updateQuery = @"
                        UPDATE Fahrzeuge 
                        SET Marke = @Marke, 
                            Modell = @Modell, 
                            Baujahr = @Baujahr, 
                            Leistung = @Leistung, 
                            Kilometerstand = @Kilometerstand, 
                            Kaufpreis = @Kaufpreis, 
                            Farbe = @Farbe
                        WHERE Id = @Id";

                    using (var command = new SQLiteCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", fahrzeug.Id);
                        command.Parameters.AddWithValue("@Marke", fahrzeug.Marke);
                        command.Parameters.AddWithValue("@Modell", fahrzeug.Modell);
                        command.Parameters.AddWithValue("@Baujahr", fahrzeug.Baujahr);
                        command.Parameters.AddWithValue("@Leistung", fahrzeug.Leistung);
                        command.Parameters.AddWithValue("@Kilometerstand", fahrzeug.Kilometerstand);
                        command.Parameters.AddWithValue("@Kaufpreis", fahrzeug.Kaufpreis);
                        command.Parameters.AddWithValue("@Farbe", fahrzeug.Farbe);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Aktualisieren des Fahrzeugs: {ex.Message}");
            }
        }

        /// <summary>
        /// Löscht ein Fahrzeug aus der Datenbank
        /// </summary>
        /// <param name="id">Die ID des zu löschenden Fahrzeugs</param>
        /// <returns>True wenn erfolgreich, sonst False</returns>
        public bool LoescheFahrzeug(int id)
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string deleteQuery = "DELETE FROM Fahrzeuge WHERE Id = @Id";

                    using (var command = new SQLiteCommand(deleteQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);

                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Löschen des Fahrzeugs: {ex.Message}");
            }
        }

        /// <summary>
        /// Gibt die Anzahl der Fahrzeuge in der Datenbank zurück
        /// </summary>
        /// <returns>Anzahl der Fahrzeuge</returns>
        public int GetAnzahlFahrzeuge()
        {
            try
            {
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    connection.Open();

                    string countQuery = "SELECT COUNT(*) FROM Fahrzeuge";

                    using (var command = new SQLiteCommand(countQuery, connection))
                    {
                        var result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Fehler beim Ermitteln der Fahrzeuganzahl: {ex.Message}");
            }
        }
    }
}