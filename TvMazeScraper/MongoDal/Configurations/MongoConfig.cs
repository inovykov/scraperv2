﻿namespace MongoDal.Configurations
{
    public class MongoConfig
    {
        public string ConnectionString { get; set; }

        public string DbName { get; set; }

        public string TasksCollectionName { get; set; }

        public string TvShowsCollectionName { get; set; }
    }
}
