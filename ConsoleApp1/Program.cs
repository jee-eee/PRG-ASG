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
        input = Convert.ToInt32(Console.ReadLine());

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

        else if (input == 4)
        {
            ProcessOrder();
        }

        else if (input == 5)
        {
            ModifyOrder();
        }

        else if (input == 6)
        {
            DeleteOrder();
        }

        else if (input == 0)
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
        string email = fields[1].Trim().ToLower();
        Customer customer = new Customer(email, name);
        customers.Add(customer);
    }
    return customers;
}

List<Order> LoadOrders(string filePath, List<Customer> customers, List<Restaurant> restaurants)
{
    List<Order> orders = new List<Order>();
    var lines = File.ReadAllLines(filePath);

    foreach (var line in lines.Skip(1))
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        // Manually parse CSV to handle quoted fields with commas
        List<string> fields = new List<string>();
        bool inQuotes = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }
        fields.Add(currentField); // Add last field

        if (fields.Count < 9)
            continue;

        if (!int.TryParse(fields[0].Trim(), out int orderId))
            continue;

        string customerEmail = fields[1].Trim().ToLower();
        string restaurantId = fields[2].Trim();

        // Parse delivery date and time separately
        DateTime deliveryDateTime;
        try
        {
            string dateStr = fields[3].Trim();
            string timeStr = fields[4].Trim();

            // Parse date (format: dd/MM/yyyy)
            string[] dateParts = dateStr.Split('/');
            int day = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[2]);

            // Parse time (format: HH:mm)
            string[] timeParts = timeStr.Split(':');
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            deliveryDateTime = new DateTime(year, month, day, hour, minute, 0);
        }
        catch
        {
            Console.WriteLine($"DEBUG: Failed to parse date/time for order {orderId}");
            continue;
        }

        string deliveryAddress = fields[5].Trim();

        // Parse order created datetime (format: dd/MM/yyyy HH:mm)
        DateTime orderDateTime;
        try
        {
            string orderDateTimeStr = fields[6].Trim();
            string[] parts = orderDateTimeStr.Split(' ');
            string[] dateParts = parts[0].Split('/');
            string[] timeParts = parts[1].Split(':');

            int day = int.Parse(dateParts[0]);
            int month = int.Parse(dateParts[1]);
            int year = int.Parse(dateParts[2]);
            int hour = int.Parse(timeParts[0]);
            int minute = int.Parse(timeParts[1]);

            orderDateTime = new DateTime(year, month, day, hour, minute, 0);
        }
        catch
        {
            Console.WriteLine($"DEBUG: Failed to parse order datetime for order {orderId}");
            continue;
        }

        Order order = new Order(
            orderId,
            orderDateTime,
            deliveryAddress,
            deliveryDateTime
        );

        if (!double.TryParse(fields[7].Trim(), out double total))
            total = 0;

        order.OrderTotal = total;
        order.OrderStatus = fields[8].Trim();
        order.SpecialRequest = "";

        // ----- ORDERED ITEMS -----
        if (fields.Count >= 10)
        {
            string itemsField = fields[9].Trim();

            Restaurant rest = restaurants.Find(r => r.restaurantId == restaurantId);

            if (rest != null && rest.menus.Count > 0)
            {
                foreach (string itemEntry in itemsField.Split('|'))
                {
                    if (string.IsNullOrWhiteSpace(itemEntry))
                        continue;

                    string[] itemData = itemEntry.Split(',');
                    if (itemData.Length < 2)
                        continue;

                    string itemName = itemData[0].Trim();
                    if (!int.TryParse(itemData[1].Trim(), out int qty))
                        continue;

                    FoodItem food = rest.menus[0].foodItems
                        .Find(f => f.itemName == itemName);

                    if (food != null)
                    {
                        order.AddOrderedFoodItem(
                            new OrderedFoodItem(food, qty)
                        );
                    }
                }
            }
        }

        // link order
        orders.Add(order);

        Customer customer = customers.Find(c =>
            c.EmailAddress.Trim().ToLower() == customerEmail
        );
        if (customer != null)
        {
            customer.AddOrder(order);
        }

        Restaurant restaurant = restaurants.Find(r => r.restaurantId == restaurantId);
        if (restaurant != null)
            restaurant.orders.Add(order);
    }

    return orders;
}


RunMenu();

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
            if (c.Orders.Contains(o))
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
        Console.WriteLine("Create New Order");
        Console.WriteLine("================");

        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine().Trim().ToLower();
        Customer cust = customers.Find(c => c.EmailAddress.ToLower() == customerEmail);
        if (cust == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        Console.WriteLine("Enter Restaurant ID:: ");
        string restaurantId = Console.ReadLine();
        Restaurant r = restaurants.Find(x => x.restaurantId == restaurantId);
        if (r == null)
        {
            Console.WriteLine("Restaurant not found.");
            return;
        }

        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        string input = Console.ReadLine();

        string[] parts = input.Split('/');

        int day = int.Parse(parts[0]);
        int month = int.Parse(parts[1]);
        int year = int.Parse(parts[2]);

        DateTime deliveryDate = new DateTime(year, month, day);

        Console.Write("Enter Delivery Time (hh:mm): ");
        TimeSpan deliveryTime = TimeSpan.Parse(Console.ReadLine()!);

        Console.Write("Enter Delivery Address: ");
        string deliveryAddress = Console.ReadLine();

        int newOrderId = orders.Count == 0 ? 1001 : orders.Max(o => o.OrderId) + 1;
        Order order = new Order(
            newOrderId,
            DateTime.Now,
            deliveryAddress,
            deliveryDate + deliveryTime
        );

        List<FoodItem> items = r.menus[0].foodItems;
        Console.WriteLine("Available Food Items: ");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].itemName} - ${items[i].itemPrice}");
        }

        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            if (choice == 0)
            {
                break;
            }
            if (choice < 1 || choice > items.Count)
            {
                Console.WriteLine("Invalid item number. Please try again.");
                continue;
            }

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            OrderedFoodItem ofi = new OrderedFoodItem(items[choice - 1], quantity);
            order.AddOrderedFoodItem(ofi);
        }

        // ----- Ensure at least one item -----
        if (order.OrderedFoodItems.Count == 0)
        {
            Console.WriteLine("No items added. Order cancelled.");
            return;
        }

        Console.Write("Add special request? [Y/N]: ");
        string srChoice = Console.ReadLine().ToUpper();
        string specialRequest = "";

        if (srChoice == "Y")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();
        }

        else if (srChoice != "N")
        {
            Console.WriteLine("Invalid choice. No special request added.");
            return;
        }
        order.SpecialRequest = specialRequest;



        double subTotal = order.CalculateOrderTotal();
        double deliveryfee = 5.0;
        double total = order.CalculateOrderTotal() + deliveryfee;
        Console.WriteLine($"Order Total: ${subTotal} + ${deliveryfee} = ${total} ");

        Console.WriteLine("Proceed to payment? [Y/N]");
        if (Console.ReadLine().ToUpper() != "Y")
        {
            Console.WriteLine("Order cancelled.");
            return;
        }

        Console.WriteLine("Payment method: ");
        Console.WriteLine("[CC Credit Card / [PP] Paypal / [CD] Cash on Delivery: ");
        order.OrderPaymentMethod = Console.ReadLine();
        order.OrderStatus = "Pending";

        cust.AddOrder(order);
        r.orders.Add(order);
        orders.Add(order);

        string itemsStr = "";
        foreach (OrderedFoodItem ofi in order.OrderedFoodItems)
        {
            itemsStr += $"{ofi.FoodItem.itemName},{ofi.QtyOrdered}|";
        }
        itemsStr = itemsStr.TrimEnd('|');

        if (!string.IsNullOrWhiteSpace(order.SpecialRequest))
        {
            itemsStr += $"||{order.SpecialRequest}";
        }

        File.AppendAllText(
        "orders - Copy.csv",
        $"{order.OrderId}," +
        $"{cust.EmailAddress}," +
        $"{r.restaurantId}," +
        $"{order.DeliveryDateTime:dd/MM/yyyy}," +
        $"{order.DeliveryDateTime:HH:mm}," +
        $"{order.DeliveryAddress}," +
        $"{order.OrderDateTime:dd/MM/yyyy HH:mm}," +
        $"{total}," +
        $"{order.OrderStatus}," +
        $"\"{itemsStr}\""
    );
        Console.WriteLine($"Order {order.OrderId} created successfully! Status: Pending");
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
            if (c.Orders.Contains(o))
            {
                custEmail = c.EmailAddress;
                break;
            }
        }
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
    string custEmail = Console.ReadLine().Trim().ToLower();
    Customer cust = customers.Find(c => c.EmailAddress.ToLower() == custEmail);
    if (cust == null)
    {
        Console.WriteLine("Customer not found.");
        return;
    }
    List<Order> pendingOrders = new List<Order>();
    Console.WriteLine("Pending Orders:");

    foreach (Order o in orders)
    {
        if (o.OrderStatus == null)
            continue;

        if (o.OrderStatus.Trim().ToLower() != "pending")
            continue;

        if (cust.Orders.Contains(o))
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
        while (true)
        {
            Console.WriteLine("Current Items: ");
            Console.WriteLine("--------------------");
            int i = 1;
            foreach (OrderedFoodItem ofi in selectedOrder.OrderedFoodItems)
            {
                Console.WriteLine(
                    $"{i}. {ofi.FoodItem.itemName} x{ofi.QtyOrdered} " +
                    $"- ${ofi.CalculateSubTotal():0.00}"
                );
                i++;
            }
            Console.WriteLine($"Current Total: ${selectedOrder.CalculateOrderTotal():0.00}");

            Console.WriteLine(" Modify Items: ");
            Console.WriteLine("[1] Add Item");
            Console.WriteLine("[2] Remove Item");
            Console.WriteLine("[0]Done");
            Console.WriteLine("Choice: ");
            if (!int.TryParse(Console.ReadLine(), out int itemChoice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }
            if (itemChoice == 0)
            {
                break;
            }
            else if (itemChoice == 1)
            {
                Restaurant rest = restaurants.Find(r => r.orders.Contains(selectedOrder));
                if (rest == null)
                {
                    Console.WriteLine("Restaurant not found for this order.");
                    continue;
                }
                Console.WriteLine("Available Food Items: ");
                List<FoodItem> items = rest.menus[0].foodItems;
                for (int j = 0; j < items.Count; j++)
                {
                    Console.WriteLine($"{j + 1}. {items[j].itemName} - ${items[j].itemPrice}");
                }
                Console.Write("Enter item number to add: ");
                if (!int.TryParse(Console.ReadLine(), out int addChoice) ||
                    addChoice < 1 || addChoice > items.Count)
                {
                    Console.WriteLine("Invalid item number.");
                    continue;
                }
                Console.Write("Enter quantity: ");
                if (!int.TryParse(Console.ReadLine(), out int qty) || qty <= 0)
                {
                    Console.WriteLine("Invalid quantity.");
                    continue;
                }
                OrderedFoodItem newOfi =
                    new OrderedFoodItem(items[addChoice - 1], qty);
                selectedOrder.AddOrderedFoodItem(newOfi);
                Console.WriteLine($"Added {qty} x {items[addChoice - 1].itemName} to order.");
            }
            else if (itemChoice == 2)
            {
                Console.Write("Enter item number to remove: ");
                if (!int.TryParse(Console.ReadLine(), out int removeChoice) ||
                    removeChoice < 1 ||
                    removeChoice > selectedOrder.OrderedFoodItems.Count)
                {
                    Console.WriteLine("Invalid item number.");
                    continue;
                }

                OrderedFoodItem ofiToRemove = selectedOrder.OrderedFoodItems[removeChoice - 1];
                selectedOrder.RemoveOrderedFoodItem(ofiToRemove);
                Console.WriteLine($"Removed {ofiToRemove.FoodItem.itemName} from order.");
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }
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
    foreach (Order o in orders)
    {
        o.CalculateOrderTotal();
    }
    // save inside csv
    List<string> lines = new List<string>();

    lines.Add("OrderID,CustomerEmail,RestaurantID,DeliveryDate,DeliveryTime,DeliveryAddress,OrderDateTime,OrderTotal,OrderStatus,OrderedItems");

    foreach (Order o in orders)
    {
        Customer owner = customers.Find(c => c.Orders.Contains(o));
        if (owner == null) continue;

        Restaurant rest = restaurants.Find(r => r.orders.Contains(o));
        if (rest == null) continue;

        string itemsStr = "";
        foreach (OrderedFoodItem ofi in o.OrderedFoodItems)
        {
            itemsStr += $"{ofi.FoodItem.itemName},{ofi.QtyOrdered}|";
        }
        itemsStr = itemsStr.TrimEnd('|');

        if (!string.IsNullOrWhiteSpace(o.SpecialRequest))
        {
            itemsStr += $"||{o.SpecialRequest}";
        }

        lines.Add(
            $"{o.OrderId}," +
            $"{owner.EmailAddress}," +
            $"{rest.restaurantId}," +
            $"{o.DeliveryDateTime:dd/MM/yyyy}," +
            $"{o.DeliveryDateTime:HH:mm}," +
            $"{o.DeliveryAddress}," +
            $"{o.OrderDateTime:dd/MM/yyyy HH:mm}," +
            $"{o.OrderTotal}," +
            $"{o.OrderStatus}," +
            $"\"{itemsStr}\""
            );
    }

    File.WriteAllLines("orders - Copy.csv", lines);
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
    foreach (Order o in cust.Orders)
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