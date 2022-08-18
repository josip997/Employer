using Employer.Data;
using Employer.Models;
using Employer.Models.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Employer.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeesDbContext employeesDbContext;

        public EmployeesController(EmployeesDbContext employeesDbContext)
        {
            this.employeesDbContext = employeesDbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var result = from employee in employeesDbContext.Employees
                         join department in employeesDbContext.Departments
                            on employee.DepartmentId equals department.Id
                         select new
                         {
                             employee.Id,
                             employee.Name,
                             employee.Email,
                             employee.Salary,
                             DepartmentName = department.Name,
                             employee.DateOfBirth
                         };

            var rtn = new List<IndexEmployeeViewModel>();

            foreach (var item in result)
            {
                rtn.Add(new IndexEmployeeViewModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Email = item.Email,
                    Salary = item.Salary,
                    DepartmentName = item.DepartmentName,
                    DateOfBirth = item.DateOfBirth,
                });
            }

            return View(rtn);
        }

        [HttpGet]
        public IActionResult Add()
        {
            List<SelectListItem> departments = GetDepartments();

            var employee = new AddEmployeeViewModel()
            {
                Departments = departments
            };


            return View(employee);
        }

        private List<SelectListItem> GetDepartments()
        {
            var departments = employeesDbContext.Departments.AsNoTracking()
                                .OrderBy(d => d.Cipher)
                                    .Select(d =>
                                    new SelectListItem
                                    {
                                        Value = d.Id.ToString(),
                                        Text = d.Name,
                                    }).ToList();

            var deptip = new SelectListItem()
            {
                Value = null,
                Text = "--- Select department ---"
            };

            departments.Insert(0, deptip);
            return departments;
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddEmployeeViewModel addEmployeeRequest)
        {

            addEmployeeRequest.Departments = GetDepartments();

            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Name = addEmployeeRequest.Name,
                Email = addEmployeeRequest.Email,
                Salary = addEmployeeRequest.Salary,
                DepartmentId = addEmployeeRequest.DepartmentId,
                DateOfBirth = addEmployeeRequest.DateOfBirth,
            };

            if (ModelState.IsValid)
            {
                await employeesDbContext.Employees.AddAsync(employee);
                await employeesDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }


            return View(addEmployeeRequest);
        }

        [HttpGet]
        public async Task<IActionResult> View(Guid id)
        {
            var employee = await employeesDbContext.Employees.FirstOrDefaultAsync(x => x.Id == id);

            if (employee != null)
            {
                var viewModel = new UpdateEmployeeViewModel()
                {
                    Id = employee.Id,
                    Name = employee.Name,
                    Email = employee.Email,
                    Salary = employee.Salary,
                    DepartmentId = employee.DepartmentId,
                    Departments = GetDepartments(),
                    DateOfBirth = employee.DateOfBirth,
                };

                return await Task.Run(() => View("View", viewModel));
            }


            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> View(UpdateEmployeeViewModel viewModel)
        {
            var employee = await employeesDbContext.Employees.FindAsync(viewModel.Id);

            viewModel.Departments = GetDepartments();

            if (employee != null)
            {

                employee.Name = viewModel.Name;
                employee.Email = viewModel.Email;
                employee.Salary = viewModel.Salary;
                employee.DepartmentId = viewModel.DepartmentId;
                employee.DateOfBirth = viewModel.DateOfBirth;

                if (ModelState.IsValid)
                {
                    await employeesDbContext.SaveChangesAsync();

                    return RedirectToAction("Index");
                }

                return await Task.Run(() => View("View", viewModel));

            }

            return RedirectToAction("Index"); //TODO: Not Found
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UpdateEmployeeViewModel viewModel)
        {
            var employee = await employeesDbContext.Employees.FindAsync(viewModel.Id);

            if (employee != null)
            {
                employeesDbContext.Employees.Remove(employee);
                await employeesDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index"); //TODO: Not Found
        }
    }
}
