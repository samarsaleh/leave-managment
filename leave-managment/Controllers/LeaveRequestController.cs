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
using Microsoft.AspNetCore.Mvc.Rendering;
namespace leave_managment.Controllers
{
    [Authorize] //had to be registered
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;


        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepo,
            ILeaveTypeRepository leaveTypeRepo,
            ILeaveAllocationRepository leaveAllocationRepo,
         IMapper mapper,
         UserManager<Employee> userManager)
        {
            _leaveRequestRepo = leaveRequestRepo;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        [Authorize(Roles ="Administrator")]
        // GET: LeaveRequest
        public ActionResult Index()// the landing page
        {
            //when the admin want to see the leave requests and total number of each type
            //need a vm to that lists
            var leaverequests = _leaveRequestRepo.FindAll();//getting them all
            var leaverequestsmodel = _mapper.Map <List<LeaveRequestVM>> ( leaverequests); //mapped to my VM
            var model = new AdminLeaveRequestViewVM
            {
                TotalRequests=leaverequestsmodel.Count,//count property
                ApprovedRequests=leaverequestsmodel.Where(q=>q.Approved==true).Count(),//count function return all like othkor اذكر
                PendingRequests=leaverequestsmodel.Count(q=>q.Approved==null),//same as above
                RejectedRequests=leaverequestsmodel.Count(q => q.Approved == false),
                LeaveRequests=leaverequestsmodel 
            };

            return View(model);
        }

        // GET: LeaveRequest/Details/5
        public ActionResult Details(int id)
        {
            
            var leaveRequest = _leaveRequestRepo.FindById(id);
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        //my work 
        public ActionResult MyLeave()
        {
            var employee = _userManager.GetUserAsync(User).Result;
            var employeeid = employee.Id;
            var employeeallocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployee(employeeid);
            var employeerequests = _leaveRequestRepo.GetLeaveRequestsByEmployee(employeeid);


            var employeeallocationModel = _mapper.Map<List<LeaveAllocationVM>>(employeeallocation);
            var employeerequestsModel = _mapper.Map<List<LeaveRequestVM>>(employeerequests);
            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = employeeallocationModel,
                LeaveRequests=employeerequestsModel
            
            };

            return View(model);
        }
        //my work for index.cshtmll for the buttons
        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestRepo.FindById(id);
                var leaveTypeid = leaveRequest.LeaveTypeId;
                var employeeid = leaveRequest.RequestingEmployeeId;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employeeid,leaveTypeid);

                //the NOof days in the alloc is less than requested 
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberOfDays = allocation.NumberOfDays - daysRequested;

                leaveRequest.Approved = true;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                _leaveRequestRepo.update(leaveRequest);
                _leaveAllocationRepo.update(allocation);
             
                    return RedirectToAction(nameof(Index));
                
            }
            catch(Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }

        }
        public ActionResult RejectRequest(int id)
        {
            try
            {


                var user = _userManager.GetUserAsync(User).Result;
                var leaveRequest = _leaveRequestRepo.FindById(id);
                leaveRequest.Approved = false;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                
                _leaveRequestRepo.update(leaveRequest);
                return RedirectToAction(nameof(Index));
                
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        //my work 
        public ActionResult CancelRequest(int id)
        {
            var leaverequests = _leaveRequestRepo.FindById(id);
            leaverequests.Cancelled = true;
            _leaveRequestRepo.update(leaverequests);
            return RedirectToAction("MyLeave");
           // if its normal remove and it worked but lets try his method to use the cancelled attribute
          /*  if (leaverequests == null)
            {
                return NotFound();
            }
            var isSuccess = _leaveRequestRepo.Delete(leaverequests);// shofe the return lal function 
            if (!isSuccess)
            {

                return BadRequest();
            }
            return RedirectToAction("MyLeave");*/


            
        }
        // GET: LeaveRequest/Create
        public ActionResult Create()
        {
            //require selectlistitem of leavetype so we need link here
            var leaveTypes = _leaveTypeRepo.FindAll();
            //i want this list and represent the data for ech item
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
            {
                Text = q.Name,
                Value=q.Id.ToString()
            });
            var model = new CreateLeaveRequestVM
            {
                LeaveTypes = leaveTypeItems
            };
            return View(model);
        }

        // POST: LeaveRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
        
            try
            {
                var StartDate = Convert.ToDateTime(model.StartDate);
                var EndDate = Convert.ToDateTime(model.EndDate);
                var leaveTypes = _leaveTypeRepo.FindAll();
                //i want this list and represent the data for ech item
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem
                {
                    Text = q.Name,
                    Value = q.Id.ToString()
                });
                model.LeaveTypes = leaveTypeItems;
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                //start date should be before the end date
                if (DateTime.Compare(StartDate, EndDate)>1)
                {
                    ModelState.AddModelError("", "!!! start date can not be in the future of the end date DUDE!! ");
                    return View(model);
                }
                var employee = _userManager.GetUserAsync(User).Result; //retrive current user
                var allocations = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id,model.LeaveTypeId);
                int daysRequested =(int) (EndDate - StartDate).TotalDays;
                if(daysRequested > allocations.NumberOfDays)
                {
                    ModelState.AddModelError("", "you request is not suffecient,Try Again! ");
                    return View(model);
                }

                var leaveRequestmodel = new LeaveRequestVM
                {
                    RequestingEmployeeId=employee.Id,
                    StartDate=StartDate,
                    EndDate=EndDate,
                    Approved=null,
                    DateRequested=DateTime.Now,
                    DateActioned =DateTime.Now, // cuz is not nullable 
                    LeaveTypeId = model.LeaveTypeId,
                    RequestComments = model.RequestComments
                };
                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestmodel);
                var isSuccess = _leaveRequestRepo.create(leaveRequest);
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went Wrong with Submitting your recored");
                    return View(model);
                }

                return RedirectToAction("MyLeave"); 
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Something Went Wrong ");
                return View(model);
            }
        }

        // GET: LeaveRequest/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequest/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Delete/5
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