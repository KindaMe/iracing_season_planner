﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using Dapper;
using System.Diagnostics;

namespace ir_planner
{
    internal class SQLiteDataAccess
    {
        public static UserSettingsModel LoadUserSettings()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<UserSettingsModel>("select * from UserSettings", new DynamicParameters());

                return output.First();
            }
        }

        public static List<CarModel> LoadCars()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CarModel>("select * from Cars order by Name", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<TrackModel> LoadTracks()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<TrackModel>("select * from Tracks order by Name", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<LeagueModel> LoadLeagues()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<LeagueModel>("select * from LeaguesDataView", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<CarModel> LoadLeagueCars(LeagueModel league)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<CarModel>("SELECT * FROM Cars_Leagues INNER JOIN Cars ON Car_ID = Cars.ID WHERE League_ID = @ID ORDER BY isOwned DESC,Name", league);
                return output.ToList();
            }
        }

        public static List<StatsModel> LoadMostUsedCar()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<StatsModel>("select * from MostUsedCars", new DynamicParameters());
                return output.ToList();
            }
        }

        public static List<StatsModel> LoadMostUsedTrack()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<StatsModel>("select * from MostUsedTracks", new DynamicParameters());
                return output.ToList();
            }
        }

        public static bool IsTrackOwned(String trackName)//placeholder
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                TrackModel tempTrack = new TrackModel();
                tempTrack.Name = trackName;
                var output = cnn.Query<TrackModel>("select * from Tracks where Name = @Name", tempTrack);
                List<TrackModel> temp = output.ToList();
                return temp[0].isOwned;
            }
        }

        public static void UpdateUserSettings(UserSettingsModel userSettings)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE UserSettings SET " +
                    "FILTER_CLASS_A = @FILTER_CLASS_A," +
                    "FILTER_CLASS_B = @FILTER_CLASS_B," +
                    "FILTER_CLASS_C = @FILTER_CLASS_C," +
                    "FILTER_CLASS_D = @FILTER_CLASS_D," +
                    "FILTER_CLASS_R = @FILTER_CLASS_R," +
                    "FILTER_TYPE_ROAD = @FILTER_TYPE_ROAD," +
                    "FILTER_TYPE_OVAL = @FILTER_TYPE_OVAL," +
                    "FILTER_TYPE_ROAD_DIRT = @FILTER_TYPE_ROAD_DIRT," +
                    "FILTER_TYPE_OVAL_DIRT = @FILTER_TYPE_OVAL_DIRT," +
                    "FILTER_AVAILABLE_ONLY = @FILTER_AVAILABLE_ONLY WHERE ID = @ID", userSettings);
            }
        }

        public static void UpdateTrackInDB(TrackModel track)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Tracks SET isOwned = @isOwned WHERE ID = @ID ", track);
            }
        }

        public static void UpdateCarInDB(CarModel car)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("UPDATE Cars SET isOwned = @isOwned WHERE ID = @ID ", car);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            bool isInVisualStudio = Debugger.IsAttached && AppDomain.CurrentDomain.FriendlyName.EndsWith("vshost.exe");
            if (isInVisualStudio)
            {
                return ConfigurationManager.ConnectionStrings["Default"].ConnectionString;
            }
            else
            {
                return ConfigurationManager.ConnectionStrings["AppData"].ConnectionString;
            }
        }
    }
}