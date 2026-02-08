//==========================================================
// Student Number : S10269305E
// Student Name : Pang Jia En
// Partner Name : Lee Ruo Yu
//==========================================================
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    internal class Customer
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        public List<Order> Orders { get; set; }

        public Customer() { }

        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
            Orders = new List<Order>();
        }

        public void AddOrder(Order order)
        {
            Orders.Add(order);
        }

        public void DisplayAllOrders()
        {
            foreach (Order order in Orders)
            {
                Console.WriteLine(order);
            }
        }
        public bool RemoveOrder(Order order)
        {
            return Orders.Remove(order);
        }
        public override string ToString()
        {
            return "Customer Name: " + CustomerName + " Email: " + EmailAddress;
        }
    }
}
