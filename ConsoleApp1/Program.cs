using ConsoleApp1;
//Q1 
//Student Number:Lee Ruo Yu
//Student Name:

//Q2 
//Student Number:S10269305E
//Student Name:Pang Jia En
List<Customer> customers = LoadCustomers("customers.csv");
List<Order> orders = LoadOrders("orders.csv", customers, restaurants);

List<Customer> LoadCustomers(string filePath)
{
    List<Customer> customers = new List<Customer>();
    var lines = File.ReadAllLines(filePath);
    foreach (var line in lines.Skip(1)) // Skip header line
    {
        var fields = line.Split(',');
        string name = fields[0].Trim();
        string email = fields[1].Trim();
        Customer customer = new Customer(email, name);
        customers.Add(customer);
    }
    return customers;
}

static List<Order> LoadOrders(
    string filePath,
    List<Customer> customers,
    List<Restaurant> restaurants)
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
        
        Customer customer = customers.Find(c => c.EmailAddress == customerEmail);        
        if (customer != null)        
        {        
            customer.AddOrder(order);            
        }        
        Restaurant restaurant = restaurants.Find(r => r.restaurantId == restaurantId);         
        if (restaurant != null)        
        {        
            restaurant.AddOrder(order);            
        }    
    }
    return orders;
}
//Q3 
//Student Number:S10269305E
//Student Name:Pang Jia En
Console.WriteLine("All Reastaurants and Menu Item");
Console.WriteLine("=================================");
foreach (Restaurant r in restaurants)
{
    Console.WriteLine($"Restaurant: {r.restaurantName} ({r.restaurantId})");
    foreach (Menu menu in r.menus)
    {
        foreach (FoodItem item in menu.menuItems)
        {
            Console.WriteLine($" - {item.ItemName} :  {item.ItemDescription}  - ${ item.Price}");
        }
    }
    Console.WriteLine();
}

//Q4 
//Student Number:Lee Ruo Ye
//Student Name:

//Q5 
//Student Number:S10269305E
//Student Name:Pang Jia En

//Q6 
//Student Number:Lee Ruo Yu
//Student Name:

//Q7 
//Student Number:S10269305E
//Student Name:Pang Jia En

//Q8 
//Student Number:Lee Ruo Yu
//Student Name:
