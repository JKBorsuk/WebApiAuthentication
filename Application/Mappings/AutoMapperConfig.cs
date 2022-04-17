namespace Application.Mappings
{
    public static class AutoMapperConfig
    {
        public static IMapper Initialize()
            => new MapperConfiguration(cfg =>
            {

                #region User

                cfg.CreateMap<UserDto, User>();
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<RegisterDto, User>();
                cfg.CreateMap<User, LoginDto>();
                cfg.CreateMap<LoginDto, User>();

                #endregion


            }).CreateMapper();
    }
}
