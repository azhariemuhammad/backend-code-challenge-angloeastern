namespace ShipManagement.Constants
{
    public static class Messages
    {
        // Ship Messages
        public static class Ship
        {
            public const string NotFound = "Ship with code '{0}' not found.";
            public const string NotFoundById = "Ship with ID {0} not found.";
            public const string DuplicateShipCode = "A ship with ShipCode '{0}' already exists.";
            public const string CreateError = "An error occurred while creating the ship.";
            public const string UpdateError = "An error occurred while updating the ship.";
            public const string DeleteError = "An error occurred while deleting the ship.";
            public const string RetrieveError = "An error occurred while retrieving the ship.";
            public const string RetrieveAllError = "An error occurred while retrieving ships.";
            public const string RetrieveUnassignedError = "An error occurred while retrieving unassigned ships.";
        }

        // User Ship Assignment Messages
        public static class UserShip
        {
            public const string AssignError = "An error occurred while assigning user to ship.";
            public const string UnassignError = "An error occurred while unassigning user from ship.";
            public const string NotAssigned = "User {0} is not assigned to ship {1} or ship does not exist.";
        }

        // Port Messages
        public static class Port
        {
            public const string ClosestPortError = "An error occurred while finding the closest port.";
            public const string EstimatedArrivalError = "An error occurred while calculating estimated arrival.";
            public const string ShipNotFoundForPort = "Ship with ID {0} not found for port calculation.";
            public const string NoPortsAvailable = "No ports available for calculation.";
        }

        // General Messages
        public static class General
        {
            public const string InvalidModelState = "Invalid model state.";
            public const string UnexpectedError = "An unexpected error occurred.";
        }
    }
}
