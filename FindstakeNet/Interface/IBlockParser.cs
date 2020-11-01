using System;

namespace FindstakeNet.Interface
{
	public interface IBlockParser
	{
		void Parse(uint height);
		uint Parse(string hash);
	}
}
