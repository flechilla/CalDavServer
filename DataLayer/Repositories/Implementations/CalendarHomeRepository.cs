﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.ACL;
using DataLayer.Models.Entities;
using DataLayer.Models.Entities.ResourcesAndCollections;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories.Implementations
{
    public class CalendarHomeRepository : IRepository<CalendarHome, string>
    {
        //private readonly CalDavContext _context;
        private readonly CalDAVSQLiteContext _context;

        public CalendarHomeRepository(CalDAVSQLiteContext context)
        {
            _context = context;
        }

        public Task<IList<CalendarHome>> GetAll()
        {
            throw new NotImplementedException();
        }

        public CalendarHome Get(string url)
        {
            return _context.CalendarHomeCollections.Include(p => p.Properties).
              Include(r => r.CalendarCollections).ThenInclude(rp => rp.Properties).FirstOrDefault(c => c.Url == url);
        }

        public async Task<CalendarHome> GetAsync(string url)
        {
            return await _context.CalendarHomeCollections.Include(p => p.Properties).
                Include(r => r.CalendarCollections)
                .ThenInclude(rp => rp.Properties)
                .FirstOrDefaultAsync(c => c.Url == url);
        }

        public void Add(CalendarHome entity)
        {
            _context.CalendarHomeCollections.Add(entity);
        }

        public async Task Remove(CalendarHome entity)
        {
            _context.CalendarHomeCollections.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Remove(string url)
        {
            var collection = await GetAsync(url);
            await Remove(collection);
        }

        public async Task<int> Count()
        {
            return await _context.CalendarHomeCollections.CountAsync();
        }

        public async Task<bool> Exist(string url)
        {
            return await _context.CalendarHomeCollections.AnyAsync(c => c.Url == url);
        }

        public async Task<IList<Property>> GetAllProperties(string url)
        {
            var collection = await GetAsync(url);

            return collection?.Properties.Where(p => p.IsVisible).ToList();
        }

        public async Task<Property> GetProperty(string url, KeyValuePair<string, string> propertyNameandNs)
        {
            var collection = await GetAsync(url);

            var property = string.IsNullOrEmpty(propertyNameandNs.Value)
                ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameandNs.Key)
                : collection?.Properties.FirstOrDefault(
                    p => p.Name == propertyNameandNs.Key && p.Namespace == propertyNameandNs.Value);

            return property;
        }

        public async Task<IList<KeyValuePair<string, string>>> GetAllPropname(string url)
        {
            var collection = await GetAsync(url);
            return
                collection?.Properties.ToList()
                    .Select(p => new KeyValuePair<string, string>(p.Name, p.Namespace))
                    .ToList();
        }

        public async Task<bool> RemoveProperty(string url, KeyValuePair<string, string> propertyNameNs,
            Stack<string> errorStack)
        {
            var collection = await GetAsync(url);
            var property = string.IsNullOrEmpty(propertyNameNs.Value)
                ? collection?.Properties.FirstOrDefault(p => p.Name == propertyNameNs.Key)
                : collection?.Properties.FirstOrDefault(
                    p => p.Name == propertyNameNs.Key && p.Namespace == propertyNameNs.Value);
            if (property == null)
                return true;
            if (!property.IsDestroyable)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }
            collection?.Properties.Remove(property);
            return true;
        }

        public async Task<bool> CreateOrModifyProperty(string url, string propName, string propNs, string propValue,
            Stack<string> errorStack, bool adminPrivilege)
        {
            var collection = await GetAsync(url);
            //get the property
            var property =
                collection.Properties
                    .FirstOrDefault(prop => prop.Name == propName && prop.Namespace == propNs);
            //if the property did not exist it is created.
            if (property == null)
            {
                collection.Properties.Add(new Property
                {
                    Name = propName,
                    Namespace = propNs,
                    IsDestroyable = true,
                    IsVisible = false,
                    IsMutable = true,
                    Value = propValue
                });
                return true;
            }
            //if this property belongs to the fix system properties, it can not be changed. Only the server can.
            if (!property.IsMutable && !adminPrivilege)
            {
                errorStack.Push("HTTP/1.1 403 Forbidden");
                return false;
            }

            //if all previous conditions don't pass then the value of the property is changed.
            property.Value = propValue;
            return true;
        }

        public async Task<int> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync();
        }


        public static CalendarHome CreateCalendarHome(Principal owner)
        {
            //check if the user is an admin user.
            //if it is the first admin user then create the public 
            //calendars
            var adminUser = owner.PrincipalStringIdentifier.EndsWith("@admin.uh.cu")&&!SystemProperties.PublicCalendarCreated;
            
            var fsm = new FileManagement();
            var defaultCalName = "DefaultCalendar";

            var defaultCalHomeName =adminUser?"PublicCollections" : "HomeCollection";
            var ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalURL}</D:href>",
               false, false);

            var aclProperty =adminUser? PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalURL) : PropertyCreation.CreateAclPropertyForUserCollections(owner.PrincipalURL);

            string calHomeUrl;
            //if the user is admin then the collection home is public so the URL change
            if (adminUser)
                calHomeUrl = SystemProperties.PublicCalendarHomeUrl;
            else
                calHomeUrl = $"{SystemProperties._userCollectionUrl}{owner.PrincipalStringIdentifier}/";


            var calHome = new CalendarHome(
                calHomeUrl, defaultCalHomeName, ownerProp, aclProperty);

            fsm.CreateFolder(calHome.Url);
            ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalURL}</D:href>",
              false, false);

            aclProperty = adminUser ? PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalURL) : PropertyCreation.CreateAclPropertyForUserCollections(owner.PrincipalURL);

            //create the initial calendar collection for the user.
            var initCollection =
                new CalendarCollection(
                    $"{calHome.Url}{defaultCalName}/",
                    defaultCalName, ownerProp, aclProperty)
                {
                    Principal = owner,
                    CalendarHome = calHome
                };

            

            //if the principal is admin then create the public calendars
            if(adminUser)
                CreatePublicCollections(calHome,owner, aclProperty, ownerProp);
            

            //add the calendar collection to the calHome
            //if the principal is not admin
            if (!adminUser)
            {
                fsm.CreateFolder(initCollection.Url);
                calHome.CalendarCollections.Add(initCollection);
                owner.CalendarCollections.Add(initCollection);
            }
            
            return calHome;
        }


        private static void CreatePublicCollections(CalendarHome publicCalendar, Principal owner, params Property[] properties)
        {
            var fsm = new FileManagement();
            foreach (var calName in SystemProperties.PublicCalendarNames)
            {
                var ownerProp = PropertyCreation.CreateProperty("owner", "D", $"<D:href>{owner.PrincipalURL}</D:href>",
              false, false);

                var aclProperty = PropertyCreation.CreateAclPropertyForGroupCollections(owner.PrincipalURL);
                var publicCollection =
                new CalendarCollection(
                    $"{SystemProperties.PublicCalendarHomeUrl}{calName}/",calName, ownerProp, aclProperty)
                {
                    Principal = owner,
                    CalendarHome = publicCalendar
                };

                fsm.CreateFolder(publicCollection.Url);
                publicCalendar.CalendarCollections.Add(publicCollection);
                owner.CalendarCollections.Add(publicCollection);
            }
        }
    }
}
