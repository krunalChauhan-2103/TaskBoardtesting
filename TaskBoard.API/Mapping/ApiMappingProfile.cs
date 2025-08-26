using AutoMapper;
using TaskBoard.API.Dtos;
using TaskBoard.Core.Models;

namespace TaskBoard.API.Mapping
{
    public sealed class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
        }
    }
}
