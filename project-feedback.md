
1. Project Structure & Code Quality
Use DTOs for both API request and response models. Entities for persistence; map manually or with AutoMapper. Using Entities directly as request models can expose internal data structure details and potentially lead to unintended data modification or security issues.
Use primary constructor syntax where applicable.
Include proper namespaces (e.g., DistanceCalculator).
Avoid unclear variable names (replace a, c with meaningful names).
Add missing appsettings.json or configuration files.
Remove unused Redis cache code or implement it properly.
2. Entity Modeling & Relationships
Review use of ICollection<UserShip> in User and Ship models. If you don’t need any extra data on the relationship, using ICollection<User> and ICollection<Ship> directly is simpler and EF Core handles the join table automatically. Use an explicit join entity like UserShip only if you need to store additional info or have special logic on the relationship.
Consider if inheriting from IdentityDbContext instead of DbContext is necessary.
3. API Design & Consistency
Add annotations like [ProducesResponseType] to clarify possible HTTP responses.
Consistently use either shipId or shipCode across endpoints.
For POST endpoints requiring mandatory data like userId, accept the data via [FromBody] in a request DTO instead of using query parameters; otherwise, ModelState.IsValid validation won’t work correctly.
Combine or clarify the two separate endpoints for closestPort and estimatedArrival.
Enforce validation on create endpoints (e.g., restrict empty shipCode).
Clarify the purpose of the auth login controller or remove it if unnecessary.
4. Validation & Error Handling
Avoid try-catch in controllers; use a global exception filter.
Handle edge cases like velocity = 0 to avoid divide-by-zero errors.
Use null-coalescing operators to avoid null reference exceptions.
Fix improper usage of RequiredValidShipCode attribute (method-level validation requires action filter).
5. Testing
Add unit tests for service layer methods.
Cover edge cases and invalid inputs in tests.
Properly mock dependencies.
6. Infrastructure & Configuration
Justify the use of Postgres or align with project standards.
Ensure all necessary config files (like appsettings.json) are present and properly configured.
