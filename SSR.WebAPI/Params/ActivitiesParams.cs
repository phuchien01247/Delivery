namespace SSR.WebAPI.Params
{
    public class ActivitiesParams : PagingParam
    {
        public string IssueId { get; set; }
        public string ProjectId { get; set; }   
    }
}
