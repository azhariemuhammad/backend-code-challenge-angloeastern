namespace ShipManagement.Constants
{
    public static class Messages
    {
        // Ship Messages
        public static class Ship
        {
            public const string NOT_FOUND = "Ship with code '{0}' not found.";
            public const string NOT_FOUND_BY_ID = "Ship with ID {0} not found.";
            public const string DUPLICATE_SHIP_CODE = "A ship with ShipCode '{0}' already exists.";
            public const string CREATE_ERROR = "An error occurred while creating the ship.";
            public const string UPDATE_ERROR = "An error occurred while updating the ship.";
            public const string DELETE_ERROR = "An error occurred while deleting the ship.";
            public const string RETRIEVE_ERROR = "An error occurred while retrieving the ship.";
            public const string RETRIEVE_ALL_ERROR = "An error occurred while retrieving ships.";
            public const string RETRIEVE_UNASSIGNED_ERROR = "An error occurred while retrieving unassigned ships.";
        }

        // User Ship Assignment Messages
        public static class UserShip
        {
            public const string ASSIGN_ERROR = "An error occurred while assigning user to ship.";
            public const string UNASSIGN_ERROR = "An error occurred while unassigning user from ship.";
            public const string NOT_ASSIGNED = "User {0} is not assigned to ship {1} or ship does not exist.";
        }

        // Port Messages
        public static class Port
        {
            public const string CLOSEST_PORT_ERROR = "An error occurred while finding the closest port.";
            public const string ESTIMATED_ARRIVAL_ERROR = "An error occurred while calculating estimated arrival.";
            public const string SHIP_NOT_FOUND_FOR_PORT = "Ship with ID {0} not found for port calculation.";
            public const string NO_PORTS_AVAILABLE = "No ports available for calculation.";
        }

        // General Messages
        public static class General
        {
            public const string INVALID_MODEL_STATE = "Invalid model state.";
            public const string UNEXPECTED_ERROR = "An unexpected error occurred.";
        }
    }
}
