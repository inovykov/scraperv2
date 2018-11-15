namespace MongoDal.Configurations
{
    public class MongoConfig
    {
        public string ConnectionString { get; set; }

        public string DbName { get; set; }

        public string SagasCollectionName { get; set; }

        public string SagasItemsCollectionName { get; set; }

        public string TvShowsCollectionName { get; set; }
    }
}
