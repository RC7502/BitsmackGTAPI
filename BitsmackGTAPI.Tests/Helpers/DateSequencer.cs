using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitsmackGTAPI.Tests.Helpers{
	
	/// <summary>
	/// Many of our Test Helpers need to create objects that have unique IDs. This class is responsible
	/// for handing out those unique IDs and for keeping track of which IDs have been used.
	/// 
	/// This will get reinitialized each time the app domain is reloaded, which happens frequently 
	/// during test cycles.
	/// 
	/// We count down from Int32.Max as a poor man's way of avoiding ID collisions with "real" IDs that
	/// may already exist in the database.
	/// </summary>
	public static class DateSequencer
	{
	    private static DateTime NEXT_VALUE = DateTime.UtcNow.Date;

		public static DateTime Next()
		{
		    NEXT_VALUE = NEXT_VALUE.AddDays(-1);
			return NEXT_VALUE;
		}

		public static bool WasIssuedBySequencer(DateTime date) {
			// We start at the max & count down; anything greater than the 
			// next value to hand out was previously issued from this sequence
			return date > NEXT_VALUE;
		}
	}


	public static class DateSequencerExtensions {
		public static bool WasIssuedByIdSequencer(this DateTime date) {
			return DateSequencer.WasIssuedBySequencer(date);
		}
	}
}
