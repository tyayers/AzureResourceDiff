using NPoco;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace AzureResourceCommon.Services
{
    public class ResourceRepository
    {
        public void InsertResourceJson(Dtos.Resource newResources)
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

        public void UpdateResourceJson(Dtos.Resource resource)
        {
            SqlConnection con = null;

            try
            {
                con = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process));
                con.Open();
                using (var db = new Database(con))
                {
                    db.Update(resource);
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

        public Dtos.Resource GetLastResource()
        {
            SqlConnection con = null;
            List<Dtos.Resource> results = null; ;
            Dtos.Resource result = null;
            try
            {
                con = new SqlConnection(System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process));
                con.Open();
                using (var db = new Database(con))
                {
                    results = db.Fetch<Dtos.Resource>($"SELECT TOP(1) * FROM Resources ORDER BY Timestamp DESC");
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

        public Dtos.Resource[] GetResources(int count = -1)
        {
            SqlConnection con = null;
            List<Dtos.Resource> results = new List<Dtos.Resource>();
            try
            {
                string conString = System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);
                con = new SqlConnection(conString);
                con.Open();
                using (var db = new Database(con))
                {
                    if (count > 0)
                        results = db.Fetch<Dtos.Resource>($"SELECT TOP({count}) * FROM Resources ORDER BY Timestamp DESC");
                    else
                        results = db.Fetch<Dtos.Resource>($"SELECT * FROM Resources ORDER BY Timestamp DESC");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error in GetResources. " + ex.ToString());
            }
            finally
            {
                if (con != null) con.Close();
            }

            return results.ToArray();
        }

        public Dtos.Resource GetResource(int Id)
        {
            SqlConnection con = null;
            Dtos.Resource result = null;
            try
            {
                string conString = System.Environment.GetEnvironmentVariable("ConnectionString", EnvironmentVariableTarget.Process);
                con = new SqlConnection(conString);
                con.Open();
                using (var db = new Database(con))
                {
                    result = db.SingleById<Dtos.Resource>(Id);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError("Error in GetResources. " + ex.ToString());
            }
            finally
            {
                if (con != null) con.Close();
            }

            return result;
        }
    }
}
