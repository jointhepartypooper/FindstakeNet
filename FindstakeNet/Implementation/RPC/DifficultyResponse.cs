using System;
using Newtonsoft.Json;

namespace FindstakeNet.Implementation.RPC
{
	public class DifficultyResponse
	{		    
		[JsonProperty("proof-of-stake")]
		public decimal pos { get; set; }

		[JsonProperty("proof-of-work")]					
		public decimal pow  { get; set; }
	 
	}
}
