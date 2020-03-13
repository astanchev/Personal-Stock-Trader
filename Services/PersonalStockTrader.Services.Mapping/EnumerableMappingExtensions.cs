namespace PersonalStockTrader.Services.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using AutoMapper;

    public static class EnumerableMappingExtensions
    {
        public static IMapper MapperInstance { get; set; }

        public static IEnumerable<TDestination> To<TDestination>(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var src in source)
            {
                yield return MapperInstance.Map<TDestination>(src);
            }
        }
    }
}