using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class RoomData
{
    [JsonPropertyName("Room")]
    public Room[]? Rooms { get; set; }
}

public class Room
{
    [JsonPropertyName("roomId")]
    public string? roomId { get; set; }

    [JsonPropertyName("roomName")]
    public string? roomName { get; set; }

    [JsonPropertyName("capacity")]
    public int capacity { get; set; } // must be int type
}




public class Reservation
{
    public DateTime Time { get; set; }
    public DateTime Date { get; set; }
    public string ReserverName { get; set; }
    public Room Room { get; set; } = new Room();
}

public class ReservationHandler
{
    public Reservation[][] Reservations; // 2D array to hold reservations

    public ReservationHandler(int daysOfWeek, int rooms)
    {
        Reservations = new Reservation[daysOfWeek][];
        for (int i = 0; i < daysOfWeek; i++)
        {
            Reservations[i] = new Reservation[rooms];
        }
    }

   public void AddReservation(Reservation reservation)
{
    // Check if the reservation date is valid
    if (reservation.Date < DateTime.Today)
    {
        Console.WriteLine("Cannot add reservation for past dates.");
        return;
    }

    // Find an empty slot for the reservation
    bool reservationAdded = false;
    for (int i = 0; i < Reservations.GetLength(0); i++)
    {
        for (int j = 0; j < Reservations[i].Length; j++)
        {
            if (Reservations[i][j] == null)
            {
                // Add the reservation to the empty slot
                Reservations[i][j] = reservation;
                reservationAdded = true;
                Console.WriteLine("Reservation added successfully.");
                break;
            }
        }
        if (reservationAdded)
            break;
    }

    if (!reservationAdded)
    {
        Console.WriteLine("All slots are occupied. Reservation could not be added.");
    }
}

public void DeleteReservation(Reservation reservation)
{
    
    bool reservationFound = false;
    for (int i = 0; i < Reservations.GetLength(0); i++)
    {
        for (int j = 0; j < Reservations[i].Length; j++)
        {
            if (Reservations[i][j] != null && Reservations[i][j].Equals(reservation))
            {
                // Rezervasyon bulunduğunda, rezervasyonu silin ve işlemi bitirin
                Reservations[i][j] = null;
                Console.WriteLine("Reservation deleted successfully.");
                reservationFound = true;
                break;
            }
        }
        if (reservationFound)
            break;
    }

    // If the reservation is not found, display an appropriate message.
    if (!reservationFound)
    {
        Console.WriteLine("The specified reservation could NOT be found. Deletion failed.");
    }
}


public void DisplayWeeklySchedule()
{
    // Haftanın günlerini bir diziye atadık.
    string[] daysOfWeek = {"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"};

    // Her bir gün için rezervasyonları ekrana yazdırılması için
    for (int i = 0; i < Reservations.GetLength(0); i++)
    {
        Console.WriteLine($"--- {daysOfWeek[i]} ---");

        // Eğer bir gün için hiç rezervasyon yapılmamışsa
        if (Reservations[i] == null || Reservations[i].All(r => r == null))
        {
            Console.WriteLine("No reservations have been made for this day.");
        }
        else
        {
            // Her bir oda için rezervasyonları kontrol edin ve ekrana yazdırın
            for (int j = 0; j < Reservations[i].Length; j++)
            {
                if (Reservations[i][j] != null)
                {
                    string reserverName = Reservations[i][j].ReserverName ?? "Unspecified";
                    Console.WriteLine($"Room ID: {Reservations[i][j].Room.roomId}, Booking Time: {Reservations[i][j].Time}, Reserver Name: {reserverName}");
                }
            }
        }

        Console.WriteLine(); // Bir sonraki gün için boş bir satır
    }
}



}

class Program
{
    static void Main(string[] args)
    {
        // Define the path to the Json file
        string jsonFilePath = "Data.json";

        string jsonString = File.ReadAllText(jsonFilePath);

        var options = new JsonSerializerOptions()
        {
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            WriteIndented = true // Optional: Formats the JSON for better readability
        };
        var roomData = JsonSerializer.Deserialize<RoomData>(jsonString, options);

        if (roomData?.Rooms != null)
        {
            foreach (var room in roomData.Rooms)
            {
                Console.WriteLine($"Room ID: {room.roomId}, Name: {room.roomName}, capacity: {room.capacity}");
            }
        }

        // Initialize ReservationHandler with appropriate parameters
        ReservationHandler reservationHandler = new ReservationHandler(7, roomData.Rooms.Length);

       bool exitRequested = false;
    while (!exitRequested)
    {
        Console.WriteLine("\n1. Add Reservation");
        Console.WriteLine("2. Delete Reservation");
        Console.WriteLine("3. View Weekly Schedule");
        Console.WriteLine("4. Exit");
        Console.Write("Make your choice: ");

        string choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                // Rezervasyon ekleme işlemi
                Console.Write("Enter the room ID to be booked: ");
                string roomId = Console.ReadLine();
                Room selectedRoom = roomData.Rooms.FirstOrDefault(r => r.roomId == roomId);
                if (selectedRoom != null)
                {
                    Console.Write("Enter the date to make the reservation (DD.MM.YYYY): ");
                    if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime reservationDate))
                    {
                        Console.Write("Enter the time to book (HH:MM): ");
                        if (DateTime.TryParseExact(Console.ReadLine(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime reservationTime))
                        {
                            Console.Write("Enter the name of the person making the reservation: ");
                            string reserverName = Console.ReadLine();

                            Reservation newReservation = new Reservation
                            {
                                Date = reservationDate,
                                Time = reservationTime,
                                ReserverName = reserverName,
                                Room = selectedRoom
                            };

                            reservationHandler.AddReservation(newReservation);
                        }
                        else
                        {
                            Console.WriteLine("Invalid time format.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid date format.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid room ID.");
                }
                break;

            case "2":
                    Console.Write("Enter the room ID whose reservation you want to delete: ");
                    string roomIdToDelete = Console.ReadLine();
                    Console.Write("Enter the date you want to delete the reservation (DD.MM.YYYY): ");
                    if (DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime dateToDelete))
                    {
                // Rezervasyonu bulmak için rezervasyonları döngüye al
                    bool reservationFound = false;
                    for (int i = 0; i < reservationHandler.Reservations.GetLength(0); i++)
                    {
                        for (int j = 0; j < reservationHandler.Reservations[i].Length; j++)
                        {
                            Reservation reservation = reservationHandler.Reservations[i][j];
                            if (reservation != null && reservation.Room.roomId == roomIdToDelete && reservation.Date == dateToDelete)
                            {
                                reservationHandler.DeleteReservation(reservation);
                                reservationFound = true;
                                break;
                            }
                        }
            if (reservationFound)
                break;
        }
        if (!reservationFound)
        {
            Console.WriteLine("No reservation was found with the specified room ID and date.");
        }
    }
    else
    {
        Console.WriteLine("Invalid date format.");
    }
    break;


            case "3":
                reservationHandler.DisplayWeeklySchedule();
                break;

            case "4":
                exitRequested = true;
                break;

            default:
                Console.WriteLine("Invalid selection. Please try again.");
                break;
        }
    }
    Console.WriteLine("Exit...");
    }
}
