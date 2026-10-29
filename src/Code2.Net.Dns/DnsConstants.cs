namespace Code2.Net.Dns;

public static class DnsConstants
{
	public static class RecordTypes
	{
		public const ushort A = 1;
		public const ushort NS = 2;
		public const ushort MD = 3;
		public const ushort MF = 4;
		public const ushort CNAME = 5;
		public const ushort SOA = 6;
		public const ushort MB = 7;
		public const ushort MG = 8;
		public const ushort MR = 9;
		public const ushort NULL = 10;
		public const ushort WKS = 11;
		public const ushort PTR = 12;
		public const ushort HINFO = 13;
		public const ushort MINFO = 14;
		public const ushort MX = 15;
		public const ushort TXT = 16;
		public const ushort AAAA = 28;
	}

	public static class QueryTypes
	{
		public const ushort AXFR = 252;
		public const ushort MAILB = 253;
		public const ushort MAILA = 254;
		public const ushort ALL = 255;
	}

	public static class RecordClasses
	{
		public const ushort IN = 1;
		public const ushort CS = 2;
		public const ushort CH = 3;
		public const ushort HS = 4;
		public const ushort ANY = 255;
	}

	public static class ResponseCodes
	{
		public const byte NoError = 0;
		public const byte FormatError = 1;
		public const byte ServerFailure = 2;
		public const byte NameError = 3;
		public const byte NotImplemented = 4;
		public const byte Refused = 5;
	}

	public static class OperationCodes
	{
		public const byte StandardQuery = 0;
		public const byte InverseQuery = 1;
		public const byte StatusRequest = 2;
	}
}
