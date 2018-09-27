﻿using NPoco;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AzureResourceFunctions.Services
{
    public class ResourceRepository
    {
        public void InsertResourceJson(Dtos.Resources newResources)
        {
            SqlConnection con = null;

            try
            {
                con = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process));
                con.Open();
                using (var db = new Database(con))
                {
                    db.Insert(newResources);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("NosyRepo error in InsertStory. " + ex.ToString());
            }
            finally
            {
                if (con != null) con.Close();
            }
        }

        public Dtos.Resources GetLastResource()
        {
            SqlConnection con = null;
            List<Dtos.Resources> results = null; ;
            Dtos.Resources result = null;
            try
            {
                con = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process));
                con.Open();
                using (var db = new Database(con))
                {
                    results = db.Fetch<Dtos.Resources>($"SELECT TOP(1) * FROM Resources ORDER BY Timestamp DESC");
                }

                if (results != null && results.Count > 0)
                    result = results[0];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("NosyRepo error in GetProviders. " + ex.ToString());
            }
            finally
            {
                if (con != null) con.Close();
            }

            return result;
        }
    }
}