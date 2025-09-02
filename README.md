# Invoice Payment Processor
## By: Eric Buell
Built using React, .NET, Postgres, Stripe.


## Project Setup
1.  Check that all the node packages are installed  
    - `cd ClientApp/`
    - `npm install`

2. Setup .env based off .env.example in ClientApp

3. Check that .NET has all needed dependencies (listed below)

4. Install Stripe CLI
    - https://docs.stripe.com/stripe-cli  

5. Setup dotnet secrets for: (key, exampleValue)
    - Stripe:WebhookKey = "whsec_..."
    - Stripe:SecretKey = "sk_test_..."
    - ConnectionStrings:postgres = "Host=localhost;Port=5432;Database=DbName;Username=DbUsername;Password=DbPassword;Trust Server Certificate=true;"  

6. Install Postgres and use tableSetup.sql to setup the tables needed with included default values

## Project Startup 
To start the project you should only need to run two commands  
1. `dotnet run`
2. `stripe listen --forward-to https://localhost:5001/webhook`



## Project .NET Dependencies  
[net9.0]  
| Top-level Package | Requested | Resolved |
| ------ | ------ | ----- |  
| Microsoft.AspNetCore.SpaServices.Extensions | 3.1.8 | 3.1.8
| Microsoft.EntityFrameworkCore.Design | 9.0.8 | 9.0.8
| Npgsql.EntityFrameworkCore.PostgreSQL | 9.0.4 | 9.0.4
| Persic.EF.Postgres | 2025.106.102.11 | 2025.106.102.11
| Stripe.net | 48.5.0 | 48.5.0



