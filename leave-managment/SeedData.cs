using leave_managment.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace leave_managment
{
    public static class SeedData
    {
        public static void Seed(UserManager<Employee> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        private static void SeedUsers(UserManager<Employee> userManager)
        {
            if (userManager.FindByNameAsync("admin").Result == null)
                //checking if there any user if its null then we need to initialize 
            {
                var user = new Employee
                {
                    UserName ="admin@localhost.com",
                    Email="admin@localhost.com"
                };
                var result = userManager.CreateAsync(user,"123@Samar").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "Administrator").Wait();
                    
                }
            }
        }
        //roles are exisit before users as logic 
        //we need admin which has the roles to add and create and other will log and apply 
        //so we have two roles 1. admin 2.employee 
        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            //we have one admin and we will create it once if the database is fresh
            if (!roleManager.RoleExistsAsync("Administrator").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Administrator"
                };
              var result =  roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Employee").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Employee"
                };
              var result =  roleManager.CreateAsync(role).Result;
            }
        }
    }
}
