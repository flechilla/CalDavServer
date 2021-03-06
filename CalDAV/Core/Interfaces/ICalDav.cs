﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CalDAV.Core
{
    public interface ICalDav
    {
        /// <summary>
        ///     CalDAV HTTP Method for create a new collection of COR (new calendar)
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        Task MkCalendar(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        ///     WebDAV PROFIND HTTP Method .
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task PropFind(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        /// Synchronization Method for read the calendar home set properties.
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="body"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task CHSetPropfind(Dictionary<string, string> propertiesAndHeaders, string body, HttpResponse response);

        /// <summary>
        ///     CalDav HTTP Method PROPPATCH
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="Body"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task PropPatch(Dictionary<string, string> propertiesAndHeaders, string Body, HttpResponse response);

        /// <summary>
        ///     CalDav HTTP Method REPORT for Calendar Collections
        ///     Call this method form the controller and will handle
        ///     the report for the collections.
        /// </summary>
        /// <returns></returns>
        Task Report(HttpContext context);

        /// <summary>
        ///     CalDAV PUT for create a new COR
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task AddCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav Method for delete a Calendar Object Resource
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task<bool> DeleteCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav & WebDav Method for delete a Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task<bool> DeleteCalendarCollection(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav HTTP Method Get for a COR
        /// </summary>
        /// <returns></returns>
        /// <param name="propertiesAndHeaders"></param>
        /// <param name="response"></param>
        Task ReadCalendarObjectResource(Dictionary<string, string> propertiesAndHeaders, HttpResponse response);

        /// <summary>
        ///     CalDav Http method for get a Calendar Collection
        /// </summary>
        /// <param name="propertiesAndHeaders"></param>
        /// <returns></returns>
        string ReadCalendarCollection(Dictionary<string, string> propertiesAndHeaders);

        /// <summary>
        ///     This method perfoms a profind on a principal.
        /// </summary>
        /// <param name="httpContext">
        ///     This contains the Request from the client, the response to be sended back and useful
        ///     data in the Session.
        /// </param>
        /// <returns></returns>
        Task ACLProfind(HttpContext httpContext);
    }
}