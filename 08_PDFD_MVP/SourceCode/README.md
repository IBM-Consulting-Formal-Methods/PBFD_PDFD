# VisitorLog_PDFD

[![DOI](https://zenodo.org/badge/DOI/10.5281/zenodo.17807586.svg)](https://doi.org/10.5281/zenodo.17807586)

This repository contains the source code for a Minimum Viable Product (MVP) developed to support the research paper, **"Formally and Empirically Verified Methodologies for Scalable Hierarchical Full-Stack Systems"**

This is an ASP.NET MVC web application that allows users to log and track their visits to different continents around the world. The application is built using the **Primary Depth-First Development (PDFD)** methodology.

> ⚠️ **Disclaimer**: The code is provided for research and verification purposes only and is not intended for production use.

## Requirements

To run this application, you'll need the following tools installed:

- Microsoft Visual Studio Professional 2022
- .NET 6.0 SDK
- SQL Server 2022
- SQL Server Management Studio (SSMS) v.20

## Setup and Installation

Follow these steps to get the application up and running on your local machine.

### 1. Clone the Repository

```bash
git clone https://github.com/PDFD-MVP/PDFD-MVP.git
```

### 2. Open the Solution
Open the `VisitorLog_PDFD.sln` solution file in Visual Studio 2022. The necessary NuGet packages should be restored automatically. If not, rebuild the solution to trigger the package restore.

### 3. Create the Database
Using SQL Server Management Studio (SSMS), create a new, empty database named `VisitorLog_PDFD`.

### 4. Run Migrations
Open the Package Manager Console in Visual Studio by navigating to:

**Tools > NuGet Package Manager > Package Manager Console**

Once the console is open, run the following command to create all the necessary tables in your new database:

```powershell
Update-Database
```
### 5. Execute the Application
Press **F5** or click the green "Start" button in Visual Studio to build and run the application. The web app will launch in your default browser.

## License
This repository is licensed under the **Apache-2.0 License**.

## Contact
For questions regarding this repository or the research it supports, please contact [dliu@us.ibm.com].
