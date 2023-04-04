using SSR.WebAPI.Models;
using SSR.WebAPI.Params;
using SSR.WebAPI.Services;

namespace SSR.WebAPI.Interfaces;

public interface IActivitiesService
{
    Task<bool> SaveChangeActivities();
    ActivitiesService WithNguoiThucHien(string user);
    ActivitiesService WithIssueId(string id);
    ActivitiesService WithAction(string noiDung);
    ActivitiesService WithContent(string noiDung);
    ActivitiesService WithProjectId(string id);
    ActivitiesService WithColor(string color);
    ActivitiesService WithStep(string step);
    ActivitiesService WithIcon(string icon);
    ActivitiesService WithLabel(List<Label> label);
    Task<List<Activities>> GetByIssueId(string IssueId);
    Task<List<Activities>> GetByProjectId(string ProjectId);
    Task<PagingModel<Activities>> GetPaging(ActivitiesParams param);
}