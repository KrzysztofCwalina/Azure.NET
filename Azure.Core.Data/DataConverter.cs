﻿using System;

namespace Azure.Data
{
    public abstract class DataConverter
    {
        public abstract Type ForType { get; }
        public abstract DynamicData ConvertToDataType(object obj);
        public abstract object ConverFromDataType(DynamicData data);

        public static ReadOnlyMemory<DataConverter> Common = new DataConverter[]
        {
            new DateTimeConverter(),
            new DateTimeOffsetConverter()
        };
    }

    sealed class DateTimeConverter : DataConverter
    {
        public override Type ForType => typeof(DateTime);

        public override object ConverFromDataType(DynamicData data)
        {
            if (DateTime.TryParse(data.ToString(), out var dt))
            {
                return dt;
            }
            throw new InvalidOperationException();
        }

        public override DynamicData ConvertToDataType(object obj)
        {
            var dt = (DateTime)obj;
            var data = new DynamicData(dt.ToString("O")).WithConverter(this);

            return data;
        }
    }

    sealed class DateTimeOffsetConverter : DataConverter
    {
        public override Type ForType => typeof(DateTimeOffset);

        public override object ConverFromDataType(DynamicData data)
        {
            if (DateTimeOffset.TryParse(data.ToString(), out var dt))
            {
                return dt;
            }
            throw new InvalidOperationException();
        }

        public override DynamicData ConvertToDataType(object obj)
        {
            var dt = (DateTimeOffset)obj;
            var data = new DynamicData(dt.ToString("O")).WithConverter(this);
            return data;
        }
    }
}