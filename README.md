# MockIt.NET - A Dynamic Mock Server using C# .NET Core

## Overview
MockIt.NET is a **dynamic mock server** built with C# and ASP.NET Core Web API. It allows users to **dynamically create API endpoints** with customizable **routes, HTTP methods, request conditions, and responses**, without needing a database. Routes and responses are stored in JSON files, making the server highly flexible and easy to configure.

## Features
- ✅ **Dynamic API route creation** via a POST endpoint
- ✅ **Supports path parameters (`{id}`), query parameters, and wildcards (`*`)**
- ✅ **Multiple response scenarios for the same endpoint**
- ✅ **No database required - JSON-based storage**
- ✅ **Handles GET, POST, PUT, DELETE, PATCH requests dynamically**

## Installation
### Prerequisites
- .NET 7.0+ installed
- Postman or cURL for testing

### Clone Repository
```sh
git clone https://github.com/yourusername/MockIt.NET.git
cd MockIt.NET
```

### Build and Run
```sh
dotnet build
dotnet run
```

The server will start at `https://localhost:7052/`.

---

## API Usage
### **1️⃣ Create a Mock Route** (`POST /create`)
This endpoint creates a new mock API route.

📥 **Request**
```http
POST /create
Content-Type: application/json
```
```json
{
  "RouteName": "api/user/{id}/details",
  "HttpMethod": "GET",
  "Responses": [
    {
      "Conditions": {
        "PathParams": { "id": "1" }
      },
      "Response": { "userId": 1, "name": "John Doe", "email": "john@example.com" },
      "StatusCode": 200
    },
    {
      "Conditions": {
        "PathParams": { "id": "*" }
      },
      "Response": { "error": "User not found" },
      "StatusCode": 404
    }
  ]
}
```

📤 **Response**
```json
{
  "message": "Mock route created successfully.",
  "route": { ... }
}
```

**💾 JSON File Created:** `MockRoutes/api_user_param_details.json`

---

### **2️⃣ Handle Mock Requests** (`GET /mock/{route}`)
Once a mock route is created, clients can send requests to it dynamically.

#### Example 1: **Valid User Request**
📥 **Request**
```http
GET /mock/api/user/1/details
```
📤 **Response**
```json
{
  "userId": 1,
  "name": "John Doe",
  "email": "john@example.com"
}
```

#### Example 2: **User Not Found**
📥 **Request**
```http
GET /mock/api/user/999/details
```
📤 **Response**
```json
{
  "error": "User not found"
}
```

---

## Folder Structure
```
MockIt.NET/
├── Controllers/
│   ├── MockController.cs  # Handles dynamic requests
├── MockRoutes/            # Stores mock API definitions in JSON files
│   ├── api_user_param_details.json
├── Program.cs             # App entry point
└── README.md
```

## Future Enhancements
- 🔄 Add UI interface for managing mock routes
- 📄 Swagger support for better API documentation
- 🚀 Docker support for containerization

## License
MIT License © 2025 Your Name

