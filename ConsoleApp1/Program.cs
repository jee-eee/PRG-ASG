using ConsoleApp1;
using System;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;


//Start of program

//Main Menu
void DisplayMenu()
{
    Console.WriteLine("=====Gruberoo Food Delivery System=====");
    Console.WriteLine("1. List all restaurants and menu items");
    Console.WriteLine("2. List all order");
    Console.WriteLine("3. Create a new order");
    Console.WriteLine("4. Process an order");
    Console.WriteLine("5. Modify an existing order");
    Console.WriteLine("6. Delete an existing order");
    Console.WriteLine("0. Exit");
}

void RunMenu()
{
    int input = -1;

    while (true)
    {
        DisplayMenu();
        Console.WriteLine("Enter your choice: ");
        input=Convert.ToInt32(Console.ReadLine());

        if (input == 1)
        {
            ListRestaurants();
        }
        else if (input == 2)
        {
            ListOrder();
        }
        else if (input == 3)
        {
            CreateOrder();
        }

        else if (input ==4)
        {
            ProcessOrder();
        }

        else if (input == 5)
        {
            ModifyOrder();
        }

        else if (input ==6)
        {
            DeleteOrder();
        }

        else if (input ==0)
        {
            break;
        }

        else
        {
            Console.WriteLine("Invalid choice. Please try again.");
        }
    }
}
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
//Student Number:S10269305E
//Student Name:Pang Jia En
List<Customer> customers = LoadCustomers("customers.csv");
List<Order> orders = LoadOrders("orders - Copy.csv", customers, restaurants);



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

List<Order> LoadOrders(string filePath,List<Customer> customers,List<Restaurant> restaurants)
{
    List<Order> orders = new List<Order>();
    var lines = File.ReadAllLines(filePath);

    foreach (var line in lines.Skip(1))
    {
        var fields = line.Split(',');
        int orderId = int.Parse(fields[0].Trim());        
        string customerEmail = fields[1].Trim();        
        string restaurantId = fields[2].Trim();        
        string[] d1 = fields[3].Split('/');
        DateTime orderDateTime = new DateTime(
            int.Parse(d1[2]), int.Parse(d1[1]), int.Parse(d1[0])
        );

        string[] d2 = fields[4].Split('/');
        DateTime deliveryDateTime = new DateTime(
            int.Parse(d2[2]), int.Parse(d2[1]), int.Parse(d2[0])
        );
       
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
            restaurant.orders.Add(order);            
        }    
    }
    return orders;
}

//Q3 
//Student Number:S10269305E
//Student Name:Pang Jia En
void ListRestaurants()
{
    Console.WriteLine("All Reastaurants and Menu Item");
    Console.WriteLine("=================================");
    foreach (Restaurant r in restaurants)
    {
        Console.WriteLine($"Restaurant: {r.restaurantName} ({r.restaurantId})");
        foreach (Menu menu in r.menus)
        {
            foreach (FoodItem item in menu.foodItems)
            {
                Console.WriteLine($" - {item.itemName} :  {item.itemDesc}  - ${item.itemPrice}");
            }
        }
        Console.WriteLine();
    }
}

//Q4 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B
void ListOrder()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");

    if (orders.Count == 0)
    {
        Console.WriteLine("No orders found.");
        return;
    }

    Console.WriteLine("Order ID   Customer Email                 Restaurant ID   Delivery Date/Time        Address                 Status");
    Console.WriteLine("--------   -----------------------------  ------------    --------------------      ----------------------  --------");

    foreach (Order o in orders)
    {
        // find email by searching in customers list
        string custEmail = "Unknown";
        foreach (Customer c in customers)
        {
            if (c.orders.Contains(o))
            {
                custEmail = c.EmailAddress;
                break;
            }
        }

        // find restaurant id by searching in restaurants list
        string restId = "Unknown";
        foreach (Restaurant r in restaurants)
        {
            if (r.orders.Contains(o))
            {
                restId = r.restaurantId;
                break;
            }
        }

        string deliveryDT = o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm");

        Console.WriteLine($"{o.OrderId,-9} {custEmail,-30} {restId,-13} {deliveryDT,-22} {o.DeliveryAddress,-24} {o.OrderStatus}");
    }

    Console.WriteLine();
}
//Q5 
//Student Number:S10269305E
//Student Name:Pang Jia En
void CreateOrder()
{
}


//Q6 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B:
void ProcessOrder()
{
    Console.WriteLine("Process Order");
    Console.WriteLine("=============");

    Console.Write("Enter Restaurant ID: ");
    string restaurantId = Console.ReadLine();

    Restaurant r = restaurants.Find(x => x.restaurantId == restaurantId);
    if (r == null)
    {
        Console.WriteLine("Invalid Restaurant ID.");
        return;
    }

    if (r.orders.Count == 0)
    {
        Console.WriteLine("No orders found for this restaurant.");
        return;
    }

    // treat restaurant.orders like a queue by processing in order of earliest delivery time
    List<Order> queueOrders = r.orders.OrderBy(x => x.DeliveryDateTime).ToList();

    foreach (Order o in queueOrders)
    {
        Console.WriteLine($"\nOrder ID: {o.OrderId}");

        // Find customer email
        string custEmail = "Unknown";
        foreach (Customer c in customers)
        {
            if (c.orders.Contains(o))
            {
                custEmail = c.EmailAddress;
                break;
            }
        }
        Console.WriteLine($"Customer Email: {custEmail}");

        Console.WriteLine("Ordered Items:");
        if (o.orderedFoodItems.Count == 0)
        {
            Console.WriteLine(" - (No items)");
        }
        else
        {
            int count = 1;
            foreach (OrderedFoodItem ofi in o.orderedFoodItems)
            {
                Console.WriteLine($"{count}. {ofi.FoodItem.itemName} - {ofi.QtyOrdered}");
                count++;
            }
        }

        Console.WriteLine($"Delivery Date/Time: {o.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Delivery Address: {o.DeliveryAddress}");
        Console.WriteLine($"Total Amount: ${o.OrderTotal:0.00}");
        Console.WriteLine($"Status: {o.OrderStatus}");

        Console.WriteLine("\nAction: [C] Confirm  [R] Reject  [S] Skip  [D] Deliver");
        string action = Console.ReadLine().ToUpper();

        if (action == "C")
        {
            if (o.OrderStatus == "Pending")
            {
                o.OrderStatus = "Preparing";
                Console.WriteLine($"Order {o.OrderId} confirmed. Status updated to Preparing.");
            }
            else
            {
                Console.WriteLine("Cannot confirm. Only Pending orders can be confirmed.");
            }
        }
        else if (action == "R")
        {
            if (o.OrderStatus == "Pending")
            {
                o.OrderStatus = "Rejected";
                refundStack.Push(o);
                Console.WriteLine($"Order {o.OrderId} rejected. Refund processed: ${o.OrderTotal:0.00}");
            }
            else
            {
                Console.WriteLine("Cannot reject. Only Pending orders can be rejected.");
            }
        }
        else if (action == "S")
        {
            if (o.OrderStatus == "Cancelled")
            {
                Console.WriteLine($"Order {o.OrderId} skipped (Cancelled).");
            }
            else
            {
                Console.WriteLine("Cannot skip. Only Cancelled orders can be skipped.");
            }
        }
        else if (action == "D")
        {
            if (o.OrderStatus == "Preparing")
            {
                o.OrderStatus = "Delivered";
                Console.WriteLine($"Order {o.OrderId} delivered. Status updated to Delivered.");
            }
            else
            {
                Console.WriteLine("Cannot deliver. Only Preparing orders can be delivered.");
            }
        }
        else
        {
            Console.WriteLine("Invalid action. Moving to next order...");
        }
    }

    Console.WriteLine();
}
//Q7 
//Student Number:S10269305E
//Student Name:Pang Jia En
void ModifyOrder()
{
}
//Q8 
//Student Name:Lee Ruo Yu
//Student Number: S10273008B
void DeleteOrder()
{
    Console.WriteLine("Delete Order");
    Console.WriteLine("============");

    Console.Write("Enter Customer Email: ");
    string custEmail = Console.ReadLine();

    Customer cust = customers.Find(c => c.EmailAddress == custEmail);
    if (cust == null)
    {
        Console.WriteLine("Invalid Customer Email.");
        return;
    }

    // find pending orders 
    List<Order> pendingOrders = new List<Order>();
    Console.WriteLine("\nPending Orders:");
    foreach (Order o in cust.orders)
    {
        if (o.OrderStatus == "Pending")
        {
            Console.WriteLine($"Order ID: {o.OrderId}");
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        return;
    }

    Console.Write("\nEnter Order ID to delete: ");
    int orderId = Convert.ToInt32(Console.ReadLine());

    Order target = pendingOrders.Find(o => o.OrderId == orderId);
    if (target == null)
    {
        Console.WriteLine("Invalid Order ID (not found in pending list).");
        return;
    }

    Console.WriteLine($"\nOrder ID: {target.OrderId}");
    Console.WriteLine("Ordered Items:");
    if (target.orderedFoodItems.Count == 0)
    {
        Console.WriteLine(" - (No items)");
    }
    else
    {
        int count = 1;
        foreach (OrderedFoodItem ofi in target.orderedFoodItems)
        {
            Console.WriteLine($"{count}. {ofi.FoodItem.itemName} - {ofi.QtyOrdered}");
            count++;
        }
    }

    Console.WriteLine($"Delivery Date/Time: {target.DeliveryDateTime:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Delivery Address: {target.DeliveryAddress}");
    Console.WriteLine($"Total Amount: ${target.OrderTotal:0.00}");
    Console.WriteLine($"Status: {target.OrderStatus}");

    Console.Write("\nConfirm deletion? [Y/N]: ");
    string confirm = Console.ReadLine().ToUpper();

    if (confirm == "Y")
    {
        target.OrderStatus = "Cancelled";
        refundStack.Push(target);

        Console.WriteLine($"Order {target.OrderId} cancelled. Refund processed: ${target.OrderTotal:0.00}");
    }
    else
    {
        Console.WriteLine("Deletion cancelled.");
    }

    Console.WriteLine();
}