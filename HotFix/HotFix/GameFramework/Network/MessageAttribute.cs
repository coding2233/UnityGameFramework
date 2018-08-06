using System;

namespace HotFix.Taurus
{
	public class MessageAttribute : Attribute
	{
		public ushort TypeCode { get; private set; }

		public MessageAttribute(ushort typeCode)
		{
			TypeCode = typeCode;
		}
	}
}