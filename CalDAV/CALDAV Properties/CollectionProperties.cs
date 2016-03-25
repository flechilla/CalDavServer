﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CalDAV.XML_Processors;
using CalDAV.Models;
using CalDAV.Utils.XML_Processors;
using TreeForXml;
using System.Reflection;

namespace CalDAV.CALDAV_Properties
{
    public static class CollectionProperties
    {
        public static string CaldavNs => "urn:ietf:params:xml:ns:caldav";
        public static string DavNs => "DAV";

        /// <summary>
        /// Contains all the properties that are common for all Calendar Collection.
        /// </summary>
        private static Dictionary<string, string> XmlGeneralProperties = new Dictionary<string, string>()
        {
            { "calendar-timezone", DateTimeKind.Local.ToString() }, {"max-resource-size", "102400"},
            { "min-date-time", MinDateTime()}, {"max-date-time", MaxDateTime()}, { "max-instances", "10"},
            {"getcontentlength", "0" }
        };

        private static List<string> VisibleGeneralProperties = new List<string>() {""};

        private static string MinDateTime()
        {
            return
                new DateTime(DateTime.Now.Year, (DateTime.Now.Month - 1)%12, DateTime.Now.Day).ToUniversalTime()
                    .ToString("yyyyMMddTHHmmssZ");
        }

        private static string MaxDateTime()
        {
            return
                   new DateTime(DateTime.Now.Year, (DateTime.Now.Month + 1) % 12, DateTime.Now.Day).ToUniversalTime()
                       .ToString("yyyyMMddTHHmmssZ");
        }

        /// <summary>
        /// Returns the value of a collection property given its name.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="propertyName"></param>
        /// <param name="mainNS"></param>
        /// <returns></returns>
        public static XmlTreeStructure ResolveProperty(this CalendarCollection collection, string propertyName, string mainNS)
        {
            ////this must be fixed later because not all properties are of type string.
            var value = (string)collection.GetType().GetProperty(propertyName).GetValue(collection);
            var prop = new XmlTreeStructure(propertyName, mainNS);
            prop.Value = value;

            return prop;
        }

        /// <summary>
        /// Returns all the properties of a collection that must be returned for
        /// an "allprop" property method of Propfind.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllVisibleProperties(this CalendarCollection collection)
        {
            var list = new List<XmlTreeStructure>();

            //calendar desription
            var description = new XmlTreeStructure("calendar-description", CaldavNs);
            description.AddNamespace("C", CaldavNs);
            description.AddValue(collection.CalendarDescription);
            list.Add(description);

            //Display Name
            var displayName = new XmlTreeStructure("displayname", DavNs);
            displayName.AddValue(collection.DisplayName);
            list.Add(displayName);

            //resource type
            var resourceType = collection.ResourceType;
            //var resourceType = new XMLTreeStructure("resourcetype", new List<string>() {"D"});
            //foreach (var res in collection.ResourceType)
            //{
            //    resourceType.AddChild(new XMLTreeStructure(res, new List<string>() { NameSpace }));
            //}
            list.Add((XmlTreeStructure)XmlTreeStructure.Parse(resourceType));

            //creation date
            var creationDate = new XmlTreeStructure("creationdate", DavNs);
            creationDate.AddValue(collection.CreationDate.ToString());
            list.Add(creationDate);

            //supported lock



            return list;

        }

        /// <summary>
        /// Returns the Name of all collection properties.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static List<XmlTreeStructure> GetAllPropertyNames(this CalendarCollection collection)
        {
            var list = new List<XmlTreeStructure>();

            //calendar desription
            var description = new XmlTreeStructure("calendar-description", CaldavNs);
            description.AddNamespace("C", CaldavNs);
            list.Add(description);
            //a todos los que estan abajo le tienes q pasar el MainNs
            //Display Name
            var displayName = new XmlTreeStructure("displayname");
            list.Add(displayName);

            //resource type
            var resourceType = new XmlTreeStructure("resourcetype");
            list.Add(resourceType);

            //calendar-timezone
            var calendarTimeZone = new XmlTreeStructure("calendar-timezone");
            list.Add(resourceType);

            //supported-calendar-component-set
            var supportedCalendarComp = new XmlTreeStructure("supported-calendar-component-set");
            list.Add(supportedCalendarComp);

            //max-resource-size
            var maxResourceSize = new XmlTreeStructure("max-resource-size");
            list.Add(maxResourceSize);

            //min-date-time
            var minDateTime = new XmlTreeStructure("min-date-time");
            list.Add(minDateTime);

            //min-date-time
            var maxDateTime = new XmlTreeStructure("min-date-time");
            list.Add(maxDateTime);

            //max-intances
            var maxIntances = new XmlTreeStructure("max-intances");
            list.Add(maxIntances);


            return list;
        }
        
    }
}
