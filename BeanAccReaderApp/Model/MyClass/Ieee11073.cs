using System;
using System.Collections.Generic;


namespace BeanAccReaderApp.Model.MyClass
{
	class Ieee11073
	{
		// SFLOAT to SINGLE

		static private Dictionary<Int32, Single> reservedValues = new Dictionary<Int32, Single> {
			{ 0x07FE, Single.PositiveInfinity },
			{ 0x07FF, Single.NaN },
			{ 0x0800, Single.NaN },
			{ 0x0801, Single.NaN },
			{ 0x0802, Single.NegativeInfinity }
		};

		static private Dictionary<Int64, Single> reservedValues32 = new Dictionary<Int64, Single> {
			{ 0x007FFFFE, Single.PositiveInfinity },
			{ 0x007FFFFF, Single.NaN },
			{ 0x00800000, Single.NaN },
			{ 0x00800001, Single.NaN },
			{ 0x00800002, Single.NegativeInfinity }
		};

		static public Single ToSingle(Byte[] bytes)
		{
			Single returnValue = Single.NaN;

			if (bytes.Length == 2)
			{
				returnValue = ToSingle16(bytes);
			}
			else if (bytes.Length == 4)
			{
				returnValue = ToSingle32(bytes);
			}

			return (returnValue);

		}

		static private Single ToSingle16(Byte[] bytes)
		{
			var ieee11073 = (UInt16)(bytes[0] + 0x100 * bytes[1]);
			var mantissa = ieee11073 & 0x0FFF;
			if (reservedValues.ContainsKey(mantissa))
				return reservedValues[mantissa];
			if (mantissa >= 0x0800)
				mantissa = -(0x1000 - mantissa);
			var exponent = ieee11073 >> 12;
			if (exponent >= 0x08)
				exponent = -(0x10 - exponent);
			var magnitude = Math.Pow(10d, exponent);
			return (Single)(mantissa * magnitude);
		}

		static private Single ToSingle32(Byte[] bytes)
		{
			var ieee11073 = (UInt32)(bytes[0] + 0x100 * bytes[1] + 0x10000 * bytes[2] + +0x1000000 * bytes[3]);
			var mantissa = (Int32)ieee11073 & 0x00FFFFFF;
			if (reservedValues32.ContainsKey(mantissa))
				return reservedValues32[mantissa];
			if (mantissa >= 0x00800000)
				mantissa = -(0x1000000 - mantissa);
			var exponent = (Int32)ieee11073 >> 24;
			if (exponent >= 0x80)
				exponent = -(0x100 - exponent);
			var magnitude = Math.Pow(10d, exponent);
			return (Single)(mantissa * magnitude);
		}
	
	}
}
