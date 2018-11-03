# AzureResourceDiff
This project gives the basis code for an Azure Function (AzureResourceFunctions) to take daily snapshots of the Azure registered resource providers for a subscription, calculate a difference to the preceding day, and save the results in a database.  Then the frontend web app (AzureResourceWeb) contains a web page that loads the information, and shows the last 20 days of changes.

# Try it here
https://azure-changes.azurewebsites.net/ 

# Project Descriptions
## AzureResourceCommon
.NET Standard library containing the DTO and repository classes.
### Configuration
Environment variable ConnectionString with the Azure SQL connection to a database.

## AzureResourceFunctions
Serverless 2.0 functions to check the Azure API each day and see if there are any changes.  If there are, they are saved to the database through the repository in AzureResourceCommon.

## AzureResourceWeb
ASP.NET Core 2.1 frontend web app to display the last 20 days of changes.

# Credits
- Many thanks to the CodyHouse example Vertical Timeline here: https://codyhouse.co/gem/vertical-timeline/
- Many thanks to the loading animation here: https://tympanus.net/codrops/2012/11/14/creative-css-loading-animations/ 
