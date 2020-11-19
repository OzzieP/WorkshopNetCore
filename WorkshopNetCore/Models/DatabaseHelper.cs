using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

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

            helper.insertOneFeu(new Feu() { matricule = "C1-HR1" });
            helper.insertOneFeu(new Feu() { matricule = "C1-VR1" });
            helper.insertOneFeu(new Feu() { matricule = "C1-HR2" });
            helper.insertOneFeu(new Feu() { matricule = "C1-VR2" });

            Dictionary<string, Feu> feux = helper.selectFeux();


            Random random = new Random();

            for (int jour = 0; jour < 7; jour++)
            {
                for (int h = 0; h < 24; h++)
                {
                    for (int m = 0; m < 60; m++)
                    {
                        bool etatFeu = random.Next(0, 2) == 0;

                        List<Feu> feuxHR = feux.Values.Where(f => f.matricule.StartsWith("C1-HR")).ToList();
                        feuxHR.ForEach(f =>
                        {
                            Etat etat = new Etat()
                            {
                                etat = etatFeu,
                                feu = f,
                                jour = (JourEnum)jour,
                                horaire = new DateTime(1, 1, 1, h, m, 0),
                                nbPassant = (genererAlea(random, h, m))
                            };
                            helper.insertOneEtat(etat);
                        });

                        List<Feu> feuxVR = feux.Values.Where(f => f.matricule.StartsWith("C1-VR")).ToList();
                        feuxVR.ForEach(f =>
                        {
                            Etat etat = new Etat()
                            {
                                etat = !etatFeu,
                                feu = f,
                                jour = (JourEnum)jour,
                                horaire = new DateTime(1, 1, 1, h, m, 0),
                                nbPassant = (genererAlea(random, h, m))
                            };
                            helper.insertOneEtat(etat);
                        });
                    }
                }
            }
        }

        // Jour

        public void insertOneEtat(Etat etat)
        {
            using (SqlConnection connection = new SqlConnection(Builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO etat(idFeu, jour, horaire, nbPassant, etat) VALUES (@feu, @jour, @horaire, @passants, @etat);";
                    command.Parameters.AddWithValue("@feu", etat.feu.idFeu);
                    command.Parameters.AddWithValue("@jour", etat.jour);
                    command.Parameters.AddWithValue("@horaire", etat.horaire);
                    command.Parameters.AddWithValue("@passants", etat.nbPassant);
                    command.Parameters.AddWithValue("@etat", etat.etat);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void insertOneFeu(Feu feu)
        {
            using (SqlConnection connection = new SqlConnection(Builder.ConnectionString))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO feu(matricule) VALUES (@matricule)";
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
    }
}
