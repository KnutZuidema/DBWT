using System;
using emensa.DataModels;
using emensa.Utility;

namespace emensa.ViewModels
{
    public class Register
    {
        public Register()
        {
            DoPasswordsMatch = true;
            IsEmailValid = true;
            DoesUserExist = false;
        }
        
        public bool DoPasswordsMatch { get; set; }
        public bool IsEmailValid { get; set; }
        public bool DoesUserExist { get; set; }

        public static void RegisterStudent(Student student)
        {
            using (var db = new EmensaContext())
            {
                db.User.Add(student.Member.User);
                db.Member.Add(student.Member);
                db.Student.Add(student);
                db.SaveChanges();
            }
        }

        public static void RegisterEmployee(Employee employee)
        {
            using (var db = new EmensaContext())
            {
                db.User.Add(employee.Member.User);
                db.Member.Add(employee.Member);
                db.Employee.Add(employee);
                db.SaveChanges();
            }
        }

        public static void RegisterGuest(Guest guest)
        {
            using (var db = new EmensaContext())
            {
                db.User.Add(guest.User);
                db.Guest.Add(guest);
                db.SaveChanges();
            }
        }
    }
}