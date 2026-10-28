# URL Shortener

## Requirements

- A web based application
- The application should have the ability to pass a long URL as part of the request and get a shorter version of it
- It should store short and long URLs into a database
- When the user pass a long URL it should be converted into a short one and stored in the database
- When the user tries to access the short URL he should be redirected to the original URL which corresponds to the short one
- Database table in which we store the URLs needs to be indexed for faster lookups
- The application should have caching layer to reduce the load over the database and improve the response time of the incoming GET requests
- It should have simple, modern and minimalistic UI which will allow the user to pass the long URL and will display the short URL which was generated
- Everything needs to be containerised in docker for easy start and easy deployment

## Technical Details

- Api and frontend should live in two separate places
- The frontend part should be written on NextJS and should be created in subdirectory called `frontend`
- The backend part should be written in the latest version of ASP.NET in subdirectory called `api`
- Latest version of EF core should be used for database communication
- Redis should be used to implement the caching layer
- There will be no user management
- As simple as possible but with an elegant UI
- Postgres will be used as a database

## Color Scheme

- Use colors of your choice but stick to the dark theme

## Strategy

1. Write plan with success criteria for each phase to be checked off. Include project scaffolding, including .gitignore, and rigorous unit testing.
2. Execute the plan ensuring all critiera are met
3. Carry out extensive integration testing, fixing defects
4. Only complete when the application is finished and tested, with the server running and ready for the user

## Coding standards

1. Use latest versions of libraries and idiomatic approaches as of today
2. Keep it simple - NEVER over-engineer, ALWAYS simplify, NO unnecessary defensive programming. No extra features - focus on simplicity.
3. Be concise. Keep README minimal. IMPORTANT: no emojis ever
4. Use clean architecture when implementing the backend