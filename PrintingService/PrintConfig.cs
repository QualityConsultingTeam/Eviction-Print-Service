namespace PrintingService
{
    internal class PrintConfig
    {
        public PrintConfig()
        {
        }

        public string[] EvictionIds { get; set; }
        public string[] Forms { get; set; }
        public string MappingFolder { get; set; }
        public string ParentUserId { get; set; }
        public string[] PrintCount { get; set; }
        public bool ReIssueCheck { get; set; }
        public string SelectedAttorney { get; set; }
        public string TemplateFolder { get; set; }
        public string UserId { get; set; }
        public string UserType { get; set; }
    }
}