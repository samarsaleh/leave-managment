using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using leave_managment.Contracts;
using leave_managment.Data;
using leave_managment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace leave_managment.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class LeaveAllocationController : Controller
    {
        private readonly ILeaveAllocationRepository _leaveallocationrepo; //reference interface which knows the functions ..
        private readonly ILeaveTypeRepository _leaverepo; //reference interface which knows the functions ..
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveAllocationController(ILeaveTypeRepository leaverepo,
            ILeaveAllocationRepository leaveallocationrepo,
            IMapper mapper,
            UserManager<Employee> userManager
            )
        {
            _leaverepo = leaverepo;
            _leaveallocationrepo = leaveallocationrepo;
            _mapper = mapper;
            _userManager = userManager;


        }

        // GET: LeaveAllocation
        public async Task<ActionResult> Index()
        {
            var leavetypes = await _leaverepo.FindAll(); // returning collection of leavetype OBJECTS
            //so its need to map to get data from data class using the view model 
            var mappedleavetypes = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leavetypes.ToList());//for recored 
            var model = new CreateLeaveAllocationVM // defulat value coming from database
            {
                Leavetypes = mappedleavetypes,
                NumberUpdated = 0
                
            };
            
            return View(model);
        }

        public async Task<ActionResult> SetLeave(int id)//find which leave type is clicked
        {
            var leavetype = await _leaverepo.FindById(id);
            //set the number that assosiated with this type in the employee
            var employees =await _userManager.GetUsersInRoleAsync("Employee");
            //now i have the type and the employee 
            //retrive leave type and the employee and add ech recored to the employees 
            foreach (var emp in employees)
            {
                if(await _leaveallocationrepo.CheckAllocation(id,emp.Id))
                    continue;  //if yes enter the iteration

                var allocation = new LeaveAllocationVM
                {
                    DateCreated = DateTime.Now,
                    EmployeeId = emp.Id,
                    LeaveTypeId = id,
                    NumberOfDays = leavetype.DefaultDays,
                    Period = DateTime.Now.Year
                };

                var leaveallocation =_mapper.Map<LeaveAllocation>(allocation);
               await _leaveallocationrepo.create(leaveallocation);
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<ActionResult> ListEmployees()
        {
            var employees=await _userManager.GetUsersInRoleAsync("Employee");
            var model = _mapper.Map<List<EmployeeVM>>(employees);
            //this gave an error in mapping cuz employee clas is extend identity user which has fn lastname .. etc which we removed it from the view 


            return View(model);
        }

        // GET: LeaveAllocation/Details/5
        public async Task<ActionResult> Details(string id) //from int to string
        {
            var employee = _mapper.Map<EmployeeVM>(await _userManager.FindByIdAsync(id));//whenever we get data we need map it
            //retrive all the leave allocation for this employee
            var allocations =_mapper.Map<List<LeaveAllocationVM>>( await _leaveallocationrepo.GetLeaveAllocationsByEmployee(id)); //list to VM
            var model = new ViewAllocationVM
            {
                Employee = employee,
                LeaveAllocations = allocations

            };
            return View(model);
        }

        // GET: LeaveAllocation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LeaveAllocation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveAllocation/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var leaveallocation = await _leaveallocationrepo.FindById(id);
            var model = _mapper.Map<EditLeaveAllocationVM>(leaveallocation); //we made a new VM 
            return View(model);
        }

        // POST: LeaveAllocation/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditLeaveAllocationVM model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                var recored =await _leaveallocationrepo.FindById(model.Id);
                recored.NumberOfDays = model.NumberOfDays;

                var isSuccess=await _leaveallocationrepo.update(recored);
                if (!isSuccess)
                {
                    ModelState.AddModelError("","Error samooooooooooraaaa while saving");
                    return View(model);
                }
                //just wanted to go back to my recored not show it from the scratch again! instead of index i choosed Details
                //details requiers id so i make a new parameter with the employee id as a value from the edit form
                return RedirectToAction(nameof(Details),new { id = model.EmployeeId });
            }
            catch
            {
                return View(model);
            }
        }

        // GET: LeaveAllocation/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveAllocation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}