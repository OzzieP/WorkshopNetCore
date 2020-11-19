using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

using WorkshopNetCore.MachineLearning;

namespace WorkshopNetCore.Models
{
    public class DatabaseHelper
    {
        public MySqlConnectionStringBuilder Builder { get; set; }

        private string _connectionString { get; set; }

        public DatabaseHelper()
        {
            _connectionString = "Data Source=146.59.229.11;Initial Catalog=Workshop;User ID=admin;Password=EPSIworkshop2020*";

            Builder = new MySqlConnectionStringBuilder
            {
                Server = "vps-f0d101aa.vps.ovh.net",
                Database = "workshop",
                UserID = "BLegendre",
                Password = "EPSIworkshop2020*",
                SslMode = MySqlSslMode.None
            };
        }

        //Script d'insertion

        private static int genererAlea(Random random, int h, int m)
        {
            int time = h * m;
            if (time < 360)
            {
                return random.Next(0, 3);
            }
            else if (time < 420)
            {
                return random.Next(2, 6);
            }
            else if (time < 540)
            {
                return random.Next(4, 9);
            }
            else if (time < 780)
            {
                return random.Next(2, 6);
            }
            else if (time < 840)
            {
                return random.Next(4, 9);
            }
            else if (time < 960)
            {
                return random.Next(2, 6);
            }
            else if (time < 1140)
            {
                return random.Next(4, 9);
            }
            else if (time < 1260)
            {
                return random.Next(2, 5);
            }
            else if (time < 1440)
            {
                return random.Next(0, 3);
            }

            return 0;
        }

        public static void ScriptInsertion()
        {
            DatabaseHelper helper = new DatabaseHelper();

            helper.insertOneFeu(new Feu() { idFeu = 1, matricule = "C1-HR1" });
            helper.insertOneFeu(new Feu() { idFeu = 2, matricule = "C1-VR1" });
            helper.insertOneFeu(new Feu() { idFeu = 3, matricule = "C1-HR2" });
            helper.insertOneFeu(new Feu() { idFeu = 4, matricule = "C1-VR2" });

            Dictionary<string, Feu> feux = helper.selectFeux();


            Random random = new Random();
            Dictionary<string, int> passants = new Dictionary<string, int>();
            feux.Values.ToList().ForEach(f => { passants.Add(f.matricule, 0); });

            int c = 1;
            for (int jour = 0; jour < 7; jour++)
            {
                for (int h = 0; h < 24; h++)
                {
                    for (int m = 0; m < 60; m++)
                    {
                        int C1_VR1 = passants["C1-VR1"] + genererAlea(random, h, m);
                        int C1_VR2 = passants["C1-VR2"] + genererAlea(random, h, m);
                        int C1_HR1 = passants["C1-HR1"] + genererAlea(random, h, m);
                        int C1_HR2 = passants["C1-HR2"] + genererAlea(random, h, m);

                        bool etatFeuHori = (C1_HR1 > C1_VR1 && C1_HR1 > C1_VR2) || (C1_HR2 > C1_VR1 && C1_HR2 > C1_VR2);

                        if (etatFeuHori)
                        {
                            passants["C1_VR1"] = C1_VR1;
                            passants["C1_VR2"] = C1_VR2;
                            passants["C1_HR1"] = 0;
                            passants["C1_HR2"] = 0;
                        }
                        else
                        {
                            passants["C1_VR1"] = 0;
                            passants["C1_VR2"] = 0;
                            passants["C1_HR1"] = C1_HR1;
                            passants["C1_HR2"] = C1_HR2;
                        }

                        Etat etat;

                        etat = new Etat()
                        {
                            idEtat = c++,
                            etat = !etatFeuHori,
                            feu = feux["C1-VR1"],
                            jour = (JourEnum)jour,
                            horaire = new DateTime(2000, 1, 1, h, m, 0),
                            semaine = 47,
                            nbPassant = C1_VR1
                        };
                        helper.insertOneEtat(etat);

                        etat = new Etat()
                        {
                            idEtat = c++,
                            etat = !etatFeuHori,
                            feu = feux["C1-VR2"],
                            jour = (JourEnum)jour,
                            horaire = new DateTime(2000, 1, 1, h, m, 0),
                            semaine = 47,
                            nbPassant = C1_VR2
                        };
                        helper.insertOneEtat(etat);

                        etat = new Etat()
                        {
                            idEtat = c++,
                            etat = etatFeuHori,
                            feu = feux["C1-HR1"],
                            jour = (JourEnum)jour,
                            horaire = new DateTime(2000, 1, 1, h, m, 0),
                            semaine = 47,
                            nbPassant = C1_HR1
                        };
                        helper.insertOneEtat(etat);

                        etat = new Etat()
                        {
                            idEtat = c++,
                            etat = etatFeuHori,
                            feu = feux["C1-HR2"],
                            jour = (JourEnum)jour,
                            horaire = new DateTime(2000, 1, 1, h, m, 0),
                            semaine = 47,
                            nbPassant = C1_HR2
                        };
                        helper.insertOneEtat(etat);
                    }
                }
            }
        }

        // Jour

        public void insertOneEtat(Etat etat)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO etat(idEtat, idFeu, jour, horaire, nbPassant, etat, numWeek) VALUES (@id, @feu, @jour, @horaire, @passants, @etat, @semaine);";
                    command.Parameters.AddWithValue("@id", etat.idEtat);
                    command.Parameters.AddWithValue("@feu", etat.feu.idFeu);
                    command.Parameters.AddWithValue("@jour", etat.jour);
                    command.Parameters.AddWithValue("@horaire", etat.horaire);
                    command.Parameters.AddWithValue("@passants", etat.nbPassant);
                    command.Parameters.AddWithValue("@etat", etat.etat);
                    command.Parameters.AddWithValue("@semaine", etat.semaine);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOneFeu(Feu feu)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO feu(idFeu, matricule) VALUES (@feu, @matricule)";
                    command.Parameters.AddWithValue("@feu", feu.idFeu);
                    command.Parameters.AddWithValue("@matricule", feu.matricule);
                    command.ExecuteNonQuery();
                }
            }
        }

        public Dictionary<string, Feu> selectFeux()
        {
            Dictionary<string, Feu> feux = new Dictionary<string, Feu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM feu";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Feu feu = new Feu
                            {
                                idFeu = (int)reader["idFeu"],
                                matricule = reader["matricule"].ToString()
                            };

                            feux.Add(feu.matricule, feu);
                        }
                    }
                }
            }

            return feux;
        }

        public List<Etat> SelectNombreVoitureParVoie(int jour)
        {
            List<Etat> etat = new List<Etat>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT e.jour, f.idFeu, f.matricule, SUM(e.nbPassant) as nombre FROM etat e INNER JOIN feu f ON e.idFeu = f.idFeu WHERE e.jour = @jour GROUP BY f.idFeu, f.matricule, e.jour";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@jour", jour);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Feu feu = new Feu
                            {
                                idFeu = (int)reader["idFeu"],
                                matricule = reader["matricule"].ToString()
                            };

                            Etat unEtat = new Etat
                            {
                                jour = (JourEnum)reader["jour"],
                                feu = feu,
                                nbPassant = (int)reader["nombre"],
                                etat = false
                            };

                            etat.Add(unEtat);
                        }
                    }
                }
            }

            return etat;
        }

        public List<Feu> GetListFeu()
        {
            List<Feu> feux = new List<Feu>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * from feu";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Feu feu = new Feu
                            {
                                idFeu = (int)reader["idFeu"],
                                matricule = reader["matricule"].ToString()
                            };

                            feux.Add(feu);
                        }
                    }
                }
            }

            return feux;
        }

        public Etat GetEtatFeu(string matricule)
        {
            Etat etat = new Etat();
            int heure = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int jour = (int)DateTime.Today.DayOfWeek;
            DateTime dateTime = new DateTime(2000, 01, 01, heure, minute, 0);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "select etat, nbPassant from etat inner join feu on feu.idFeu = etat.idFeu where feu.matricule = @matricule and etat.horaire = @dateTime and etat.jour = @jour";


                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@matricule", matricule));
                    command.Parameters.Add(new SqlParameter("@jour", jour));
                    command.Parameters.Add(new SqlParameter("@dateTime", dateTime));
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            etat.etat = (bool)reader["etat"];
                            etat.nbPassant = (int)reader["nbPassant"];
                        }
                    }
                }
            }
            return etat;
        }

        public Boolean LastFiveMinuteFalse(string matricule)
        {
            Boolean falseLastFiveMinutes = true;
            int heure = DateTime.Now.Hour;
            int heureCinqMinutesAvant = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int cinqMinutesAvant = DateTime.Now.Minute - 6;
            if (cinqMinutesAvant < 0)
            {
                cinqMinutesAvant += 60;
                heureCinqMinutesAvant -= 1;
            }
            int jour = (int)DateTime.Today.DayOfWeek;
            DateTime dateTime1 = new DateTime(2000, 01, 01, heureCinqMinutesAvant, cinqMinutesAvant, 0);
            DateTime dateTime2 = new DateTime(2000, 01, 01, heure, minute, 0);
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "select etat from etat inner join feu on feu.idFeu = etat.idFeu where feu.matricule = @matricule and etat.horaire > @datetime1 and etat.horaire < @datetime2 and etat.jour = @jour";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@matricule", matricule));
                    command.Parameters.Add(new SqlParameter("@jour", jour));
                    command.Parameters.Add(new SqlParameter("@datetime1", dateTime1));
                    command.Parameters.Add(new SqlParameter("@datetime2", dateTime2));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            falseLastFiveMinutes = (bool)reader["etat"] ? true : falseLastFiveMinutes;
                        }
                    }
                }
            }
            return falseLastFiveMinutes;
        }

        public void SetEtatFeu(string matricule, int etat, DateTime datetime, int jour, int nbPassant)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE etat SET etat = @etat, nbPassant = @nbPassant from etat inner join feu on feu.idFeu = etat.idFeu WHERE feu.matricule = @matricule and etat.horaire = @datetime and etat.jour = @jour";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@matricule", matricule));
                    command.Parameters.Add(new SqlParameter("@jour", jour));
                    command.Parameters.Add(new SqlParameter("@datetime", datetime));
                    command.Parameters.Add(new SqlParameter("@etat", etat));
                    command.Parameters.Add(new SqlParameter("@nbPassant", nbPassant));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetnbPassant(string matricule, DateTime datetime, int jour, int nbPassant)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "UPDATE etat SET nbPassant = @nbPassant from etat inner join feu on feu.idFeu = etat.idFeu WHERE feu.matricule = @matricule and etat.horaire = @datetime and etat.jour = @jour";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.Add(new SqlParameter("@matricule", matricule));
                    command.Parameters.Add(new SqlParameter("@jour", jour));
                    command.Parameters.Add(new SqlParameter("@datetime", datetime));
                    command.Parameters.Add(new SqlParameter("@nbPassant", nbPassant));
                    command.ExecuteNonQuery();
                }
            }
        }

        public void InsertFeuForecast(List<FeuForecast> forecasts)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                DeleteFeuForecast(forecasts.FirstOrDefault().IdFeu);

                string query = "INSERT INTO etatPrev (idFeu, jour, nbPassantActuel, numWeek, estimation, estimationInferieure, estimationSuperieure) " +
                    "VALUES (@IdFeu, @Jour, @NbPassantActuel, @NumWeek, @Estimation, @EstimationInferieure, @EstimationSuperieure)";

                foreach (FeuForecast feu in forecasts)
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdFeu", feu.IdFeu);
                        command.Parameters.AddWithValue("@Jour", feu.Jour);
                        command.Parameters.AddWithValue("@NbPassantActuel", feu.PassantsActuel);
                        command.Parameters.AddWithValue("@NumWeek", feu.Semaine);
                        command.Parameters.AddWithValue("@Estimation", feu.Estimation);
                        command.Parameters.AddWithValue("@EstimationInferieure", feu.EstimationInferieure);
                        command.Parameters.AddWithValue("@EstimationSuperieure", feu.EstimationSuperieure);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private void DeleteFeuForecast(int idFeu)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "DELETE FROM etatPrev WHERE idFeu = @IdFeu";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@IdFeu", idFeu);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
