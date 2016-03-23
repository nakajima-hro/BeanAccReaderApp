using System;
using System.Collections.Generic;


namespace BeanAccReaderApp.Model.MyClass
{
	public struct LightBlueBeanScratch1Value
	{
		public UInt16 Count;
		public Int16 AccXFiltered;
		public Int16 AccXRaw;
		public string Result;

		public LightBlueBeanScratch1Value(UInt16 count, Int16 accXFiltered, Int16 accXRaw) : this()
		{
			this.Count = count;
			this.AccXFiltered = accXFiltered;
			this.AccXRaw = accXRaw;
		}
	}

}
