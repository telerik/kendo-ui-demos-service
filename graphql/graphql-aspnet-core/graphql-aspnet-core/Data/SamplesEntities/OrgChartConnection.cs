namespace graphql_aspnet_core.Data
{
    public partial class OrgChartConnection
    {
        public long Id { get; set; }
        public long? FromPointX { get; set; }
        public long? FromPointY { get; set; }
        public long? FromShapeId { get; set; }
        public string Text { get; set; }
        public long? ToPointX { get; set; }
        public long? ToPointY { get; set; }
        public long? ToShapeId { get; set; }
    }
}
