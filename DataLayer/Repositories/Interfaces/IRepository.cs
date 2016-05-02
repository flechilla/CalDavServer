﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;

namespace DataLayer.Repositories
{
    public interface IRepository<TEnt, in TPk> where TEnt : class
    {
        Task<IList<TEnt>> GetAll();
        Task<TEnt> Get(TPk url);
        Task Add(TEnt entity);
        Task Remove(TEnt entity);

        Task Remove(TPk url);

        int Count();

        Task<bool> Exist(TPk url);

        /// <summary>
        /// Returns all the visible properties 
        /// related to the given url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        Task<IList<Property>> GetAllProperties(TPk url);

        /// <summary>
        /// Returns the properties that match the given 
        /// properties name and namespace for the given url;
        /// </summary>
        /// <param name="url"></param>
        /// <param name="propertiesNameandNs"></param>
        /// <returns></returns>
        Task<IList<Property>> GetProperties(TPk url, List<KeyValuePair<string, string>> propertiesNameandNs);

        /// <summary>
        /// Returns all the properties
        /// </summary>
        /// <returns></returns>
        Task<IList<KeyValuePair<string, string>>> GetAllPropname(TPk url);

        /// <summary>
        /// Remove a property with the given name and namespace.
        /// </summary>
        /// <param name="url">The object's identifier</param>
        /// <param name="propertyNameNs">The property name and namespace.</param>
        /// <param name="errorStack">The error stack.</param>
        Task RemoveProperty(TPk url, KeyValuePair<string, string> propertyNameNs, Stack<string> errorStack );

        /// <summary>
        /// Create a modify a property
        /// </summary>
        /// <param name="url">The property father url.</param>
        /// <param name="propName"></param>
        /// <param name="propNs"></param>
        /// <param name="propValue"></param>
        /// <param name="errorStack"></param>
        /// <param name="adminPrivilege"></param>
        /// <returns></returns>
        Task CreateOrModifyProperty(TPk url, string propName, string propNs, string propValue, Stack<string> errorStack, bool adminPrivilege);
    }
}
