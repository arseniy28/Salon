using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pomoi.Models
{
    public class View
    {

        public string FirstName { get; set; } // Имя клиента
        public string LastName { get; set; } // Фамилия клиента
        public string Phone { get; set; } // Телефон клиента
        public string Email { get; set; } // Email клиента
        public DateOnly BirthDate { get; set; } // Дата рождения клиента
        public DateTime VisitDate { get; set; } // Дата посещения
    }
}
