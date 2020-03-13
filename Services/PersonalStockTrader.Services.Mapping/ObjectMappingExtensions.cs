namespace PersonalStockTrader.Services.Mapping
{
    using System;

    using AutoMapper;

    public static class ObjectMappingExtensions
    {
        public static IMapper MapperInstance { get; set; }

        public static T To<T>(this object origin)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            return MapperInstance.Map<T>(origin);
        }
    }
}