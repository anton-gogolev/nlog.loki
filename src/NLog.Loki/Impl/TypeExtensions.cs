using System;

namespace NLog.Loki.Impl
{
    internal static class TypeExtensions
    {
        public static bool IsNumericType(this Type type)
        {
            var typeCode = Type.GetTypeCode(type);

            switch(typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}