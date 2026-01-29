using ConsoleApp1;
//Q1 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B
List<Restaurant> restaurants = LoadRestaurants("restaurants.csv");
LoadFoodItems("fooditems.csv", restaurants);
foreach (Restaurant r in restaurants)
{
    Console.WriteLine(r);

    if (r.menus.Count > 0)
    {
        Console.WriteLine("Menu: " + r.menus[0].menuName);
        r.menus[0].DisplayFoodItems();
    }

    Console.WriteLine();
}

List<Restaurant> LoadRestaurants(string filePath)
{
    List<Restaurant> restaurants = new List<Restaurant>();
    var lines = File.ReadAllLines(filePath);

    foreach (var line in lines.Skip(1)) // dont include header
    {
        var fields = line.Split(',');

        if (fields.Length >= 3) //id name email header
        {
            string restaurantId = fields[0].Trim();
            string restaurantName = fields[1].Trim();
            string restaurantEmail = fields[2].Trim();

            Restaurant r = new Restaurant(restaurantId, restaurantName, restaurantEmail);

            Menu defaultMenu = new Menu("M" + restaurantId, "Main Menu");
            r.AddMenu(defaultMenu);

            restaurants.Add(r);
        }
    }

    return restaurants;
}

void LoadFoodItems(string filePath, List<Restaurant> restaurants)
{
    var lines = File.ReadAllLines(filePath);

    foreach (var line in lines.Skip(1)) // skip header
    {
        var fields = line.Split(',');

        if (fields.Length >= 4) // id name desc and price
        {
            string restaurantId = fields[0].Trim();
            string itemName = fields[1].Trim();
            string itemDesc = fields[2].Trim();
            double itemPrice = double.Parse(fields[3].Trim());

            FoodItem foodItem = new FoodItem(itemName, itemDesc, itemPrice, "");

            // find and add food item to menu
            Restaurant? r = restaurants.FirstOrDefault(x => x.restaurantId == restaurantId);
            if (r != null)
            {
                if (r.menus.Count == 0)
                {
                    r.AddMenu(new Menu("M" + restaurantId, "Main Menu"));
                }

                r.menus[0].AddFoodItem(foodItem);
            }
        }
    }
}

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
//Student Name:Lee Ruo Yu
//Student Number: S10273008B

//Q5 
//Student Number:Pang Jia En
//Student Name:S10269305E

//Q6 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B:

//Q7 
//Student Number:Pang Jia En
//Student Name:S10269305E

//Q8 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B
