using TrackingApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TrackingApp.Models;

namespace TrackingApp.Services
{
    public class DataService
    {
        public T DeleteAllUsersAndInsert<T>(T model) where T : class
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldRecords = da.GetUser<T>(false);
                    foreach (var oldRecord in oldRecords)
                    {
                        da.Delete(oldRecord);
                    }
                    da.Insert(model);
                    return model;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return model;
            }
        }

        public T DeleteAllThemesAndInsert<T>(T model) where T : class
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldRecords = da.GetTheme<T>(false);
                    foreach (var oldRecord in oldRecords)
                    {
                        da.Delete(oldRecord);
                    }
                    da.Insert(model);
                    return model;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return model;
            }
        }

        public void DeleteAllProjectsAndInsert(List<Project> projects=null)
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldRecords = da.GetProjects<Project>();
                    foreach (var oldRecord in oldRecords)
                    {
                        da.Delete(oldRecord);
                    }
                    if (projects != null)
                    {
                        foreach (var project in projects)
                        {
                            da.Insert(project);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        public void DeleteAll()
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var users = da.GetTheme<User>(false);
                    foreach (var user in users)
                    {
                        da.Delete(user);
                    }
                    var themes = da.GetTheme<Theme>(false);
                    foreach (var theme in themes)
                    {
                        da.Delete(theme);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        //public T InsertOrUpdate<T>(T model) where T : class
        //{
        //    try
        //    {
        //        using (var da = new DataAccess())
        //        {
        //            var oldRecord = da.Find<T>(model.GetHashCode(), false);
        //            if (oldRecord != null)
        //            {
        //                da.Update(model);
        //            }
        //            else
        //            {
        //                da.Insert(model);
        //            }

        //            return model;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //        return model;
        //    }
        //}

        public T Insert<T>(T model)
        {
            using (var da = new DataAccess())
            {
                da.Insert(model);
                return model;
            }
        }

        //public T Find<T>(int pk, bool withChildren) where T : class
        //{
        //    using (var da = new DataAccess())
        //    {
        //        //return da.Find<T>(pk, withChildren);
        //    }
        //}

        public User GetUser<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.GetUser<T>(withChildren).FirstOrDefault();
            }
        }

        public Theme GetTheme<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.GetTheme<T>(withChildren).FirstOrDefault();
            }
        }

        public List<Project> GetProjects<T>() where T : class
        {
            using (var da = new DataAccess())
            {
                return da.GetProjects<T>();
            }
        }

        //public List<T> Get<T>(bool withChildren) where T : class
        //{
        //    using (var da = new DataAccess())
        //    {
        //        return da.GetList<T>(withChildren).ToList();
        //    }
        //}

        public void Update<T>(T model)
        {
            using (var da = new DataAccess())
            {
                da.Update(model);
            }
        }

        public void Delete<T>(T model)
        {
            using (var da = new DataAccess())
            {
                da.Delete(model);
            }
        }

        public void Save<T>(List<T> list) where T : class
        {
            using (var da = new DataAccess())
            {
                foreach (var record in list)
                {
                    //InsertOrUpdate(record);
                }
            }
        }
    }

}
