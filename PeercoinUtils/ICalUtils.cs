//using System;
//using System.Text;
//using System.IO;
//using System.Collections.Generic;
//using Ical.Net;
//using Ical.Net.DataTypes;
//using Ical.Net.CalendarComponents;
//using Ical.Net.Serialization;
//
//namespace PeercoinUtils
//{
//	public class ICalUtils
//	{
//		private DateTime eventtime;
//		private string title;
//		private string description;
//		
//		public ICalUtils(DateTime eventtime, string title, string description)
//		{
//			this.eventtime = eventtime;
//			this.title = title;
//			this.description = description;
//		}
//		
//		public override string ToString()
//		{
//			var vEvent = new CalendarEvent
//			{
//				Start = new CalDateTime(eventtime.AddHours(-1)),
//				End = new CalDateTime(eventtime.AddMinutes(5)),
//			    Description = description,
//			    Name = title,
//			    IsAllDay = false,
//			    Uid = Guid.NewGuid().ToString()
//			};
//			
//		
//			var calendar = new Ical.Net.Calendar();		
//            calendar.Events.Add(vEvent);
//
//            var fileName = title + eventtime.ToString("dddd, dd MMMM yyyy HH:mm:ss").Replace(" ", "_") + ".ics";
//            
//            
//            var serializer = new CalendarSerializer(new SerializationContext());
//            return serializer.SerializeToString(calendar);
//            
//
//		}	
//		
//	}
//}
