﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ICalendar.Utils;
using ICalendar.ValueTypes; 

namespace CalDAV.Models
{
    /// <summary>
    /// This class contains method extension for the CaldavContext.
    /// </summary>
    public static class Queries
    {
        /// <summary>
        /// Check if a User exist in the system
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public static bool UserExist(this CalDavContext source, string userEmail)
        {
            return (
                from user in source.Users
                where user.Email == userEmail
                select user).Any();
        }
        /// <summary>
        /// return a User for a given name
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static User GetUser(this CalDavContext source, string userEmail)
        {
            return source.Users.First(u => u.Email == userEmail);
        }

        /// <summary>
        /// Check if a collection exist in the system.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static bool CollectionExist(this CalDavContext source, string userEmail, string collectionName)
        {
            if (!UserExist(source, userEmail)) 
            return false;

            return (
                from collection in GetUser(source, userEmail).CalendarCollections
                where collection.Name == collectionName
                select collection
                ).Any();
        }
        /// <summary>
        /// return a collection for a given user and collectionName
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static CalendarCollection GetCollection(this CalDavContext source, string userEmail, string collectionName)
        {
            return source.GetUser(userEmail).CalendarCollections.First(cl => cl.Name == collectionName);
        }
        /// <summary>
        /// Check if a CalendarResource Exist
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail"></param>
        /// <param name="collectionName"></param>
        /// <param name="calResource"></param>
        /// <returns></returns>
        public static bool CalendarResourceExist(this CalDavContext source, string userEmail, string collectionName,
            string calResource)
        {
            if (!CollectionExist(source, userEmail, collectionName))
                return false;

            return (
                from resource in GetCollection(source, userEmail, collectionName).CalendarResources
                where resource.FileName == calResource
                select resource
                ).Any();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="userEmail">The email of the calendarResource's user.</param>
        /// <param name="collectionName">The collection where is the resource</param>
        /// <param name="calResource">the name of the resource</param>
        /// <returns>return a calendar resource by the given name.</returns>
        public static CalendarResource GetCalendarResource(this CalDavContext source, string userEmail,
            string collectionName, string calResource)
        {
            return source.GetCollection(userEmail, collectionName)
                .CalendarResources
                .First(cr => cr.FileName == calResource);
        }

        /// <summary>
        /// Filter the resources of the user in the given collection
        /// by filter of the dates.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="starTime">The startTime of the  </param>
        /// <param name="endTime"></param>
        /// <param name="ownerName"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static IEnumerable<CalendarResource> TimeRangeFilter(this CalDavContext source, DateTime starTime,
            DateTime endTime, string ownerName, string colName)
        {
            //TODO: expand the recurrence instances of the resources.
            //TODO: convert the datetimes to the UTC of the request
            //TODO: evaluate the condition depending on the definition fo the DTEND and DURATION(pg64 CALDAV)
            //TODO: check for when the dtStartTime if of type date.
            var resources= source.GetCollection(ownerName, colName).CalendarResources;

            var output =
                resources.Select(resource => resource)
                    .Where(resource =>
                    {
                        //If the comp defines a DTEND property then should be use
                        if (resource.DtEnd != DateTime.MaxValue)
                            return starTime < resource.DtEnd && endTime > resource.DtStart;
                        //if exist the DURATION property
                        if (resource.Duration != "")
                        {
                            DurationType duration;
                            var result = resource.Duration.ToDuration(out duration);
                            var startPlusDuration = resource.DtStart.AddDuration(duration);
                            if (duration.IsPositive)
                                return starTime < startPlusDuration && endTime > resource.DtStart;
                            else
                                return starTime <= resource.DtStart && endTime > resource.DtStart;
                        }
                        //if there is not DTEND nor DURATION then this is the default behavior
                        return starTime < resource.DtStart.AddDays(1) && endTime > resource.DtStart;
                    });
            return output;

        }



        


        /// <summary>
        /// To add a duration Property to the dtStart propery.
        /// </summary>
        /// <param name="dtStart"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        private static DateTime AddDuration(this DateTime dtStart, DurationType duration)
        {
            if (duration.Weeks != null)
                if (duration.IsPositive)
                    return dtStart.AddDays(7*duration.Weeks.Value);
                else
                    return dtStart.Subtract(new TimeSpan(7*duration.Weeks.Value, 0, 0, 0));
            var durationSpan = new TimeSpan(
                duration.Days ?? 0,
                duration.Hours ?? 0,
                duration.Minutes ?? 0,
                duration.Seconds ?? 0);
            return duration.IsPositive?dtStart.Add(durationSpan):dtStart.Subtract(durationSpan);
        }



    }
}