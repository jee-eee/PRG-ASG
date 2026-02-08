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

    while (input != 0)
    {
        DisplayMenu();
        Console.Write("Enter your choice: ");
        input=Convert.ToInt32(Console.ReadLine());
        Console.WriteLine();
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
List<Order> orders = LoadOrders("orders.csv", customers, restaurants);
Stack<Order> refundStack = new Stack<Order>();

RunMenu();

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
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");

    foreach (Restaurant r in restaurants)
    {
        Console.WriteLine($"Restaurant: {r.restaurantName} ({r.restaurantId})");

        if (r.menus.Count > 0)
        {
            foreach (FoodItem fi in r.menus[0].foodItems)
            {
                Console.WriteLine($" - {fi.foodName}: {fi.description} - ${fi.price:0.00}");
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
        Console.WriteLine();
        return;
    }

    Dictionary<int, (string custEmail, string restId)> orderInfo = new Dictionary<int, (string, string)>();
    if (File.Exists("orders.csv"))
    {
        var lines = File.ReadAllLines("orders.csv");
        foreach (var line in lines.Skip(1))
        {
            var fields = line.Split(',');
            if (fields.Length >= 3)
            {
                int id;
                if (int.TryParse(fields[0].Trim(), out id))
                {
                    string custEmail = fields[1].Trim();
                    string restId = fields[2].Trim();
                    orderInfo[id] = (custEmail, restId);
                }
            }
        }
    }

    Console.WriteLine("Order ID  Customer Email                  Restaurant ID  Delivery Date/Time        Address                   Total     Status");
    Console.WriteLine("--------  ------------------------------  ------------   --------------------      ------------------------  --------  --------");

    foreach (Order o in orders)
    {
        string custEmail = "Unknown";
        string restId = "Unknown";

        if (orderInfo.ContainsKey(o.OrderId))
        {
            custEmail = orderInfo[o.OrderId].custEmail;
            restId = orderInfo[o.OrderId].restId;
        }

        Console.WriteLine($"{o.OrderId,-8}  {custEmail,-30}  {restId,-12}  {o.DeliveryDateTime:dd/MM/yyyy HH:mm}      {o.DeliveryAddress,-24}  ${o.OrderTotal,6:0.00}  {o.OrderStatus}");
    }

    Console.WriteLine();
}
//Q5 
//Student Number:S10269305E
//Student Name:Pang Jia En
void CreateOrder()
{
    Console.WriteLine("Create New Order");
    Console.WriteLine("================");
    Console.Write("Enter Customer Email: ");
    string customerEmail = Console.ReadLine();
    Console.WriteLine("Enter Restaurant ID:: ");
    string restaurantId = Console.ReadLine();
    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
    DateTime deliveryData = DateTime.Parse(Console.ReadLine()!);
    Console.Write("Enter Delivery Time (hh:mm): ");
    DateTime deliveryTime = DateTime.Parse(Console.ReadLine()!);
    Console.Write("Enter Delivery Address: ");
    string deliveryAddress = Console.ReadLine();

    Restaurant selectedRestaurant = restaurants.Find(r => r.restaurantId == restaurantId);
    if (selectedRestaurant == null)
    {
        Console.WriteLine("Invalid Restaurant ID.");
        return;
    }

    List<FoodItem> availableItems = new List<FoodItem>();
    int itemNumber = 1;

    Console.WriteLine("Available Food Items: ");
    foreach (Menu menu in selectedRestaurant.menus)
    {
        foreach (FoodItem item in menu.foodItems)
        {
            Console.WriteLine($"{itemNumber}. {item.itemName} - ${item.itemPrice}");
            availableItems.Add(item);
            itemNumber++;
        }
    }

    double total = 0;
    int choice;
    Console.Write("Enter item number (0 to finish): ");
    choice = int.Parse(Console.ReadLine()!);

    if (choice > 0 && choice <= availableItems.Count)
    {
        Console.Write("Enter quantity: ");
        int quantity = int.Parse(Console.ReadLine());

        FoodItem selectedItem = availableItems[choice - 1];
        total += selectedItem.itemPrice * quantity;
    }

    string specialRequest = "";
    Console.WriteLine("Add special request? [Y/N]");
    string specialChoice = Console.ReadLine()!;

    if (specialChoice.ToUpper() == "Y")
    {
        Console.Write("Enter special request: ");
        specialRequest = Console.ReadLine()!;
    }

    double deliveryfee = 5.0;

    Console.WriteLine($"Order Total: ${total} + ${deliveryfee} = ${total+deliveryfee} ");

    Console.WriteLine("Proceed to payment? [Y/N]");
    string proceedPayment = Console.ReadLine()!;
    if (proceedPayment.ToUpper() != "Y")
    {
        Console.WriteLine("Order cancelled.");
        return;
    }

    Console.WriteLine("Payment method:");
    Console.WriteLine("[CC Credit Card / [PP] Paypal / [CD] Cash on Delivery: ");
    string paymentMethod = Console.ReadLine()!;

    //create order
    int newOrderId = orders.Count + 1;
    Order newOrder = new Order(newOrderId, DateTime.Now, deliveryAddress, deliveryData.AddHours(deliveryTime.Hour).AddMinutes(deliveryTime.Minute));

    newOrder.OrderStatus = "Pending";
    newOrder.OrderPaymentMethod = paymentMethod;

    //add to customer
    Customer customer = customers.Find(c => c.EmailAddress == customerEmail);
    if (customer != null)
    {
        customer.AddOrder(newOrder);
    }

    //add to restaurant
    selectedRestaurant.orders.Add(newOrder);

    string csvline = $"{newOrderId},{customerEmail},{restaurantId},{DateTime.Now},{deliveryData.AddHours(deliveryTime.Hour).AddMinutes(deliveryTime.Minute)},{deliveryAddress},{paymentMethod},Pending";
    File.AppendAllText("orders.csv", csvline + Environment.NewLine);

    Console.WriteLine($"Order {newOrderId} created successfully! Status:Preaparing ");
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
        Console.WriteLine();
        return;
    }

    Dictionary<int, (string custEmail, string restId)> orderInfo = new Dictionary<int, (string, string)>();
    if (File.Exists("orders.csv"))
    {
        var lines = File.ReadAllLines("orders.csv");
        foreach (var line in lines.Skip(1))
        {
            var fields = line.Split(',');
            if (fields.Length >= 3)
            {
                int id;
                if (int.TryParse(fields[0].Trim(), out id))
                {
                    string custEmail = fields[1].Trim();
                    string restId = fields[2].Trim();
                    orderInfo[id] = (custEmail, restId);
                }
            }
        }
    }

    List<Order> restOrders = new List<Order>();
    foreach (Order o in orders)
    {
        if (orderInfo.ContainsKey(o.OrderId) && orderInfo[o.OrderId].restId == restaurantId)
        {
            restOrders.Add(o);
        }
    }

    if (restOrders.Count == 0)
    {
        Console.WriteLine("No orders found for this restaurant.");
        Console.WriteLine();
        return;
    }

    restOrders = restOrders.OrderBy(x => x.DeliveryDateTime).ToList();

    foreach (Order o in restOrders)
    {
        string custEmail = "Unknown";
        if (orderInfo.ContainsKey(o.OrderId))
            custEmail = orderInfo[o.OrderId].custEmail;

        Console.WriteLine($"\nOrder ID: {o.OrderId}");
        Console.WriteLine($"Customer Email: {custEmail}");
        Console.WriteLine("Ordered Items:");

        if (o.OrderedFoodItems.Count == 0)
        {
            Console.WriteLine(" - (No items)");
        }
        else
        {
            int count = 1;
            foreach (OrderedFoodItem ofi in o.OrderedFoodItems)
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
    Console.WriteLine("Modify Order");
    Console.WriteLine("============");
    Console.WriteLine("Enter Customer Email: ");
    string custEmail = Console.ReadLine();

    Customer cust = customers.Find(c => c.EmailAddress == custEmail);
    if (cust == null)
    {
        Console.WriteLine("Customer not found.");
        return;
    }

    List<Order> pendingOrders = new List<Order>();

    Console.WriteLine("Pending Orders: ");
    foreach (Order o in orders)
    {
        if (o.OrderStatus == "Pending")
        {
            Console.WriteLine($"Order ID: {o.OrderId}");
            pendingOrders.Add(o);
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders.");
        return;
    }

    Console.WriteLine("Enter Order ID: ");
    int orderId = int.Parse(Console.ReadLine());

    Order selectedOrder = pendingOrders.Find(o => o.OrderId == orderId);
    if (selectedOrder == null)
    {
        Console.WriteLine("Invalid Order ID.");
        return;
    }


    Console.WriteLine("Order Items: ");
    int count = 1;
    foreach (OrderedFoodItem ofi in selectedOrder.OrderedFoodItems)
    {
        Console.WriteLine($"{count}. {ofi.FoodItem.itemName} - {ofi.QtyOrdered}");
        count++;
    }
    Console.WriteLine($"Address: ");
    Console.WriteLine($"{selectedOrder.DeliveryAddress}");
    Console.WriteLine("Delivery Data/Time");
    Console.WriteLine($"{selectedOrder.DeliveryDateTime}");

    Console.WriteLine("Modify: [1]Items [2]Address [3] Delivery Time: ");
    int choices = int.Parse(Console.ReadLine());

    if (choices == 1)
    {
        Console.WriteLine("Enter new Items: ");
        string item = Console.ReadLine();
        Console.WriteLine($"Order {orderId} updated. New Delivery Time: {item}");
    }

    else if (choices == 2)
    {
        Console.Write("Enter new Address: ");
        string newAddress = Console.ReadLine();
        selectedOrder.DeliveryAddress = newAddress;

        Console.WriteLine($"Order {orderId} updated. New Address: {newAddress}");
    }

    else if (choices == 3)
    {
        Console.Write("Enter new Delivery Time (hh:mm): ");
        TimeSpan newTime = TimeSpan.Parse(Console.ReadLine());

        selectedOrder.DeliveryDateTime = selectedOrder.DeliveryDateTime.Date + newTime;

        Console.WriteLine($"\nOrder {orderId} updated. New Delivery Time: {selectedOrder.DeliveryDateTime:HH:mm}");
    }
    else
    {
        Console.WriteLine("Invalid choice.");
    }

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

    // Build lookup from orders.csv: OrderId -> (customerEmail, restaurantId)
    Dictionary<int, (string custEmail, string restId)> orderInfo = new Dictionary<int, (string, string)>();
    if (File.Exists("orders.csv"))
    {
        var lines = File.ReadAllLines("orders.csv");
        foreach (var line in lines.Skip(1))
        {
            var fields = line.Split(',');
            if (fields.Length >= 3)
            {
                int id;
                if (int.TryParse(fields[0].Trim(), out id))
                {
                    string email = fields[1].Trim();
                    string restId = fields[2].Trim();
                    orderInfo[id] = (email, restId);
                }
            }
        }
    }

    // Find pending orders for this customer (by matching email in CSV)
    List<Order> pendingOrders = new List<Order>();

    foreach (Order o in orders)
    {
        if (orderInfo.ContainsKey(o.OrderId))
        {
            if (orderInfo[o.OrderId].custEmail == custEmail && o.OrderStatus == "Pending")
            {
                pendingOrders.Add(o);
            }
        }
    }

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found for this customer.");
        Console.WriteLine();
        return;
    }

    Console.WriteLine("\nPending Orders:");
    foreach (Order o in pendingOrders)
    {
        Console.WriteLine($"Order ID: {o.OrderId}");
    }

    Console.Write("\nEnter Order ID to delete: ");
    int orderId = Convert.ToInt32(Console.ReadLine());

    Order target = pendingOrders.Find(o => o.OrderId == orderId);
    if (target == null)
    {
        Console.WriteLine("Invalid Order ID (not found in pending list).");
        Console.WriteLine();
        return;
    }

    Console.WriteLine($"\nOrder ID: {target.OrderId}");
    Console.WriteLine("Ordered Items:");

    if (target.OrderedFoodItems.Count == 0)
    {
        Console.WriteLine(" - (No items)");
    }
    else
    {
        int count = 1;
        foreach (OrderedFoodItem ofi in target.OrderedFoodItems)
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