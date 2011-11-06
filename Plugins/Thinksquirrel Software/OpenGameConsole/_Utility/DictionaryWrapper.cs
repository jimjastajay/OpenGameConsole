using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ThinksquirrelSoftware.OpenGameConsole.Utility
{
	public class DictionaryWrapper
	{ 
		public List<string> Keys { get; set; }

		public List<string> Values { get; set; }
		
		public DictionaryWrapper()
		{
		}
		
		public DictionaryWrapper(Dictionary<string,string> map)
		{
			Keys = new List<string>(map.Keys);
			Values = new List<string>(map.Values);
		}
		
		public Dictionary<string,string> GetMap()
		{
			Dictionary<string,string> map = new Dictionary<string,string>();
			for (int i = 0; i < Keys.Count; i++)
			{
				map[Keys[i]] = Values[i];
			}  
			return map;
		}
	}
}
