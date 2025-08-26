using AutoMapper;
using TaskBoard.Web.Contracts;
using TaskBoard.Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskBoard.Web.Mapping
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            CreateMap<TaskItemDto, TaskItemViewModel>().ReverseMap();
        }
    }
}
