using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public interface IElevator
{
    int CurrentFloor { get; }
    bool IsMoving { get; }
    void MoveToFloor(int destinationFloor);
    void AddPassengers(int passengerCount);
}

// Class for Elevator
public class Elevator : IElevator
{
    public int Id { get; }
    public int CurrentFloor { get; private set; } = 0;
    public bool IsMoving { get; private set; } = false;
    private readonly int Capacity = 10;
    private int CurrentPassengers = 0;

    public Elevator(int id)
    {
        Id = id;
    }

    public void AddPassengers(int passengerCount)
    {
        if (CurrentPassengers + passengerCount > Capacity)
        {
            Console.WriteLine($"Elevator {Id} cannot exceed its capacity of {Capacity} passengers.");
        }
        else
        {
            CurrentPassengers += passengerCount;
            Console.WriteLine($"Elevator {Id} now has {CurrentPassengers} passengers.");
        }
    }

    public void MoveToFloor(int destinationFloor)
    {
        if (IsMoving)
        {
            Console.WriteLine($"Elevator {Id} is already moving.");
            return;
        }

        IsMoving = true;
        Console.WriteLine($"Elevator {Id} moving from floor {CurrentFloor} to floor {destinationFloor}.");
        Task.Delay(Math.Abs(destinationFloor - CurrentFloor) * 500).Wait(); // Simulate time to move
        CurrentFloor = destinationFloor;
        Console.WriteLine($"Elevator {Id} arrived at floor {CurrentFloor}.");
        IsMoving = false;
    }
}

// Request Class for User Input
public class PassengerRequest
{
    public int FloorNumber { get; }
    public int PassengersWaiting { get; }

    public PassengerRequest(int floorNumber, int passengersWaiting)
    {
        FloorNumber = floorNumber;
        PassengersWaiting = passengersWaiting;
    }
}

// Class for Elevator Controller 
public class ElevatorController
{
    private readonly List<Elevator> Elevators;

    public ElevatorController(int elevatorCount)
    {
        Elevators = new List<Elevator>();
        for (int i = 1; i <= elevatorCount; i++)
        {
            Elevators.Add(new Elevator(i));
        }
    }

    public void ProcessRequest(PassengerRequest request)
    {
        var nearestElevator = Elevators
            .Where(e => !e.IsMoving)
            .OrderBy(e => Math.Abs(e.CurrentFloor - request.FloorNumber))
            .FirstOrDefault();

        if (nearestElevator != null)
        {
            Console.WriteLine($"Dispatching Elevator {nearestElevator.Id} to floor {request.FloorNumber}.");
            nearestElevator.MoveToFloor(request.FloorNumber);
            nearestElevator.AddPassengers(request.PassengersWaiting);
        }
        else
        {
            Console.WriteLine("No available elevators at the moment. Please wait...");
        }
    }

    public void DisplayStatus()
    {
        Console.WriteLine("\nElevator Status:");
        foreach (var elevator in Elevators)
        {
            Console.WriteLine($"Elevator {elevator.Id} | Floor: {elevator.CurrentFloor} | Moving: {elevator.IsMoving}");
        }
    }
}

// Main Program
class Program
{
    static void Main()
    {
        int elevatorCount = 2; // Number of Elevators
        int totalFloors = 10;

        var controller = new ElevatorController(elevatorCount);

        while (true)
        {
            Console.WriteLine("\n1. Request an Elevator\n2. Show Elevator Status\n3. Exit");
            Console.Write("Select an option: ");
            var option = Console.ReadLine();

            if (option == "1")
            {
                int floor;
                Console.Write("Enter floor number (0-10): ");
                while (!int.TryParse(Console.ReadLine(), out floor) || floor < 0 || floor > totalFloors)
                {
                    Console.WriteLine("Invalid input. Please enter a valid floor number (0-10): ");
                }

                int passengers;
                Console.Write("Enter passengers waiting: ");
                while (!int.TryParse(Console.ReadLine(), out passengers) || passengers < 0)
                {
                    Console.WriteLine("Invalid input. Please enter a valid number of passengers (0 or greater): ");
                }

                var request = new PassengerRequest(floor, passengers);
                controller.ProcessRequest(request);
            }
            else if (option == "2")
            {
                controller.DisplayStatus();
            }
            else if (option == "3")
            {
                Console.WriteLine("Exiting program...");
                break;
            }
            else
            {
                Console.WriteLine("Invalid option. Try again.");
            }
        }
    }
}
