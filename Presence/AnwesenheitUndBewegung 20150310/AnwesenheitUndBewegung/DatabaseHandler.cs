using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AnwesenheitUndBewegung.Utilities
{
    class DatabaseHandler
    {
        private string connectionString;

        public DatabaseHandler()
        {
            this.connectionString = AnwesenheitUndBewegung.Properties.Settings.Default.AbsentDBConnectionString;
            TryCreateTables();
            
        }
        
        
        private void TryCreateTables()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                    "CREATE TABLE AbsentTimes (AbsentTimeStart DATETIME, AbsentTimeStop DATETIME)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                    using (SqlCommand command = new SqlCommand(
                    "CREATE TABLE NotMovingTimes (NotMovingTimeStart DATETIME, NotMovingTimeStop DATETIME)", con))
                    {
                        command.ExecuteNonQuery();
                    }
                
                }
                catch(Exception e){

                }
                finally
                {
                    con.Close();
                }
            }

        }
        public void addAbsentTime(DateTime absentTimeStart, DateTime absentTimeStop)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                    "INSERT INTO AbsentTimes VALUES(@AbsentTimeStart, @AbsentTimeStop)", con))
                    {
                        command.Parameters.Add(new SqlParameter("AbsentTimeStart", absentTimeStart));
                        command.Parameters.Add(new SqlParameter("AbsentTimeStop", absentTimeStop));
                        command.ExecuteNonQuery();
                    }
                }
                catch(Exception e)
                {

                }
                finally
                {
                    con.Close();
                }
            }

        }
        public void addNotMovingTime(DateTime notMovingTimeStart, DateTime notMovingTimeStop)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(
                    "INSERT INTO NotMovingTimes VALUES(@NotMovingTimeStart, @NotMovingTimeStop)", con))
                    {
                        command.Parameters.Add(new SqlParameter("NotMovingTimeStart", notMovingTimeStart));
                        command.Parameters.Add(new SqlParameter("NotMovingTimeStop", notMovingTimeStop));
                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    con.Close();
                }
            }
        }
    
    }
}
