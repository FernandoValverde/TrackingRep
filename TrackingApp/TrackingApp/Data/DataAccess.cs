//using SQLite.Net;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using TrackingApp.Interfaces;
using TrackingApp.Models;
using Xamarin.Forms;


namespace TrackingApp.Data
{
    public class DataAccess : IDisposable
    {
        private SQLiteConnection connection;

        public DataAccess()
        {
            var config = DependencyService.Get<IConfig>();
            connection = new SQLiteConnection(System.IO.Path.Combine(config.DirectoryDB, "Tracking.db3"));
            connection.CreateTable<User>();
            connection.CreateTable<Theme>();
            connection.CreateTable<Project>();
            //connection.CreateTable<Parameter>();
        }

        public void Insert<T>(T model)
        {
            connection.Insert(model);
        }

        public void Update<T>(T model)
        {
            connection.Update(model);
        }

        public void Delete<T>(T model)
        {
            connection.Delete(model);
        }

        public List<User> GetUser<T>(bool WithChildren) where T : class
        {
            return connection.Table<User>().ToList();
        }

        public List<Project> GetProjects<T>() where T : class
        {
            return connection.Table<Project>().ToList();
        }

        public List<Theme> GetTheme<T>(bool WithChildren) where T : class
        {
            return connection.Table<Theme>().ToList();
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }

}
