using System;
using System.Globalization;
using AutoMapper;
using CoreProfilerUnitTest.Repository.Models;
using CoreProfilerUnitTest.Service.DTOs;

namespace CoreProfilerUnitTest.Service.Infrastructure.Mapping
{
    /// <summary>
    /// class MappingProfile
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class MappingProfile : Profile
    {
        private const string DateTimeFormat = "yyyyMMddHHmmssfff";

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            this.CreateMap<StationModel, StationDto>()
                .ForMember(d => d.No, o => o.MapFrom(s => s.StationNo))
                .ForMember(d => d.Name, o => o.MapFrom(s => s.StationName))
                .ForMember(d => d.Total, o => o.MapFrom(s => s.Total))
                .ForMember(d => d.Bikes, o => o.MapFrom(s => s.StationBikes))
                .ForMember(d => d.Area, o => o.MapFrom(s => s.StationArea))
                .ForMember
                (
                    d => d.ModifyTime,
                    o => o.MapFrom
                    (
                        s => string.IsNullOrWhiteSpace(s.ModifyTime)
                            ? DateTime.MinValue
                            : DateTime.ParseExact($"{s.ModifyTime}000", DateTimeFormat, CultureInfo.InvariantCulture)
                    )
                )
                .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Latitude))
                .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Longitude))
                .ForMember(d => d.Address, o => o.MapFrom(s => s.Address))
                .ForMember(d => d.AreaEnglish, o => o.MapFrom(s => s.StationAreaEnglish))
                .ForMember(d => d.NameEnglish, o => o.MapFrom(s => s.StationNameEnglish))
                .ForMember(d => d.AddressEnglish, o => o.MapFrom(s => s.AddressEnglish))
                .ForMember(d => d.BikeEmpty, o => o.MapFrom(s => s.BikeEmpty))
                .ForMember(d => d.Active, o => o.MapFrom(s => s.Active));
        }
    }
}