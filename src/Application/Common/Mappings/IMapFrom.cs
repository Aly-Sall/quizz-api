using AutoMapper;

namespace _Net6CleanArchitectureQuizzApp.Application.Common.Mappings;

public interface IMapFrom<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}
