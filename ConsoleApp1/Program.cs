using ConsoleApp1;
//Q1 
//Student Number:Lee Ruo Yu
//Student Name:

//Q2 
//Student Number:Pang Jia En
//Student Name:S10269305E
List <Customer> customers = LoadCustomers("customers.csv");
foreach (Customer c in customers)
{
    Console.WriteLine(c);
}

List<Customer> LoadCustomers(string filePath)
{
    List<Customer> customers = new List<Customer>();
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines.Skip(1)) // Skip header line
    {
        var fields = line.Split(',');
        if (fields.Length >= 2)
        {
            string name = fields[0].Trim();
            string email = fields[1].Trim();
            Customer customer = new Customer(email, name);
            customers.Add(customer);
        }
    }
    return customers;
}

List<Order> order = LoadOrder("orders.csv");
foreach (Order o in order)
{
    Console.WriteLine(o);
}

List<Order> LoadOrder(string filePath,List<Customer> customers,List<Restaurant> restaurants)
{
    List<Order> orders = new List<Order>();
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines.Skip(1)) 
    {
        var fields = line.Split(',');
        int orderId = int.Parse(fields[0].Trim());
        string customerEmail = fields[1].Trim();
        string restaurantId = fields[2].Trim();
        DateTime orderDateTime = DateTime.Parse(fields[3].Trim());
        DateTime deliveryDateTime = DateTime.Parse(fields[4].Trim());
        string deliveryAddress = fields[5].Trim();

        Order order = new Order(orderId, orderDateTime, deliveryAddress, deliveryDateTime);
        orders.Add(order);
    }
    return orders;
}

//Q3 
//Student Number:Pang Jia En
//Student Name:S10269305E

//Q4 
//Student Number:Lee Ruo Ye
//Student Name:

//Q5 
//Student Number:Pang Jia En
//Student Name:S10269305E

//Q6 
//Student Number:Lee Ruo Yu
//Student Name:

//Q7 
//Student Number:Pang Jia En
//Student Name:S10269305E

//Q8 
//Student Number:Lee Ruo Yu
//Student Name:
