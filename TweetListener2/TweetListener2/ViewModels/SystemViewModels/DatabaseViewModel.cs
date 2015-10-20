using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class DatabaseViewModel : ViewModelBase
    {
        private Database database;

        public Database Database
        {
            get
            {
                return database;
            }

            set
            {
                database = value;
            }
        }

        public override string Name
        {
            get
            {
                return "Database";
            }
        }


        public string PhpPostPath
        {
            get
            {
                return database.localPhpJsonLink;
            }
            set
            {
                database.localPhpJsonLink = value;
            }
        }

        public string TextFileDatabasePath
        {
            get
            {
                return database.TextFileDatabasePath;
            }
            set
            {
                database.TextFileDatabasePath = value;
            }
        }

        public string DatabaseTableName
        {
            get
            {
                return database.DatabaseTableName;
            }
            set
            {
                database.DatabaseTableName = value;
            }
        }

        public string ConnectionString
        {
            get
            {
                return database.ConnectionString;
            }
            set
            {
                database.ConnectionString = value;
            }
        }

        public int MaxTweetDatabaseSendRetries
        {
            get
            {
                return database.MaxTweetDatabaseSendRetries;
            }
            set
            {
                database.MaxTweetDatabaseSendRetries = value;
            }
        }

        public bool SaveToDatabase
        {
            get
            {
                return database.SaveToDatabase;
            }
            set
            {
                database.SaveToDatabase = value;
            }
        }

        public bool SaveToTextFileProperty
        {
            get
            {
                return database.SaveToTextFileProperty;
            }
            set
            {
                database.SaveToTextFileProperty = value;
            }
        }
        
        public DatabaseViewModel(LogViewModel log)
        {
            // create new model
            database = new Database(log.Log);
        }
    }
}